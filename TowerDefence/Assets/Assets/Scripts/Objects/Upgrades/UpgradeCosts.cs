using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeCosts : MonoBehaviour {

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
    public int PlayerLevel;
    public int SupplyCost;
    public int PowerCost;

}