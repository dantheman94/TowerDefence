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

public class Squad : Ai
{

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
    public float CornerDistanceThreshold = 10f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private int _SquadCurrentSize;
    private GameObject _SeekWaypoint = null;
    private List<Vector3> _PathCorners;
    private List<Unit> _Squad;

    private bool _UpdateBoids = false;
    private bool _HasPath = false;

    public LayerMask layerMask;

    private bool _PathCalculated = false;
    private bool _SeekPathComplete = false;
    private NavMeshPath _CurrentPath;
    private Vector3 _CurrentCornerTarget;
    private int _NodeIterator = 0;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called before Start().
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // Update sphere collider radius to match the flocking radius
        GetComponent<SphereCollider>().radius = FlockingRadius;

        // Create lists
        _Squad = new List<Unit>();
        _PathCorners = new List<Vector3>();

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
            if (_HealthBar != null) {

                _HealthBar.gameObject.SetActive(false);
            }

            // Despawn build counter widget (it is unused)
            if (_BuildingProgressCounter != null) {

                ObjectPooling.Despawn(_BuildingProgressCounter.gameObject);
            }
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

        // Set if the path has been calculated
        if (_CurrentPath != null) {

            _PathCalculated = _CurrentPath.status == NavMeshPathStatus.PathComplete;

            // Draw debug lines of the calculated path
            for (int i = 0; i < _CurrentPath.corners.Length - 1; i++) {

                Debug.DrawLine(_CurrentPath.corners[i], _CurrentPath.corners[i + 1], Color.red);
            }
        }

        // Calculate distance from path corner
        float dist = Vector3.Distance(transform.position, _CurrentCornerTarget);

