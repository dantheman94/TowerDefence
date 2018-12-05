using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 17/10/2018
//
//******************************

public class Flamethrower : Weapon {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _FlamethrowerStreamIsPlaying = false;
    private ParticleSystem _FlamethrowerStreamEffect = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    protected override void ProjectileRaycast() {
        base.ProjectileRaycast();

        // Play particle effect stream
        if (FiringEffect != null && !_FlamethrowerStreamIsPlaying) {
            
            // Spawn
            _FlamethrowerStreamEffect = ObjectPooling.Spawn(FiringEffect).GetComponentInChildren<ParticleSystem>();
            _FlamethrowerStreamEffect.GetComponentInChildren<ParticleBasedDamage>().SetWeaponAttached(this);
            _FlamethrowerStreamEffect.transform.position = _MuzzleLaunchPoints[_MuzzleIterator].position;
            _FlamethrowerStreamEffect.transform.rotation = _MuzzleLaunchPoints[_MuzzleIterator].rotation;

            // Play
            _FlamethrowerStreamEffect.Stop();
            var main = _FlamethrowerStreamEffect.main;
            main.duration = FiringDuration;
            _FlamethrowerStreamEffect.Play();
            _FlamethrowerStreamIsPlaying = true;

            if (LoopingFire) {
                
                // Despawn the effect
                AutoDespawn auto = _FlamethrowerStreamEffect.GetComponentInParent<AutoDespawn>();
                StartCoroutine(StopFlamethrowerStream());
                StartCoroutine(auto.DelayedDespawnSeconds((int)FiringDuration));
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator StopFlamethrowerStream() {
        
        yield return new WaitForSeconds(FiringDuration);
        _FlamethrowerStreamIsPlaying = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}