using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 7/9/2018
//
//******************************

public class UI_LockdownPadHUD : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UI PROPERTIES")]
    [Space]
    [Space]
    public Text TextAttached = null;
    public float FlashTime = 3f;
    [Space]
    public Color ColourA = Color.black;
    public Color ColourB = Color.white;
    public float ColourTransitionSpeed = 1f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _Flashing = false;
    private float _TimerOnScreen = 0f;

    private Color _Colour;
    private Color _FlashEndColour;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        if (_Flashing) {

            // Lerp between Colour A & B
            _Colour = Color.Lerp(ColourA, ColourB, (Mathf.Sin(Time.time * ColourTransitionSpeed) + 1) / 2f);
            if (TextAttached != null) { TextAttached.color = _Colour; }

            // Add to timer
            _TimerOnScreen += Time.deltaTime;
            if (_TimerOnScreen >= FlashTime) {
                
                // Stop flashing
                _Flashing = false;
                _TimerOnScreen = 0f;

                _FlashEndColour = _Colour;
            }
        }

        // Not flashing
        else {

            // Return text color back to black
            _Colour = Color.Lerp(_FlashEndColour, Color.black, (Mathf.Sin(Time.time * ColourTransitionSpeed) + 1) / 2f);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Starts the flashing sequence.
    /// </summary>
    public void Flash() {

        _TimerOnScreen = 0f;
        _Flashing = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
