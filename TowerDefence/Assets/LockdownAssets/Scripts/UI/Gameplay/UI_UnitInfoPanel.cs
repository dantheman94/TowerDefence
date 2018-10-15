using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/10/2018
//
//******************************

public class UI_UnitInfoPanel : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" WIDGET COMPONENTS")]
    [Space]
    public Image LogoComponent = null;
    public TMP_Text UnitName = null;
    public TMP_Text AmountCounter = null;
    public TMP_Text AbilityText = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private UI_SelectedUnits.UnitInfos _PanelInfo;

    private Player _Player;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void SelectUnit() {

        Unit.EUnitType type = _PanelInfo._UnitType;
        if (_Player != null) {

            // Deselect all units
            _Player.DeselectAllObjects();

            // Select all units in the player's army that have the same type
            for (int i = 0; i < _Player.GetArmy().Count; i++) {

                // Matching unit type?
                Unit unit = _Player.GetArmy()[i];
                if (unit.UnitType == type) {

                    // Select the unit
                    _Player.SelectedUnits.Add(unit);
                    unit.SetPlayer(_Player);
                    unit.SetIsSelected(true);
                }
            }

            // Update units selected panels
            GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void SetPanelInfo(UI_SelectedUnits.UnitInfos info) { _PanelInfo = info; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    public void SetPlayer(Player player) { _Player = player; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}