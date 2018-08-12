using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 12/8/2018
//
//******************************

public class Spire : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SPIRE PROPERTIES")]
    [Space]
    public GameObject MinimapQuad = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Renderer _MinimapRenderer;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        // Get component references
        if (MinimapQuad != null) { _MinimapRenderer = MinimapQuad.GetComponent<Renderer>(); }

        // Set minimap icon colour to blue
        if (_MinimapRenderer != null) { _MinimapRenderer.material.color = Color.cyan; }
    }

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
                _HealthBar.SetObjectAttached(this);
                healthBarObj.gameObject.SetActive(true);
                healthBarObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);

                if (_Player == null) {

                    Player plyr = GameManager.Instance.Players[0];
                    _HealthBar.SetCameraAttached(plyr.PlayerCamera);
                }
                else { _HealthBar.SetCameraAttached(_Player.PlayerCamera); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
