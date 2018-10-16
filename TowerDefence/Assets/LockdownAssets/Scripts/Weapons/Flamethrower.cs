using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 15/10/2018
//
//******************************

public class Flamethrower : Weapon {

    protected override void ProjectileRaycast() {
        base.ProjectileRaycast();

        // Play particle effect stream
        if (FiringEffect != null) {

            // Spawn
            ParticleSystem effect = ObjectPooling.Spawn(FiringEffect).GetComponentInChildren<ParticleSystem>();
            effect.GetComponentInChildren<ParticleBasedDamage>().SetWeaponAttached(this);
            effect.transform.position = _MuzzleLaunchPoints[_MuzzleIterator].position;
            effect.transform.rotation = _MuzzleLaunchPoints[_MuzzleIterator].rotation;

            // Play
            effect.Stop();
            var main = effect.main;
            main.duration = FiringDuration;
            effect.Play();

            if (LoopingFire) {
                
                AutoDespawn auto = effect.GetComponentInParent<AutoDespawn>();
                StartCoroutine(auto.DelayedDespawnSeconds((int)FiringDuration));
            }
        }
    }

}