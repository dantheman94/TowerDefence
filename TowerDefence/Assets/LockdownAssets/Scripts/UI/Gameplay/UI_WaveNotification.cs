using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/11/2018
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

    [Space]
    [Header("-----------------------------------")]
    [Header(" FADING WIDGET")]
    [Space]
    public Text NewWaveTitle = null;
    public Text WaveNameText = null;
    public Image LineHorizontal = null;
    [Space]
    public float FadeDuration = 0.5f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _OnScreen = false;
    private float _TimerOnScreen = 0f;

    public bool _BossWave { get; set; }

    private bool _FadingOut = false;
    private bool _FadingIn = false;
    private float _FadeProgress = 0f;
    private Color _FadeColour;

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
                _OnScreen = false;
                _TimerOnScreen = 0f;
                _FadingOut = true;
            }
        }

        // Widget fading in sequence
        if (_FadingIn) {

            // Lerp the widget components into full alpha
            _FadeProgress -= Time.deltaTime / FadeDuration;
            _FadeColour = Color.Lerp(Color.white, Color.clear, _FadeProgress);

            if (NewWaveTitle != null) { NewWaveTitle.color = _FadeColour; }
            if (WaveNameText != null) { WaveNameText.color = _FadeColour; }
            if (LineHorizontal != null) { LineHorizontal.color = _FadeColour; }

            // Fade in complete
            if (_FadeColour == Color.white) {

                _FadingIn = false;
                _FadeProgress = 0f;
            }
        }

        // Widget fading out sequence
        if (_FadingOut) {

            // Lerp the widget components into full transparency
            _FadeProgress += Time.deltaTime / FadeDuration;
            _FadeColour = Color.Lerp(Color.white, Color.clear, _FadeProgress);

            if (NewWaveTitle != null) { NewWaveTitle.color = _FadeColour; }
            if (WaveNameText != null) { WaveNameText.color = _FadeColour; }
            if (LineHorizontal != null) { LineHorizontal.color = _FadeColour; }

            // Fade out complete
            if (_FadeColour == Color.clear) {

                _FadingOut = false;
                _FadeProgress = 0f;
                gameObject.SetActive(false);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called whenever a new wave starts. Displays the wave name as a sub-heading in the center of the screen.
    /// </summary>
    public void NewWaveNotification(WaveManager.WaveInfo waveInfo) {
        
        gameObject.SetActive(true);
        StartCoroutine(WaitForWidgetAvailiablity(waveInfo));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator WaitForWidgetAvailiablity(WaveManager.WaveInfo waveInfo) {
        
        yield return new WaitUntil(() => !WaveManager.Instance.WaveCompleteWidget.gameObject.activeInHierarchy);

        // Play new wave starting sound
        SoundManager.Instance.PlaySound("SFX/_SFX_WaveStart", 1f, 1f);

        if (WaveNameDescription != null) {

            // Next wave is a boss wave - only show title text
            if (_BossWave) {

                WaveNameTitle.text = "BOSS WAVE";
                _TimerOnScreen = 0f;
                _OnScreen = true;
                WaveNameDescription.enabled = false;
            }

            // Not a boss wave
            else {

                WaveNameTitle.text = "NEW WAVE";
                WaveNameDescription.text = waveInfo.Name;
                _TimerOnScreen = 0f;
                _OnScreen = true;
                WaveNameDescription.enabled = true;
            }

            // Reset center message colours
            _FadeColour = Color.white;
            if (NewWaveTitle != null) { NewWaveTitle.color = _FadeColour; }
            if (WaveNameText != null) { WaveNameText.color = _FadeColour; }
            if (LineHorizontal != null) { LineHorizontal.color = _FadeColour; }
            _FadingIn = true;
            _FadeProgress = 1f;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
