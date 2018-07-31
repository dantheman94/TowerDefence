using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 7/7/2018
//
//******************************

public class UpgradeValues : Abstraction {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" COSTS")]
    [Space]
    public int PlayerLevel = 1;
    public int SupplyCost;
    public int PowerCost;
    public int BuildTime = 5;

    [Space]
    [Header("-----------------------------------")]
    [Header(" UPGRADE")]
    [Space]
    public float UpgradeValue = 0f;

}