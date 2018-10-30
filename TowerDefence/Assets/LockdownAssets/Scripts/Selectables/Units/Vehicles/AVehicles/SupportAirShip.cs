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

            _AttackTarget = _HealingTarget;

            // Close the gap if were too far away from our target
            float dist = Vector3.Distance(transform.position, _HealingTarget.transform.position);
            if (dist > HangingDistance) {
                
                // Just go to our targets position, this will update again if they get too far anyway
                AgentSeekPosition(_HealingTarget.transform.position, false, false);
            }

            // Check if the target is within attacking range
            _DistanceToTarget = Vector3.Distance(transform.position, _HealingTarget.transform.position);
            if (_DistanceToTarget <= MaxAttackingRange && PrimaryWeapon != null) {

                // Make the muzzle point face the healing target
                MuzzleLaunchPoints[0].transform.LookAt(_HealingTarget.transform);

                // Heal the target
                if (PrimaryWeapon.CanFire()) { OnFireWeapon(); }
            }

            // Healing target does not become target if it has full health or is dead
            if (!_HealingTarget.IsAlive() || (_HealingTarget.GetHealth() >= 1f) || _HealingTarget.GetHitPoints() >= _HealingTarget.MaxHitPoints) { _HealingTarget = null; }
        }

        // No healing target, means no attack target
        else { _AttackTarget = null; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="highlight"></param>
    public override void DrawSelection(bool draw) {
        base.DrawSelection(draw);

        // Highlight our healing target
        if (_HealingTarget != null) {

            if (_HealingTarget.Team != Team) { _HealingTarget.SetForceSelectOutline(draw); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
    //  
    /// </summary>
    /// <param name="highlight"></param>
    public override void DrawHighlight(bool highlight) {
        base.DrawHighlight(highlight);

        // highlight our healing target
        if (_HealingTarget != null) {

            if (_HealingTarget.Team != GameManager.Team.Defending) { _HealingTarget.SetForceHighlightOutline(highlight); }
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

    /// <summary>
    //  Returns reference to the current healing target for this unit.
    /// </summary>
    /// <returns>
    //  WorldObject
    /// </returns>
    public WorldObject GetHealingTarget() { return _HealingTarget; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}