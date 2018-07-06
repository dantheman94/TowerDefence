using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 6/7/2018
//
//******************************

public class MobileArtillery : Vehicle {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected bool _Stationary = false;
    protected float _MobileAcceleration;
    protected float _MobileDecceleration;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        _MobileAcceleration = Acceleration;
        _MobileDecceleration = Deceleration;
    }

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Force disable manual unit control
        CanBePlayerControlled = false;
        _IsBeingPlayerControlled = false;

        // Flip mobility state
        if (Input.GetKeyDown(KeyCode.R)) { _Stationary = !_Stationary; }

        // Vehicle is stationary
        if (_Stationary) {

            Acceleration = 0;
            Deceleration = 0;
            _CurrentSpeed = 0;

            _Agent.isStopped = true;
            _Agent.enabled = false;
            _Agent.speed = 0;
            _IsSeeking = false;
            _SeekWaypoint.SetActive(false);
        }

        // Vehicle is mobile
        else {

            // Reset movement speeds
            Acceleration = _MobileAcceleration;
            Deceleration = _MobileDecceleration;

            _Agent.enabled = true;
        }
    }

}