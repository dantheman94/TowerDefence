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
    //      FUNCTIONS
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

            // Hide the selection wheel
            GameManager.Instance.SelectionWheel.SetActive(false);

            // Show the confirm screen popup
            GameObject confirmScreen = GameManager.Instance.ConfirmRecycleScreen;
            if (confirmScreen != null) {

                UI_RecycleScreenConfirmation recycleScreen = confirmScreen.GetComponent<UI_RecycleScreenConfirmation>();
                if (recycleScreen != null) {

                    recycleScreen.SetBuildingSlotInFocus(buildingSlot);
                    recycleScreen.SetBuildingToRecycle(_BuildingToRecycle);
                    recycleScreen.SetRecycleClass(this);
                    recycleScreen.gameObject.SetActive(true);
                }
            }
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
    
    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetToBeDestroyed() { return _ToBeDestroyed; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}