using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/7/2018
//
//******************************

public class CinematicBars : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" CINEMATIC BARS")]
    [Space]
    [Range(100f, 300f)]
    public float MovementSpeed;
    public GameObject UpperBar;
    public GameObject LowerBar;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum AnimationDirection { Enter, Exit }

    public static CinematicBars Instance;

    private AnimationDirection _Direction;
    private bool _AnimationComplete = true;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  This is called before Startup().
    /// </summary>
    private void Awake() {

        // Initialize singleton
        if (Instance != null && Instance != this) {

            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        
        if (!_AnimationComplete) {

            switch (_Direction) {

                // Move cinematic bars inwards
                case AnimationDirection.Enter:

                    if (UpperBar != null && LowerBar != null) {

                        if (LowerBar.transform.position.y < 0) {

                            // Move upper bar down
                            Vector3 upPos = new Vector3(UpperBar.transform.position.x, UpperBar.transform.position.y - MovementSpeed * Time.deltaTime, UpperBar.transform.position.z);
                            UpperBar.transform.position = upPos;

                            // Move lower bar up
                            Vector3 lowPos = new Vector3(LowerBar.transform.position.x, LowerBar.transform.position.y + MovementSpeed * Time.deltaTime, LowerBar.transform.position.z);
                            LowerBar.transform.position = lowPos;
                        }
                        else { _AnimationComplete = true; }
                    }
                    break;

                case AnimationDirection.Exit:

                    if (UpperBar != null && LowerBar != null) {

                        if (LowerBar.transform.position.y > -100) {

                            // Move upper bar up
                            Vector3 upPos = new Vector3(UpperBar.transform.position.x, UpperBar.transform.position.y + MovementSpeed * Time.deltaTime, UpperBar.transform.position.z);
                            UpperBar.transform.position = upPos;

                            // Move lower bar down
                            Vector3 lowPos = new Vector3(LowerBar.transform.position.x, LowerBar.transform.position.y - MovementSpeed * Time.deltaTime, LowerBar.transform.position.z);
                            LowerBar.transform.position = lowPos;
                        }
                        else { _AnimationComplete = true; }
                    }
                    break;

                default: break;
            }

        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="directon"></param>
    public void StartAnimation(AnimationDirection directon) {

        // Set direction
        _Direction = directon;

        // Begin animation
        _AnimationComplete = false;
    }

}