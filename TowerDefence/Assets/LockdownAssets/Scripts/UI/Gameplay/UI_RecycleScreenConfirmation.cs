using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/12/2018
//
//******************************

public class UI_RecycleScreenConfirmation : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" CONTROLLER PROPERTIES")]
    [Space]
    public GameObject AButton = null;
    public GameObject BButton = null;

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

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        Player plyr = GameManager.Instance.Players[0];
        if (AButton) { AButton.SetActive(plyr._XboxGamepadInputManager.IsPrimaryController); }
        if (BButton) { AButton.SetActive(plyr._XboxGamepadInputManager.IsPrimaryController); }

        if (plyr._XboxGamepadInputManager.GetButtonAClicked()) { ConfirmRecycle(); }
        if (plyr._XboxGamepadInputManager.GetButtonBClicked()) { HideWidget(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Starts the building recycle process
    /// </summary>
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
