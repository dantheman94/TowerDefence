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

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected NavMeshAgent _Agent = null;
    protected Squad _SquadAttached = null;

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
               
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="seekTarget"></param>
    public void AgentSeekPosition(Vector3 seekTarget) {

        // Set agent's new goto target
        _Agent.destination = seekTarget;
        _Agent.speed = MovementSpeed;
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