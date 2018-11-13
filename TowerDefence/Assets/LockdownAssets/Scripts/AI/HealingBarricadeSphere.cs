using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBarricadeSphere : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private HealingBarricade _BarricadeAttached = null;

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
        _BarricadeAttached = GetComponentInParent<HealingBarricade>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {

        // Valid unit or building
        if (other.CompareTag("Unit")) {

            // Valid worldObject check
            WorldObject obj = other.gameObject.GetComponent<WorldObject>();
            if (obj != null) {

                // This component is attached to a unit/AI object
                if (_BarricadeAttached != null) {

                    // Friendly team?
                    if (obj.Team == _BarricadeAttached.Team && obj.Team != GameManager.Team.Undefined) {

                        // Active in the world?
                        if (obj._ObjectState == Abstraction.WorldObjectStates.Active) {

                            // Add to healing array
                            _BarricadeAttached.AddHealingTarget(obj);
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
    private void OnTriggerExit(Collider other) {

        if (other.CompareTag("Unit")) {

            // This component is attached to a unit/AI object
            if (_BarricadeAttached != null) {

                // Exists in the barricades healing array?
                WorldObject obj = other.GetComponent<WorldObject>();
                if (_BarricadeAttached.GetHealingTargets().Contains(obj)) {

                    // Remove it from the healing array
                    _BarricadeAttached.GetHealingTargets().Remove(obj);
                }                
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
