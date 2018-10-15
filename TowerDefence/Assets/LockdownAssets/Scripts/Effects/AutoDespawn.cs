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
    // Called when the gameObject is created.
    /// </summary>
    private void Start() {

        _ParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

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
