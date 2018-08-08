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

public class MenuNavigation : MonoBehaviour
{

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

    public GameObject MainMenu;
    public GameObject HighScore;
    public GameObject Credits;
    public GameObject Settings;
    public GameObject PlayObject;



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
    private bool _IsInMain = false;
    private bool _IsInGameSelect = false;
    private bool _IsInLeaderboards = false;
    private bool _IsInSettings = false;
    private bool _IsInCredits = false;


    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************


    // Use this for initialization
    void Start()
    {
        gamepad = GamepadManager.Instance.GetGamepad(1);
        _NavCdCpy = _NavigationCooldown;
        GamemodeButton.Select();
    }

    // Update is called once per frame
    void Update()
    {
        _NavigationCooldown -= Time.deltaTime;
        GoBack();
    }


    /// <summary>
    /// Goes back depending active gameobject.
    /// </summary>
    private void GoBack()
    {
       if(MainMenu.activeInHierarchy)
        {

        }
      else if(PlayObject.activeInHierarchy)
        {
            if(gamepad.GetButtonDown("B"))
            {
                PlayObject.SetActive(false);
                MainMenu.SetActive(true);
                GamemodeButton.Select();
            }
        }
       else if(HighScore.activeInHierarchy)
        {
            if(gamepad.GetButtonDown("B"))
            {
                HighScore.SetActive(false);
                MainMenu.SetActive(true);
                LeaderboardButton.Select();
            }
        }
       else if(Settings.activeInHierarchy)
        {
            if(gamepad.GetButtonDown("B"))
            {
                Settings.SetActive(false);
                MainMenu.SetActive(true);
                SettingsButton.Select();
            }
        }
    }

    private void GoIntoMenu()
    {

    }
}