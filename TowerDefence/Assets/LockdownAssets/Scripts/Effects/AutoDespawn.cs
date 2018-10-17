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

public class AutoDespawn : MonoBehaviour {
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private ParticleSystem _ParticleSystem = null;
    private bool _DespawnNow = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        if (_ParticleSystem != null) {

            // Wait until the particle has finished its cycle (for force to stop)
            if (!_ParticleSystem.IsAlive() || _DespawnNow) {

                // Despawn
                ObjectPooling.Despawn(gameObject);
            }
        }

        // Get reference to the particle system
        else {

            _ParticleSystem = GetComponent<ParticleSystem>();
            if (_ParticleSystem == null) { _ParticleSystem = GetComponentInChildren<ParticleSystem>(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Waits for the specified seconds, then despawns the particle effect next frame.
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns>
    //  IEnumerator
    /// </returns>
    public IEnumerator DelayedDespawnSeconds(int seconds) {

        // Wait for seconds specified
        yield return new WaitForSeconds(seconds);

        // Despawn next frame
        _DespawnNow = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
