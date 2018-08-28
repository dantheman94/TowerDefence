using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 27/08/2018
//
//******************************


public class SphereSelection : MonoBehaviour
{
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player;

    // Use this for initialization
    void Start()
    {
        _Player = GameManager.Instance.Players[0];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Ground" && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            // Deselect any objects that are currently selected
            foreach (var obj in _Player.SelectedWorldObjects) { obj.SetIsSelected(false); }
            _Player.SelectedWorldObjects.Clear();

            if (_Player.SelectedBuildingSlot != null)
            {

                _Player.SelectedBuildingSlot.SetIsSelected(false);
                _Player.SelectedBuildingSlot = null;
            }


            // Cast hit object to selectable objects
            Base baseObj = null;
            Building buildingObj = null;
            BuildingSlot buildingSlot = null;

            WorldObject worldObj = null;
            Squad squadObj = null;
            Unit unitObj = null;

            baseObj = other.gameObject.GetComponentInParent<Base>();
            buildingSlot = other.gameObject.GetComponent<BuildingSlot>();
            worldObj = other.gameObject.GetComponentInParent<WorldObject>();
            // Left clicking on something attached to a base
            if (baseObj != null)
            {

                buildingObj = other.gameObject.GetComponent<Building>();

                // Left clicking on a base
                if (buildingObj == null && buildingSlot == null)
                {

                    // Matching team
                    if (baseObj.Team == _Player.Team)
                    {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(baseObj);
                        baseObj.SetPlayer(_Player);
                        baseObj.SetIsSelected(true);
                        baseObj.OnSelectionWheel();
                    }
                }

                // Left clicking on a building
                if (buildingObj != null)
                {

                    if (buildingSlot == null)
                    {

                        // Matching team
                        if (buildingSlot.Team == _Player.Team)
                        {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingObj);
                            buildingObj.SetPlayer(_Player);
                            buildingObj.SetIsSelected(true);
                            buildingObj.OnSelectionWheel();
                        }
                    }
                }

                // Left clicking on a building slot
                if (buildingSlot != null)
                {

                    // Empty building slot
                    if (buildingSlot.GetBuildingOnSlot() == null)
                    {

                        // Matching team
                        if (buildingSlot.AttachedBase.Team == _Player.Team)
                        {

                            _Player.SelectedBuildingSlot = buildingSlot;
                            buildingSlot.SetPlayer(_Player);
                            buildingSlot.SetIsSelected(true);
                        }
                    }

                    // Builded slot
                    else
                    {

                        // Matching team
                        if (buildingSlot.GetBuildingOnSlot().Team == _Player.Team)
                        {

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
            else
            {

                buildingObj = other.gameObject.GetComponentInParent<Building>();

                // Left clicking on a building
                if (buildingObj != null)
                {

                    if (baseObj == null && buildingSlot == null)
                    {

                        // Matching team
                        if (buildingObj.Team == _Player.Team)
                        {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingObj);
                            buildingObj.SetPlayer(_Player);
                            buildingObj.SetIsSelected(true);
                            buildingObj.OnSelectionWheel();
                        }
                    }
                }

                // Hit an AI object?
                squadObj = other.gameObject.GetComponent<Squad>();
                unitObj = other.gameObject.GetComponentInParent<Unit>();

                // Left clicking on a squad
                if (squadObj != null)
                {

                    // Squad is active in the world
                    if (squadObj.GetObjectState() == WorldObject.WorldObjectStates.Active)
                    {

                        // Matching team
                        if (squadObj.Team == _Player.Team)
                        {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(squadObj);
                            squadObj.SetPlayer(_Player);
                            squadObj.SetIsSelected(true);
                        }
                    }
                }

                // Left clicking on a unit
                if (unitObj != null)
                {

                    // Is the unit part of a squad?
                    if (unitObj.IsInASquad())
                    {

                        squadObj = unitObj.GetSquadAttached();

                        // Squad is active in the world
                        if (squadObj.GetObjectState() == WorldObject.WorldObjectStates.Active)
                        {

                            // Matching team
                            if (squadObj.Team == _Player.Team)
                            {

                                // Add selection to list
                                _Player.SelectedWorldObjects.Add(squadObj);
                                squadObj.SetPlayer(_Player);
                                squadObj.SetIsSelected(true);
                            }
                        }
                    }

                    // Unit is NOT in a squad
                    else
                    {

                        // Unit is active in the world
                        if (unitObj.GetObjectState() == WorldObject.WorldObjectStates.Active)
                        {

                            // Matching team
                            if (unitObj.Team == _Player.Team)
                            {

                                // Add selection to list
                                _Player.SelectedWorldObjects.Add(unitObj);
                                unitObj.SetPlayer(_Player);
                                unitObj.SetIsSelected(true);
                            }
                        }
                    }
                }

                // Left clicking on a world object
                if (worldObj != null)
                {

                    if (buildingSlot == null && buildingObj == null && baseObj == null && unitObj == null && squadObj == null)
                    {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(worldObj);
                        worldObj.SetPlayer(_Player);
                        worldObj.SetIsSelected(true);
                    }
                }

                // Left clicking on a building slot
                if (buildingSlot != null)
                {

                    // Empty building slot
                    if (buildingSlot.GetBuildingOnSlot() == null)
                    {

                        _Player.SelectedBuildingSlot = buildingSlot;
                        buildingSlot.SetPlayer(_Player);
                        buildingSlot.SetIsSelected(true);
                    }

                    // Builded slot
                    else
                    {

                        // Matching team
                        if (buildingSlot.GetBuildingOnSlot().Team == _Player.Team)
                        {

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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "Ground")
        {
            // Deselect any objects that are currently selected
            foreach (var obj in _Player.SelectedWorldObjects) { obj.SetIsSelected(false); }
            _Player.SelectedWorldObjects.Clear();

            if (_Player.SelectedBuildingSlot != null)
            {

                _Player.SelectedBuildingSlot.SetIsSelected(false);
                _Player.SelectedBuildingSlot = null;
            }


            // Cast hit object to selectable objects
            Base baseObj = null;
            Building buildingObj = null;
            BuildingSlot buildingSlot = null;

            WorldObject worldObj = null;
            Squad squadObj = null;
            Unit unitObj = null;

            baseObj = other.gameObject.GetComponentInParent<Base>();
            buildingSlot = other.gameObject.GetComponent<BuildingSlot>();
            worldObj = other.gameObject.GetComponentInParent<WorldObject>();
            // Left clicking on something attached to a base
            if (baseObj != null)
            {

                buildingObj = other.gameObject.GetComponent<Building>();

                // Left clicking on a base
                if (buildingObj == null && buildingSlot == null)
                {

                    // Matching team
                    if (baseObj.Team == _Player.Team)
                    {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(baseObj);
                        baseObj.SetPlayer(_Player);
                        baseObj.SetIsSelected(true);
                        baseObj.OnSelectionWheel();
                    }
                }

                // Left clicking on a building
                if (buildingObj != null)
                {

                    if (buildingSlot == null)
                    {

                        // Matching team
                        if (buildingSlot.Team == _Player.Team)
                        {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingObj);
                            buildingObj.SetPlayer(_Player);
                            buildingObj.SetIsSelected(true);
                            buildingObj.OnSelectionWheel();
                        }
                    }
                }

                // Left clicking on a building slot
                if (buildingSlot != null)
                {

                    // Empty building slot
                    if (buildingSlot.GetBuildingOnSlot() == null)
                    {

                        // Matching team
                        if (buildingSlot.AttachedBase.Team == _Player.Team)
                        {

                            _Player.SelectedBuildingSlot = buildingSlot;
                            buildingSlot.SetPlayer(_Player);
                            buildingSlot.SetIsSelected(true);
                        }
                    }

                    // Builded slot
                    else
                    {

                        // Matching team
                        if (buildingSlot.GetBuildingOnSlot().Team == _Player.Team)
                        {

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
            else
            {

                buildingObj = other.gameObject.GetComponentInParent<Building>();

                // Left clicking on a building
                if (buildingObj != null)
                {

                    if (baseObj == null && buildingSlot == null)
                    {

                        // Matching team
                        if (buildingObj.Team == _Player.Team)
                        {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingObj);
                            buildingObj.SetPlayer(_Player);
                            buildingObj.SetIsSelected(true);
                            buildingObj.OnSelectionWheel();
                        }
                    }
                }

                // Hit an AI object?
                squadObj = other.gameObject.GetComponent<Squad>();
                unitObj = other.gameObject.GetComponentInParent<Unit>();

                // Left clicking on a squad
                if (squadObj != null)
                {

                    // Squad is active in the world
                    if (squadObj.GetObjectState() == WorldObject.WorldObjectStates.Active)
                    {

                        // Matching team
                        if (squadObj.Team == _Player.Team)
                        {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(squadObj);
                            squadObj.SetPlayer(_Player);
                            squadObj.SetIsSelected(true);
                        }
                    }
                }

                // Left clicking on a unit
                if (unitObj != null)
                {

                    // Is the unit part of a squad?
                    if (unitObj.IsInASquad())
                    {

                        squadObj = unitObj.GetSquadAttached();

                        // Squad is active in the world
                        if (squadObj.GetObjectState() == WorldObject.WorldObjectStates.Active)
                        {

                            // Matching team
                            if (squadObj.Team == _Player.Team)
                            {

                                // Add selection to list
                                _Player.SelectedWorldObjects.Add(squadObj);
                                squadObj.SetPlayer(_Player);
                                squadObj.SetIsSelected(true);
                            }
                        }
                    }

                    // Unit is NOT in a squad
                    else
                    {

                        // Unit is active in the world
                        if (unitObj.GetObjectState() == WorldObject.WorldObjectStates.Active)
                        {

                            // Matching team
                            if (unitObj.Team == _Player.Team)
                            {

                                // Add selection to list
                                _Player.SelectedWorldObjects.Add(unitObj);
                                unitObj.SetPlayer(_Player);
                                unitObj.SetIsSelected(true);
                            }
                        }
                    }
                }

                // Left clicking on a world object
                if (worldObj != null)
                {

                    if (buildingSlot == null && buildingObj == null && baseObj == null && unitObj == null && squadObj == null)
                    {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(worldObj);
                        worldObj.SetPlayer(_Player);
                        worldObj.SetIsSelected(true);
                    }
                }

                // Left clicking on a building slot
                if (buildingSlot != null)
                {

                    // Empty building slot
                    if (buildingSlot.GetBuildingOnSlot() == null)
                    {

                        _Player.SelectedBuildingSlot = buildingSlot;
                        buildingSlot.SetPlayer(_Player);
                        buildingSlot.SetIsSelected(true);
                    }

                    // Builded slot
                    else
                    {

                        // Matching team
                        if (buildingSlot.GetBuildingOnSlot().Team == _Player.Team)
                        {

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
}
