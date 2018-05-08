using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/8/2018
//
//******************************

public class WatchTower : Tower {

    //******************************************************************************************************************************
    // INSPECTOR

    [Space]
    [Header("-----------------------------------")]
    [Header(" TOWER PROPERTIES")]
    public ETowerType TowerType = ETowerType.WatchTower;

    [Space]
    [Header("-----------------------------------")]
    [Header(" WATCHTOWER PROPERTIES")]
    public int PopulationCapacity = 8;

    //******************************************************************************************************************************
    // VARIABLES



    //******************************************************************************************************************************
    // FUNCTIONS

}
