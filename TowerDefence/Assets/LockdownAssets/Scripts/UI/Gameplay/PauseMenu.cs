using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 7/08/2018
//
//******************************


public class PauseMenu : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR  
    //
    //******************************************************************************************************************************

    public GameObject PauseMenuObject;
    public GameObject SettingsMenuObject;    

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _IsPaused = false;
    private bool _DisplaySettings = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

	// Update is called once per frame
	void Update () {
        DisplayPauseMenu();
	}

    ////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Activates Pause menu gui and sets timescale to 0.
    /// </summary>
    void DisplayPauseMenu()
    {
        switch(GameManager.Instance.Players[0]._CurrentController)
        {
            case Player.InputController.Keyboard:

                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    if (_IsPaused)
                    {
                        _IsPaused = false;
                        gameObject.SetActive(false);
                        Time.timeScale = 1;
                    }
                    else
                    {
                        _IsPaused = true;
                        gameObject.SetActive(true);
                        Time.timeScale = 0;
                    }
                }

                break;
            case Player.InputController.XboxGamepad:

                
                if (_IsPaused)
                {
                    
                }
                else
                {

                }
                break;
            default:
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Disables pause menu.
    /// </summary>
    public void Continue()
    {
        if(_IsPaused)
        {
            _IsPaused = false;
            PauseMenuObject.SetActive(false);
            Time.timeScale = 1;
        }
        
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Displays settings.
    /// </summary>
    public void DisplaySettings()
    {
        SettingsMenuObject.SetActive(true);
    }

    //////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Restarts game.
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Loads menu.
    /// </summary>
    public void GoBackToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    ////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// returns pause state.
    /// </summary>
    /// <returns></returns>
    public bool IsPaused() { return _IsPaused; }
}
