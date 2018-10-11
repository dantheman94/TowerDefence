using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/10/2018
//
//******************************

public class ParticleDespawner : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public static ParticleDespawner Instance;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  This is called before Startup().
    /// </summary>
    private void Awake() {

        // Initialize singleton
        if (Instance != null && Instance != this) {

            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits for the seconds specified then attempts to repool
    //  the particle effect (or destroyed entirely if re-pooling isn't possible)
    /// </summary>
    /// <param name="particleEffect"></param>
    /// <param name="delay"></param>
    public IEnumerator ParticleDespawn(ParticleSystem particleEffect, float delay) {

        // Delay
        ///yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => !particleEffect.isPlaying);

        // Despawn the system
        ObjectPooling.Despawn(particleEffect.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits for the seconds specified then attempts to repool
    //  the particle effect (or destroyed entirely if re-pooling isn't possible)
    /// </summary>
    /// <param name="particleEffect"></param>
    /// <param name="delay"></param>
    public IEnumerator ParticleParentDespawn(ParticleSystem particleEffect, float delay) {

        // Delay
        ///yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => !particleEffect.isPlaying);

        // Despawn the system
        ObjectPooling.Despawn(particleEffect.transform.parent.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
