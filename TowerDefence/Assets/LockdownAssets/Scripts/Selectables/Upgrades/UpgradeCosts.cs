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

public class UpgradeCosts : Abstraction {

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

}