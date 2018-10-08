using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/10/2018
//
//******************************

public class CoreVehicle : Vehicle {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" GRUMBLEBUSTER PROPERTIES")]
    [Space]
    public GameObject WeaponBarrel = null;
    public float BarrelRotationTime = 1f;
    public float BarrelRotationSpeed = 1f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _RotatingBarrels = false;
    private bool _CoroutineReset = true;

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
        
        // Cancel & restarting the barrel rotation stopper
        if (_IsFiring) {

            if (_CoroutineReset) { StartCoroutine(StopBarrelRotation()); }
            else                 { StopCoroutine(StopBarrelRotation()); }
            _CoroutineReset = !_CoroutineReset;
        }
        
        // Check if the barrels should rotate
        if (_RotatingBarrels && WeaponBarrel != null) {

            // Rotate the barrel by rotation speed
            WeaponBarrel.transform.Rotate(Vector3.left * (BarrelRotationSpeed * Time.deltaTime));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called whenever the unit fires its weapon. 
    //  (Used for animation).
    /// </summary>
    public override void OnFireWeapon() {
        base.OnFireWeapon();

        // Rotate the barrels
        if (WeaponBarrel != null) {

            _RotatingBarrels = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator StopBarrelRotation() {

        yield return new WaitForSeconds(BarrelRotationTime);
        _RotatingBarrels = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}