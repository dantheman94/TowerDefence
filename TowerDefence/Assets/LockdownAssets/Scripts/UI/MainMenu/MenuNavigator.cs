using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Serialization;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 3/09/2018
//
//******************************


public class MenuNavigator : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      DEFINITION
    //
    //******************************************************************************************************************************

    public enum SceneAreaState
    {
        ACTIVE,
        INACTIVE
    }

    [System.Serializable]
    public struct MenuArea
    {
        public SceneAreaState AreaState;
        [Tooltip("Reference to menu area wrapper.")]
        public GameObject WholeAreaObject;
        [Tooltip("The button to return to the previous menu area.")]
        public Button PreviousButton;
        [Tooltip("The button where selection starts after entering a new area.")]
        public Button StartButton;
        public Dropdown DropButton;
    }

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Tooltip("Main menu area.")]
    public MenuArea MainMenu;
    [Tooltip("Play Menu Area")]
    public MenuArea PlayMenu;
    [Tooltip("Settings Menu Area")]
    public MenuArea SettingsMenu;
    [Tooltip("Leaderboard Menu Area")]
    public MenuArea LeaderboardMenu;
    [Tooltip("Credit Menu Area")]
    public MenuArea CreditsMenu;


    [Header("----------------------")]
    [Space]
    [Header("PLAYGAME MENU AREA SUB-MENU REFERENCES")]
    public GameObject LevelUIObject;

    public GameObject DifficultyUIObject;

    public GameObject FactionUIObject;

    public GameObject OverviewObject;


    [Header("----------------------")]
    [Space]
    [Header("PLAYGAME SUB MENU BUTTON REFERENCES")]
    public Button LevelStartButton;
    public Button DifficultyStartButton;
    public Button FactionStartButton;

    public List<Image> ButtonImage;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private MenuArea _ActiveMenu;

    private xb_gamepad gamepad;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    // Use this for initialization
    void Start () {
        gamepad = GamepadManager.Instance.GetGamepad(1);

        MainMenu.AreaState = SceneAreaState.ACTIVE;
        PlayMenu.AreaState = SceneAreaState.INACTIVE;
        LeaderboardMenu.AreaState = SceneAreaState.INACTIVE;
        SettingsMenu.AreaState = SceneAreaState.INACTIVE;
        CreditsMenu.AreaState = SceneAreaState.INACTIVE;
        MainMenu.StartButton.Select();
	}

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {
        NavigateMenu();
        ChangeButtonColor();
        DisableButtonUI();
	}

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Disables button UI.
    /// </summary>
    void DisableButtonUI()
    {
        if(gamepad.IsConnected)
        {
            for (int i = 0; i < ButtonImage.Count; ++i)
            {
                ButtonImage[i].enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < ButtonImage.Count; ++i)
            {
                ButtonImage[i].enabled = false;
            }
        }
       
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Enter desired menu area.
    /// </summary>
    /// <param name="DesiredMenu"></param>
    public void EnterMenuArea(string DesiredMenu)
    {
        switch(DesiredMenu)
        {
            case "Main":
                MainMenu.AreaState = SceneAreaState.ACTIVE;
                SettingsMenu.AreaState = SceneAreaState.INACTIVE;
                LeaderboardMenu.AreaState = SceneAreaState.INACTIVE;
                PlayMenu.AreaState = SceneAreaState.INACTIVE;
                CreditsMenu.AreaState = SceneAreaState.INACTIVE;
                if (MainMenu.StartButton != null)
                    StartCoroutine(DelayedSelectButton(MainMenu.StartButton));
                
                break;
            case "Settings":
                SettingsMenu.AreaState = SceneAreaState.ACTIVE;
                MainMenu.AreaState = SceneAreaState.INACTIVE;
                LeaderboardMenu.AreaState = SceneAreaState.INACTIVE;
                PlayMenu.AreaState = SceneAreaState.INACTIVE;
                CreditsMenu.AreaState = SceneAreaState.INACTIVE;
                if (SettingsMenu.StartButton != null)
                    StartCoroutine(DelayedSelectButton(SettingsMenu.StartButton));
        
                break;
            case "Leaderboard":
                LeaderboardMenu.AreaState = SceneAreaState.ACTIVE;
                SettingsMenu.AreaState = SceneAreaState.INACTIVE;
                PlayMenu.AreaState = SceneAreaState.INACTIVE;
                MainMenu.AreaState = SceneAreaState.INACTIVE;
                CreditsMenu.AreaState = SceneAreaState.INACTIVE;
                if (LeaderboardMenu.StartButton != null)
                    StartCoroutine(DelayedSelectButton(LeaderboardMenu.StartButton));
                break;
            case "Play":
                PlayMenu.AreaState = SceneAreaState.ACTIVE;
                MainMenu.AreaState = SceneAreaState.INACTIVE;
                SettingsMenu.AreaState = SceneAreaState.INACTIVE;
                LeaderboardMenu.AreaState = SceneAreaState.INACTIVE;
                CreditsMenu.AreaState = SceneAreaState.INACTIVE;
                if (PlayMenu.StartButton != null)
                    StartCoroutine(DelayedSelectButton(PlayMenu.StartButton));
                break;
            case "Credits":
                CreditsMenu.AreaState = SceneAreaState.ACTIVE;
                MainMenu.AreaState = SceneAreaState.INACTIVE;
                SettingsMenu.AreaState = SceneAreaState.INACTIVE;
                LeaderboardMenu.AreaState = SceneAreaState.INACTIVE;
                PlayMenu.AreaState = SceneAreaState.INACTIVE;
                if (CreditsMenu.StartButton != null)
                    StartCoroutine(DelayedSelectButton(CreditsMenu.StartButton));
                break;
            default:
                break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Enters player menus sub-menus
    /// </summary>
    /// <param name="DesiredSubMenu"></param>
    public void EnterPlayArea(string DesiredSubMenu)
    {
        switch (DesiredSubMenu)
        {
            case "Level":
                LevelUIObject.SetActive(true);
                DifficultyUIObject.SetActive(false);
                OverviewObject.SetActive(false);
                StartCoroutine(DelayedSelectButton(LevelStartButton));
                break;
            case "Difficulty":
                LevelUIObject.SetActive(false);
                DifficultyUIObject.SetActive(true);
                OverviewObject.SetActive(false);
                StartCoroutine(DelayedSelectButton(DifficultyStartButton));
                break;
            case "Faction":
                FactionUIObject.SetActive(true);
                DifficultyUIObject.SetActive(false);
                OverviewObject.SetActive(false);
                LevelUIObject.SetActive(false);
                StartCoroutine(DelayedSelectButton(FactionStartButton));
                break;
            case "Start Match":

                break;
        }
    
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Switches between menu states.
    /// </summary>
    private void NavigateMenu()
    {
        if(MainMenu.AreaState == SceneAreaState.ACTIVE)
        {
            MainMenu.WholeAreaObject.SetActive(true);
            
        
        }
        else
        {
            MainMenu.WholeAreaObject.SetActive(false);
        }

        if(SettingsMenu.AreaState == SceneAreaState.ACTIVE)
        {
            SettingsMenu.WholeAreaObject.SetActive(true);
            if(gamepad.GetButtonDown("B"))
            {
                SettingsMenu.AreaState = SceneAreaState.INACTIVE;
                MainMenu.AreaState = SceneAreaState.ACTIVE;
                if (MainMenu.StartButton != null)
                    StartCoroutine(DelayedSelectButton(MainMenu.StartButton));
            }

        }
        else
        {
            SettingsMenu.WholeAreaObject.SetActive(false);
        }

        if(LeaderboardMenu.AreaState == SceneAreaState.ACTIVE)
        {
            LeaderboardMenu.WholeAreaObject.SetActive(true);
            if(gamepad.GetButtonDown("B"))
            {
                LeaderboardMenu.AreaState = SceneAreaState.INACTIVE;
                MainMenu.AreaState = SceneAreaState.ACTIVE;
                if (MainMenu.StartButton != null)
                    StartCoroutine(DelayedSelectButton(MainMenu.StartButton));
            }

        }
        else
        {
            LeaderboardMenu.WholeAreaObject.SetActive(false);
        }

        if(PlayMenu.AreaState == SceneAreaState.ACTIVE)
        {
            PlayMenu.WholeAreaObject.SetActive(true);
            if (gamepad.GetButtonDown("B"))
            {
                if (LevelUIObject.activeInHierarchy)
                {
                    LevelUIObject.SetActive(false);
                    StartCoroutine(DelayedSelectButton(PlayMenu.StartButton));
                    OverviewObject.SetActive(true);
                }
                else if (DifficultyUIObject.activeInHierarchy)
                {
                    DifficultyUIObject.SetActive(false);
                    StartCoroutine(DelayedSelectButton(PlayMenu.StartButton));
                    OverviewObject.SetActive(true);
                }
                else if(FactionUIObject.activeInHierarchy)
                {
                    FactionUIObject.SetActive(false);
                    StartCoroutine(DelayedSelectButton(PlayMenu.StartButton));
                    OverviewObject.SetActive(true);
                }
                else
                {
                    PlayMenu.AreaState = SceneAreaState.INACTIVE;
                    MainMenu.AreaState = SceneAreaState.ACTIVE;
                    if (MainMenu.StartButton != null)
                        StartCoroutine(DelayedSelectButton(MainMenu.StartButton));
                }
            }

        }
        else
        {
            PlayMenu.WholeAreaObject.SetActive(false);
        }

        if(CreditsMenu.AreaState == SceneAreaState.ACTIVE)
        {
            CreditsMenu.WholeAreaObject.SetActive(true);
            if(gamepad.GetButtonDown("B"))
            {
                MainMenu.AreaState = SceneAreaState.ACTIVE;
                CreditsMenu.AreaState = SceneAreaState.INACTIVE;
                if (MainMenu.StartButton != null)
                    StartCoroutine(DelayedSelectButton(MainMenu.StartButton));
            }

        }
        else
        {
            CreditsMenu.WholeAreaObject.SetActive(false);
        }   
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Selects button after 0.05 seconds as unity wont register
    /// the highlight without it.
    /// </summary>
    /// <param name="a_button"></param>
    /// <returns></returns>
    IEnumerator DelayedSelectButton(Button a_button)
    {
        yield return new WaitForSeconds(0.05f);
        a_button.Select();
    }

    IEnumerator DelayedSelectDropdown(Dropdown dropdown)
    {
        yield return new WaitForSeconds(0.05f);
        dropdown.Select();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Greys out buttons if they are down.
    /// </summary>
    private void ChangeButtonColor()
    {
        if(gamepad.GetButton("B"))
        {
            for(int i = 0; i < ButtonImage.Count; i++)
            {
                ButtonImage[i].color = Color.grey;  
            }
        }
        else if(!gamepad.GetButton("B"))
        {
            for (int i = 0; i < ButtonImage.Count; i++)
            {
                ButtonImage[i].color = Color.white;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
