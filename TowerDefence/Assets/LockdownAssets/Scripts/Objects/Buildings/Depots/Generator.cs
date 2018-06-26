using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 24/5/2018
//
//******************************

public class Generator : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" GENERATOR PROPERTIES")]
    public eResourceType ResourceType;
    public int ResourcesGivenWhenBuilt = 100;
    public int ResourcesGivenPerTickOver = 1;
    public float GeneratorRate = 0.1f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    public enum eResourceType { Supplies, Power }
    
    private float _SupplyTimer = 0f;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        if (_ObjectState == WorldObjectStates.Active) {

            if (_Player) {

                // Keep generating resources for the player
                if (_SupplyTimer < GeneratorRate) { _SupplyTimer += Time.deltaTime; }
                else {

                    _SupplyTimer = 0f;
                    switch (ResourceType) {

                        // Supplies
                        case eResourceType.Supplies: {

                                _Player.SuppliesCount += ResourcesGivenPerTickOver;
                                break;
                            }

                        // Power
                        case eResourceType.Power: {

                                _Player.PowerCount += ResourcesGivenPerTickOver;
                                break;
                            }

                        default: break;
                    }
                }
            }
        }
    }

}