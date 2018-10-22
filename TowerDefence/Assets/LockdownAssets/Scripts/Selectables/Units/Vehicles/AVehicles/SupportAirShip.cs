using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 22/10/2018
//
//******************************

public class SupportAirShip : AirVehicle {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SUPPORT AIRSHIP PROPERTIES")]
    [Space]
    public float HealingAmount = 15.0f;
    public float HealingRange = 10.0f;
    [Space]
    public float HangingDistance = 50f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private WorldObject _HealingTarget = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called every frame - updates the soldier/unit's movement and combat behaviours.
    /// </summary>
    protected override void UpdateAIControllerMovement() {
        base.UpdateAIControllerMovement();
        
        // Follow our healing target
        if (_HealingTarget != null) {

            // Close the gap if were too far away from our target
            float dist = Vector3.Distance(transform.position, _HealingTarget.transform.position);
            if (dist > HangingDistance) {

                // Just go to our targets position, this will update again if they get too far anyway
                AgentSeekPosition(_HealingTarget.transform.position, false, false);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Set's the current healing target for this unit to follow.
    /// </summary>
    /// <param name="target"></param>
    public void SetHealingTarget(WorldObject target) {

        // Only objects of the same team can be healed by this object
        if (target.Team == Team) { _HealingTarget = target; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}