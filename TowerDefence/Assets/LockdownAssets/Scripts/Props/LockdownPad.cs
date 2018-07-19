using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/7/2018
//
//******************************

public class LockdownPad : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MINIMAP PROPERTIES")]
    [Space]
    public GameObject MinimapQuad = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" LOCKDOWNPAD PROPERTIES")]
    [Space]
    public BuildingSlot BuildingSlotAttached = null;

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
    private void Start() {

        // Get component references
        if (MinimapQuad != null) { _MinimapRenderer = MinimapQuad.GetComponent<Renderer>(); }

        // Set minimap icon colour to red
        if (_MinimapRenderer != null) { _MinimapRenderer.material.color = Color.red; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}
