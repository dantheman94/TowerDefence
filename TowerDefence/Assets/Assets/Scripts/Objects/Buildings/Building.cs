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

    protected BuildingSlot _BuildingSlot = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Force the building to skip the deployable state and go straight to being active in the world
        if (_ObjectState == WorldObjectStates.Deployable) { _ObjectState = WorldObjectStates.Active; }
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnSelectionWheel() {

        // Only if the building is active
        if (_ObjectState == WorldObjectStates.Active) {

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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buildingSlot"></param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {
        base.OnWheelSelect(buildingSlot);

        // Get reference to the newly cloned building
        if (_ClonedWorldObject != null) {

            Building building = _ClonedWorldObject.GetComponent<Building>();
            building._BuildingSlot = buildingSlot;

            // Update building slot ref with building
            buildingSlot.setBuildingOnSlot(building);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void RecycleBuilding() {

        // Add resources back to player
        Player player = GameManager.Instance.Players[0];
        player.SuppliesCount += RecycleSupplies;
        player.PowerCount += RecyclePower;

        // Destroy building
        Destroy(_BuildingSlot._BuildingOnSlot.gameObject);

        // Make building slot availiable again
        _BuildingSlot.setBuildingOnSlot(null);
    }

    //******************************************************************************************************************************
    //
    //      EVENTS
    //
    //******************************************************************************************************************************

    //**************************************
    //      LIGHT BARRACKS
    //**************************************

    /// <summary>
    /// 
    /// </summary>
    public void CreateLightInfantrySquad() {
                
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateLightAntiInfantrySquad() {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateLightSniperUnit() {
        
    }

    //**************************************
    //      HEAVY BARRACKS
    //**************************************

    /// <summary>
    /// 
    /// </summary>
    public void CreateHeavyGrenadierSquad() {

    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateHeavyAntiAirSquad() {

    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateHeroUnit() {

    }

    //**************************************
    //      GARAGE
    //**************************************
    

    //**************************************
    //      AIRPAD
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

}