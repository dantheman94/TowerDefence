using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/10/2018
//
//******************************

public class Ballista : Vehicle {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BALLISTA PROPERTIES")]
    [Space]
    public List<GameObject> Wheels;
    public float WheelMaxRotationSpeed = 400f;

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

}
