using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 30/10/2018
//
//******************************

public class HealingSphere : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Unit _UnitAttached = null;
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

                    // Friendly team?
                    if (_WorldObjectInFocus.Team == _UnitAttached.Team && _WorldObjectInFocus.Team != GameManager.Team.Undefined) {

                        // Active in the world?
                        if (_WorldObjectInFocus._ObjectState == Abstraction.WorldObjectStates.Active) {

                            // No current healing target for the support ship?
                            if ((_UnitAttached as SupportAirShip).GetHealingTarget() == null) {

                                // Set as healing target
                                (_UnitAttached as SupportAirShip).SetHealingTarget(_WorldObjectInFocus);
                            }
                        }
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

                // The support ship has no current healing target
                if ((_UnitAttached as SupportAirShip).GetHealingTarget() == null) {
                    
                    // Friendly team?
                    WorldObject worldObject = other.gameObject.GetComponent<WorldObject>();
                    if (worldObject.Team == _UnitAttached.Team && worldObject.Team != GameManager.Team.Undefined) {

                        // Active in the world?
                        if (worldObject._ObjectState == Abstraction.WorldObjectStates.Active) {

                            // No current healing target for the support ship?
                            if ((_UnitAttached as SupportAirShip).GetHealingTarget() == null) {

                                // object in focus doesnt have full health?
                                if (worldObject.GetHitPoints() < worldObject.MaxHitPoints) {

                                    // Set as healing target
                                    (_UnitAttached as SupportAirShip).SetHealingTarget(worldObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
