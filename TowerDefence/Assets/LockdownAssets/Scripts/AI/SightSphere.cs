using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/8/2018
//
//******************************

public class SightSphere : MonoBehaviour {
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Unit _UnitAttached = null;
    private Tower _TowerAttached = null;
    private WorldObject _WorldObjectInFocus = null;

    private SphereCollider _SphereComponent = null;

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

        // Get component references
        _SphereComponent = GetComponent<SphereCollider>();

        // Determine which type of AI this is (unit or tower)?
        _UnitAttached = GetComponentInParent<Unit>();
        _TowerAttached = GetComponentInParent<Tower>();

        if (_UnitAttached != null && _SphereComponent != null) { _SphereComponent.radius = _UnitAttached.MaxAttackingRange * 1.3f; }
        if (_TowerAttached != null && _SphereComponent != null) { _SphereComponent.radius = _TowerAttached.MaxAttackingRange * 1.3f; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {

        // Valid unit or building
        if (other.CompareTag("Unit") || other.CompareTag("Building")) {

            // Valid worldObject check
            _WorldObjectInFocus = other.gameObject.GetComponent<WorldObject>();
            if (_WorldObjectInFocus != null) {

                // This component is attached to a unit/AI object
                if (_UnitAttached != null) {

                    // Enemy team?
                    if (_WorldObjectInFocus.Team != _UnitAttached.Team && _WorldObjectInFocus.Team != GameManager.Team.Undefined) {

                        // Active in the world?
                        if (_WorldObjectInFocus._ObjectState == Abstraction.WorldObjectStates.Active) {

                            // Try to add to weighted list
                            _UnitAttached.AddPotentialTarget(_WorldObjectInFocus);
                        }
                    }
                }

                // This component is attached to a tower object
                if (_TowerAttached != null) {

                    // Enemy team?
                    if (_WorldObjectInFocus.Team != _TowerAttached.Team && _WorldObjectInFocus.Team != GameManager.Team.Undefined) {

                        // Active in the world?
                        if (_WorldObjectInFocus._ObjectState == Abstraction.WorldObjectStates.Active) {

                            // Try to add to weighted list
                            _TowerAttached.AddPotentialTarget(_WorldObjectInFocus);
                        }
                    }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called whenever something exits the sphere collider
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {

        // Valid unit or building
        if (other.CompareTag("Unit") || other.CompareTag("Building")) {

            // Valid worldObject
            _WorldObjectInFocus = other.gameObject.GetComponent<WorldObject>();
            if (_WorldObjectInFocus != null) {

                // This component is attached to a unit/AI object
                if (_UnitAttached != null) {

                    // Enemy team?
                    if (_WorldObjectInFocus.Team != _UnitAttached.Team) {

                        // Remove from weighted list
                        _UnitAttached.RemovePotentialTarget(_WorldObjectInFocus);

                        // Update new attack target (if the target that just left was the current target)
                        if (_WorldObjectInFocus == _UnitAttached.GetAttackTarget()) { _UnitAttached.DetermineWeightedTargetFromList(_UnitAttached.TargetWeights); }
                    }
                }

                // This component is attached to a tower object
                if (_TowerAttached != null) {

                    // Enemy team?
                    if (_WorldObjectInFocus.Team != _TowerAttached.Team) {

                        // Remove from weighted list
                        _TowerAttached.RemovePotentialTarget(_WorldObjectInFocus);

                        // Update new attack target (if the target that just left was the current target)
                        if (_WorldObjectInFocus == _TowerAttached.GetAttackTarget()) { _TowerAttached.DetermineWeightedTargetFromList(_TowerAttached.TargetWeights); }
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
    private void OnTriggerStay(Collider other) {

        if (other.CompareTag("Unit") || other.CompareTag("Building")) {

            // This component is attached to a unit/AI object
            if (_UnitAttached != null) {

                // The unit attached already has an attack target
                if (_UnitAttached.GetAttackTarget() != null) {

                    // Currently the same target that is the AI's attack target
                    if (_UnitAttached.GetAttackTarget().gameObject == other.gameObject) {

                        // Try to chase the target
                        _UnitAttached.AddPotentialTarget(_UnitAttached.GetAttackTarget());
                        _UnitAttached.TryToChaseTarget(_UnitAttached.GetAttackTarget());
                    }
                }

                // The unit attached has no attack target
                else {

                    WorldObject worldObject = other.gameObject.GetComponent<WorldObject>();

                    // Enemy team?
                    if (worldObject.Team != _UnitAttached.Team && worldObject.Team != GameManager.Team.Undefined) {

                        // Try to chase the target
                        _UnitAttached.AddPotentialTarget(worldObject);
                        _UnitAttached.TryToChaseTarget(worldObject);
                    }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}
