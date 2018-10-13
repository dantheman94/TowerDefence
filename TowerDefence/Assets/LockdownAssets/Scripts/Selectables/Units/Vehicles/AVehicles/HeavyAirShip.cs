using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 13/10/2018
//
//******************************

public class HeavyAirShip : AirVehicle {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" HEAVY AIRSHIP PROPERTIES")]
    [Space]
    public List<VehicleGunner> IndependentGunners = null;
    [Space]
    public float PropellorRotationSpeed = 10f;
    public GameObject PropellorObject = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Initializes the object.
    /// </summary>
    protected override void Init() {
        base.Init();

        // Set the top gunner's properties to match the heavy airships
        for (int i = 0; i < IndependentGunners.Count; i++) {

            IndependentGunners[i].Team = Team;
            IndependentGunners[i]._ObjectState = _ObjectState;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    protected override void Update() {
        base.Update();

        // Constantly rotate the propellor
        if (PropellorObject != null) { PropellorObject.transform.Rotate(Vector3.forward * PropellorRotationSpeed * Time.deltaTime); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the new current object state (Ie: Building, Deployable, Active).
    /// </summary>
    /// <param name="newState"></param>
    public override void SetObjectState(WorldObjectStates newState) {
        base.SetObjectState(newState);

        // Update gunner(s) object states
        for (int i = 0; i < IndependentGunners.Count; i++) { IndependentGunners[i]._ObjectState = _ObjectState; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}