using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class FogOfWar : MonoBehaviour {

    //***************************************************************
    // INSPECTOR
    
    [HideInInspector]
    public float _Radius;
    
    [Header("Tier 2")]
    public float _FogOfWarTier2Radius;

    [Space]
    [Header("Tier 3")]
    public float _FogOfWarTier3Radius;

    [Space]
    [Header("Tier 4")]
    public float _FogOfWarTier4Radius;

    [Space]
    [Header("Tier 5")]
    public float _FogOfWarTier5Radius;

    private SphereCollider _SphereCollider;

    //***************************************************************
    // VARIABLES

    //***************************************************************
    // FUNCTIONS

    private void Awake () {

        // Get component references
        _SphereCollider = GetComponent<SphereCollider>();
        _Radius = _SphereCollider.radius;
    }
	
	private void Update () {

        // Update sphere radius
        _SphereCollider.radius = _Radius;
    }

}