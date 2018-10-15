using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 15/10/2018
//
//******************************

public class Particles : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player;
    private Renderer _Renderer;

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

        _Renderer = GetComponent<Renderer>();
        _Renderer.enabled = true;
        _Player = GameManager.Instance.Players[0];
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update()
    {

        // Calculate distance from the current position to the screen center
        _DistanceFromScreenCenter = Vector3.Distance(transform.position, _ScreenCenter);

        // Get player camera position and store it into a Vector3
        Vector3 camPos = _Player.CameraAttached.WorldToScreenPoint(transform.position);

        // If the player is using a keyboard
        if (_Player._KeyboardInputManager.IsPrimaryController) {

            // If the particle object is within the rect
            if (Player.SelectionScreen.Contains(camPos)) {

                // Set to true
                _IsInScreenSpace = true;
                _Renderer.enabled = true;
            }

            // If the particle object is not within the rect
            else {

                // Set to false
                _IsInScreenSpace = false;
                _Renderer.enabled = false;
            }
        }

        // If the player is using a gamepad
        if (_Player._XboxGamepadInputManager.IsPrimaryController)
        {

            // If the particle object is within the rect
            if (Player.SelectionScreen.Contains(camPos))
            {

                // Set to true
                _IsInScreenSpace = true;
                _Renderer.enabled = true;
            }

            // If the particle object is not within the rect
            else
            {

                // Set to false
                _IsInScreenSpace = false;
                _Renderer.enabled = false;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}