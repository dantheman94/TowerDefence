using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 6/8/2018
//
//******************************

public class Squad : Ai {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SQUAD PROPERTIES")]
    [Space]
    public int SquadMaxSize = 0;
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
    protected GameObject _SeekWaypoint = null;

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
            float maxSquadHealth = 0, currentSquadHealth = 0;
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
            _ClonedWorldObject._Player.AddToPopulation(thisSquad);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="thisSquad"></param>
    protected void CreateUnits(Squad thisSquad) {
        
        // Loop for each unit
        for (int i = 0; i < SquadMaxSize; i++) {

            // Create unit
            Unit unit = ObjectPooling.Spawn(SquadUnit.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
            unit.SetObjectState(WorldObjectStates.Building);
            unit.SetSquadAttached(this);
            thisSquad._Squad.Add(unit);
            thisSquad._SquadCurrentSize = i;

            // Update unit build time to match the squad build time
            unit.BuildTime = BuildTime;

            // Creating the first unit at the center
            if (i == 0) { unit.transform.position = gameObject.transform.position; ; }
            else {

                // Creating the units in a circle around the flocking radius
                float angle = i * Mathf.PI * 2 / SquadMaxSize;
                Vector3 pos = thisSquad.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * FlockingRadius;
                unit.transform.position = pos;
            }
            unit.gameObject.SetActive(true);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SpawnUnits() {
        
        // Loop for each unit
        for (int i = 0; i < SquadMaxSize; i++) {

            // Create unit
            Unit unit = ObjectPooling.Spawn(SquadUnit.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Unit>();
            unit.SetObjectState(WorldObjectStates.Active);
            unit.SetSquadAttached(this);
            _Squad.Add(unit);
            _SquadCurrentSize = i;

            // Update unit build time to match the squad build time
            unit.BuildTime = BuildTime;
            
            // Creating the first unit at the center
            if (i == 0) { unit.transform.position = gameObject.transform.position; ; }
            else {

                // Creating the units in a circle around the flocking radius
                float angle = i * Mathf.PI * 2 / SquadMaxSize;
                Vector3 pos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * FlockingRadius;
                unit.transform.position = pos;
            }
            unit.gameObject.SetActive(true);
            unit.OnSpawn();
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

        // Loop for each unit
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < size; i++) {

            // Create individual position
            Vector2 rand = Random.insideUnitCircle * FlockingRadius;
            Vector3 randPos = point + new Vector3(rand.x, point.y, rand.y);
            positions.Add(randPos);
        }
        
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

            float angle = i * (Mathf.PI * 10.0f / size + (facingAngle / 10));
            Vector3 pos = new Vector3(Mathf.Cos((angle / size) / size), 
                                      worldObject.transform.position.y, 
                                      Mathf.Sin((angle / size) / size)) * _Squad[0].GetAgent().radius * _Squad[0].AttackingRange * 0.4f;
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
            
            unit.AgentSeekPosition(positions[i], false);
            i++;
        }

        // Create waypoint
        if (_SeekWaypoint == null) { _SeekWaypoint = ObjectPooling.Spawn(GameManager.Instance.AgentSeekObject, Vector3.zero, Quaternion.identity); }
        if (_SeekWaypoint != null) {

            // Display waypoint if not already being displayed
            if (_SeekWaypoint.activeInHierarchy != true) { _SeekWaypoint.SetActive(true); }

            // Update waypoint position
            _SeekWaypoint.transform.position = seekTarget;
            _SeekWaypoint.transform.position += Vector3.up;
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