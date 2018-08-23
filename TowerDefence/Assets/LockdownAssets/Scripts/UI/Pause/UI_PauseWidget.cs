using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/8/2018
//
//******************************

public class UI_PauseWidget : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR  
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PAUSE SCREEN")]
    [Space]
    public Text DifficultyTrackerText = null;
    public Text ScoreTrackerText = null;
    public Text CurrentWaveTrackerText = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private enum EButtonFocused { Resume, Restart, Settings, SaveNQuit, ENUM_COUNT }

    private EButtonFocused _ButtonFocused = EButtonFocused.Resume;

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

        // When widget is active
        if (gameObject.activeInHierarchy) {

            // Update difficulty text
            if (DifficultyTrackerText != null) { DifficultyTrackerText.text = DifficultyManager.Instance.CurrentDifficulty.ToString(); }

            // Update player score
            if (ScoreTrackerText != null) { ScoreTrackerText.text = GameManager.Instance.Players[0].GetScore().ToString(); }

            // Update current wave count
            if (CurrentWaveTrackerText != null) { CurrentWaveTrackerText.text = string.Concat(WaveManager.Instance.GetWaveCount() - 1).ToString(); }

            // Update inputs
            UpdateKeyboardInput();
            UpdateGamepadInput();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Update gamepad input
    /// </summary>
    private void UpdateKeyboardInput() {

        // If keyboard is active
        Player player = GameManager.Instance.Players[0];
        if (player._KeyboardInputManager.IsPrimaryController) {

            // Navigate UP
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {

                // Lower the index >> further up
                if (_ButtonFocused > 0) { _ButtonFocused--; }

                // Clamp to bottom
                else { _ButtonFocused = EButtonFocused.ENUM_COUNT - 1; }
            }

            // Navigate DOWN
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {

                // Higher the index >> further down
                if (_ButtonFocused < EButtonFocused.ENUM_COUNT) { _ButtonFocused++; }

                // Clamp to top
                else { _ButtonFocused = 0; }
            }

            // Select input
            if (Input.GetKeyDown(KeyCode.KeypadEnter)) {

                switch (_ButtonFocused) {

                    case EButtonFocused.Resume: { OnResumeGameplay(); break; }
                    case EButtonFocused.Restart: { OnRestartLevel(); break; }
                    case EButtonFocused.Settings: { OnSettings(); break; }
                    case EButtonFocused.SaveNQuit: { OnSaveAndQuit(); break; }
                    default: break;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Update gamepad input
    /// </summary>
    private void UpdateGamepadInput() {

        // If gamepad is active
        Player player = GameManager.Instance.Players[0];
        if (player._XboxGamepadInputManager.IsPrimaryController) {

            // Get input reference
            XboxGamepadInput xboxInput = player._XboxGamepadInputManager;

            // Navigate UP
            if (xboxInput.GetLeftThumbstickYaxis() > 0 || xboxInput.GetDpadUpClicked() || xboxInput.OnLeftTrigger()) {

                // Lower the index >> further up
                if (_ButtonFocused > 0) { _ButtonFocused--; }

                // Clamp to bottom
                else { _ButtonFocused = EButtonFocused.ENUM_COUNT - 1; }
            }

            // Navigate DOWN
            if (xboxInput.GetLeftThumbstickYaxis() < 0 || xboxInput.GetDpadDownClicked() || xboxInput.OnRightTrigger()) {

                // Higher the index >> further down
                if (_ButtonFocused < EButtonFocused.ENUM_COUNT) { _ButtonFocused++; }

                // Clamp to top
                else { _ButtonFocused = 0; }
            }

            // Select input
            if (xboxInput.GetButtonAClicked()) {

                switch (_ButtonFocused) {

                    case EButtonFocused.Resume: { OnResumeGameplay(); break; }
                    case EButtonFocused.Restart: { OnRestartLevel(); break; }
                    case EButtonFocused.Settings: { OnSettings(); break; }
                    case EButtonFocused.SaveNQuit: { OnSaveAndQuit(); break; }
                    default: break;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnResumeGameplay() {

        // Unpause the game
        GameManager.Instance.OnUnpause();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnRestartLevel() {

        // Load the "loading" scene
        ASyncLoading.Instance.LoadLevel(1);
        ASyncLoading.Instance.ActivateLevel();

        // Load the gameplay scene
        ASyncLoading.Instance.LoadLevel(InstanceManager.Instance._Level.LevelIndex);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnSettings() {

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnSaveAndQuit() {

        // Load the "loading" scene
        ASyncLoading.Instance.LoadLevel(1);
        ASyncLoading.Instance.ActivateLevel();

        // Load the main menu scene
        ASyncLoading.Instance.LoadLevel(0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
