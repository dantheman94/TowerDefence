using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 22/6/2018
//
//******************************

public class Projectile : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header("MOVEMENT")]
    [Space]
    public float _MovementSpeed = 40f;
    public float _MaxDistance = 1000f;

    [Space]
    [Header("-----------------------------------")]
    [Header("DAMAGE")]
    [Space]
    public int DamageLightMarine = 1;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    private float _DistanceTravelled  = 0f;
    private Vector3 _Velocity;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    private void Start () {

        // Set forward velocity
        _Velocity = transform.forward;
	}

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update () {
        
        // Constantly move forward
        transform.position += _Velocity * _MovementSpeed * Time.deltaTime;
        
        // Re-pool projectile when it has reached max distance threshold
        if (_DistanceTravelled < _MaxDistance) _DistanceTravelled += Time.deltaTime;
        else { RepoolProjectile(); }
    }

    /// <summary>
    //  Sends the projectile back to the object pool
    //  (derived projectile classes should determine which pool to go to)
    /// </summary>
    protected virtual void RepoolProjectile() { }

}