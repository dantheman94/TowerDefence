using System.Collections;
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

public class Core : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" CORE PROPERTIES")]
    [Space]
    public GameObject MinimapQuad = null;
    public float SeekSphereRadius = 80f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" SPIRES")]
    [Space]
    public Spire SpireA;
    public Spire SpireB;
    public Spire SpireC;

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
        if (_MinimapRenderer != null) { _MinimapRenderer.material.color = Color.blue; }
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

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Vector3
    /// </returns>
    public Vector3 GetSeekPosition() { return Random.onUnitSphere * SeekSphereRadius; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}