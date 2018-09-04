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

public class UI_ScreenFade : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SCREEN FADE")]
    [Space]
    public Image ImageComponent = null;
        
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    public static UI_ScreenFade Instance;
        
    private float _FadeTime = 100f;
    private Color _StartColour;
    private Color _TargetColour;

    private float _CurrentLerpTime = 0f;
    private bool _IsFading = false;

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

        if (ImageComponent != null) {

            if (_IsFading = ImageComponent.enabled) {

                // Increment lerp timer once per frame
                _CurrentLerpTime += Time.deltaTime;
                if (_CurrentLerpTime > _FadeTime) { _CurrentLerpTime = _FadeTime; }

                // Lerp image colour to the target colour
                float percent = _CurrentLerpTime / _FadeTime;
                ImageComponent.color = Color.Lerp(_StartColour, _TargetColour, percent);

                // Check for fade out (we need to hide the image component if so)
                if (ImageComponent.color.a <= 0f) { ImageComponent.enabled = false; }
            }

            // Reset the current lerp time since the image component is disabled
            else { _CurrentLerpTime = 0f; }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Starts a screen fade animation sequence.
    /// </summary>
    /// <param name="directon"></param>
    public void StartAnimation(Color startCol, Color endcol, float fadeTime) {

        if (!_IsFading) {

            // Set values
            _StartColour = startCol;
            _TargetColour = endcol;
            _FadeTime = fadeTime;
            if (ImageComponent != null) { ImageComponent.color = startCol; }

            // Begin animation
            ImageComponent.enabled = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
