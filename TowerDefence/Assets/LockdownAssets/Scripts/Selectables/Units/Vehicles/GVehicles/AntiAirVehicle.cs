using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 12/11/2018
//
//******************************

public class AntiAirVehicle : Vehicle {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SKYLANCER PROPERTIES")]
    [Space]
    public List<GameObject> WheelAxis;
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

        // Rotate the wheels when the vehicle moves
        if (_CurrentSpeed > 0) {

            // The wheels will speed up/slow down by the current over max
            float fraction = _CurrentSpeed / MaxSpeed;
            for (int i = 0; i < WheelAxis.Count; i++) {

                // Rotate the wheels
                WheelAxis[i].transform.Rotate(Vector3.right * (_CurrentSpeed * WheelMaxRotationSpeed * Time.deltaTime));
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds a WorldObject to the weighted target list
    /// </summary>
    /// <param name="target"></param>
    public override void AddPotentialTarget(WorldObject target) {

        // Target can ONLY be a air vehicle
        AirVehicle air = target.GetComponent<AirVehicle>();
        if (air != null) { base.AddPotentialTarget(air); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="worldObject"></param>
    public override bool ForceChaseTarget(WorldObject objTarget, bool playerCommand = false) {

        // Target can ONLY be a air vehicle
        AirVehicle air = objTarget.GetComponent<AirVehicle>();
        if (air != null) { return base.ForceChaseTarget(objTarget, playerCommand); }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="worldObject"></param>
    public override bool TryToChaseTarget(WorldObject objTarget) {

        // Target can ONLY be a air vehicle
        if (objTarget is AirVehicle) { return base.ForceChaseTarget(objTarget); }
        else { return false; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}