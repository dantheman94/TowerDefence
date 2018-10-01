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
    public Unit _UnitAttached = null;
    public VehicleGunner _GunnerAttached = null;

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
    //  Adds the other collider to the attached AI's target list
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {

        // Valid worldObject
        WorldObject worldObject = other.gameObject.GetComponent<WorldObject>();
        if (worldObject == null) { worldObject = other.gameObject.GetComponentInParent<WorldObject>(); }

        // Unit attached
        if (worldObject != null && _UnitAttached != null) {

            // Enemy team?
            if (worldObject.Team != _UnitAttached.Team && worldObject.Team != GameManager.Team.Undefined) {

                // Active in the world?
                if (worldObject._ObjectState == Abstraction.WorldObjectStates.Active) {

                    // Try to add to target list
                    _UnitAttached.AddPotentialTarget(worldObject);
                }
            }
        }

        // Vehicle gunner attached
        if (worldObject != null && _GunnerAttached != null) {

            // Enemy team?
            if (worldObject.Team != _GunnerAttached.Team && worldObject.Team != GameManager.Team.Undefined) {

                // Active in the world?
                if (worldObject._ObjectState == Abstraction.WorldObjectStates.Active) {

                    // Try to add to target list
                    _GunnerAttached.AddPotentialTarget(worldObject);
                }
            }
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
