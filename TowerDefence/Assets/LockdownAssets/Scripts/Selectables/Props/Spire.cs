﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 14/7/2018
//
//******************************

public class Spire : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // object is active in the world
        if (_ObjectState == WorldObjectStates.Active && IsAlive()) {

            // Show the healthbar
            if (_HealthBar != null) { _HealthBar.gameObject.SetActive(true); }

            // Create a healthbar if the unit doesn't have one linked to it
            else {

                GameObject healthBarObj = ObjectPooling.Spawn(GameManager.Instance.UnitHealthBar.gameObject);
                _HealthBar = healthBarObj.GetComponent<UnitHealthBar>();
                _HealthBar.setObjectAttached(this);
                healthBarObj.gameObject.SetActive(true);
                healthBarObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);

                if (_Player == null) {

                    Player plyr = GameManager.Instance.Players[0];
                    _HealthBar.setCameraAttached(plyr.PlayerCamera);
                }
                else { _HealthBar.setCameraAttached(_Player.PlayerCamera); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
