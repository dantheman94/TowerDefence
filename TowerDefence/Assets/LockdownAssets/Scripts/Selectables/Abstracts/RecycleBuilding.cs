using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/15/2018
//
//******************************

public class RecycleBuilding : Abstraction {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Building _BuildingToRecycle = null;
    private bool _ToBeDestroyed = false;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object
    //  linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        // Update building reference
        _BuildingToRecycle = buildingSlot.GetBuildingOnSlot();

        // Valid building check
        if (_BuildingToRecycle != null) {

            _BuildingToRecycle.SetIsSelected(false);

            // Remove the building from any queues it is currently in
            if (buildingSlot.AttachedBase != null)          { buildingSlot.AttachedBase.RemoveFromQueue(_BuildingToRecycle); }
            if (buildingSlot.GetBuildingOnSlot() != null)   { buildingSlot.GetBuildingOnSlot().RemoveFromQueue(_BuildingToRecycle); }

            // Recycle building
            _BuildingToRecycle.RecycleBuilding();

            // Free resources by destroying this instance (if needed)
            if (_ToBeDestroyed) { Destroy(this.gameObject); }
        }
        else { Debug.Log("_BuildingToRecycle == null"); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="building"></param>
    public void SetBuildingToRecycle(Building building) { _BuildingToRecycle = building; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetToBeDestroyed(bool value) { _ToBeDestroyed = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}