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

        _ParticleSystem = GetComponent<ParticleSystem>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        
        if (_ParticleSystem != null) {

            // Wait until the particle has finished its cycle
            if (!_ParticleSystem.IsAlive()) {

                // Despawn
                ObjectPooling.Despawn(gameObject);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
