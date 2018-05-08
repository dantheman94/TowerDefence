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
//  Last edited on: 5/8/2018
//
//******************************

public class Player : MonoBehaviour {

    //******************************************************************************************************************************
    // INSPECTOR

    public PlayerIndex _PlayerIndex = PlayerIndex.One;

    //******************************************************************************************************************************
    // VARIABLES

    // Input
    public UserInput _Input { get; private set; }
    public WorldObject SelectedObject { get; set; }

    // Economy
    private int _SupplyCount = 0;
    private int _PowerCount = 0;
    private int _Level = 1;
    private int _Score = 0;
    private int _WavesSurvived = 0;

    // HUD
    public HUD _HUD { get; private set; }

    //******************************************************************************************************************************
    // FUNCTIONS

    private void Start() {

        // Get component references
        _Input = GetComponent<UserInput>();
        _HUD = GetComponent<HUD>();

        // Initialize new player entity
        _Score = 0;
        _WavesSurvived = 0;
    }

    private void Update() {

    }

}
