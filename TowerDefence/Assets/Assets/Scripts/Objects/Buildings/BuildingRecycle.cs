using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 11/6/2018
//
//******************************

public class BuildingRecycle : WorldObject {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Building _BuildingToRecycle = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="buildingSlot"></param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        // Update building reference
        _BuildingToRecycle = buildingSlot._BuildingOnSlot;

        // Valid building check
        if (_BuildingToRecycle != null) {

            // Recycle building
            _BuildingToRecycle.RecycleBuilding();
        }
    }

}