using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Daniel Marton
//  Last edited on: 15/10/2018
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
    //  Called when the gameObject is created.
    /// </summary>
    void Start () {

        // Get Player component
        _Player = GetComponent<Player>();	
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    void Update()
    {
        if (_Player != null)
        {
            if (_SupplyGenerators > 0) {

                // Keep generating supplies for the player
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
            }

            if (_PowerGenerators > 0) {

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

            // Get selection wheel reference
            SelectionWheel selectionWheel = null;
            if (GameManager.Instance._IsRadialMenu) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
            else { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }
            selectionWheel.UpdateButtonStates();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Temorarily boosts the players resource generation rate by the time & boost additive specified.
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="boostAmount"></param>
    /// <returns>
    //  Ienumerator
    /// </returns>
    public IEnumerator ResourceBoost(float seconds, int boostSupply, int boostPower) {

        // Boost the resource generation rate
        _SupplyGenerators += boostSupply;
        _PowerGenerators += boostPower;

        yield return new WaitForSeconds(seconds);

        // Slow down the resource generation & clamp to 0
        _SupplyGenerators -= boostSupply;
        if (_SupplyGenerators < 0) { _SupplyGenerators = 0; }

        _PowerGenerators -= boostPower;
        if (_PowerGenerators < 0) { _PowerGenerators = 0; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void AddSupplyGeneratorCount() { _SupplyGenerators += 1; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void AddPowerGeneratorCount() { _PowerGenerators += 1; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public int GetPowerGeneratorCount() { return _PowerGenerators; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public int GetSupplyGeneratorCount() { return _SupplyGenerators; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void ResourceBoostComplete(int supply, int power) {

        _SupplyGenerators -= supply;
        if (_SupplyGenerators < 0) { _SupplyGenerators = 0; }

        _PowerGenerators -= power;
        if (_PowerGenerators < 0) { _PowerGenerators = 0; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
