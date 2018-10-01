using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 25/7/2018
//
//******************************

public class UI_SelectedUnits : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SELECTION PANELS")]
    [Space]
    public List<UI_UnitInfoPanel> SelectionPanels;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private class UnitInfos {

        public Unit.EUnitType _UnitType;
        public int _Amount;
        public Texture2D _Logo;
    }

    private List<WorldObject> _CurrentlySelected;
    private List<UnitInfos> _UnitInfos;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    private void Start() {

        // Initialize lists
        _CurrentlySelected = new List<WorldObject>();
        _UnitInfos = new List<UnitInfos>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        for (int i = 0; i < _UnitInfos.Count; i++) {

            if (SelectionPanels.Count >= i) {

                // Update unit logo image
                ///UnitInfoPanels[i].LogoComponent.sprite = _UnitInfos[i]._Logo;

                // Update name
                ///UnitInfoPanels[i].UnitName.text = _UnitInfos[i]._UnitType.ToString();

                // Update amount selected text
                ///UnitInfoPanels[i].AmountCounter.text = _UnitInfos[i]._Amount.ToString();

                // Show the gameobject
                SelectionPanels[i].gameObject.SetActive(true);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void NewSelectionOLD(List<Selectable> selected) {

        // Clear all the panels
        for (int i = 0; i < SelectionPanels.Count; i++) { SelectionPanels[i].Wipe(); }
        _CurrentlySelected.Clear();
        _UnitInfos.Clear();

        // Cast Selectables to WorldObjects
        for (int i = 0; i < selected.Count; i++) {

            WorldObject obj = selected[i].GetComponent<WorldObject>();
            if (obj != null) { _CurrentlySelected.Add(obj); }
        }

        // Loop through selected WorldObjects
        for (int i = 0; i < _CurrentlySelected.Count; i++) {

            // As long as the WorldObject is a valid AI object
            if (GetUnitType(_CurrentlySelected[i]) != Unit.EUnitType.Undefined) {

                // First unit type
                if (_UnitInfos.Count == 0) {

                    // Add new type to known list
                    UnitInfos info = new UnitInfos {

                        _UnitType = GetUnitType(_CurrentlySelected[i]),
                        _Amount = 1,
                        _Logo = _CurrentlySelected[i].Logo
                    };
                    _UnitInfos.Add(info);
                }

                else {

                    // Loop through known unit types
                    for (int j = 0; j < _UnitInfos.Count; j++) {

                        // This unit type is already known
                        if (GetUnitType(_CurrentlySelected[i]) == _UnitInfos[j]._UnitType) {

                            // Add to unit amount
                            _UnitInfos[j]._Amount++;
                            break;
                        }

                        // Reached the end of the known unit types with no match
                        if (j + 1 == _UnitInfos.Count) {

                            // Add new type to known list
                            UnitInfos info = new UnitInfos {

                                _UnitType = GetUnitType(_CurrentlySelected[i]),
                                _Amount = 1,
                                _Logo = _CurrentlySelected[i].Logo
                            };
                            _UnitInfos.Add(info);
                        }
                    }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void NewSelection(List<Selectable> selected) {

        // Clear all the panels
        for (int i = 0; i < SelectionPanels.Count; i++) { SelectionPanels[i].Wipe(); }
        _CurrentlySelected.Clear();
        _UnitInfos.Clear();

        // Cast Selectables to WorldObjects
        for (int i = 0; i < selected.Count; i++) {

            WorldObject obj = selected[i].GetComponent<WorldObject>();
            if (obj != null) { _CurrentlySelected.Add(obj); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="worldObject"></param>
    /// <returns>
    //  Unit.UnitType
    /// </returns>
    private Unit.EUnitType GetUnitType(WorldObject worldObject) {

        // If the object is a unit
        Unit unit = worldObject.GetComponent<Unit>();
        if (unit != null) { return unit.UnitType; }
        
        // The object isn't a valid AI type
        return Unit.EUnitType.Undefined;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}