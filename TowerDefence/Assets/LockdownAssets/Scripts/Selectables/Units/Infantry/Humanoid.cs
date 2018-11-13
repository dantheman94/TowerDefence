using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 25/7/2018
//
//******************************

public class Humanoid : Vehicle {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" ANIMATIONS")]
    [Space]
    public Animation LegsAnimator = null;
    public Animation TorsoAnimator = null;
    [Space]
    public Animation DeathLegsAnimator = null;
    public Animation DeathTorsoAnimator = null;
    [Space]
    public AnimationClip IdleAnimation = null;
    public AnimationClip WalkingAnimation = null;
    public AnimationClip ShootingAnimation = null;
    [Space]
    public AnimationClip DeathLegAnimation = null;
    public AnimationClip DeathTorsoAnimation = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected Quaternion _LookRotation = Quaternion.identity;
    private Animator _Animator = null;
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Initializes the object.
    /// </summary>
    protected override void Init() {
        base.Init();

        if (_Agent != null) { _Agent.acceleration = 1000; }
        _Animator = GetComponentInChildren<Animator>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called every frame - updates the soldier/unit's movement and combat behaviours.
    /// </summary>
    protected override void UpdateAIControllerMovement() {
        base.UpdateAIControllerMovement();

        // Update the speed variable in the animator controller
        if (_Animator != null) {

            // Precaution check
            if (_Animator.gameObject.activeSelf) { _Animator.SetFloat("Speed", _CurrentSpeed); }
        }
        
        // Not currently moving
        if (_CurrentSpeed == 0) {

            // Play idle animation on the legs
            if (LegsAnimator != null && IdleAnimation != null) {

                LegsAnimator.clip = IdleAnimation;
                LegsAnimator.Play();
            }

            // Play idle animation on the torso
            if (TorsoAnimator != null && IdleAnimation != null) {

                TorsoAnimator.clip = IdleAnimation;
                TorsoAnimator.Play();
            }
        }

        // Is attacking/shooting
        if (_IsAttacking) {

            TorsoAnimator.clip = ShootingAnimation;
            TorsoAnimator.Play();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="instigator"></param>
    public override void OnDeath(WorldObject instigator) {

        // Play some fancy animations
        if (DeathLegsAnimator != null && DeathLegAnimation != null) {

            DeathLegsAnimator.clip = DeathLegAnimation;
            DeathLegsAnimator.Play();
        }
        if (DeathTorsoAnimator != null && DeathTorsoAnimation != null) {

            DeathTorsoAnimator.clip = DeathTorsoAnimation;
            DeathTorsoAnimator.Play();
        }

        // Despawn it
        base.OnDeath(instigator);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    protected override void LookAtLerp(Vector3 position) {

        // This is a temporary fix - will need to make it so the rotating looks 
        // realistic and not a snap to target as this function call does.
        LookAtSnap(position);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}