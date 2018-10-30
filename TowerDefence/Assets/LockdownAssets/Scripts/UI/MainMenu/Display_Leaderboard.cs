using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 9/08/2018
//
//******************************


public class Display_Leaderboard : MonoBehaviour
{

    public struct MapPanel
    {
        public Image MapImage;
        public Text MapName;
        public Transform PanelPosition;
    }
    public GameObject PanelObject;
    public GameObject ListObject;

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
    private List<MapPanel> _MapPanels;
    private List<Leaderboard.SaveData> _SaveDataList;
    private List<Leaderboard.SaveData> _SortedSaveData;
    private GameObject _PreviousObject;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    // Use this for initialization
    void Start()
    {
        _SaveDataList = Leaderboard.Instance.LoadData();
        SortHighscores();

        InstantiatePanels();
    }

    /// <summary>
    /// Sorts high score data.
    /// </summary>
    public void SortHighscores()
    {
        if(_SaveDataList != null)
        _SortedSaveData = _SaveDataList.OrderByDescending(o => o.Score).ToList();
    }

    /// <summary>
    /// Instantiates each panel for the highscore list.
    /// </summary>
    public void InstantiatePanels()
    {
        int counterIncrement = 75;
        float counterIncrement2 = PanelObject.GetComponentInChildren<Image>().GetComponent<RectTransform>().rect.height;
        if(_SortedSaveData != null)
        for (int i = 0; i < _SortedSaveData.Count; ++i)
        {

                GameObject go = Instantiate(PanelObject);
                go.SetActive(true);
                go.transform.parent = ListObject.transform;
                
                if (i != 0)
                {
                    go.transform.localPosition = new Vector3(PanelObject.transform.localPosition.x, PanelObject.transform.localPosition.y - counterIncrement2);
                    counterIncrement2 += counterIncrement2;
                }
                else
                {
                    go.transform.localPosition = new Vector3(PanelObject.transform.localPosition.x, PanelObject.transform.localPosition.y);
                }
                 go.transform.localScale = new Vector3(1, 1, 1);
                _PreviousObject = go;

                Panel panel = go.GetComponent<Panel>();
                panel.Score.text = _SortedSaveData[i].Score.ToString();
                panel.Difficulty.text = _SortedSaveData[i].Difficulty;
                panel.Name.text = _SortedSaveData[i].Name;
                panel.Win.text = _SortedSaveData[i].Outcome;
                panel.Waves.text = _SortedSaveData[i].Waves.ToString();
                panel.Rank.text = (i + 1).ToString();
               
                Debug.Log(counterIncrement);
        }
    }
}
