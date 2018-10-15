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

    private ParticleSystem _ParticleSystem = null;

    private void Start() {

        _ParticleSystem = GetComponent<ParticleSystem>();
    }

    private void Update() {
        
        if (_ParticleSystem != null) {

            if (!_ParticleSystem.IsAlive()) {

                ObjectPooling.Despawn(transform.parent.gameObject);
            }
        }
    }

}
