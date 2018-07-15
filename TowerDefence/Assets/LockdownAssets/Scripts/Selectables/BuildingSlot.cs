using System.Collections;
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
    public bool OnlyAppearWhenAttachedToBase = false;
    public Base AttachedBase = null;
    [Space]
    public List<BuildingSlot> LinkedSlots;
    [Space]
    public List<Building> Buildings;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    private Building _BuildingOnSlot = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Display the slot if its currently attached to a base
        if (OnlyAppearWhenAttachedToBase) {

            if (AttachedBase != null)   { gameObject.SetActive(true); }
            else                        { gameObject.SetActive(false); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
                _Player._HUD.SelectionWheel.DetailedHighlightDescriptionLong.text = "";

                // Show selection wheel
                GameManager.Instance.SelectionWheel.SetActive(true);

                _IsCurrentlySelected = true;
            }

            // There IS already a building on the slot
            else { _BuildingOnSlot.OnSelectionWheel(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="selectObj"></param>
    protected override void ChangeSelection(Selectable selectObj) {
        base.ChangeSelection(selectObj);

        // This should be called by the following line, but there is an outside chance it will not
        SetIsSelected(false);

        // Clear the world objects selection list
        foreach (var obj in _Player.SelectedWorldObjects) { obj.SetIsSelected(false); }
        _Player.SelectedWorldObjects.Clear();

        // Clear the selected building slot (if theres one already been selected)
        if (_Player.SelectedBuildingSlot) { _Player.SelectedBuildingSlot.SetIsSelected(false); }

        // Update new selection with the building slot
        _Player.SelectedBuildingSlot = this;
        selectObj.SetIsSelected(true);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="newBase"></param>
    public void SetLinkedSlotsBase(Base newBase) {

        foreach (var slot in LinkedSlots) {

            slot.AttachedBase = newBase;

            // Update slot "activeness"
            if (slot.AttachedBase != null)  { slot.gameObject.SetActive(true); }
            else                            { slot.gameObject.SetActive(false); }

        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="building"></param>
    public void SetBuildingOnSlot(Building building) { _BuildingOnSlot = building; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public Building GetBuildingOnSlot() { return _BuildingOnSlot; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}