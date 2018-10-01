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

            // Not holding LEFT CONTROL and LEFT SHIFT
            if (!Input.GetKey(KeyCode.LeftControl)) {

                if (!Input.GetKey(KeyCode.LeftShift)) {

                    // Deselect any objects that are currently selected
                    foreach (var obj in _Player.SelectedWorldObjects) { obj.SetIsSelected(false); }
                    _Player.SelectedWorldObjects.Clear();
                    foreach (var obj in _Player.SelectedUnits) { obj.SetIsSelected(false); }
                    _Player.SelectedUnits.Clear();

                    if (_Player.SelectedBuildingSlot != null) {

                        _Player.SelectedBuildingSlot.SetIsSelected(false);
                        _Player.SelectedBuildingSlot = null;
                    }
                }
            }

            // Cast hit object to selectable objects
            Base baseObj = null;
            Building buildingObj = null;
            BuildingSlot buildingSlot = null;
            Unit unit = null;
            WorldObject worldObj = null;

            baseObj = other.gameObject.GetComponentInParent<Base>();
            buildingSlot = other.gameObject.GetComponent<BuildingSlot>();
            worldObj = other.gameObject.GetComponentInParent<WorldObject>();

            // Left clicking on something attached to a base
            if (baseObj != null) {

                buildingObj = other.gameObject.GetComponent<Building>();

                // Left clicking on a base
                if (buildingObj == null && buildingSlot == null) {

                    // Matching team
                    if (baseObj.Team == _Player.Team) {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(baseObj);
                        baseObj.SetPlayer(_Player);
                        baseObj.SetIsSelected(true);
                        baseObj.OnSelectionWheel();
                    }
                }

                // Left clicking on a building
                if (buildingObj != null) {

                    if (buildingSlot == null) {

                        // Matching team
                        if (buildingObj.Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingObj);
                            buildingObj.SetPlayer(_Player);
                            buildingObj.SetIsSelected(true);
                            buildingObj.OnSelectionWheel();
                        }
                    }
                }

                // Left clicking on a building slot
                if (buildingSlot != null) {

                    // Empty building slot
                    if (buildingSlot.GetBuildingOnSlot() == null) {

                        // Matching team
                        if (buildingSlot.AttachedBase.Team == _Player.Team) {

                            _Player.SelectedBuildingSlot = buildingSlot;
                            buildingSlot.SetPlayer(_Player);
                            buildingSlot.SetIsSelected(true);
                        }
                    }

                    // Builded slot
                    else {

                        // Matching team
                        if (buildingSlot.GetBuildingOnSlot().Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingSlot.GetBuildingOnSlot());
                            buildingSlot.GetBuildingOnSlot().SetPlayer(_Player);
                            buildingSlot.GetBuildingOnSlot().SetIsSelected(true);
                            buildingSlot.GetBuildingOnSlot().OnSelectionWheel();
                        }
                    }
                }
            }

            // Left clicking on something NOT attached to a base
            else {

                buildingObj = other.gameObject.GetComponentInParent<Building>();

                // Left clicking on a building
                if (buildingObj != null) {

                    if (baseObj == null && buildingSlot == null) {

                        // Matching team
                        if (buildingObj.Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingObj);
                            buildingObj.SetPlayer(_Player);
                            buildingObj.SetIsSelected(true);
                            buildingObj.OnSelectionWheel();
                        }
                    }
                }

                // Hit an AI object?
                unit = other.gameObject.GetComponentInParent<Unit>();

                // Left clicking on a unit
                if (unit != null) {

                    // Unit is active in the world
                    if (unit.GetObjectState() == Abstraction.WorldObjectStates.Active) {

                        // Matching team
                        if (unit.Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(unit);
                            _Player.SelectedUnits.Add(unit);
                            unit.SetPlayer(_Player);
                            unit.SetIsSelected(true);
                        }
                    }
                }

                // Left clicking on a world object
                if (worldObj != null) {

                    if (buildingSlot == null && buildingObj == null && baseObj == null && unit == null) {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(worldObj);
                        worldObj.SetPlayer(_Player);
                        worldObj.SetIsSelected(true);
                    }
                }

                // Left clicking on a building slot
                if (buildingSlot != null) {

                    // Empty building slot
                    if (buildingSlot.GetBuildingOnSlot() == null) {

                        _Player.SelectedBuildingSlot = buildingSlot;
                        buildingSlot.SetPlayer(_Player);
                        buildingSlot.SetIsSelected(true);
                    }

                    // Builded slot
                    else {

                        // Matching team
                        if (buildingSlot.GetBuildingOnSlot().Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingSlot.GetBuildingOnSlot());
                            buildingSlot.GetBuildingOnSlot().SetPlayer(_Player);
                            buildingSlot.GetBuildingOnSlot().SetIsSelected(true);
                            buildingSlot.GetBuildingOnSlot().OnSelectionWheel();
                        }
                    }
                }
            }

        }
        
        // Just clicked on the ground so deselect all objects
        else { _Player.DeselectAllObjects(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other) {

        if (other.gameObject.tag != "Ground") {

            // Not holding LEFT CONTROL and LEFT SHIFT
            if (!Input.GetKey(KeyCode.LeftControl)) {

                if (!Input.GetKey(KeyCode.LeftShift)) {

                    // Deselect any objects that are currently selected
                    foreach (var obj in _Player.SelectedWorldObjects) { obj.SetIsSelected(false); }
                    _Player.SelectedWorldObjects.Clear();
                    foreach (var obj in _Player.SelectedUnits) { obj.SetIsSelected(false); }
                    _Player.SelectedUnits.Clear();

                    if (_Player.SelectedBuildingSlot != null) {

                        _Player.SelectedBuildingSlot.SetIsSelected(false);
                        _Player.SelectedBuildingSlot = null;
                    }
                }
            }

            // Cast hit object to selectable objects
            Base baseObj = null;
            Building buildingObj = null;
            BuildingSlot buildingSlot = null;
            Unit unit = null;
            WorldObject worldObj = null;

            baseObj = other.gameObject.GetComponentInParent<Base>();
            buildingSlot = other.gameObject.GetComponent<BuildingSlot>();
            worldObj = other.gameObject.GetComponentInParent<WorldObject>();

            // Left clicking on something attached to a base
            if (baseObj != null) {

                buildingObj = other.gameObject.GetComponent<Building>();

                // Left clicking on a base
                if (buildingObj == null && buildingSlot == null) {

                    // Matching team
                    if (baseObj.Team == _Player.Team) {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(baseObj);
                        baseObj.SetPlayer(_Player);
                        baseObj.SetIsSelected(true);
                        baseObj.OnSelectionWheel();
                    }
                }

                // Left clicking on a building
                if (buildingObj != null) {

                    if (buildingSlot == null) {

                        // Matching team
                        if (buildingObj.Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingObj);
                            buildingObj.SetPlayer(_Player);
                            buildingObj.SetIsSelected(true);
                            buildingObj.OnSelectionWheel();
                        }
                    }
                }

                // Left clicking on a building slot
                if (buildingSlot != null) {

                    // Empty building slot
                    if (buildingSlot.GetBuildingOnSlot() == null) {

                        // Matching team
                        if (buildingSlot.AttachedBase.Team == _Player.Team) {

                            _Player.SelectedBuildingSlot = buildingSlot;
                            buildingSlot.SetPlayer(_Player);
                            buildingSlot.SetIsSelected(true);
                        }
                    }

                    // Builded slot
                    else {

                        // Matching team
                        if (buildingSlot.GetBuildingOnSlot().Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingSlot.GetBuildingOnSlot());
                            buildingSlot.GetBuildingOnSlot().SetPlayer(_Player);
                            buildingSlot.GetBuildingOnSlot().SetIsSelected(true);
                            buildingSlot.GetBuildingOnSlot().OnSelectionWheel();
                        }
                    }
                }
            }

            // Left clicking on something NOT attached to a base
            else {

                buildingObj = other.gameObject.GetComponentInParent<Building>();

                // Left clicking on a building
                if (buildingObj != null) {

                    if (baseObj == null && buildingSlot == null) {

                        // Matching team
                        if (buildingObj.Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingObj);
                            buildingObj.SetPlayer(_Player);
                            buildingObj.SetIsSelected(true);
                            buildingObj.OnSelectionWheel();
                        }
                    }
                }

                // Hit an AI object?
                unit = other.gameObject.GetComponentInParent<Unit>();

                // Left clicking on a unit
                if (unit != null) {

                    // Unit is active in the world
                    if (unit.GetObjectState() == Abstraction.WorldObjectStates.Active) {

                        // Matching team
                        if (unit.Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(unit);
                            _Player.SelectedUnits.Add(unit);
                            unit.SetPlayer(_Player);
                            unit.SetIsSelected(true);
                        }
                    }
                }

                // Left clicking on a world object
                if (worldObj != null) {

                    if (buildingSlot == null && buildingObj == null && baseObj == null && unit == null) {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(worldObj);
                        worldObj.SetPlayer(_Player);
                        worldObj.SetIsSelected(true);
                    }
                }

                // Left clicking on a building slot
                if (buildingSlot != null) {

                    // Empty building slot
                    if (buildingSlot.GetBuildingOnSlot() == null) {

                        _Player.SelectedBuildingSlot = buildingSlot;
                        buildingSlot.SetPlayer(_Player);
                        buildingSlot.SetIsSelected(true);
                    }

                    // Builded slot
                    else {

                        // Matching team
                        if (buildingSlot.GetBuildingOnSlot().Team == _Player.Team) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingSlot.GetBuildingOnSlot());
                            buildingSlot.GetBuildingOnSlot().SetPlayer(_Player);
                            buildingSlot.GetBuildingOnSlot().SetIsSelected(true);
                            buildingSlot.GetBuildingOnSlot().OnSelectionWheel();
                        }
                    }
                }
            }
        }

        // Just clicked on the ground so deselect all objects
        else { _Player.DeselectAllObjects(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}