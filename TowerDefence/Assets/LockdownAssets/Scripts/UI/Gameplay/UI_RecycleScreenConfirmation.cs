using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 11/9/2018
//
//******************************

public class UI_RecycleScreenConfirmation : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Building _BuildingToRecycle = null;
    private BuildingSlot _BuildingSlotInFocus = null;
    private RecycleBuilding _RecycleBuildingClass = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ConfirmRecycle() {

        if (_BuildingToRecycle != null && _BuildingSlotInFocus != null) {
            
            _BuildingToRecycle.SetIsSelected(false);

            // Remove the building from any queues it is currently in
            if (_BuildingSlotInFocus.AttachedBase != null)          { _BuildingSlotInFocus.AttachedBase.RemoveFromQueue(_BuildingToRecycle); }
            if (_BuildingSlotInFocus.GetBuildingOnSlot() != null)   { _BuildingSlotInFocus.GetBuildingOnSlot().RemoveFromQueue(_BuildingToRecycle); }

            // Recycle building
            _BuildingToRecycle.RecycleBuilding();
            
            // Free resources by destroying this instance (if needed)
            if (_RecycleBuildingClass.GetToBeDestroyed()) { Destroy(_RecycleBuildingClass.gameObject); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void HideWidget() {

        _BuildingToRecycle = null;
        _BuildingSlotInFocus = null;
        _RecycleBuildingClass = null;
        gameObject.SetActive(false);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="building"></param>
    public void SetBuildingToRecycle(Building building) { _BuildingToRecycle = building; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="slot"></param>
    public void SetBuildingSlotInFocus(BuildingSlot slot) { _BuildingSlotInFocus = slot; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="cls"></param>
    public void SetRecycleClass(RecycleBuilding cls) { _RecycleBuildingClass = cls; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
