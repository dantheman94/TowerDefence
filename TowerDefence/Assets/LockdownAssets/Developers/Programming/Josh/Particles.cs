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
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Awake() {

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update() {

        if (_Player == null) {

            _Player = GameManager.Instance.Players[0];
        }

        else {

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
                    // Set to false
                    gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Play();
                    gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Play();

                    Debug.Log("Object is within the camera view.");
                }

                // If the particle object is not within the rect
                else {

                    // Set to false
                    _IsInScreenSpace = false;
                    _Renderer.enabled = false;
                    // Set to true
                    gameObject.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().Pause();
                    gameObject.transform.GetChild(1).gameObject.GetComponent<ParticleSystem>().Pause();

                    Debug.Log("Object is not within the camera view.");
                }
            }

            // If the player is using a gamepad
            if (_Player._XboxGamepadInputManager.IsPrimaryController) {

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
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}