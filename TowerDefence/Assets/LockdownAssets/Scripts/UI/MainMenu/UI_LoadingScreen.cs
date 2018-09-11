using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 29/7/2018
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
    [Header(" PLAYERS")]
    [Space]
    public Text LevelName = null;
    public Text LevelDescription = null;
    [Space]
    public Text Gamemode = null;
    [Space]
    public Slider LoadingProgressSlider = null;
    public Text LoadingText = null;
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
    private void Update()
    {
        // Update level name text
        if (LevelName != null) { LevelName.text = InstanceManager.Instance._Level.LevelName.ToUpper(); }

        // Update level description text
        if (LevelDescription != null) { LevelDescription.text = InstanceManager.Instance._Level.LevelDescription; }

        // Update gamemode text
        if (Gamemode != null)
        {
            string text = string.Concat(InstanceManager.Instance._Difficulty.GetUIEnumerator().ToUpper() + " | CORE DEFENCE");
            Gamemode.text = text;
        }

        UpdateProgressSlider();
        ChangeButtonUI();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void UpdateProgressSlider()
    {
        if (LoadingProgressSlider != null)
        {

            // Update progress slider value
            float progress = ASyncLoading.Instance.GetSceneLoadProgress();
            LoadingProgressSlider.value = progress;

            // Load is complete and waiting for activation
            if (progress >= 0.9f)
            {
                LoadingText.text = "PRESS [SPACE] TO CONTINUE";

                // Activate level
                if (Input.GetKeyDown(KeyCode.Space)) { ASyncLoading.Instance.ActivateLevel(); }
            }
            else { LoadingText.text = "LOADING"; }
        }

    }

    /// <summary>
    /// Switches between keyboard and buttons.
    /// </summary>
    private void ChangeButtonUI()
    {
        if (GamepadManager.Instance.GetGamepad(1).IsConnected)
        {
            XboxButton.enabled = true;
            if (LoadingProgressSlider.value >= 0.9f)
            {
                LoadingText.text = "PRESS        TO CONTINUE";

                if (GamepadManager.Instance.GetGamepad(1).GetButtonDown("A"))
                {
                    ASyncLoading.Instance.ActivateLevel();
                }
            }
            else
            {
                LoadingText.text = "LOADING";
            }
        }
        else
        {
            XboxButton.enabled = false;
            if (LoadingProgressSlider.value >= 0.9f)
            {
                LoadingText.text = "PRESS [SPACE] TO CONTINUE";
            }
            else
            {
                LoadingText.text = "LOADING";
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}