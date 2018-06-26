using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/27/2018
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

}