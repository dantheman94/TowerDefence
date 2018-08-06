﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 6/8/2018
//
//******************************

public class CoreVehicle : Vehicle {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private VehicleGunner _VehicleGunnerAI = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        // Initialize vehicle gunner
        _VehicleGunnerAI = GetComponentInChildren<VehicleGunner>();
        if (_VehicleGunnerAI != null) {

            _VehicleGunnerAI.Team = Team;
            _VehicleGunnerAI.SetPlayer(_Player);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}