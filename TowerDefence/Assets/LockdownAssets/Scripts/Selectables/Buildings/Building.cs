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
//  Last edited on: 23/6/2018
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
    protected BuildingRecycle _RecycleOption = null;
    private bool _RebuildNavmesh = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Force the building to skip the deployable state and go straight to being active in the world
        if (_ObjectState == WorldObjectStates.Deployable) {
            
            _ObjectState = WorldObjectStates.Active;
            _RebuildNavmesh = true;
        }
    }

    protected void LateUpdate() {

        if (_RebuildNavmesh) { 

            // Re-bake navMeshes
            ///GameManager.Instance.RebakeNavmesh();
            _RebuildNavmesh = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void OnSelectionWheel() {

        // Show the building's options if its active in the world
        if (_ObjectState == WorldObjectStates.Active) {

            // Show building slot wheel
            if (_Player && Selectables.Count > 0) {

                // Update list then display on screen
                _Player._HUD.SelectionWheel.UpdateListWithBuildables(Selectables, _BuildingSlot);

                // Show selection wheel
                GameManager.Instance.SelectionWheel.SetActive(true);
            }
            _IsCurrentlySelected = true;
        }

        // Show a wheel with recycle only as the selection (so you can cancel building the world object)
        else if (_ObjectState == WorldObjectStates.Building) {

            if (_Player) {

                List<Abstraction> wheelOptions = new List<Abstraction>();
                for (int i = 0; i < 10; i++) {

                    // Recycle option
                    if (i == 5) {

                        if (_RecycleOption == null) {

                            _RecycleOption = Instantiate(GameManager.Instance.RecycleBuilding.GetComponent<BuildingRecycle>());
                        }
                        _RecycleOption.SetBuildingToRecycle(this);
                        _RecycleOption.SetToBeDestroyed(true);
                        wheelOptions.Add(_RecycleOption);
                    }

                    // Empty option
                    else { wheelOptions.Add(null); }
                }

                // Update list then display on screen
                _Player._HUD.SelectionWheel.UpdateListWithBuildables(wheelOptions, _BuildingSlot);

                // Show selection wheel
                GameManager.Instance.SelectionWheel.SetActive(true);
            }
            _IsCurrentlySelected = true;
        }
    }

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object
    //  linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {
        base.OnWheelSelect(buildingSlot);

        // Get reference to the newly cloned building
        if (_ClonedWorldObject != null) {

            Building building = _ClonedWorldObject.GetComponent<Building>();
            building._BuildingSlot = buildingSlot;

            // Update building slot ref with building
            buildingSlot._BuildingOnSlot = building;

            // Disable building slot (is re-enabled when the building is recycled)
            buildingSlot.SetIsSelected(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void RecycleBuilding() {

        // Deselect all objects
        foreach (var selectable in GameManager.Instance.Selectables) { selectable.SetIsSelected(false); }

        // Add resources back to player
        Player player = GameManager.Instance.Players[0];
        player.SuppliesCount += RecycleSupplies;
        player.PowerCount += RecyclePower;

        // Destroy building
        if (_RecycleOption) {

            Destroy(_RecycleOption.gameObject);
            Destroy(_BuildingSlot._BuildingOnSlot.gameObject);
        }

        // Make building slot available again
        _BuildingSlot._BuildingOnSlot = null;
        _BuildingSlot.gameObject.SetActive(true);
    }
    
}