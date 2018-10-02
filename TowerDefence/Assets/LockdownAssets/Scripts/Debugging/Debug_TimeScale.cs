using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 18/9/2018
//
//******************************

public class Debug_TimeScale : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [TextArea]
    public string Notes = "HOLD down the END button to activate timescale mode. Then press 1-9 to multiply the timescale by that amount.";
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Text _TextString;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Called when the object is created.
    /// </summary>
    private void Start() {

        _TextString = GetComponent<Text>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        // Holding down END button
        if (Input.GetKey(KeyCode.End)) {

            // Pressing 1-9
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) { Time.timeScale = 1; _TextString.text = "Game is " + Time.timeScale + "x speed" ;}
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) { Time.timeScale = 2; _TextString.text = "Game is " + Time.timeScale + "x speed" ;}
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) { Time.timeScale = 3; _TextString.text = "Game is " + Time.timeScale + "x speed" ;}
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) { Time.timeScale = 4; _TextString.text = "Game is " + Time.timeScale + "x speed" ;}
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) { Time.timeScale = 5; _TextString.text = "Game is " + Time.timeScale + "x speed" ;}
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) { Time.timeScale = 6; _TextString.text = "Game is " + Time.timeScale + "x speed" ;}
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) { Time.timeScale = 7; _TextString.text = "Game is " + Time.timeScale + "x speed" ;}
            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) { Time.timeScale = 8; _TextString.text = "Game is " + Time.timeScale + "x speed" ;}
            if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) { Time.timeScale = 9; _TextString.text = "Game is " + Time.timeScale + "x speed"; }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
