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

public class Widget_Leaderboards : MonoBehaviour {

    public Button EnterButton = null;

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
        if (EnterButton != null)
        {
            EnterButton.Select();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the widget is de-activated/unfocused.
    /// </summary>
    public void OnWidgetExit() { gameObject.SetActive(false); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}