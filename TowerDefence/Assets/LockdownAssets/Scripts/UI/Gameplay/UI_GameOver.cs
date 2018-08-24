using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 13/8/2018
//
//******************************

public class UI_GameOver : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" TEXT COMPONENTS")]
    [Space]
    public Text TextMatchResult = null;
    public float TimeTillDetailsShow = 3f;
    [Space]
    public GameObject MatchDetailsPanel = null;
    public Text TextScore = null;
    public Text TextWavesSurvived = null;
    public Text TextDifficulty = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private float _GameOverDetailsTimer = 0f;
    private bool _GameOverTimerActive = false;

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

            // Update details panel visibilty
            if (_GameOverTimerActive && _GameOverDetailsTimer < TimeTillDetailsShow) {

                // Add to timer
                _GameOverDetailsTimer += Time.fixedUnscaledDeltaTime;
                if (_GameOverDetailsTimer >= TimeTillDetailsShow) {

                    // Show details panel
                    if (MatchDetailsPanel != null) { MatchDetailsPanel.SetActive(true); }

                    // Stop timer
                    _GameOverTimerActive = false;
                }
            } 

            // Update match result
            if (TextMatchResult != null) {

                string result;
                if (GameManager.Instance.GetMatchVictory()) { result = "VICTORY"; }
                else { result = "DEFEAT"; }

                TextMatchResult.text = result;
            }

            // Update player score
            if (TextScore != null) { TextScore.text = GameManager.Instance.Players[0].GetScore().ToString(); }

            // Update waves survived
            if (TextWavesSurvived != null) { TextWavesSurvived.text = string.Concat(WaveManager.Instance.GetWaveCount() - 1).ToString(); }

            // Update difficulty text
            if (TextDifficulty != null) { TextDifficulty.text = DifficultyManager.Instance._Difficulty.Difficulty.ToString(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called once when the match is over.
    /// </summary>
    public void OnGameOver() {

        // Hide details panel
        if (MatchDetailsPanel != null) { MatchDetailsPanel.SetActive(false); }

        // Start the details panel timer
        _GameOverTimerActive = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}