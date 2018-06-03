﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/24/2018
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
    public eGeneratorType GeneratorType;
    public eGeneratorType UpgradedGeneratorType;
    public eSupplyType SupplyType;
    public int SuppliesGivenWhenBuilt = 100;
    public int SuppliesGivenPerTick = 1;
    public float SupplyRate = 0.1f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum eGeneratorType  { SupplyPad, HeavySupplyPad, PowerGenerator, HeavyPowerGenerator }
    public enum eSupplyType     { Resources, Power }

    private int _CurrentSupplies = 0;
    private float _SupplyTimer = 0f;
    private float _CurrentSupplyRate;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    protected override void Start() { base.Start();

        // Initialize starting supply rate
        _CurrentSupplyRate = SupplyRate;
    }

    protected override void Update() { base.Update();

        if (IsAlive() && _Deployed) {

            // Keep generating supplies for its player
            if (_SupplyTimer < SupplyRate) { _SupplyTimer += Time.deltaTime; }
            else {
                
                _SupplyTimer = 0f;
                switch (SupplyType) {

                    case eSupplyType.Resources: {

                        _Player.SuppliesCount += SuppliesGivenPerTick;
                        break;
                    }
                    case eSupplyType.Power: {

                        _Player.PowerCount += SuppliesGivenPerTick;
                        break;
                    }

                    default: break;
                }
            }
        }
    }

}