using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 23/10/2018
//
//******************************

public class Widget_Settings : MonoBehaviour {

    public Button EnterButton = null;
    public Button ExitButton;

    public List<Button> GamepadButtons;
    public List<Button> KeyboardButtons;
    public List<Button> AmpiDextrousButtons;

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

    private void Update()
    {
        SwitchButtons();
    }

    public void SwitchButtons()
    {
        if(InputChecker.CurrentController == "Keyboard")
        {
            for(int i = 0; i < GamepadButtons.Count; ++i)
            {
                GamepadButtons[i].gameObject.SetActive(false);
            }
            for(int i = 0; i < KeyboardButtons.Count; ++i)
            {
                KeyboardButtons[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < AmpiDextrousButtons.Count; ++i)
            {

            }
        }
        else if(InputChecker.CurrentController == "Controller")
        {
            for (int i = 0; i < GamepadButtons.Count; ++i)
            {
                GamepadButtons[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < KeyboardButtons.Count; ++i)
            {
      //          KeyboardButtons[i].gameObject.SetActive(false);
            }
            for(int i = 0; i < AmpiDextrousButtons.Count; ++i)
            {

            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="previous"></param>
    public void SetPreviousWidget(GameObject previous) { PreviousWidget = previous; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}