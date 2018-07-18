using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 14/7/2018
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
    [Space]
    public eResourceType ResourceType;
    public bool UpgradedGenerator = false;
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        if (_ObjectState == WorldObjectStates.Active && _Player) {

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object
    //  linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        // Gets reference to the original generator (before the upgrade)
        Generator originalGenerator = null;
        if (buildingSlot != null) {

            if (buildingSlot.GetBuildingOnSlot() != null) { originalGenerator = buildingSlot.GetBuildingOnSlot().GetComponent<Generator>(); }
        }

        // Remove old healthbar (if valid)
        int hitpoints = MaxHitPoints;
        if (originalGenerator != null) {

            hitpoints = originalGenerator._HitPoints;
            if (originalGenerator._HealthBar != null) { ObjectPooling.Despawn(originalGenerator._HealthBar.gameObject); }

        }

        // Start building process
        base.OnWheelSelect(buildingSlot);

        // Only proceed if there was a previous building and we are upgrading from that
        if (originalGenerator != null) {

            // Update player ref
            _ClonedWorldObject._Player = originalGenerator._Player;

            // Set the new bases building state object to be the currently active base
            _ClonedWorldObject.BuildingState = originalGenerator.gameObject;            
        }

        // Update attached buildingSlot generator reference
        if (buildingSlot != null) { buildingSlot.SetBuildingOnSlot(_ClonedWorldObject.GetComponent<Generator>()); }

        // Reset building's health
        _ClonedWorldObject.SetHitPoints(_ClonedWorldObject.MaxHitPoints);
    }

}