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
    public Text TextScore = null;
    public Text TextWavesSurvived = null;
    public Text TextDifficulty = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

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
        if (TextDifficulty != null) { TextDifficulty.text = DifficultyManager.Instance.CurrentDifficulty.ToString(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}