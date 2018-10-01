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

public class HearingSphere : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Unit _UnitAttached = null;
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

        _UnitAttached = GetComponentInParent<Unit>();
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called whenever something exits the sphere collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {

        // Valid Unit
        if (other.CompareTag("Unit")) {

            _WorldObjectInFocus = other.gameObject.GetComponent<WorldObject>();
            if (_WorldObjectInFocus != null) {

                // Enemy team?
                if (_WorldObjectInFocus.Team != _UnitAttached.Team) {

                    // Remove from weighted list
                    _UnitAttached.RemovePotentialTarget(_WorldObjectInFocus);

                    // Update new attack target (if the target that just left was the current target)
                    if (_WorldObjectInFocus == _UnitAttached.GetAttackTarget()) { _UnitAttached.DetermineWeightedTargetFromList(null); }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other) {

        // Valid Unit
        if (other.CompareTag("Unit")) {
            _WorldObjectInFocus = other.gameObject.GetComponent<WorldObject>();
            if (_WorldObjectInFocus != null) {

                // Enemy team?
                if (_WorldObjectInFocus.Team != _UnitAttached.Team && _WorldObjectInFocus.Team != GameManager.Team.Undefined) {

                    // Active in the world?
                    if (_WorldObjectInFocus._ObjectState == WorldObject.WorldObjectStates.Active) {

                        // Is it currently shooting right now?
                        Unit unit = _WorldObjectInFocus.GetComponent<Unit>();

                        if (unit.PrimaryWeapon != null) {

                            if (unit.PrimaryWeapon.IsFiring()) {

                                // Add to weighted list
                                _UnitAttached.AddPotentialTarget(unit);
                            }
                        }

                    }
                }
            }
        }

        if (_UnitAttached.GetAttackTarget() != null) {
            
            // Currently the same target that is the AI's attack target
            if (_UnitAttached.GetAttackTarget().gameObject == other.gameObject) {

                // Try to chase the target
                _UnitAttached.AddPotentialTarget(_UnitAttached.GetAttackTarget());
                _UnitAttached.TryToChaseTarget(_UnitAttached.GetAttackTarget());
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
