using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/8/2018
//
//******************************

public class PlayerManager : MonoBehaviour {

    //******************************************************************************************************************************
    // INSPECTOR

    public Player _PlayerOne;

    //******************************************************************************************************************************
    // VARIABLES

    public static PlayerManager _Instance;

    //******************************************************************************************************************************
    // FUNCTIONS

    private void Awake() {

        // If the singleton has already been initialized
        if (_Instance != null && _Instance != this) {

            Destroy(this.gameObject);
            return;
        }

        // Set singleton
        _Instance = this;
    }

}
