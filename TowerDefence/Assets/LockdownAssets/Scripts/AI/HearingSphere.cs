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

public class HearingSphere : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PROPERTIES")]
    [Space]
    public Vehicle _VehicleAttached = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private SphereCollider _HearingSphere = null;

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
        _HearingSphere = GetComponent<SphereCollider>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called whenever something exits the hearing sphere collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {

        // Valid worldObject
        WorldObject worldObject = other.gameObject.GetComponent<WorldObject>();
        if (worldObject != null) {

            // Enemy team?
            if (worldObject.Team != _VehicleAttached.Team) {

                // Remove from weighted list
                _VehicleAttached.RemovePotentialTarget(worldObject);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
