using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 12/9/2018
//
//******************************

public class CoreAirShip : AirVehicle {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" LIGHT AIRSHIP PROPERTIES")]
    [Space]
    public float RotationSpeed = 10f;
    public List<GameObject> PropellorsClockwise = null;
    public List<GameObject> PropellorsAntiClockwise = null;

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

        // Constantly rotate the propellors
        for (int i = 0; i < PropellorsClockwise.Count; i++)     { PropellorsClockwise[i].transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime); }
        for (int i = 0; i < PropellorsAntiClockwise.Count; i++) { PropellorsAntiClockwise[i].transform.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}