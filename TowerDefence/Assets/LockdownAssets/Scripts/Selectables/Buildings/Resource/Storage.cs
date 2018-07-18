using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//**************************
//
// Created by: Angus Secomb
//
// Last edited by: Angus Secomb
// Last edited on: 24/5/2018
//
//**************************

public class Storage : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" STORAGE PROPERTIES")]
    [Space]
    public int StartingPowerStorageIncrease = 1500;
    public int StartingSupplyStorageIncrease = 1500;

    //******************************************************************************************************************************
    //
    //      Variables
    //
    //******************************************************************************************************************************
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  This is called before Startup().
    /// </summary>
    protected override void Awake()
    {


    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object's state switches to active (Only once).
    /// </summary>
    protected override void OnBuilt() {
        base.OnBuilt();

        AddToCap(StartingSupplyStorageIncrease, StartingPowerStorageIncrease);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds the specified amounts to the attached player's resource counts.
    /// </summary>
    /// <param name="supplies"></param>
    /// <param name="power"></param>
    public void AddToCap(int supplies, int power) {

        if (_Player) {

            // Add to max cap
            _Player.MaxSupplyCount += supplies;
            _Player.MaxPowerCount += power;
        }
    }

}