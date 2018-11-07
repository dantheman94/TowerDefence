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
    public Building AttachedBuilding = null;
    [Space]
    public bool ObjectsCreatedAreQueued = true;
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

    private List<Abstraction> _AbstractionBuildings = null;

    private MeshRenderer _Renderer = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    protected override void Start() {
        base.Start();

        // Get component references
        _Renderer = GetComponent<MeshRenderer>();
    }

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
        
        // Hide building slot render if theres a building on it (but still keep it enabled)
        if (_Renderer != null) { _Renderer.enabled = _BuildingOnSlot == null; }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is "clicked on" and the selection wheel appears.
    /// </summary>
    public override void OnSelectionWheel() {
        base.OnSelectionWheel();

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

                // Get selection wheel reference
                SelectionWheel selectionWheel = null;
                if (GameManager.Instance._IsRadialMenu) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
                else { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }
                selectionWheel.SetDefaultAbstraction(this);
                selectionWheel.HideItemPurchaseInfoPanel();

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
    public override void DrawSelection(bool draw) {
        
        // Select the building slot if its empty - otherwise select the building on it instead
        if (_BuildingOnSlot == null) { base.DrawSelection(draw); }
        else { _BuildingOnSlot.DrawSelection(draw); }

        // Show selection
        if (draw) {

            if (!_Player._HUD.SelectionWheel.transform.parent.gameObject.activeSelf) {

                OnSelectionWheel();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="highlight"></param>
    public override void DrawHighlight(bool highlight) {
                
        // Show white outline
        if (_OutlineComponent != null) {

            // Change the outline colour to match the player's colour
            if (highlight) {

                Color col = new Color();
                col = _Player.TeamColor;

                // Slightly darker shade for the highlighting colour
                col.r -= 0.1f;
                col.g -= 0.1f;
                col.b -= 0.1f;
                col.a /= 2f;

                _HighlightingOutlineColour = col;
            }

            // By default, the outline is white on all building slots
            else { _HighlightingOutlineColour = Color.white; }

            // Set outline properties
            _OutlineComponent.OutlineColor = _HighlightingOutlineColour;
            _OutlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
            _OutlineComponent.OutlineWidth = OutlineHighlightedWidth;
            _OutlineComponent.enabled = true;
        }

        // Hide selection
        else { if (_OutlineComponent != null && !_IsCurrentlySelected) { _OutlineComponent.enabled = false; } }
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
        foreach (var obj in _Player.SelectedUnits) { obj.SetIsSelected(false); }
        _Player.SelectedUnits.Clear();

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

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<Abstraction> GetBuildingsAsAbstraction() {

        if (_AbstractionBuildings != null && _AbstractionBuildings.Count > 0) { return _AbstractionBuildings; }
        else if (_AbstractionBuildings == null) {

            _AbstractionBuildings = new List<Abstraction>();
            for (int i = 0; i < Buildings.Count; i++) {

                _AbstractionBuildings.Add(Buildings[i]);
            }
        }
        return _AbstractionBuildings;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}