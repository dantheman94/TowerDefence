using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 4/7/2018
//
//******************************

public class Squad : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SQUAD PROPERTIES")]
    [Space]
    public int SquadMaxSize;
    public Unit SquadUnit;
    public float FlockingRadius;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private int _SquadCurrentSize;
    private float _SquadHealth;
    private List<Unit> _Squad;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called before Start().
    /// </summary>
    protected override void Awake() {
        base.Awake();

        // Update sphere collider radius to match the flocking radius
        GetComponent<SphereCollider>().radius = FlockingRadius;

        // Create lists
        _Squad = new List<Unit>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Hide the unit UI widgets if it is building
        if (_ObjectState == WorldObjectStates.Building) {

            // Hide the healthbar
            if (_HealthBar != null) { _HealthBar.gameObject.SetActive(false); }

            // Despawn build counter widget (it is unused)
            if (_BuildingProgressCounter != null) { ObjectPooling.Despawn(_BuildingProgressCounter.gameObject); }
        }

        // Force the unit to skip the deployable state and go straight to being active in the world
        else if (_ObjectState == WorldObjectStates.Deployable) {

            _ObjectState = WorldObjectStates.Active;
            foreach (var unit in _Squad) { unit.SetObjectState(WorldObjectStates.Active); }
        }

        // Update squad health
        else if (_ObjectState == WorldObjectStates.Active && IsAlive()) {

            // Show the healthbar
            if (_HealthBar != null) { _HealthBar.gameObject.SetActive(true); }

            // Get total sum of health for all units in the squad
            int maxSquadHealth = 0, currentSquadHealth = 0;
            foreach (var unit in _Squad) {

                maxSquadHealth += unit.MaxHitPoints;
                currentSquadHealth += unit.GetHitPoints();
            }

            // Normalize the health between a range of 0.0 - 1.0
            if (maxSquadHealth > 0) { _SquadHealth = currentSquadHealth / maxSquadHealth; }
        }

        // Update squad's position to match the average position of all the units
        Vector3 position = FindCenterOfAllUnits();
        this.gameObject.transform.position = position;
        if (_SelectionObj) { _SelectionObj.gameObject.transform.position = position; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {
        base.OnWheelSelect(buildingSlot);

        // Get reference to the newly cloned unit
        if (_ClonedWorldObject != null) {

            // Despawn build counter widget
            if (_BuildingProgressCounter != null) { ObjectPooling.Despawn(_BuildingProgressCounter.gameObject); }
                        
            // Let the building attached know that it is "building" something
            if (buildingSlot.GetBuildingOnSlot() != null) {

                buildingSlot.GetBuildingOnSlot().SetIsBuildingSomething(true);
                buildingSlot.GetBuildingOnSlot().SetObjectBeingBuilt(_ClonedWorldObject);
            }

            // Set position to be at the bases spawn vector while it is building
            // (the gameobject should be hidden completely until its deployed)
            if (buildingSlot.AttachedBase != null) {

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.AttachedBase.UnitSpawnTransform.transform.position;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.AttachedBase.UnitSpawnTransform.transform.rotation;
            }

            // No base attached
            else {

                // Set position to be at the buildings spawn vector while it is building
                // (the gameobject should be hidden completely until its deployed)
                _ClonedWorldObject.gameObject.transform.position = buildingSlot.transform.position + buildingSlot.transform.forward * 50.0f;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.transform.rotation;
            }

            // Create units
            Squad thisSquad = _ClonedWorldObject as Squad;
            CreateUnits(thisSquad);

            // Add squad to list of AI (army)
            _Player.AddToPopulation(thisSquad);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="thisSquad"></param>
    protected void CreateUnits(Squad thisSquad) {
        
        // Use the selection sphere for offsetting each unit's spawn location
        SphereCollider spawnSphere = GetComponent<SphereCollider>();
        List<Vector3> offsets = new List<Vector3>();

        // Loop for each unit
        for (int i = 0; i < SquadMaxSize; i++) {

            // Create unit
            Unit unit = ObjectPooling.Spawn(SquadUnit.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
            unit.SetObjectState(WorldObjectStates.Building);
            unit.SetSquadAttached(thisSquad);
            thisSquad._Squad.Add(unit);
            thisSquad._SquadCurrentSize = i;

            // Update unit build time to match the squad build time
            unit.BuildTime = thisSquad.BuildTime;

            // Offset each unit somewhere within the flocking radius
            spawnSphere.transform.position = thisSquad.gameObject.transform.position;
            spawnSphere.radius = FlockingRadius;
            float randX = Random.Range(spawnSphere.transform.position.x - FlockingRadius, spawnSphere.transform.position.x + FlockingRadius);
            float randZ = Random.Range(spawnSphere.transform.position.z - FlockingRadius, spawnSphere.transform.position.z + FlockingRadius);
            Vector3 vecPos = new Vector3(randX, thisSquad.gameObject.transform.position.y, randZ);

            // Check if the offset is overlapping any other unit offsets & rectify if true
            List<bool> nonOverlaps = new List<bool>();
            float agentRadius = thisSquad.SquadUnit.GetComponent<NavMeshAgent>().radius;
            bool overlapping = true;
            while (overlapping) {

                // Restarting the overlap checks
                if (nonOverlaps.Count > 0) { nonOverlaps.Clear(); }

                // Create a bounds at current offset position * agent radius
                Bounds offsetBounds = new Bounds(vecPos, new Vector3(agentRadius, agentRadius, agentRadius));

                // Test intersection against other unit offsets
                if (offsets.Count > 0) {

                    foreach (var testOffset in offsets) {

                        // Do the bounds intersect?
                        Bounds testBounds = new Bounds(testOffset, new Vector3(agentRadius, agentRadius, agentRadius));
                        if (offsetBounds.Intersects(testBounds)) {

                            // Move the offset until its no longer overlapping 
                            float posDistance = Vector3.Distance(vecPos, testOffset);
                            int additive = (int)Random.Range(0, 3);
                            if (additive == 0) { vecPos.Set(vecPos.x + (posDistance - agentRadius) / 2, vecPos.y, vecPos.z + (posDistance + agentRadius) / 2); }
                            else if (additive == 1) { vecPos.Set(vecPos.x + (posDistance - agentRadius) / 2, vecPos.y, vecPos.z - (posDistance - agentRadius) / 2); }
                            else if (additive == 2) { vecPos.Set(vecPos.x - (posDistance - agentRadius) / 2, vecPos.y, vecPos.z - (posDistance + agentRadius) / 2); }
                            else if (additive == 3) { vecPos.Set(vecPos.x - (posDistance - agentRadius) / 2, vecPos.y, vecPos.z - (posDistance - agentRadius) / 2); }
                        }
                        else { nonOverlaps.Add(new bool()); }
                    }

                    // If this returns TRUE, then all offsets are correctly in place
                    if (nonOverlaps.Count == i) { overlapping = false; }
                }

                // Bounds aren't overlapping if theres no other bounds to test against (IE: this is the first unit being created)
                else { overlapping = false; }
            }

            // Offset isn't intersecting against other units so we add it to the list to check against for other units
            offsets.Add(vecPos);

            // Move new unit position to the offset
            unit.transform.position = vecPos;
            unit.gameObject.SetActive(true);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector3 FindCenterOfAllUnits() {

        // No units in squad, so gameObject's position is technically its current position anyway
        if (_Squad.Count == 0) { return gameObject.transform.position; }

        // Just return the position of the last alive unit
        else if (_Squad.Count == 1) { return _Squad[0].gameObject.transform.position; }

        // Squad size is greater than 1
        else {

            // Get average position for all units in squad
            Bounds centerBounds = new Bounds(_Squad[0].gameObject.transform.position, Vector3.zero);

            for (int i = 1; i < _Squad.Count; i++) { centerBounds.Encapsulate(_Squad[i].gameObject.transform.position); }
            return centerBounds.center;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private List<Vector3> GetPositionsWithinFlockingBoundsOfPoint(Vector3 point, int size) {

        List<Vector3> positions = new List<Vector3>();

        // Use the selection sphere for offsetting each unit's spawn location
        SphereCollider spawnSphere = this.gameObject.AddComponent<SphereCollider>();
        spawnSphere.center = point;
        spawnSphere.radius = FlockingRadius;

        for (int i = 0; i < size; i++) {

            // Random position somewhere within the flocking radius
            float randX = Random.Range(spawnSphere.center.x - FlockingRadius, spawnSphere.center.x + FlockingRadius);
            float randZ = Random.Range(spawnSphere.center.z - FlockingRadius, spawnSphere.center.z + FlockingRadius);
            Vector3 vecPos = new Vector3(randX, this.gameObject.transform.position.y, randZ);

            // Check if the offset is overlapping any other unit offsets & rectify if true
            List<bool> nonOverlaps = new List<bool>();
            float agentRadius = SquadUnit.GetComponent<NavMeshAgent>().radius;
            bool overlapping = true;
            while (overlapping) {

                // Restarting the overlap checks
                if (nonOverlaps.Count > 0) { nonOverlaps.Clear(); }

                // Create a bounds at current offset position * agent radius
                Bounds offsetBounds = new Bounds(vecPos, new Vector3(agentRadius, agentRadius, agentRadius));

                // Test intersection against other unit offsets
                if (positions.Count > 0) {

                    foreach (var testOffset in positions) {

                        // Do the bounds intersect?
                        Bounds testBounds = new Bounds(testOffset, new Vector3(agentRadius, agentRadius, agentRadius));
                        if (offsetBounds.Intersects(testBounds)) {

                            // Move the offset in a random direction until its no longer overlapping 
                            float posDistance = Vector3.Distance(vecPos, testOffset);
                            int additive = (int)Random.Range(0, 3);
                            if      (additive == 0) { vecPos.Set(vecPos.x + (posDistance - agentRadius), vecPos.y, vecPos.z + (posDistance + agentRadius)); }
                            else if (additive == 1) { vecPos.Set(vecPos.x + (posDistance - agentRadius), vecPos.y, vecPos.z - (posDistance - agentRadius)); }
                            else if (additive == 2) { vecPos.Set(vecPos.x - (posDistance - agentRadius), vecPos.y, vecPos.z - (posDistance + agentRadius)); }
                            else if (additive == 3) { vecPos.Set(vecPos.x - (posDistance - agentRadius), vecPos.y, vecPos.z - (posDistance - agentRadius)); }
                        }
                        else { nonOverlaps.Add(new bool()); }
                    }

                    // If this returns TRUE, then all offsets are correctly in place
                    if (nonOverlaps.Count == i) { overlapping = false; }
                }

                // Bounds aren't overlapping if theres no other bounds to test against (IE: this is the first unit being created)
                else { overlapping = false; }
            }

            // Offset isn't intersecting against other units so we add it to the list to check against for other units
            positions.Add(vecPos);
        }

        // Destroy obsolete sphere
        Destroy(spawnSphere);
        return positions;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="worldObject"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private List<Vector3> GetAttackingPositionsAtObject(WorldObject worldObject, int size) {
        
        float facingAngle = Vector3.Angle(worldObject.transform.forward, worldObject.transform.position - transform.position);
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < size; i++) {

            ///float angle = i * (Mathf.PI * 10.0f / size + /*worldObject.*/transform.rotation.y);
            float angle = i * (Mathf.PI * 10.0f / size + (facingAngle / 10));
            Vector3 pos = new Vector3(Mathf.Cos((angle / size) / size), worldObject.transform.position.y, Mathf.Sin((angle / size) / size)) * _Squad[0].GetAgent().radius * _Squad[0].AttackingRange * 0.4f;
            pos += worldObject.transform.position;

            positions.Add(pos);
        }

        return positions;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="seekTarget"></param>
    public void SquadSeek(Vector3 seekTarget) {

        // Get positions with an offset for each unit to seek towards
        List<Vector3> positions = GetPositionsWithinFlockingBoundsOfPoint(seekTarget, _Squad.Count);

        // Get all alive units to seek to the squad seek target
        int i = 0;
        foreach (var unit in _Squad) {
            
            unit.AgentSeekPosition(positions[i]);
            i++;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="attackTarget"></param>
    public void SquadAttackObject(WorldObject attackTarget) {

        // Get positions with an offset for each unit to seek towards
        List<Vector3> positions = GetAttackingPositionsAtObject(attackTarget, _Squad.Count);

        // Get all alive units to attack the object (while positioning ourselves)
        int i = 0;
        foreach (var unit in _Squad) {
            
            unit.AgentAttackObject(attackTarget, positions[i]);
            i++;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}