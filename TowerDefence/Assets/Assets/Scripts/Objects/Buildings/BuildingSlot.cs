using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/10/2018
//
//******************************

public class BuildingSlot : Selectable {

    //******************************************************************************************************************************
    // INSPECTOR

    public List<Building> _Buildings;

    //******************************************************************************************************************************
    // VARIABES

    //******************************************************************************************************************************
    // FUNCTIONS
    
    protected void DrawSelectionWheel() {

        // Show building slot wheel
        if (_Player) {

            // Cast object
            List<Selectable> selectables = new List<Selectable>();
            foreach (var item in _Buildings) { selectables.Add(item); }

            // Update list then display on screen
            _Player._HUD.SelectionWheel.UpdateList(selectables);
            _Player._HUD.SelectionWheel.gameObject.SetActive(true);

            _IsCurrentlySelected = true;
        }
    }

    protected override void DrawSelection(bool draw) { base.DrawSelection(draw);

        // Show selection
        if (draw) {

            if (!_Player._HUD.SelectionWheel.gameObject.activeSelf) {

                DrawSelectionWheel();
            }
        }

        // Hide selection
        else { if (_Player) { _Player._HUD.SelectionWheel.gameObject.SetActive(false); } }
    }

    protected override void ChangeSelection(Selectable selectObj) { base.ChangeSelection(selectObj);

        // This should be called by the following line, but there is an outside chance it will not
        SetSelection(false);

        // Clear the world objects selection list
        foreach (var obj in _Player.SelectedWorldObjects) { obj.SetSelection(false); }
        _Player.SelectedWorldObjects.Clear();

        // Clear the selected building slot (if theres one already been selected)
        if (_Player.SelectedBuildingSlot)
            _Player.SelectedBuildingSlot.SetSelection(false);

        // Update new selection with the building slot
        _Player.SelectedBuildingSlot = this;
        selectObj.SetSelection(true);
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint) { base.MouseClick(hitObject, hitPoint);

        // Only handle input if currently selected
        if (_IsCurrentlySelected && hitObject && hitObject.name != "Ground") {

            // Cast hit object to world object
            BuildingSlot buildingObj = hitObject.transform.root.GetComponent<BuildingSlot>();

            if (buildingObj)
                ChangeSelection(buildingObj);
        }
    }
}