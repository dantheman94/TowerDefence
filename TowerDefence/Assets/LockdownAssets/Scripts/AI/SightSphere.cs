using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 7/8/2018
//
//******************************

public class SightSphere : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PROPERTIES")]
    [Space]
    public VehicleGunner _VehicleGunnerAI = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {

        // Valid worldObject
        WorldObject worldObject = other.gameObject.GetComponent<WorldObject>();
        if (worldObject != null) {

            // Enemy team?
            if (worldObject.Team != _VehicleGunnerAI.GetVehicleAttached().Team && worldObject.Team != GameManager.Team.Undefined) {

                // Add to weighted list
                _VehicleGunnerAI.GetVehicleAttached().AddPotentialTarget(worldObject);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
