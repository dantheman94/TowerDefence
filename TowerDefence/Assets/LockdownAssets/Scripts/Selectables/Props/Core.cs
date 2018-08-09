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

        // Initialize spires
        if (SpireA != null) { SpireA.SetPlayer(_Player); }
        if (SpireB != null) { SpireB.SetPlayer(_Player); }
        if (SpireC != null) { SpireC.SetPlayer(_Player); }
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
            else { CreateHealthBar(this, _Player.PlayerCamera); }
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

    /// <summary>
    //  Damages the object by a set amount.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
    public override void Damage(int damage, Ai instigator = null) {

        // Only damage the core if all spires are destroyed
        if (SpireA.GetHitPoints() <= 0 && SpireB.GetHitPoints() <= 0 && SpireC.GetHitPoints() <= 0) {

            // Damage the core
            base.Damage(damage, instigator);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}