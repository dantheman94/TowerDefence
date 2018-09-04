using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 1/9/2018
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
    public GameObject Barrel = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Quaternion _DefaultRotation = Quaternion.identity;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        // Store the original rotation so that the turret can return to this when it has no target to fire at
        _DefaultRotation = transform.rotation;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Rotating turret weapon
        if (Head != null && Barrel != null) {

            // A valid target is known
            if (_AttackTarget != null) {

                if (_AttackTarget.IsAlive()) {

                    // Aim the turret's head at the target (Z AXIS ONLY)
                    Vector3 hDirection = (_AttackTarget.transform.position - transform.position).normalized;
                    hDirection.y = 0;
                    Quaternion hLookAtRot = Quaternion.LookRotation(hDirection);
                    Head.transform.rotation = Quaternion.Lerp(Head.transform.rotation, hLookAtRot, WeaponAimingSpeed * Time.deltaTime);

                    // Aim the turret's barrel at the target (Y AXIS ONLY)
                    Vector3 bDirection = (_AttackTarget.transform.position - transform.position).normalized;
                    bDirection.z = 0;
                    Quaternion bLookAtRot = Quaternion.LookRotation(Barrel.transform.forward, bDirection);
                    Barrel.transform.rotation = Quaternion.Lerp(Barrel.transform.rotation, bLookAtRot, WeaponAimingSpeed * Time.deltaTime);

                    if (TowerWeapon != null) {
                        
                        // Fire the turret's weapon
                        if (TowerWeapon.CanFire()) { TowerWeapon.FireWeapon(); }
                    }
                }

                // Target is dead so null it out (so that the turret returns to the default state)
                else { _AttackTarget = null; }
            }

            // Return to the default rotation
            else { Head.transform.rotation = Quaternion.Lerp(Head.transform.rotation, _DefaultRotation, WeaponAimingSpeed * Time.deltaTime); }
        }
    }

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

        // Create weapon
        if (TowerWeapon != null) { TowerWeapon = ObjectPooling.Spawn(TowerWeapon.gameObject).GetComponent<Weapon>(); }

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