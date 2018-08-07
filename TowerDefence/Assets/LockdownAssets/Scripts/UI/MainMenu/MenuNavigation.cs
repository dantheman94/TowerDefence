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

public class MenuNavigation : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR  
    //
    //******************************************************************************************************************************

    public Button GamemodeButton;
    public Button LeaderboardButton;
    public Button CreditsButton;
    public Button SettingsButton;
    public Button ExitButton;
    

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************


    private int _MenuIndexX = 0;
    private int _MenuIndexY = 0;
    private float _Deadzone = 0.9f;
    private float _NavigationCooldown = 0.3f;
    private float _NavCdCpy;
    private xb_gamepad gamepad;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************


    // Use this for initialization
    void Start () {
        gamepad = GamepadManager.Instance.GetGamepad(1);
        _NavCdCpy = _NavigationCooldown;
        GamemodeButton.Select();
	}
	
	// Update is called once per frame
	void Update () {
        _NavigationCooldown -= Time.deltaTime;
        ControllerNavigation();
        ChangeNavigationIndex();
	}


    /// <summary>
    /// Controls controller menu navigation
    /// </summary>
    void ControllerNavigation()
    {
        switch(_MenuIndexX)
        {
            case -1:
                break;

            case 0:
         
                
                break;

            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }

        switch(_MenuIndexY)
        {
            case -1:
                break;
            case 0:
                break;
            case 1:
                break;
            case 2:
                _MenuIndexY = 0;
                break;
            default:
                break;

        }
    }
    /// <summary>
    /// Edits controllers navigation index.
    /// </summary>
    void ChangeNavigationIndex()
    {
        if(gamepad.GetStick_L().Y > _Deadzone && _NavigationCooldown < 0)
        {
            _MenuIndexY++;
        }
        if (gamepad.GetStick_L().Y < -_Deadzone && _NavigationCooldown < 0)
        {
            _MenuIndexY--;
        }
        if(gamepad.GetStick_L().X > _Deadzone && _NavigationCooldown < 0)
        {
            _MenuIndexX++;
        }
        if (gamepad.GetStick_L().X < -_Deadzone && _NavigationCooldown < 0)
        {
            _MenuIndexX--;
        }
        
    }
}
