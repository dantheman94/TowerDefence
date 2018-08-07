using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 7/08/2018
//
//******************************


public class MatchResults : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR  
    //
    //******************************************************************************************************************************

    public GameObject EndScreenObject;
    public Text ScoreText;
    public Text WaveText;
    public Text DifficultyText;
    public Text OutcomeText;

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


    // Use this for initialization
    void Start () {
        _IsGameOver = true;
	}

    /////////////////////////////////////////////////////////////////////
	
	// Update is called once per frame
	void Update () {
 //       _IsGameOver = GameManager.Instance.GetGameOverState();


        if(_IsGameOver)
        {
            DisplayEndScreen();
        }
	}

    /////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Displays end screen hud components
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
    
     

        EndScreenObject.SetActive(true);
        Time.timeScale = 0;
    }
}
