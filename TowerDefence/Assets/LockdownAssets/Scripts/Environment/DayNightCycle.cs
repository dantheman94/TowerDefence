using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 10/09/2018
//
//******************************

// Main reference:
// http://twiik.net/articles/simplest-possible-day-night-cycle-in-unity-5

public class DayNightCycle : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" DAY/NIGHT CYCLE PROPERTIES")]
    [Space]
    [Tooltip("Length of each day in seconds.")]
    public float _DayDuration = 24.0f;
    [Tooltip("The speed of each day.")]
    public float _DaySpeed = 1.0f;
    public float _CurrentTime = 0.0f;
    [Space]
    public Light Sun;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    public float _CurrentDay;
    float _SunInitialIntensity;
    bool _IsNightTime;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start () {

        // Set the initial intensity
        _SunInitialIntensity = Sun.intensity;
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update () {

        // Update the sun
        UpdateSun();

        // Update the time
        _CurrentTime += (Time.deltaTime / _DayDuration) * _DaySpeed;

        // If the current time of day >= 1 then it needs to be set to 0 to start the next one
        if (_CurrentTime >= 1) { _CurrentTime = 0; }

        if (_CurrentTime >= 0.75 && _CurrentTime <= 0.25) { _IsNightTime = true; }

        else { _IsNightTime = false; }
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void UpdateSun() {

        // Set the sun's transform
        Sun.transform.localRotation = Quaternion.Euler((_CurrentTime * 360.0f) - 90, 170, 0);

        // Intensity multiplier
        float intensityMultiplier = 1.0f;

        if (_CurrentTime <= 0.23f || _CurrentTime >= 0.75f) { intensityMultiplier = 0.0f; }

        else if (_CurrentTime <= 0.23f) {

            intensityMultiplier = Mathf.Clamp01((_CurrentTime - 0.23f) * (1 / 0.02f));
        }

        else if (_CurrentTime >= 0.73f) {

            intensityMultiplier = Mathf.Clamp01(1 - ((_CurrentTime - 0.73f) * (1 / 0.02f)));
        }

        // Set the sun's intensity
        Sun.intensity = _SunInitialIntensity * intensityMultiplier;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
