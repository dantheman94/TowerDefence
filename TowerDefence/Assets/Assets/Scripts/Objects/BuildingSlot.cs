﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/5/2018
//
//******************************

public class BuildingSlot : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUILDING SLOT PROPERTIES")]
    [Space]
    public Base AttachedBase = null;
    [Space]
    public List<Building> Buildings;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    ///[HideInInspector]
    public Building _BuildingOnSlot = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    //  
    /// </summary>
    protected override void DrawSelectionWheel() {
        base.DrawSelectionWheel();

        // Show building slot wheel
        if (_Player) {

            // There ISNT a building on the slot
            if (_BuildingOnSlot == null) {

                // Cast object
                List<Abstraction> selectables = new List<Abstraction>();
                foreach (var item in Buildings) { selectables.Add(item); }

                // Update list on the selection wheel
                _Player._HUD.SelectionWheel.UpdateListWithBuildings(selectables, this);

                // Reset selection wheel highlight
                _Player._HUD.SelectionWheel.DetailedHighlightTitle.text = "";
                _Player._HUD.SelectionWheel.DetailedHighlightDescription.text = "";

                // Show selection wheel
                GameManager.Instance.SelectionWheel.SetActive(true);

                _IsCurrentlySelected = true;
            }

            // There IS already a building on the slot
            else { _BuildingOnSlot.OnSelectionWheel(); }
        }
    }

    /// <summary>
    //  
    /// </summary>
    /// <param name="draw"></param>
    protected override void DrawSelection(bool draw) {
        base.DrawSelection(draw);

        // Show selection
        if (draw) {

            if (!_Player._HUD.SelectionWheel.transform.parent.gameObject.activeSelf) {

                DrawSelectionWheel();
            }
        }
    }

    /// <summary>
    //  
    /// </summary>
    /// <param name="selectObj"></param>
    protected override void ChangeSelection(Selectable selectObj) {
        base.ChangeSelection(selectObj);

        // This should be called by the following line, but there is an outside chance it will not
        SetSelection(false);

        // Clear the world objects selection list
        foreach (var obj in _Player.SelectedWorldObjects) { obj.SetSelection(false); }
        _Player.SelectedWorldObjects.Clear();

        // Clear the selected building slot (if theres one already been selected)
        if (_Player.SelectedBuildingSlot) { _Player.SelectedBuildingSlot.SetSelection(false); }

        // Update new selection with the building slot
        _Player.SelectedBuildingSlot = this;
        selectObj.SetSelection(true);
    }

    /// <summary>
    //  
    /// </summary>
    /// <param name="hitObject"></param>
    /// <param name="hitPoint"></param>
    public override void MouseClick(GameObject hitObject, Vector3 hitPoint) {
        base.MouseClick(hitObject, hitPoint);

        // Only handle input if currently selected
        if (_IsCurrentlySelected && hitObject && hitObject.name != "Ground") {

            // Cast hit object to world object
            BuildingSlot buildingObj = hitObject.transform.root.GetComponent<BuildingSlot>();

            // Change selection
            if (buildingObj) { ChangeSelection(buildingObj); }
        }
    }

}