using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BARRIER PROPERTIES")]
    [Space]
    public EBarricadeType BarricadeType;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum EBarricadeType { MineField, GarrisonBarricade, HeavyBarricade, HealingBarricade }

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
        Barrier originalBarrier = null;
        if (buildingSlot != null) {

            if (buildingSlot.GetBuildingOnSlot() != null) { originalBarrier = buildingSlot.GetBuildingOnSlot().GetComponent<Barrier>(); }
        }

        // Remove old healthbar (if valid)
        int hitpoints = MaxHitPoints;
        if (originalBarrier != null) {

            hitpoints = originalBarrier.GetHitPoints();
            if (originalBarrier._HealthBar != null) { ObjectPooling.Despawn(originalBarrier._HealthBar.gameObject); }
        }

        // Start building process
        base.OnWheelSelect(buildingSlot);
        if (_ClonedWorldObject != null) {

            // Only proceed if there was a previous building & we're upgrading from that
            if (originalBarrier != null) {

                // Update player ref
                _ClonedWorldObject.SetPlayer(originalBarrier._Player);

                // Set the new base's building state object to be the currently active object
                _ClonedWorldObject.BuildingState = originalBarrier.gameObject;
            }

            // Update attached building slot barrier reference
            if (buildingSlot != null) { buildingSlot.SetBuildingOnSlot(_ClonedWorldObject.GetComponent<Barrier>()); }

            // Reset barrier's health
            _ClonedWorldObject.SetHitPoints(_ClonedWorldObject.MaxHitPoints);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
