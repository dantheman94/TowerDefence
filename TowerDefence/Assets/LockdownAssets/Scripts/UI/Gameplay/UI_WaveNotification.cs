using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/8/2018
//
//******************************

public class UI_WaveNotification : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" COMPONENTS")]
    [Space]
    public Text WaveNameText = null;
    public float TimeOnScreen = 3f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _OnScreen = false;
    private float _TimerOnScreen = 0f;

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
    //  
    /// </summary>
    public void NewWaveNotification(WaveManager.WaveInfo waveInfo) {

        if (WaveNameText != null) {

            WaveNameText.text = waveInfo.Name;
            _TimerOnScreen = 0f;
            _OnScreen = true;
            gameObject.SetActive(true);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
