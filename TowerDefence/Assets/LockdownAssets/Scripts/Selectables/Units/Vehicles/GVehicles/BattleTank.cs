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

public class BattleTank : Vehicle {
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds a WorldObject to the weighted target list
    /// </summary>
    /// <param name="target"></param>.
    public override void AddPotentialTarget(WorldObject target) {

        // The siege engine CANNOT fire at air units so never allow it to reach the target list
        AirVehicle air = target.GetComponent<AirVehicle>();
        if (air == null) { base.AddPotentialTarget(target); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="worldObject"></param>
    public override bool ForceChaseTarget(WorldObject objTarget, bool playerCommand = false) {

        // The siege engine CANNOT fire at air units so never allow it to reach the target list
        AirVehicle air = objTarget.GetComponent<AirVehicle>();
        if (air == null) { return base.ForceChaseTarget(objTarget, playerCommand); }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="worldObject"></param>
    public override bool TryToChaseTarget(WorldObject objTarget) {

        // Target can ONLY be a air vehicle
        AirVehicle air = objTarget.GetComponent<AirVehicle>();
        if (air == null) { return base.ForceChaseTarget(objTarget); }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}