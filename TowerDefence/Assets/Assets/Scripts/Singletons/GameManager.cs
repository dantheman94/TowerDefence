using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/21/2018
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
    [Header("HUD")]
    public GameObject SelectionWheel;
    public GameObject AbilityWheel;
    public GameObject UnitHealthBar;

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

    private void Start() {
        
        // Get all player entities
        Players = new List<Player>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in objs) {

            Players.Add(item.GetComponent<Player>());
        }
    }

}
