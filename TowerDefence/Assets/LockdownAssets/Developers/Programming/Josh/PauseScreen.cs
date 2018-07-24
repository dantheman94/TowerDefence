using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 24/07/2018
//
//******************************

public class PauseScreen : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    bool _PauseScreenEnabled = false;
    public static GameManager _GameManager;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [SerializeField]
    [Tooltip("The canvas used for the Pause screen.")]
    public Canvas PauseScreenCanvas;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  This is called before Startup().
    /// </summary>
    void Start () {

	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called when the gameObject is created.
    /// </summary>
    void Update () {

        if (Input.GetKeyDown("space"))
            _PauseScreenEnabled = !(_PauseScreenEnabled);

        if (_PauseScreenEnabled == false)
            PauseScreenCanvas.enabled = false;

        if (_PauseScreenEnabled == true)
            PauseScreenCanvas.enabled = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void PauseScreenAudioButton() {

    }

    public void PauseScreenVideoButton() {

    }

    public void PauseScreenGameButton() {

    }

    public void PauseScreenResumeButton() {

    }

    public void PauseScreenGameMenuUIButton() {

        // Toggle the radical menu
        _GameManager._IsRadialMenu = !(_GameManager._IsRadialMenu);
    }
}
