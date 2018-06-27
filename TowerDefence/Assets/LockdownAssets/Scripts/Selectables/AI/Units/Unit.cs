using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Force the unit to skip the deployable state and go straight to being active in the world
        if (_ObjectState == WorldObjectStates.Deployable) { _ObjectState = WorldObjectStates.Active; }
               
    }
}