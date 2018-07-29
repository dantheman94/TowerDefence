
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 29/7/2018
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
    public GameObject WidgetOverview = null;
    public GameObject WidgetLevels = null;
    public GameObject WidgetDifficulties = null;
    public GameObject WidgetFactions = null;
    
    //******************************************************************************************************************************
    //
    //      EVENTS
    //
    //******************************************************************************************************************************
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the widget is activated/focused.
    /// </summary>
    public void OnWidgetEnter() {

        gameObject.SetActive(true);
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

        if (WidgetOverview != null)     { WidgetOverview.gameObject.SetActive(false); }
        if (WidgetLevels != null)       { WidgetLevels.gameObject.SetActive(false); }
        if (WidgetDifficulties != null) { WidgetDifficulties.gameObject.SetActive(false); }
        if (WidgetFactions != null)     { WidgetFactions.gameObject.SetActive(false); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}