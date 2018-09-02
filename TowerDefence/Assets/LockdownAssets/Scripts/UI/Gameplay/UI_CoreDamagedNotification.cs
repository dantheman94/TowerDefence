using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 2/9/2018
//
//******************************

public class UI_CoreDamagedNotification : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" COMPONENTS")]
    [Space]
    public Text NotificationText = null;
    public float TimeOnScreen = 3f;
    [Space]
    public Color ColourA = Color.black;
    public Color ColourB = Color.white;
    public float ColourTransitionSpeed = 1f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _OnScreen = false;
    private float _TimerOnScreen = 0f;

    private Color _Colour;

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

        if (_OnScreen) {

            // Lerp between Colour A & B
            _Colour = Color.Lerp(ColourA, ColourB, (Mathf.Sin(Time.time * ColourTransitionSpeed) + 1) / 2f);
            if (NotificationText != null) { NotificationText.color = _Colour; }

            // Add to timer
            _TimerOnScreen += Time.deltaTime;
            if (_TimerOnScreen >= TimeOnScreen) {

                // Hide widget
                gameObject.SetActive(false);
                _OnScreen = false;
                _TimerOnScreen = 0f;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Starts the core damaged warning process.
    /// </summary>
    public void ShowNotification() {

        // Reset timers and display the widget gameObject
        _TimerOnScreen = 0f;
        _OnScreen = true;
        gameObject.SetActive(true);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
