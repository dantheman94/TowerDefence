using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/5/2018
//
//******************************

public class Building : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUILDABLES ")]
    [Space]
    public List<Abstraction> Selectables;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************



    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    public void OnSelectionWheel() {

        // Show building slot wheel
        if (_Player && Selectables.Count > 0) {

            // Update list then display on screen
            ///_Player._HUD.SelectionWheel.setBuildingSlotInFocus(BuildingSlot);
            _Player._HUD.SelectionWheel.UpdateListWithBuildables(Selectables);

            // Show selection wheel
            GameManager.Instance.SelectionWheel.SetActive(true);

            _IsCurrentlySelected = true;
        }
    }

    public override void OnWheelSelect(BuildingSlot buildingSlot) { base.OnWheelSelect(buildingSlot);
        
        // Update building slot ref with building
        buildingSlot.setBuildingOnSlot(this.GetComponent<Building>());
    }

    //******************************************************************************************************************************
    //
    //      EVENTS
    //
    //******************************************************************************************************************************

    //**************************************
    //      LIGHT BARRACKS
    //**************************************

    public void CreateLightInfantrySquad() {
                
    }

    public void CreateLightAntiInfantrySquad() {
        
    }

    public void CreateLightSniperUnit() {
        
    }

    //**************************************
    //      HEAVY BARRACKS
    //**************************************

    public void CreateHeavyGrenadierSquad() {

    }

    public void CreateHeavyAntiAirSquad() {

    }

    public void CreateHeroUnit() {

    }

    //**************************************
    //      GARAGE
    //**************************************


    //**************************************
    //      AIRPAD
    //**************************************


    //**************************************
    //      SUPPLY PAD
    //**************************************


    //**************************************
    //      POWER GENERATOR
    //**************************************


    //**************************************
    //      MINIBASE
    //**************************************


    //**************************************
    //      OUTPOST
    //**************************************


    //**************************************
    //      HEADQUARTERS
    //**************************************


    //**************************************
    //      COMMAND CENTER
    //**************************************


    //**************************************
    //      BASE TURRET
    //**************************************


    //**************************************
    //      WATCH TOWER
    //**************************************

}