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

public class Widget_MainMenu : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" SUB-WIDGETS")]
    [Space]
    public GameObject WidgetCredits = null;
    public Button EnterButton;
    public Button ExitButton;
    
    //******************************************************************************************************************************
    //
    //      EVENTS
    //
    //******************************************************************************************************************************
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the widget is activated/focused.
    /// </summary>
    public void OnWidgetEnter() { gameObject.SetActive(true); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the widget is de-activated/unfocused.
    /// </summary>
    public void OnWidgetExit()
    {
        gameObject.SetActive(false);
        if(ExitButton != null)
        {
         //   ExitButton.Select();
            ExitButton.GetComponentInChildren<Text>().color = Color.black;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Quits/Exits the game
    /// </summary>
    public void OnQuitGame() {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
        //   Application.Quit();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}