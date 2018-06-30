using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 27/6/2018
//
//******************************

public class Unit : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UNIT PROPERTIES")]
    [Space]
    public float MovementSpeed = 10f;
    public float AttackingRange = 100f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" UNIT SENSES")]
    [Space]
    public float SightRange = 200f;
    public float HearingRange = 200f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected NavMeshAgent _Agent = null;
    public Squad _SquadAttached = null;
    protected bool _IsSeeking = false;
    protected GameObject _SeekWaypoint = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    //  Called before Start().
    /// </summary>
    protected override void Awake() {
        base.Awake();

        // Set recycle amount to the same as the cost amount
        RecycleSupplies = CostSupplies;
        RecyclePower = CostPower;

        // Get component references
        _Agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Force the unit to skip the deployable state and go straight to being active in the world
        if (_ObjectState == WorldObjectStates.Deployable) { _ObjectState = WorldObjectStates.Active; }

        // Update agent seeking status
        _IsSeeking = _Agent.remainingDistance > 1f;

        // Update seeking waypoint visibility
        if (_SeekWaypoint && _IsCurrentlySelected) {

            if (_IsSeeking) { _SeekWaypoint.SetActive(true); }
            else            { _SeekWaypoint.SetActive(false); }
        }
    }

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {
        base.OnWheelSelect(buildingSlot);

        List<Vector3> offsets = new List<Vector3>();

        // Get reference to the newly cloned unit
        if (_ClonedWorldObject != null) {

            // Set position to be at the spawn vector while it is building (it should be hidden until its deployed)
            if (buildingSlot.AttachedBase != null) {

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.AttachedBase.UnitSpawnTransform.transform.position;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.AttachedBase.UnitSpawnTransform.transform.rotation;

            }
            else {

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.transform.position + buildingSlot.transform.forward * 50.0f;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.transform.rotation;
            }

            // Add to list of AI
            _Player.AddToPopulation(_ClonedWorldObject as Unit);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="seekTarget"></param>
    public void AgentSeekPosition(Vector3 seekTarget) {

        // Set agent's new goto target
        _Agent.destination = seekTarget;
        _Agent.speed = MovementSpeed;

        // Show seeking waypoint
        if (_SeekWaypoint == null) { _SeekWaypoint = Instantiate(GameManager.Instance.AgentSeekObject); }
        if (_SeekWaypoint != null) { 

            // Display waypoint if not already being displayed
            if (_SeekWaypoint.activeInHierarchy != true) { _SeekWaypoint.SetActive(true); }

            // Update waypoint position
            _SeekWaypoint.transform.position = seekTarget;
        }
        _IsSeeking = true;
    }

    public void AgentAttackObject(WorldObject attackTarget) {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public NavMeshAgent GetAgent() { return _Agent; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsInASquad() { return _SquadAttached != null; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="squad"></param>
    public void SetSquadAttached(Squad squad) { _SquadAttached = squad; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Squad GetSquadAttached() { return _SquadAttached; }

}