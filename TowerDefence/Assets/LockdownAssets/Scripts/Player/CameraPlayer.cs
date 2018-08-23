using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/8/2018
//
//******************************

public class CameraPlayer : MonoBehaviour {

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
    public float TransparencyLevel = 120f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Renderer _MinimapRenderer;
    private Player _PlayerAttached = null;

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
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayerAttached(Player player) {

        // Set player reference
        _PlayerAttached = player;

        // Set colour reference
        if (_PlayerAttached && _MinimapRenderer != null) {

            Color col = _PlayerAttached.TeamColor;
            col.a = TransparencyLevel;
            _MinimapRenderer.material.SetColor("Albedo", col);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
