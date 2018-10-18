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
    [Header(" WIDGET PROPERTIES")]
    [Space]
    public UI_UnitInfoPanel StencilObject = null;
    [Space]
    public float StartingPositionX = 85f;
    public float StartingPositionY = 140f;
    public float OffsetX = 5f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public class UnitInfos {

        public Unit.EUnitType _UnitType;
        public int _Amount;
        public Sprite _Logo;
    }
    
    private Player _Player = null;
    private List<Unit> _SelectedUnits = null;

    private List<UnitInfos> _UnitInfos;
    private List<UI_UnitInfoPanel> _UnitInfoPanels = null;

    private float _PanelSize = 100f;

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
        _SelectedUnits = new List<Unit>();
        _UnitInfos = new List<UnitInfos>();
        _UnitInfoPanels = new List<UI_UnitInfoPanel>();
        if (StencilObject != null) {

            // Get panel sizing for offset
            _PanelSize = StencilObject.GetComponent<RectTransform>().rect.width;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {
        
        // Get reference to the player class
        if (_Player == null) { _Player = GameManager.Instance.Players[0]; }
        else {

            // Match the currently selected array to the players array
            _SelectedUnits = _Player.SelectedUnits;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void RefreshPanels() {

        // Reset lists
        for (int i = 0; i < _UnitInfoPanels.Count; i++) {

            ObjectPooling.Despawn(_UnitInfoPanels[i].gameObject);
        }
        _UnitInfos.Clear();
        _UnitInfoPanels.Clear();

        // Add a new panel for each type of unit selected by the player
        for (int i = 0; i < _SelectedUnits.Count; i++) {

            // Check if this unit already exists in the info lists
            bool matchingType = false;
            for (int j = 0; j < _UnitInfos.Count; j++) {

                // Matching type to existing info
                if (_SelectedUnits[i].UnitType == _UnitInfos[j]._UnitType) {

                    // Add to count of the unit info
                    _UnitInfos[j]._Amount++;
                    matchingType = true;
                    break;
                }
            }

            // No unit infos match the selected unit were testing against
            if (!matchingType) {

                // Create a new iterator in the info lists
                UnitInfos info = new UnitInfos {

                    _UnitType = _SelectedUnits[i].UnitType,
                    _Logo = _SelectedUnits[i].Logo,
                    _Amount = 1
                };
                _UnitInfos.Add(info);
            }
        }

        // Create a new panel for each unit info
        for (int i = 0; i < _UnitInfos.Count; i++) {

            if (StencilObject != null) {

                // Spawn panel & pass unit info data
                UI_UnitInfoPanel panel = ObjectPooling.Spawn(StencilObject.gameObject).GetComponent<UI_UnitInfoPanel>();
                panel.UnitName.text = _UnitInfos[i]._UnitType.ToString();
                panel.AmountCounter.text = _UnitInfos[i]._Amount.ToString();
                panel.SetPanelInfo(_UnitInfos[i]);
                panel.SetPlayer(_Player);

                // Set anchoring position
                RectTransform rectT = panel.GetComponent<RectTransform>();
                panel.transform.SetParent(gameObject.transform);
                if (i > 0) {

                    // Not the first panel
                    rectT.anchoredPosition = new Vector2((StartingPositionX + (_PanelSize * i) + (OffsetX * i)), StartingPositionY);
                }
                else {

                    // First panel
                    rectT.anchoredPosition = new Vector2(StartingPositionX, StartingPositionY);
                }
                _UnitInfoPanels.Add(panel);
            }
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
        
        // If the object is a unit - return its type
        Unit unit = worldObject.GetComponent<Unit>();
        if (unit != null) { return unit.UnitType; }
        
        // The object isn't a valid AI type
        return Unit.EUnitType.Undefined;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}