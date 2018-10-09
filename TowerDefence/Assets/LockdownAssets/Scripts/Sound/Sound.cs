using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 3/09/2018
//
//******************************

public class Sound : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player       _Player;
    private SoundManager _SoundManager;
    private AudioSource  _AudioSource;

    public static Rect Selection = new Rect(0, 0, 0, 0);
    public bool _IsInScreenSpace = false;
    private Vector3 _ScreenCenter;
    private float _DistanceFromScreenCenter;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start () {
		_AudioSource = GetComponent<AudioSource>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update () {

        // Calculate distance from the current position to the screen center
        _DistanceFromScreenCenter = Vector3.Distance(transform.position, _ScreenCenter);

        // Get player camera position and store it into a Vector3
        Vector3 camPos = _Player.CameraAttached.WorldToScreenPoint(transform.position);

        // The y axis is upside down so we need to invert it
        camPos.y = KeyboardInput.InvertMouseY(camPos.y);

        // If the sound object is within the rect
        if (KeyboardInput.Selection.Contains(camPos)) {

            // Set to true
            _IsInScreenSpace = true;
        }

        // If the sound object is not within the rect
        else {

            // Set to false
            _IsInScreenSpace = false;
            _AudioSource.pitch = 0;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}