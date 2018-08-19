using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/18/2018
//
//******************************

public class SightSphere : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" TARGETTING OBJECT WEIGHTS")]
    [Space]
    public Ai.TargetWeight[] TargetWeights = new Ai.TargetWeight[Ai._WeightLength];

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Ai _AIAttached = null;
    private Tower _TowerAttached = null;
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

        // Determine which type of AI this is (unit or tower)?
        _AIAttached = GetComponentInParent<Ai>();
        _TowerAttached = GetComponentInParent<Tower>();
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

            // This component is attached to a unit/AI object
            if (_AIAttached != null) {

                // Enemy team?
                if (_WorldObjectInFocus.Team != _AIAttached.Team && _WorldObjectInFocus.Team != GameManager.Team.Undefined) {

                    // Not a squad object?
                    Squad squad = _WorldObjectInFocus.GetComponent<Squad>();
                    if (squad == null) {

                        // Not a building slot?
                        BuildingSlot slot = _WorldObjectInFocus.GetComponent<BuildingSlot>();
                        if (slot == null) {

                            // Active in the world?
                            if (_WorldObjectInFocus._ObjectState == WorldObject.WorldObjectStates.Active) {

                                // Try to add to weighted list
                                _AIAttached.AddPotentialTarget(_WorldObjectInFocus);
                            }
                        }
                    }
                }
            }

            // This component is attached to a tower object
            if (_TowerAttached != null) {

                // Enemy team?
                if (_WorldObjectInFocus.Team != _TowerAttached.Team && _WorldObjectInFocus.Team != GameManager.Team.Undefined) {

                    // Not a squad object?
                    Squad squad = _WorldObjectInFocus.GetComponent<Squad>();
                    if (squad == null) {

                        // Not a building slot?
                        BuildingSlot slot = _WorldObjectInFocus.GetComponent<BuildingSlot>();
                        if (slot == null) {
                            
                            // Active in the world?
                            if (_WorldObjectInFocus._ObjectState == WorldObject.WorldObjectStates.Active) {

                                // Try to add to weighted list
                                _TowerAttached.AddPotentialTarget(_WorldObjectInFocus);
                            }
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
        
        // Valid worldObject
        _WorldObjectInFocus = other.gameObject.GetComponent<WorldObject>();
        if (_WorldObjectInFocus != null) {

            // This component is attached to a unit/AI object
            if (_AIAttached != null) {

                // Enemy team?
                if (_WorldObjectInFocus.Team != _AIAttached.Team) {

                    // Remove from weighted list
                    _AIAttached.RemovePotentialTarget(_WorldObjectInFocus);

                    // Update new attack target (if the target that just left was the current target)
                    if (_WorldObjectInFocus == _AIAttached.GetAttackTarget()) { _AIAttached.DetermineWeightedTargetFromList(TargetWeights); }
                }
            }

            // This component is attached to a tower object
            if (_TowerAttached != null) {

                // Enemy team?
                if (_WorldObjectInFocus.Team != _TowerAttached.Team) {

                    // Remove from weighted list
                    _TowerAttached.RemovePotentialTarget(_WorldObjectInFocus);

                    // Update new attack target (if the target that just left was the current target)
                    if (_WorldObjectInFocus == _TowerAttached.GetAttackTarget()) { _TowerAttached.DetermineWeightedTargetFromList(TargetWeights); }
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

        // This component is attached to a unit/AI object
        if (_AIAttached != null) {

            if (_AIAttached.GetAttackTarget() != null) {

                // Currently the same target that is the AI's attack target
                if (_AIAttached.GetAttackTarget().gameObject == other.gameObject) {

                    // Try to chase the target
                    _AIAttached.AddPotentialTarget(_AIAttached.GetAttackTarget());
                    _AIAttached.TryToChaseTarget(_AIAttached.GetAttackTarget());
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}
