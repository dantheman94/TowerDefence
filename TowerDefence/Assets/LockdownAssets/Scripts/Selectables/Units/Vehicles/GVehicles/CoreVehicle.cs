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
    public float BarrelRotationSpeed = 500f;
    [Space]
    public List<GameObject> Wheels;
    public float WheelMaxRotationSpeed = 400f;

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
        
        // Check if the barrels should rotate
        if (/*_IsFiring &&*/ WeaponBarrel != null) {

            // Rotate the barrel by rotation speed
            WeaponBarrel.transform.Rotate(Vector3.forward * (BarrelRotationSpeed * Time.deltaTime));
        }

        // Rotate the wheels when the vehicle moves
        if (_CurrentSpeed > 0) {

            // The wheels will speed up/slow down by the current over max
            float fraction = _CurrentSpeed / MaxSpeed;
            for (int i = 0; i < Wheels.Count; i++) {

                // Rotate the wheels
                Wheels[i].transform.Rotate(Vector3.right * (_CurrentSpeed * WheelMaxRotationSpeed * Time.deltaTime));
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called whenever the unit fires its weapon. 
    //  (Used for animation).
    /// </summary>
    public override void OnFireWeapon() {
        base.OnFireWeapon();
        
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}