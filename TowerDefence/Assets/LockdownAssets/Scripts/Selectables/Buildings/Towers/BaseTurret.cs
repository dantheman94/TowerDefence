using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/7/2018
//
//******************************

public class BaseTurret : Tower {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" TURRET PROPERTIES")]
    [Space]
    public bool UpgradedTurret = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

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

        // Gets reference to the original turret (before the upgrade)
        BaseTurret originalTurret = null;
        if (buildingSlot != null) {

            if (buildingSlot.GetBuildingOnSlot() != null) { originalTurret = buildingSlot.GetBuildingOnSlot().GetComponent<BaseTurret>(); }
        }

        // Remove old healthbar (if valid)
        float hitpoints = MaxHitPoints;
        if (originalTurret != null) {

            hitpoints = originalTurret.GetHitPoints();
            if (originalTurret._HealthBar != null) { ObjectPooling.Despawn(originalTurret._HealthBar.gameObject); }
        }

        // Start building process
        base.OnWheelSelect(buildingSlot);
        if (_ClonedWorldObject != null) {

            // Only proceed if there was a previous building & we're upgrading from that
            if (originalTurret != null) {

                // Update player ref
                _ClonedWorldObject.SetPlayer(originalTurret._Player);

                // Set the new base's building state object to be the currently active object
                _ClonedWorldObject.BuildingState = originalTurret.gameObject;
            }

            // Update attached building slot turret reference
            if (buildingSlot != null) { buildingSlot.SetBuildingOnSlot(_ClonedWorldObject.GetComponent<BaseTurret>()); }

            // Reset turret's health
            _ClonedWorldObject.SetHitPoints(_ClonedWorldObject.MaxHitPoints);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}