using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 30/10/18
//
//******************************

public class UI_LoadingScreen : MonoBehaviour
{

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" USER INTERFACE")]
    [Space]
    public GameObject LoadingGameplayScreen = null;
    public GameObject LoadingMainmenuScreen = null;
    [Space]
    public Text LevelName = null;
    public Text LevelDescription = null;
    public Text PlayerName = null;
    public Image LevelImage = null;
    [Space]
    public Text Gamemode = null;
    [Space]
    public Slider LoadingProgressSlider = null;
    public Text LoadingText = null;
    [Space]
    public Image XboxButton;
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

   

        // Update loading screen panel widgets
        if (InstanceManager.Instance._LoadingGameplay) { 

            // Show gameplay loading screen
            if (LoadingGameplayScreen != null) { LoadingGameplayScreen.SetActive(true); }
            if (LoadingMainmenuScreen != null) { LoadingMainmenuScreen.SetActive(false); }
        }
        else {

            // Show mainmenu loading screen
            if (LoadingGameplayScreen != null) { LoadingGameplayScreen.SetActive(false); }
            if (LoadingMainmenuScreen != null) { LoadingMainmenuScreen.SetActive(true); }
        }

        // Update level name text
        if (LevelName != null /*&& InstanceManager.Instance._Level != null*/) { LevelName.text = InstanceManager.Instance._Level.LevelName.ToUpper(); }

        // Update level description text
        if (LevelDescription != null/* && InstanceManager.Instance._Level != null*/) { LevelDescription.text = InstanceManager.Instance._Level.LevelDescription; }

        if(LevelImage != null) { LevelImage.sprite = InstanceManager.Instance._Level.LevelThumbnailSprite; }

        //Update player name
        if(PlayerName != null) { PlayerName.text = InstanceManager.Instance.PlayerName; }

        // Update gamemode text
        if (Gamemode != null) {
            //if(InstanceManager.Instance._Difficulty != null)
            //{
                string text = string.Concat(InstanceManager.Instance._Difficulty.GetUIEnumerator().ToUpper() + " | CORE DEFENCE");
                Gamemode.text = text;
        //    }
            
            
        }

        UpdateProgressSlider();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void UpdateProgressSlider() {

        if (LoadingProgressSlider != null) {

            // Update progress slider value
            float progress = ASyncLoading.Instance.GetSceneLoadProgress();
            LoadingProgressSlider.value = progress;

            // Load is complete and waiting for activation
            if (progress >= 0.9f) {

                // Gamepad connected
                if (GamepadManager.Instance.GetGamepad(1).IsConnected) {

                    LoadingText.text = "PRESS        TO CONTINUE";
                }

                // Keyboard input
                else {

                    LoadingText.text = "PRESS [SPACE] TO CONTINUE";
                }

                // Activate level once valid input is recieved
                if (Input.GetKeyDown(KeyCode.Space) || GamepadManager.Instance.GetGamepad(1).GetButtonDown("A")) { ASyncLoading.Instance.ActivateLevel(); }
            }
            else { LoadingText.text = "LOADING"; }
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}