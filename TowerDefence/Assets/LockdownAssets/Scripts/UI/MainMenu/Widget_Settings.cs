﻿using System.Collections;
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

public class Widget_Settings : MonoBehaviour {

    public Button EnterButton = null;
    public Button ExitButton;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private GameObject PreviousWidget;

    //******************************************************************************************************************************
    //
    //      EVENTS
    //
    //******************************************************************************************************************************
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the widget is activated/focused.
    /// </summary>
    public void OnWidgetEnter()
    {
        gameObject.SetActive(true);
        if(EnterButton != null)
        {
            EnterButton.Select();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the widget is de-activated/unfocused.
    /// </summary>
    public void OnWidgetExit()
    {
        ExitButton.Select();
        ExitButton.GetComponentInChildren<Text>().color = Color.black;
        gameObject.SetActive(false);
                                    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the previous widget's gameobject active state to true.
    /// </summary>
    public void OnGoBack() {

        if (PreviousWidget != null) { PreviousWidget.gameObject.SetActive(true); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Saves the current values specified in the settings widget to the player's profile settings.
    /// </summary>
    public void SaveSettings() {

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="previous"></param>
    public void SetPreviousWidget(GameObject previous) { PreviousWidget = previous; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}