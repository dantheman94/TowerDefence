using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class Tower : GroundVehicle {

    //***************************************************************
    // VARIABLES

    public enum ETowerType { Turret, Watchtower, Barracks }

    //***************************************************************
    // FUNCTIONS

    protected override void Start () {

        _VehicleType = EVehicleType.StationaryTower;
        _ControllerType = EControllerType.AiControlled;
	}

    protected override void Update () {
		
	}

}