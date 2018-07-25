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

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public override void OnDeath() {

        // Play some fancy animation maybe?

        // Despawn it
        base.OnDeath();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected override void UpdatePlayerControlledMovement() {

        // Directional movement with WASD
        ///Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        ///transform.SetPositionAndRotation(transform.position + inputDirection * (InfantryMovementSpeed * Time.deltaTime), transform.rotation);

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

}