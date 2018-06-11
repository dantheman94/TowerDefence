using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/5/2018
//
//******************************

public class Player : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    public PlayerIndex _PlayerIndex = PlayerIndex.One;
    public Camera PlayerCamera = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    // Input
    public UserInput _Input { get; private set; }
    public BuildingSlot SelectedBuildingSlot { get; set; }
    public List<WorldObject> SelectedWorldObjects { get; set; }

    // Economy
    public int MaxPopulation { get; set; }
    public int PopulationCount { get; set; }
    public int SuppliesCount { get; set; }
    public int PowerCount { get; set; }
    public int Level { get; set; }
    private int Score = 0;
    private int WavesSurvived = 0;

    // HUD
    public HUD _HUD { get; private set; }

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
    /// </summary>
    private void Start() {

        // Get component references
        _Input = GetComponent<UserInput>();
        _HUD = GetComponent<HUD>();

        // Initialize new player entity
        Score = 0;
        WavesSurvived = 0;

        SuppliesCount = GameManager.Instance.StartingSupplyCount;
        PowerCount = GameManager.Instance.StartingPowerCount;
        Level = GameManager.Instance.StartingPlayerLevel;

        PopulationCount = 0;
        MaxPopulation = GameManager.Instance.StartingMaxPopulation;
        SelectedWorldObjects = new List<WorldObject>();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update() {

    }

}
