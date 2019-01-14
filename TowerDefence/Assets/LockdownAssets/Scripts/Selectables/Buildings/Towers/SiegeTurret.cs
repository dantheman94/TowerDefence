using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 13/7/2018
//
//******************************

public class SiegeTurret : Tower {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" SIEGE TURRET PROPERTIES")]
    public float MortarRangeMin = 50f;
    public float MortarRangeMax = 400f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
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

        // A valid target is known
        if (_AttackTarget != null && _ObjectState == WorldObjectStates.Active) {

            if (_AttackTarget.IsAlive()) {

                // Rotating the head of the siege turret
                if (Head != null) {

                    // Aim the turret's head at the target (Z AXIS ONLY)
                    Vector3 hDirection = (_AttackTarget.transform.position - transform.position).normalized;
                    hDirection.y = 0;
                    Quaternion hLookAtRot = Quaternion.LookRotation(hDirection);
                    Head.transform.rotation = Quaternion.Lerp(Head.transform.rotation, hLookAtRot, WeaponAimingSpeed * Time.deltaTime);
                }

                if (_TowerWeapon != null) {

                    // Fire the turret's weapon
                    if (_TowerWeapon.CanFire()) { _TowerWeapon.FireWeapon(); }
                }
            }

            // Target is dead so null it out (so that the turret returns to the default state)
            else { _AttackTarget = null; }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
