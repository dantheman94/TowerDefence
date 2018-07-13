using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BARRIER PROPERTIES")]
    [Space]
    public EBarricadeType BarricadeType;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum EBarricadeType { MineField, GarrisonBarricade, HeavyBarricade, HealingBarricade }

}
