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
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {

        for (int i = 0; i < IndependentGunners.Count; i++) {

            IndependentGunners[i].Team = Team;
            IndependentGunners[i]._ObjectState = _ObjectState;
        }
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