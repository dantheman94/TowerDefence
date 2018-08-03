using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 3/08/2018
//
//******************************
[System.Serializable]
public class SaveData
{
    public int Score;
    public string Name;
    public int Waves;
    public string Difficulty;
    public string Outcome;
}

public class Leaderboard : MonoBehaviour {


    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player;


    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    // Use this for initialization
    void Start () {
        _Player = GameManager.Instance.Players[0];
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    /// <summary>
    /// Saves highscores when game finishes.
    /// </summary>
    public void OnGameOver()
    {
        SavePlayerStats();
    }

    /// <summary>
    /// Loads the highscores.
    /// </summary>
    public SaveData LoadData(string filePath)
    {
        if (File.Exists(Application.persistentDataPath + "/" + filePath + ".save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + filePath + ".save",FileMode.Open);
            SaveData saveData = (SaveData)bf.Deserialize(file);
            file.Close();

            Debug.Log(filePath + " loaded.");
            return saveData;
        }
        else
        {
            Debug.Log("File not found.");
            return null;
        }
    }


    /// <summary>
    /// Serializes save class as a file.
    /// </summary>
    public void SavePlayerStats()
    {
        SaveData saveData = CreateNewSave();
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/"+ saveData.Name +".save");
        bf.Serialize(file, saveData);
        file.Close();

        Debug.Log("Player data saved.");
        
    }


    /// <summary>
    /// Gets player data and saves it to save class.
    /// </summary>
    /// <returns></returns>
    private SaveData CreateNewSave()
    {
        SaveData _SaveData = new SaveData();
        _SaveData.Difficulty = _Player.Difficulty;
        _SaveData.Name = _Player.Name;
        _SaveData.Score = _Player.GetScore();
        _SaveData.Waves = _Player.GetWavesSurvived();
        _SaveData.Outcome = _Player.Outcome;

        return _SaveData;
    }

}
