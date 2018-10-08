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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds a WorldObject to the weighted target list
    /// </summary>
    /// <param name="target"></param>.
    public override void AddPotentialTarget(WorldObject target) {

        // The siege engine CANNOT fire at air units so never allow it to reach the target list
        if (!(target is AirVehicle)) { base.AddPotentialTarget(target); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}