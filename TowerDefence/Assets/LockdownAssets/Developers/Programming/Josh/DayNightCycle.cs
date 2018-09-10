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

    public float _DayDuration = 240.0f; // 24 x 10 (In seconds)
    public float _CurrentTimeOfDay = 0.0f;
    public float _CurrentDay;
    public float _TimeMultiplier = 1.0f;
    float _SunInitialIntensity;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    public Light Sun;

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
        _CurrentTimeOfDay += (Time.deltaTime / _DayDuration) * _TimeMultiplier;

        // If the current time of day >= 1 then it needs to be set to 0 to start the next one
        if (_CurrentTimeOfDay >= 1) { _CurrentTimeOfDay = 0; }
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void UpdateSun() {

        Sun.transform.localRotation = Quaternion.Euler((_CurrentTimeOfDay * 360.0f) - 90, 170, 0);

        float intensityMultiplier = 1.0f;

        if (_CurrentTimeOfDay <= 0.23f || _CurrentTimeOfDay >= 0.75f) { intensityMultiplier = 0.0f; }

        else if (_CurrentTimeOfDay <= 0.23f) {

            intensityMultiplier = Mathf.Clamp01((_CurrentTimeOfDay - 0.23f) * (1 / 0.02f));
        }

        else if (_CurrentTimeOfDay >= 0.73f) {

            intensityMultiplier = Mathf.Clamp01(1 - ((_CurrentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }

        Sun.intensity = _SunInitialIntensity * intensityMultiplier;

        // Log the current time of day
        Debug.Log("Current time of day is: " + _CurrentTimeOfDay);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
