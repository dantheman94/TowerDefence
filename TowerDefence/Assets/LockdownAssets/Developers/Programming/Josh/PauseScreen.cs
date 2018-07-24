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

    enum MenuState
    {
        Paused,
        Unpaused,

        Audio,
        Video,
        Game
    }

    MenuState _MenuState;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [SerializeField]
    [Tooltip("The canvas used for the Pause screen.")]
    public Canvas PauseScreenCanvas;
    [SerializeField]
    [Tooltip("The canvas used for the Audio menu of the Pause screen.")]
    public Canvas AudioMenuCanvas;
    [SerializeField]
    [Tooltip("The canvas used for the Video menu of the Pause screen.")]
    public Canvas VideoMenuCanvas;
    [SerializeField]
    [Tooltip("The canvas used for the Game menu of the Pause screen.")]
    public Canvas GameMenuCanvas;

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

        _MenuState = MenuState.Unpaused;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  Called each frame. 
    /// </summary>
    void Update () {

        if (Input.GetKeyDown("space") && _MenuState == MenuState.Paused) {

            _MenuState = MenuState.Unpaused;
        }
        else if (Input.GetKeyDown("space") && _MenuState == MenuState.Unpaused) {

            _MenuState = MenuState.Paused;
        }

        //////////////////////////////////////////////////////

        if (_MenuState == MenuState.Paused) {

            PauseScreenCanvas.enabled = true;
            AudioMenuCanvas.enabled = false;
            VideoMenuCanvas.enabled = false;
            GameMenuCanvas.enabled  = false;
            Time.timeScale = 0.0f;
        }
        if (_MenuState == MenuState.Unpaused) {

            PauseScreenCanvas.enabled = false;
            AudioMenuCanvas.enabled = false;
            VideoMenuCanvas.enabled = false;
            GameMenuCanvas.enabled  = false;
            Time.timeScale = 1.0f;
        }

        //////////////////////////////////////////////////////

        if (_MenuState == MenuState.Audio) {

            PauseScreenCanvas.enabled = false;
            AudioMenuCanvas.enabled = true;
        }
        if (_MenuState == MenuState.Video) {

            PauseScreenCanvas.enabled = false;
            VideoMenuCanvas.enabled = true;
        }
        if (_MenuState == MenuState.Game) {

            PauseScreenCanvas.enabled = false;
            GameMenuCanvas.enabled = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void PauseScreenAudioButton() {

        _MenuState = MenuState.Audio;
    }

    public void PauseScreenVideoButton() {

        _MenuState = MenuState.Video;
    }

    public void PauseScreenGameButton() {

        _MenuState = MenuState.Game;
    }

    public void PauseScreenBackButton() {

        _MenuState = MenuState.Paused;
    }

    public void PauseScreenResumeButton() {

        _MenuState = MenuState.Unpaused;
    }

    public void PauseScreenGameMenuUIButton() {

        // Toggle the radical menu
        _GameManager._IsRadialMenu = !(_GameManager._IsRadialMenu);
    }
}