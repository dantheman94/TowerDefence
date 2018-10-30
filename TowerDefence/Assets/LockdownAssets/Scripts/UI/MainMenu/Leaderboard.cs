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
//  Last edited on: 7/08/2018
//
//******************************


public class Leaderboard : MonoBehaviour
{

    [System.Serializable]
    public class SaveData
    {
        public int Score;
        public string Name;
        public int Waves;
        public string Difficulty;
        public string Outcome;
    }

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player;
    private List<SaveData> _HighscoreList = new List<SaveData>();

    public static Leaderboard Instance;


    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    // Use this for initialization
    void Start()
    {
        Instance = this;
        _HighscoreList = LoadData();
        //CreateNewSave();

    }

    /// <summary>
    /// Saves highscores when game finishes.
    /// </summary>
    public void OnGameOver()
    {
        CreateNewSave();
    }

    /// <summary>
    /// Loads player data list.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public List<SaveData> LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/" + "highscores" + ".save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + "highscores" + ".save", FileMode.Open);
            // SaveData saveData = (SaveData)bf.Deserialize(file);
            List<SaveData> a_data = (List<SaveData>)bf.Deserialize(file);
            file.Close();



            Debug.Log("highscores " + " loaded.");
            return a_data;
        }
        else
        {
            Debug.Log("File not found.");
            return null;
        }
    }

    /// <summary>
    /// Gets player data and saves it to save class then adds to list.
    /// </summary>
    /// <returns></returns>
    private SaveData CreateNewSave()
    {
        _Player = GameManager.Instance.Players[0];
        SaveData _SaveData = new SaveData();
        _SaveData.Difficulty = _Player.Difficulty;
        _SaveData.Name = _Player.Name;
        _SaveData.Score = _Player.GetScore();
        _SaveData.Waves = _Player.GetWavesSurvived();
        _SaveData.Outcome = _Player.Outcome;

        if(_HighscoreList == null)
        {
            _HighscoreList = new List<SaveData>();
        }
        _HighscoreList.Add(_SaveData);


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + "highscores" + ".save");
        bf.Serialize(file, _HighscoreList);
        file.Close();

        Debug.Log("Player data saved.");


        return _SaveData;
    }

}
