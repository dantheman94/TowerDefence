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
    public EBarricadeType BarricadeType;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum EBarricadeType { MineField, LaserTrap, Barricade, GarrisonBarricade, HeavyBarricade, HealingBarricade }
}
