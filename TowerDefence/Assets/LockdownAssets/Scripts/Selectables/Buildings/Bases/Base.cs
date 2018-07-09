using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 25/6/2018
//
//******************************

public class Base : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BASE PROPERTIES")]
    public eBaseType BaseType;
    public GameObject UnitSpawnTransform;
    [Space]
    public List<BuildingSlot> DepotSlots;
    public List<BuildingSlot> TowerSlots;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum eBaseType { Minibase, Outpost, CommandCenter, Headquarters }

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

        // Gets reference to the original base (before the upgrade)
        Base originalBase = null;
        if (buildingSlot.AttachedBase != null) { originalBase = buildingSlot.AttachedBase; }

        // Remove old healthbar (if valid)
        int hitpoints = MaxHitPoints;
        if (originalBase != null) {

            hitpoints = originalBase._HitPoints;
            if (originalBase._HealthBar != null) { ObjectPooling.Despawn(originalBase._HealthBar.gameObject); }

        }
        // Start building process
        base.OnWheelSelect(buildingSlot);

        // Only proceed if there was a previous building and we are upgrading from that
        if (originalBase != null) {
            
            // Set the new bases building state object to be the currently active base
            _ClonedWorldObject.BuildingState = originalBase.gameObject;

            // Get references to all the buildings that were in the previous base & allocate them to the new one
            Base newBase = _ClonedWorldObject.GetComponent<Base>();

            // Depot/Generator slots
            for (int i = 0; i < newBase.DepotSlots.Count; i++) {

                // Only go as far as the previous bases slot size
                if (i < originalBase.DepotSlots.Count) {

                    // Reallocate building to new base (if there is one)
                    if (originalBase.DepotSlots[i] != null) {

                        // Send building to new base
                        newBase.DepotSlots[i].SetBuildingOnSlot(originalBase.DepotSlots[i].GetBuildingOnSlot());
                        originalBase.DepotSlots[i].SetBuildingOnSlot(null);
                    }
                    else { continue; }
                }
                else { break; }
            }

            // Tower slots
            for (int i = 0; i < newBase.TowerSlots.Count; i++) {

                // Only go as far as the previous bases slot size
                if (i < originalBase.TowerSlots.Count) {

                    // Reallocate tower to new base (if there is one)
                    if (originalBase.TowerSlots[i] != null) {

                        // Send tower to new base
                        newBase.TowerSlots[i].SetBuildingOnSlot(originalBase.TowerSlots[i].GetBuildingOnSlot());
                        originalBase.TowerSlots[i].SetBuildingOnSlot(null);
                    }
                    else { continue; }
                }
                else { break; }
            }
        }

        // Update attached base reference
        buildingSlot.AttachedBase = _ClonedWorldObject.GetComponent<Base>();

        // Update new bases health with the old bases health
        _ClonedWorldObject._HitPoints = hitpoints;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}