using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 24/07/2018
//
//******************************

public class SupportAirShip : AirVehicle {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header("SUPPORT AIRSHIP PROPERTIES")]
    [Space]
    public float HealingAmount = 15.0f;
    public float HealingRange = 10.0f;


}