using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 3/08/2018
//
//******************************


public class Display_Leaderboard : MonoBehaviour {

    public struct Panel
    {
        public Text Rank;
        public Text Name;
        public Text Waves;
        public Text Score;
        public Text Difficulty;
        public Text Win;
        public Image Backdrop;
        public Color ColorBackdrop;
        public Color TextColor;
        public Transform PanelPosition;
    }
    public struct MapPanel
    {
        public Image MapImage;
        public Text MapName;
        public Transform PanelPosition;
    }


    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    public Color FirstPanel;
    public Color SecondPanel;


    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    //Variables
    private List<Panel> _HighscorePanels;
    private List<MapPanel> _MapPanels;


    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
