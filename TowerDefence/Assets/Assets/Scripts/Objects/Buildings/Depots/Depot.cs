using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/24/2018
//
//******************************

public class Depot : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" DEPOT PROPERTIES")]
    public eDepotType DepotType;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum eDepotType { Airpad, Barracks, Garage, Labratory }

    //******************************************************************************************************************************
    //
    //      EVENTS
    //
    //******************************************************************************************************************************

    /// <summary>
    //  
    /// </summary>
    public void CreateCoreInfantrySquad() {

    }

    /// <summary>
    //  
    /// </summary>
    public void CreateAntiInfantrySquad() {

    }

    /// <summary>
    //  
    /// </summary>
    public void CreateAntiVehicleUnit() {

    }
    /// <summary>
    //  
    /// </summary>
    public void CreateCoreVehicle() {

    }

    /// <summary>
    //  
    /// </summary>
    public void CreateAntiAirVehicle() {

    }

    /// <summary>
    //  
    /// </summary>
    public void CreateMobileArtillery() {

    }

    /// <summary>
    //  
    /// </summary>
    public void CreateBattleTank() {

    }

    /// <summary>
    //  
    /// </summary>
    public void CreateCoreAirship() {

    }

    /// <summary>
    //  
    /// </summary>
    public void CreateSupportShip() {

    }

    /// <summary>
    //  
    /// </summary>
    public void CreateBattleAirship() {

    }

}