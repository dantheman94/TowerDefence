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
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player;
    
    private int _SupplyGenerators;
    private int _PowerGenerators;
    
    private float _SupplyTimer = 0f;
    private float _PowerTimer = 0f;

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
            if (_SupplyTimer < SupplyGenerator.GeneratorTickDelay / _SupplyGenerators) {

                _SupplyTimer += Time.deltaTime;
            }
            else {

                if ((_Player.SuppliesCount < _Player.MaxSupplyCount) && _SupplyGenerators >= 1) {

                    _Player.SuppliesCount += 1;
                }
                // Reset timer
                _SupplyTimer = 0f;
            }

            // Keep generating power for the player
            if (_PowerTimer < PowerStation.GeneratorTickDelay / _PowerGenerators) {

                _PowerTimer += Time.deltaTime;
            }
            else {

                if ((_Player.PowerCount < _Player.MaxSupplyCount) && _PowerGenerators >= 1) {

                    _Player.PowerCount += 1;
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
