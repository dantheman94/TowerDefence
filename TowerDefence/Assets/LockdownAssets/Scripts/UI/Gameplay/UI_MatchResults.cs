using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/8/2018
//
//******************************


public class UI_MatchResults : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR  
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" GAME OVER PROPERTIES")]
    [Space]
    public GameObject GameOverWidget;
    [Space]
    public Text OutcomeText;
    [Space]
    public Text ScoreText;
    public Text WaveText;
    public Text DifficultyText;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private GameManager _Manager = GameManager.Instance;
    private bool _IsGameOver = false;
    private bool _EndScreenOpen = false;
    private Player _Player;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    void Update () {

        // _IsGameOver = GameManager.Instance.GetGameOverState();

        if (_IsGameOver)
        {
            DisplayEndScreen();
            Leaderboard.Instance.OnGameOver();
            _IsGameOver = false;
        }
	}

    /// <summary>
    //   Sets game over to true.
    /// </summary>
    public void SetGameOver() { _IsGameOver = true; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Displays end screen hud components.
    /// </summary>
    void DisplayEndScreen()
    {
        _Player = GameManager.Instance.Players[0];
        ScoreText.text = "Score: " + _Player.GetScore().ToString();
        WaveText.text = "Waves: " + _Player.GetWavesSurvived().ToString();
        DifficultyText.text = "Difficulty: " + _Player.Difficulty;

        if(_Player.Outcome == "Victory")
        {
            OutcomeText.text = _Player.Outcome;
            OutcomeText.color = Color.green;
        }
        else
        {
            OutcomeText.color = Color.red;
            OutcomeText.text = _Player.Outcome;
        }
       
        GameOverWidget.SetActive(true);
        Time.timeScale = 0;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}