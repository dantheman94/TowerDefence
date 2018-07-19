using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 13/7/2018
//
//******************************

public class Tower : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" TOWER PROPERTIES")]
    [Space]
    public ETowerType TowerType;
    public Weapon TowerWeapon = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum ETowerType { WatchTower, SiegeTurret, MiniTurret, AntiInfantryTurret, AntiVehicleTurret, AntiAirTurret }
    
}