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
    public Ai _AIAttached = null;

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
        if (worldObject != null && _AIAttached != null) {

            // Enemy team?
            if (worldObject.Team != _AIAttached.Team && worldObject.Team != GameManager.Team.Undefined) {
                
                // Not a squad object?
                Squad squad = worldObject.GetComponent<Squad>();
                if (squad == null) {

                    // Active in the world?
                    if (worldObject._ObjectState == WorldObject.WorldObjectStates.Active) {

                        // Try to add to target list
                        _AIAttached.AddPotentialTarget(worldObject);
                    }
                }
            }
        } 
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="other"></param>
    /*
    private void OnTriggerStay(Collider other) {

        if (_AIAttached.GetAttackTarget() != null) {

            // Currently the same target that is the AI's attack target
            if (_AIAttached.GetAttackTarget().gameObject == other.gameObject) {

                // Attempt to pursue
                _AIAttached.TryToChaseTarget(_AIAttached.GetAttackTarget());
            }
        }
    }
    */
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
