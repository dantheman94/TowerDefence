using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 24/07/2018
//
//******************************

public class MainMenu : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    enum MenuState
    {
        MainMenu,
        Settings
    }

    MenuState _MenuState;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [SerializeField]
    [Tooltip("The canvas used for the Main Menu screen.")]
    public Canvas MainMenuCanvas;
    [SerializeField]
    [Tooltip("The canvas used for the Settings menu of the Main Menu screen.")]
    public Canvas SettingsMenuCanvas;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  This is called before Startup().
    /// </summary>
    void Start() {

        _MenuState = MenuState.MainMenu;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  Called each frame. 
    /// </summary>
    void Update() {

        if (_MenuState == MenuState.MainMenu) {

            MainMenuCanvas.enabled = true;
            SettingsMenuCanvas.enabled = false;
        }
        if (_MenuState == MenuState.Settings) {

            MainMenuCanvas.enabled = false;
            SettingsMenuCanvas.enabled = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void MainMenuPlayGame() {

    }

    public void MainMenuQuitGame() {

        // Exit application
        Application.Quit();
    }

    public void MainMenuSettings() {

        _MenuState = MenuState.Settings;
    }

    public void MainMenuLoadTestScene() {

        // Load "TestScene"
        SceneManager.LoadScene("TestScene", LoadSceneMode.Additive);
    }

    public void MainMenuSettingsMenuBackButton() {

        _MenuState = MenuState.MainMenu;
    }
}