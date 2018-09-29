using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 29/8/2018
//
//******************************

public class UpgradeManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UPGRADE MANAGER PROPERTIES")]
    [Space]
    public GameManager.Team _Team = GameManager.Team.Undefined;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    public float _BuildingSpeedMultiplier { get; set; }
    public float _UnitBuildingSpeedMultiplier { get; set; }

    public float _GroundInfantryDamageMultiplier { get; set; }
    public float _GroundInfantryHealthMultiplier { get; set; }
    public float _GroundVehiclesDamageMultiplier { get; set; }
    public float _GroundVehiclesHealthMultiplier { get; set; }
    public float _AirVehiclesDamageMultiplier { get; set; }
    public float _AirVehiclesHealthMultiplier { get; set; }

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this gameObject is created.
    /// </summary>
    private void Start() {
        
        // Initialize defaults
        _BuildingSpeedMultiplier = 1f;
        _UnitBuildingSpeedMultiplier = 1f;

        _GroundInfantryDamageMultiplier = 1f;
        _GroundInfantryHealthMultiplier = 1f;
        _GroundVehiclesDamageMultiplier = 1f;
        _GroundVehiclesHealthMultiplier = 1f;
        _AirVehiclesDamageMultiplier = 1f;
        _AirVehiclesHealthMultiplier = 1f;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}
