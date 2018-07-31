using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Joshua Peake
//  Last edited on: 24/7/2018
//
//  Comment: Working on implementing target tracking.
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

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public Transform  _TargetTransform;
    public float      _TargetDistance = 0.0f;
    public bool       _TargetAquired = false;

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

    private void Update() {

        // Calculate the _TargetDistance
        _TargetDistance = Vector3.Distance(_TargetTransform.position, transform.position);

        if(_TargetAquired == true) {

            // When _TargetAquired is true, look at the target
            transform.LookAt(_TargetTransform);
        }

        if(_TargetDistance >= 250.0f) {

            // When the _TargetDistance exceeds a certain limit, stop looking
            _TargetAquired = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnCollisionEnter(Collision collision) {

        // Store the collision
        Collider contact = collision.contacts[0].thisCollider;

        if (contact.gameObject.tag == "Enemy") {

            _TargetTransform = collision.gameObject.transform;
            _TargetAquired = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}