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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        DisplayPauseMenu();
	}

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
                        PauseMenuObject.SetActive(false);
                        Time.timeScale = 1;
                    }
                    else
                    {
                        _IsPaused = true;
                        PauseMenuObject.SetActive(true);
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

    public void Continue()
    {
        if(_IsPaused)
        {
            _IsPaused = false;
            PauseMenuObject.SetActive(false);
            Time.timeScale = 1;
        }
        
    }

    public void DisplaySettings()
    {
        SettingsMenuObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }


    public bool IsPaused() { return _IsPaused; }
}
