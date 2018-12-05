using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 1/10/2018
//
//******************************


public class SphereSelection : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
    
    void Start() {

        _Player = GameManager.Instance.Players[0];
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag != "Ground" && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")) {
            
            // Cast hit object to selectable objects
            Base baseObj = null;
            Building buildingObj = null;
            BuildingSlot buildingSlot = null;
            Unit unit = null;
            WorldObject worldObj = null;

            baseObj = other.gameObject.GetComponentInParent<Base>();
            buildingSlot = other.gameObject.GetComponent<BuildingSlot>();
            worldObj = other.gameObject.GetComponentInParent<WorldObject>();
            
            // Hit an AI object?
            unit = other.gameObject.GetComponentInParent<Unit>();

            // Left clicking on a unit
            if (unit != null) {

                // Unit is active in the world
                if (unit.GetObjectState() == Abstraction.WorldObjectStates.Active) {

                    // Matching team
                    if (unit.Team == _Player.Team) {

                        // Add selection to list
                        if (!_Player.SelectedWorldObjects.Contains(unit)) { _Player.SelectedWorldObjects.Add(unit); }
                        if (!_Player.SelectedUnits.Contains(unit)) {

                            _Player.SelectedUnits.Add(unit);

                            // Gamepad rumble
                            _Player._XboxGamepadInputManager.StartRumble(0.35f, 0.35f, 0.2f);
                        }
                        unit.SetPlayer(_Player);
                        unit.SetIsSelected(true);
                    }
                }
            }

            // Left clicking on a world object
            if (worldObj != null) {

                if (buildingSlot == null && buildingObj == null && baseObj == null && unit == null) {

                    // Add selection to list
                    if (!_Player.SelectedWorldObjects.Contains(unit)) {

                        _Player.SelectedWorldObjects.Add(worldObj);

                        // Gamepad rumble
                        _Player._XboxGamepadInputManager.StartRumble(0.35f, 0.35f, 0.2f);
                    }
                    worldObj.SetPlayer(_Player);
                    worldObj.SetIsSelected(true);
                }
            }
        }
        
        // Just clicked on the ground so deselect all objects
        else { _Player.DeselectAllObjects(); }

        // Update units selected panels
        GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}