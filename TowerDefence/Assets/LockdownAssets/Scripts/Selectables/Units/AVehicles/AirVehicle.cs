using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/7/2018
//
//******************************

public class AirVehicle : Vehicle {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" AIR-VEHICLE PROPERTIES")]
    [Space]
    protected float ForwardAvoidanceRange = 20f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Update agent height
        UpdateHeight();
    }

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void UpdateHeight() {


        // Fire a raycast forward
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, ForwardAvoidanceRange)) {

            // Move up or down?

        }

    }

}