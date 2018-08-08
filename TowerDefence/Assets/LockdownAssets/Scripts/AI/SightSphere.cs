using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/8/2018
//
//******************************

public class SightSphere : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Ai _AIAttached = null;
    private WorldObject _WorldObjectInFocus = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void Start() {

        _AIAttached = GetComponentInParent<Ai>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {

        // Valid worldObject
        _WorldObjectInFocus = other.gameObject.GetComponent<WorldObject>();
        if (_WorldObjectInFocus != null) {

            // Enemy team?
            if (_WorldObjectInFocus.Team != _AIAttached.Team && _WorldObjectInFocus.Team != GameManager.Team.Undefined) {

                // Add to weighted list
                _AIAttached.AddPotentialTarget(_WorldObjectInFocus);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called whenever something exits the hearing sphere collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {

        // Valid worldObject
        _WorldObjectInFocus = other.gameObject.GetComponent<WorldObject>();
        if (_WorldObjectInFocus != null) {

            // Enemy team?
            if (_WorldObjectInFocus.Team != _AIAttached.Team) {

                // Should we chase the enemy?
                ///if (_AIAttached.GetAttackTarget() == _WorldObjectInFocus) { _AIAttached.SetChasingTarget(_AIAttached.GetAttackTarget()); }
                ///else {

                    // Remove from weighted list
                    _AIAttached.RemovePotentialTarget(_WorldObjectInFocus);

                    // Update new target (if the target that just left was the current target)
                    if (_WorldObjectInFocus == _AIAttached.GetAttackTarget()) { _AIAttached.DetermineWeightedTargetFromList(); }
                ///}
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other) {

        // valid unit
        if (other.gameObject.CompareTag("Unit")) {

            // Same gameobject in focus?
            if (_WorldObjectInFocus != null) {

                if (_WorldObjectInFocus.gameObject == other.gameObject) {

                    // Chase the enemy?
                    ///if (_AIAttached.GetAttackTarget() == _WorldObjectInFocus) { _AIAttached.SetChasingTarget(_AIAttached.GetAttackTarget()); }

                    // Add to weighted list
                    _AIAttached.AddPotentialTarget(_WorldObjectInFocus);
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
