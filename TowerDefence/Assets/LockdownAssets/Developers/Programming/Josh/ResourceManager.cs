using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 01/10/2018
//
//******************************

public class ResourceManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player;

    [Space]
    [Header("-----------------------------------")]
    [Header(" GENERATOR COUNT")]
    [Space]
    private int _SupplyGenerators;
    private int _PowerGenerators;

    [Space]
    [Header("-----------------------------------")]
    [Header(" TIMERS")]
    [Space]
    private float _SupplyTimer = 0f;
    private float _PowerTimer = 0f;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    [Space]
    [Header("-----------------------------------")]
    [Header(" GENERATOR OBJECTS")]
    [Space]
    public Generator SupplyGenerator;
    public Generator PowerStation;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  Called when the gameObject is created.
    /// </summary>
    void Start () {
        // Get Player component
        _Player = GetComponent<Player>();	
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  Called each frame.
    /// </summary>
    void Update()
    {
        if (_Player != null)
        {

            // Keep generating resources for the player
            if (_SupplyTimer < SupplyGenerator.GeneratorRate / _SupplyGenerators) {

                _SupplyTimer += Time.deltaTime;
            }
            else {

                if ((_Player.SuppliesCount < _Player.MaxSupplyCount) && _SupplyGenerators >= 1) {

                    _Player.SuppliesCount += SupplyGenerator.ResourcesGivenPerTickOver;
                }
                // Reset timer
                _SupplyTimer = 0f;
            }

            // Keep generating power for the player
            if (_PowerTimer < PowerStation.GeneratorRate / _PowerGenerators) {

                _PowerTimer += Time.deltaTime;
            }
            else {

                if ((_Player.PowerCount < _Player.MaxSupplyCount) && _PowerGenerators >= 1) {

                    _Player.PowerCount += PowerStation.ResourcesGivenPerTickOver;
                }
                // Reset timer
                _PowerTimer = 0f;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void AddSupplyGeneratorCount() { _SupplyGenerators += 1; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void AddPowerGeneratorCount() { _PowerGenerators += 1; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
