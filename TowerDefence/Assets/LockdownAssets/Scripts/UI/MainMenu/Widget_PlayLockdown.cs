
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 08/08/2018
//
//******************************

public class Widget_PlayLockdown : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" SUB-WIDGETS")]
    [Space]
    public GameObject WidgetHome = null;
    public GameObject WidgetOverview = null;
    public GameObject WidgetLevels = null;
    public GameObject WidgetDifficulties = null;
    public GameObject WidgetFactions = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUTTONS")]
    [Space]
    public Button StartButton = null;
    public Button GuideButton;
    public Button DifficultyEnterButton = null;
    public Button FactionEnterButton = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private PreLockdownMatchSettings _PreLockdownMatchSettings = null;
    private xb_gamepad gamepad;

    //******************************************************************************************************************************
    //
    //      EVENTS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Get component references
        _PreLockdownMatchSettings = GetComponent<PreLockdownMatchSettings>();
        gamepad = GamepadManager.Instance.GetGamepad(1);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        // Update start match button interactablility
        if (StartButton != null && _PreLockdownMatchSettings != null) {

            // Has the player selected a Map / Difficulty / Faction ?
            if (_PreLockdownMatchSettings.GetMapDefined()        && 
                _PreLockdownMatchSettings.GetDifficultyDefined() && 
                _PreLockdownMatchSettings.GetFactionDefined())   {

                StartButton.interactable = true;
            }
            ///else { StartButton.interactable = false; }
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the widget is activated/focused.
    /// </summary>
    public void OnWidgetEnter() {

        gameObject.SetActive(true);
        GuideButton.Select();
  //      GuideButton.GetComponentInChildren<Text>().color = Color.black;
        HideAllSubWidgets();
        if (WidgetOverview != null) { WidgetOverview.gameObject.SetActive(true); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the widget is de-activated/unfocused.
    /// </summary>
    public void OnWidgetExit() { gameObject.SetActive(false); }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnLevelButton() {

        HideAllSubWidgets();
        if (WidgetLevels != null) { WidgetLevels.gameObject.SetActive(true); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnDifficultyButton() {

        HideAllSubWidgets();
        if (WidgetDifficulties != null) { WidgetDifficulties.gameObject.SetActive(true); }
        if(DifficultyEnterButton != null)
        {
            DifficultyEnterButton.Select();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnFactionButton() {

        HideAllSubWidgets();
        if (WidgetFactions != null) { WidgetFactions.gameObject.SetActive(true); }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Hides the gameobjects of all referenced subwidgets.
    /// </summary>
    private void HideAllSubWidgets() {

        if (WidgetHome != null) { WidgetOverview.gameObject.SetActive(false); }
        if (WidgetOverview != null)     { WidgetOverview.gameObject.SetActive(false); }
        if (WidgetLevels != null)       { WidgetLevels.gameObject.SetActive(false); }
        if (WidgetDifficulties != null) { WidgetDifficulties.gameObject.SetActive(false); }
        if (WidgetFactions != null)     { WidgetFactions.gameObject.SetActive(false); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}