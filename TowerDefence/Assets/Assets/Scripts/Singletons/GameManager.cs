using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 21/5/2018
//
//******************************

public class GameManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header("START OF MATCH")]
    [Space]
    public int StartingSupplyCount;
    public int StartingPowerCount;
    public int StartingPlayerLevel;
    public int StartingMaxPopulation;

    [Space]
    [Header("-----------------------------------")]
    [Header("HUD")]
    [Space]
    public GameObject SelectionWheel;
    public GameObject AbilityWheel;
    public GameObject UnitHealthBar;
    public GameObject BuildingInProgressPanel;
    public GameObject RecycleBuilding;
    public GameObject ObjectSelected;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [HideInInspector]
    public List<Player> Players { get; set; }

    [HideInInspector]
    public List<Selectable> Selectables { get; set; }

    public static GameManager Instance;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
    /// </summary>
    private void Awake() {

        // Initialize singleton
        if (Instance != null && Instance != this) {

            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // Initialize lists
        Selectables = new List<Selectable>();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Start() {
        
        // Get all player entities
        Players = new List<Player>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in objs) {

            Players.Add(item.GetComponent<Player>());
        }
    }

}
