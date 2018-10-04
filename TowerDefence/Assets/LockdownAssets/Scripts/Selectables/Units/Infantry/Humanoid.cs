using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 25/7/2018
//
//******************************

public class Humanoid : Unit {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" HUMANOID MOVEMENT")]
    [Space]
    public float RotatingSpeed = 100f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private float _ControlRotation = 0f;
    protected Vector3 _DirectionToTarget = Vector3.zero;
    protected Quaternion _LookRotation = Quaternion.identity;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public override void OnDeath(WorldObject instigator) {

        // Play some fancy animation maybe?

        // Despawn it
        base.OnDeath(instigator);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected override void UpdatePlayerControlledMovement() {
        
        // Forward input
        float forward = Input.GetAxis("Vertical");
        if (forward != 0f) { transform.position = transform.position + transform.forward * forward * InfantryMovementSpeed * Time.deltaTime; }

        // Right input
        float right = Input.GetAxis("Horizontal");
        if (right != 0f) { transform.position = transform.position + transform.right * right * InfantryMovementSpeed * Time.deltaTime; }

        // Update base rotation
        _ControlRotation = Input.GetAxis("Mouse X") * RotatingSpeed * Time.deltaTime;
        if (_ControlRotation != 0) { transform.eulerAngles += new Vector3(0f, _ControlRotation, 0f); }

        // Check for weapon firing input
        if (Input.GetMouseButtonDown(0)) {

            // If theres a weapon attached
            if (PrimaryWeapon) {

                // Fire the weapon if possible
                if (PrimaryWeapon.CanFire()) { PrimaryWeapon.FireWeapon(); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    protected override void LookAtLerp(Vector3 position) {

        // This is a temporary fix - will need to make it so the rotating looks 
        // realistic and not a snap to target as this function call does.
        LookAtSnap(position);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}