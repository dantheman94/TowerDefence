using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 6/8/2018
//
//******************************

public class SightCone : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PROPERTIES")]
    [Space]
    public VehicleGunner _GunnerAI = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private ConeCollider _ConeCollider = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    private void Start() {

        // Get component references
        _ConeCollider = GetComponent<ConeCollider>();
    }

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
            if (worldObject.Team != _GunnerAI.GetVehicleAttached().Team && worldObject.Team != GameManager.Team.Undefined) {

                // Add to weighted list
                _GunnerAI.GetVehicleAttached().AddPotentialTarget(worldObject);
            }
        } 
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