        // If the distance to the corner is less than the threshold
        if (dist < CornerDistanceThreshold && _PathCorners != null) {

            _SeekPathComplete = false;

            // Update node iterator
            if (_NodeIterator + 1 <= _PathCorners.Count) {

                _NodeIterator++;
                _CurrentCornerTarget = _CurrentPath.corners[_NodeIterator];

                // Go to offsetted paths
                List<Vector3> positions = GetPositionsWithinFlockingBoundsOfPoint(_CurrentCornerTarget, _Squad.Count);
                for (int i = 0; i < _Squad.Count; i++) {

                    _Squad[i].AgentSeekPosition(positions[i], false, true);
                }

            }
            else if (_NodeIterator == _PathCorners.Count)
            {
                _SeekPathComplete = true;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot)
    {
        base.OnWheelSelect(buildingSlot);

        // Get reference to the newly cloned unit
        if (_ClonedWorldObject != null)
        {

            // Despawn build counter widget
            if (_BuildingProgressCounter != null) { ObjectPooling.Despawn(_BuildingProgressCounter.gameObject); }

            // Let the building attached know that it is "building" something
            if (buildingSlot.GetBuildingOnSlot() != null)
            {

                buildingSlot.GetBuildingOnSlot().SetIsBuildingSomething(true);
                buildingSlot.GetBuildingOnSlot().SetObjectBeingBuilt(_ClonedWorldObject);
            }

            // Set position to be at the bases spawn vector while it is building
            // (the gameobject should be hidden completely until its deployed)
            if (buildingSlot.AttachedBase != null)
            {

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.AttachedBase.GroundUnitSpawnTransform.transform.position;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.AttachedBase.GroundUnitSpawnTransform.transform.rotation;
            }

            // No base attached
            else
            {

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
    ///
    /// </summary>
    public void SquadCalculatePath(Vector3 targetPos) {

        _SeekTarget = targetPos;

        // Create new path
        _CurrentPath = new NavMeshPath();

        // Calculate the path
        NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, _CurrentPath);

        // Start coroutine
        StartCoroutine(SeekPathCalculated());
    }

    IEnumerator SeekPathCalculated() {

        yield return new WaitUntil(() => _PathCalculated);

        // Set the path corners into a list
        if (_PathCorners != null) {

            _PathCorners.Clear();
        }

        // Draw debug lines of the calculated path
        for (int i = 0; i < _CurrentPath.corners.Length - 1; i++) {

            Debug.DrawLine(_CurrentPath.corners[i], _CurrentPath.corners[i + 1], Color.red);
           
            // Add to path node list

            if (_PathCorners != null) {
                _PathCorners.Add(_CurrentPath.corners[i]);
            }
        }

        // Set starting point
        _NodeIterator = 0;

        if (_CurrentPath.corners.Length > 0) {

            _CurrentCornerTarget = _CurrentPath.corners[_NodeIterator];

            // Go to starting point
            List<Vector3> positions = GetPositionsWithinFlockingBoundsOfPoint(_CurrentCornerTarget, _Squad.Count);
            for (int i = 0; i < _Squad.Count; i++) {

                _Squad[i].AgentSeekPosition(positions[i], false, true);
            }
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
    protected void CreateUnits(Squad squad)
    {

        // Loop for each unit
        for (int i = 0; i < SquadMaxSize; i++)
        {

            // Create unit
            Unit unit = ObjectPooling.Spawn(SquadUnit.gameObject, squad.transform.position, squad.transform.rotation).GetComponent<Unit>();
            unit.SetObjectState(WorldObjectStates.Building);
            unit.SetSquadAttached(squad);
            unit.Team = squad.Team;
            unit.SetPlayer(squad._Player);
            squad._Squad.Add(unit);
            squad._SquadCurrentSize = i;

            // Update unit build time to match the squad build time
            unit.BuildingTime = BuildingTime;

            // Creating the first unit at the center
            if (i == 0) { unit.transform.position = squad.transform.position; ; }
            else
            {

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
    public void SpawnUnits(Squad squad)
    {

        // Loop for each unit
        for (int i = 0; i < SquadMaxSize; i++)
        {

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
            else
            {

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
    public Vector3 FindCenterOfAllUnits()
    {

        // No units in squad, so gameObject's position is technically its current position anyway
        if (_Squad.Count == 0) { return gameObject.transform.position; }

        // Just return the position of the last alive unit
        else if (_Squad.Count == 1) { return _Squad[0].gameObject.transform.position; }

        // Squad size is greater than 1
        else
        {

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
    private List<Vector3> GetPositionsWithinFlockingBoundsOfPoint(Vector3 point, int size)
    {

        // Loop for each unit
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < size; i++)
        {

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
    private List<Vector3> GetAttackingPositionsAtObject(WorldObject worldObject, int size)
    {

        float facingAngle = Vector3.Angle(worldObject.transform.forward, worldObject.transform.position - transform.position);
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < size; i++)
        {

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
    public void SquadSeek(Vector3 seekTarget, bool overwrite = false)
    {

        SquadCalculatePath(seekTarget);

        // Get all alive units to seek to the squad seek target
        //_UpdateBoids = true;
        //for (int i = 0; i < _Squad.Count; i++)
        //{
        //    _Squad[i].HasReachedTarget = true;
        //}

        // Create waypoint
        if (_SeekWaypoint == null) { _SeekWaypoint = ObjectPooling.Spawn(GameManager.Instance.AgentSeekObject, Vector3.zero, Quaternion.identity); }
        if (_SeekWaypoint != null)
        {

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

        // Set seek target and calculate a path to it
        SquadSeek(attackTarget.transform.position, overwrite);

        //StartCoroutine(AttackPathComplete(attackTarget));
    }

    IEnumerator AttackPathComplete(WorldObject attackTarget)
    {

        yield return new WaitUntil(() => _SeekPathComplete);

        // Get positions with an offset for each unit to seek towards
        List<Vector3> positions = GetAttackingPositionsAtObject(attackTarget, _Squad.Count);
        for (int i = 0; i < _Squad.Count; i++)
        {

            // Get all alive units to attack the object (while positioning ourselves)
            foreach (var unit in _Squad)
            {

                unit.AgentAttackObject(attackTarget, positions[i], false);
                i++;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnitFromSquad(Unit unit)
    {

        GameObject g = this.gameObject;

        // Loop through current squad
        for (int i = 0; i < unit.GetSquadAttached().GetSquadMembers().Count; i++)
        {

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