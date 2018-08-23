using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/8/2018
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
    private GameObject _SeekWaypoint = null;
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

        MaxHitPoints = 0;
        MaxShieldPoints = 0;
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
            foreach (var unit in _Squad) { unit.OnSpawn(); }
        }

        // Update squad health
        else if (_ObjectState == WorldObjectStates.Active) {
            
            if (IsAlive()) {

                // Show the healthbar
                if (_HealthBar != null) { _HealthBar.gameObject.SetActive(true); }
            }

            // Update sum hitpoints of the squad
            float currentSquadHitPoints = 0f;
            foreach (var unit in _Squad) {
                
                currentSquadHitPoints += unit.GetHitPoints();
            }
            _HitPoints = currentSquadHitPoints;
            _SquadCurrentSize = _Squad.Count;
        }

        // Update squad's position to match the average position of all the units
        Vector3 position = FindCenterOfAllUnits();
        gameObject.transform.position = position;
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

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.AttachedBase.GroundUnitSpawnTransform.transform.position;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.AttachedBase.GroundUnitSpawnTransform.transform.rotation;
            }

            // No base attached
            else {

                // Set position to be at the buildings spawn vector while it is building
                // (the gameobject should be hidden completely until its deployed)
                _ClonedWorldObject.gameObject.transform.position = buildingSlot.transform.position + buildingSlot.transform.forward * 60.0f;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.transform.rotation;
            }

            // Create units
            CreateUnits(_ClonedWorldObject as Squad);

            // Add squad to list of AI (army)
            _ClonedWorldObject._Player.AddToPopulation(_ClonedWorldObject as Squad);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void UpdateChasingEnemy() {
        base.UpdateChasingEnemy();

        if (_AttackTarget != null) { SquadAttackObject(_AttackTarget); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected override void ResetToOriginPosition() {
        base.ResetToOriginPosition();

        for (int i = 0; i < _Squad.Count; i++) { _Squad[i].ResetToOrigin(); }
        SquadSeek(_ChaseOriginPosition);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="thisSquad"></param>
    protected void CreateUnits(Squad squad) {
        
        // Loop for each unit
        for (int i = 0; i < SquadMaxSize; i++) {

            // Create unit
            Unit unit = ObjectPooling.Spawn(SquadUnit.gameObject, squad.transform.position, squad.transform.rotation).GetComponent<Unit>();
            unit.SetObjectState(WorldObjectStates.InQueue);
            unit.SetSquadAttached(squad);
            unit.Team = squad.Team;
            unit.SetPlayer(squad._Player);
            squad._Squad.Add(unit);
            squad._SquadCurrentSize = i;

            // Update unit build time to match the squad build time
            unit.BuildingTime = BuildingTime;

            // Creating the first unit at the center
            if (i == 0) { unit.transform.position = squad.transform.position; ; }
            else {

                // Creating the units in a circle around the flocking radius
                float angle = i * Mathf.PI * 2 / squad.SquadMaxSize;
                Vector3 pos = squad.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * squad.FlockingRadius;
                unit.transform.position = pos;
            }
            unit.ResetHealth();
            unit.gameObject.SetActive(true);

            squad.MaxHitPoints += unit.MaxHitPoints;
            squad.MaxShieldPoints += unit.MaxShieldPoints;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void SpawnUnits(Squad squad) {
        
        // Loop for each unit
        for (int i = 0; i < SquadMaxSize; i++) {

            // Create unit
            Unit unit = ObjectPooling.Spawn(SquadUnit.gameObject, squad.transform.position, squad.transform.rotation).GetComponent<Unit>();
            unit.SetObjectState(WorldObjectStates.Active);
            unit.SetSquadAttached(squad);
            unit.Team = squad.Team;
            unit.SetPlayer(squad._Player);
            squad._Squad.Add(unit);
            squad._SquadCurrentSize = i;

            // Update unit build time to match the squad build time
            unit.BuildingTime = squad.BuildingTime;
            
            // Creating the first unit at the center
            if (i == 0) { unit.transform.position = squad.transform.position; ; }
            else {

                // Creating the units in a circle around the flocking radius
                float angle = i * Mathf.PI * 2 / squad.SquadMaxSize;
                Vector3 pos = squad.transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * squad.FlockingRadius;
                unit.transform.position = pos;
            }
            unit.ResetHealth();
            unit.gameObject.SetActive(true);
            unit.OnSpawn();

            squad.MaxHitPoints += unit.MaxHitPoints;
            squad.MaxShieldPoints += unit.MaxShieldPoints;
        }
        squad.SetObjectState(WorldObjectStates.Active);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
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
    /// <returns>
    //  List<Vector3>
    /// </returns>
    private List<Vector3> GetAttackingPositionsAtObject(WorldObject worldObject, int size) {
        
        float facingAngle = Vector3.Angle(worldObject.transform.forward, worldObject.transform.position - transform.position);
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < size; i++) {

            float angle = i * (Mathf.PI * 10.0f / size + (facingAngle / 10));
            Vector3 pos = new Vector3(Mathf.Cos((angle / size) / size), 
                                      worldObject.transform.position.y, 
                                      Mathf.Sin((angle / size) / size)) * _Squad[0].GetAgent().radius * _Squad[0].MaxAttackingRange * 0.4f;
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
    /// <param name="overwrite"></param>
    public void SquadSeek(Vector3 seekTarget, bool overwrite = false) {

        // Get positions with an offset for each unit to seek towards
        _SeekTarget = seekTarget;
        List<Vector3> positions = GetPositionsWithinFlockingBoundsOfPoint(_SeekTarget, _Squad.Count);

        // Get all alive units to seek to the squad seek target
        int i = 0;
        foreach (var unit in _Squad) {
                            
            unit.AgentSeekPosition(positions[i], overwrite, false);
            i++;
        }

        // Create waypoint
        if (_SeekWaypoint == null) { _SeekWaypoint = ObjectPooling.Spawn(GameManager.Instance.AgentSeekObject, Vector3.zero, Quaternion.identity); }
        if (_SeekWaypoint != null) {

            // Display waypoint if not already being displayed
            if (_SeekWaypoint.activeInHierarchy != true && Team == GameManager.Team.Defending) { _SeekWaypoint.SetActive(true); }

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
    /// <param name="overwrite"></param>
    public void SquadAttackObject(WorldObject attackTarget, bool overwrite = false) {

        // Get positions with an offset for each unit to seek towards
        List<Vector3> positions = GetAttackingPositionsAtObject(attackTarget, _Squad.Count);

        // Get all alive units to attack the object (while positioning ourselves)
        int i = 0;
        foreach (var unit in _Squad) {

            unit.AgentAttackObject(attackTarget, positions[i], overwrite);
            i++;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnitFromSquad(Unit unit) {

        GameObject g = this.gameObject;

        // Loop through current squad
        for (int i = 0; i < unit.GetSquadAttached().GetSquadMembers().Count; i++) {

            // Match found so remove it
            if (unit.GetSquadAttached().GetSquadMembers()[i] == unit) { unit.GetSquadAttached().GetSquadMembers().RemoveAt(i); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int GetSquadCount() { return _SquadCurrentSize; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  List<Unit>
    /// </returns>
    public List<Unit> GetSquadMembers() { return _Squad; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}