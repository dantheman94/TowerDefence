using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeCosts : Abstraction {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    
    [Space]
    [Header("-----------------------------------")]
    [Header("LEVEL")]
    public int UpgradeLevel;

    [Space]
    [Header("-----------------------------------")]
    [Header("COSTS")]
    public int PlayerLevel = 1;
    public int SupplyCost;
    public int PowerCost;

}