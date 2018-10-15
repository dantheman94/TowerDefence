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
    public Text WaveNameTitle = null;
    public Text WaveNameDescription = null;
    public Image HorizontalLine = null;
    [Space]
    public float FadeInSpeed = 2f;
    public float TimeOnScreen = 3f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _OnScreen = false;
    private float _TimerOnScreen = 0f;

    public bool _BossWave { get; set; }

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

        if (WaveNameDescription != null) {

            // Next wave is a boss wave - only show title text
            if (_BossWave) {

                WaveNameTitle.text = "BOSS WAVE";
                _TimerOnScreen = 0f;
                gameObject.SetActive(true);
                _OnScreen = true;
                WaveNameDescription.enabled = false;
            }

            // Not a boss wave
            else {

                WaveNameTitle.text = "NEW WAVE";
                WaveNameDescription.text = waveInfo.Name;
                _TimerOnScreen = 0f;
                _OnScreen = true;
                gameObject.SetActive(true);
                WaveNameDescription.enabled = true;
            }
            ///StartCoroutine(FadeInWidget());
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Enuermator
    /// </returns>
    private IEnumerator FadeInWidget() {

        // Hide components
        if (WaveNameTitle != null)          { WaveNameTitle.color = new Color(1, 1, 1, 0); }
        if (WaveNameDescription != null)    { WaveNameDescription.color = new Color(1, 1, 1, 0); }
        if (HorizontalLine != null)         { HorizontalLine.color = new Color(1, 1, 1, 0); }

        float a1 = 0f;
        float a2 = 0f;

        bool fadingInComplete = false;
        while(!fadingInComplete) {

            // Fade in title
            a1 += Time.deltaTime * FadeInSpeed;
            if (WaveNameTitle != null)  { WaveNameTitle.color = new Color(1, 1, 1, a1); }
            if (HorizontalLine != null) { HorizontalLine.color = new Color(1, 1, 1, a1); }
            if (a1 >= 1f) { a1 = 1f; }
            yield return new WaitForEndOfFrame();

            // Title has faded in?
            yield return new WaitUntil(() => (a1 >= 1f));

            // Fade in description
            a2 += Time.deltaTime * FadeInSpeed;
            if (WaveNameDescription != null) { WaveNameDescription.color = new Color(1, 1, 1, a2); }
            if (a2 >= 1f) { a2 = 1f; }
            yield return new WaitForEndOfFrame();

            // Description has faded in?
            yield return new WaitUntil(() => (a2 >= 1f));

            fadingInComplete = true;
            _OnScreen = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
