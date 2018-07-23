using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 
//
//******************************

public class LeaderBoards : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    struct LeaderBoardData
    {
        public bool  _GameModeBeat;
        public int   _WavesSurvived;
        public float _PlayerScore;
        public int   _SupplyPowerCount;
        public float _MatchTime;
    }

    LeaderBoardData  _CurrentData;

    public static LeaderBoards Instance;

    public static string _ReadWritePath = Application.dataPath + "/LeaderBoardTest";
    public static string _FileExtention = ".txt";

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  This is called before Startup().
    /// </summary>
    private void Awake()
    {

        // Initialize singleton
        if (Instance != null && Instance != this)
        {

            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called when the gameObject is created.
    /// </summary>
    void Start () {
		
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  Called each frame. 
    /// </summary>
    void Update () {
		
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  .
    /// </summary>
    void SaveTextData() {

        // Begin serialization
        var writer = new System.Xml.Serialization.XmlSerializer(typeof(LeaderBoardData));
        var wfile  = new System.IO.StreamWriter(_ReadWritePath);
        writer.Serialize(wfile, _CurrentData);

        // End serialization
        wfile.Close();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  .
    /// </summary>
    void ReadTextData() {

        // Begin serialization
        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(LeaderBoardData));
        System.IO.StreamReader file = new System.IO.StreamReader(_ReadWritePath);
        _CurrentData = (LeaderBoardData)reader.Deserialize(file);

        // End serialization
        file.Close();
    }
}
