using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 9/10/2018
//
//******************************

public class Airpad : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" AIRPAD PROPERTIES")]
    [Space]
    public float LightUpDuration = 2f;
    public float MinLuminosity = 0f;
    public float MaxLuminosity = 5f;
    [Space]
    public List<Light> _LightGroupA;
    public List<Light> _LightGroupB;
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();
        
        // Start the light sequence
        if (_BuildingStarted) {

            for (int i = 0; i < _LightGroupA.Count; i++) { StartCoroutine(FadeInAndOut(_LightGroupA[i], 1f)); }
            for (int i = 0; i < _LightGroupB.Count; i++) { StartCoroutine(DelayedFadeInAndOut(_LightGroupB[i], 1f, LightUpDuration)); }
            _BuildingStarted = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="light"></param>
    /// <param name="waitTime"></param>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator FadeInAndOut(Light light, float waitTime) {

        WaitForSeconds waitForX = new WaitForSeconds(waitTime);

        while(_IsBuildingSomething) {

            // Fade out
            yield return FadeLerp(light, false);

            // Wait
            yield return waitForX;

            // Fade in
            yield return FadeLerp(light, true);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="light"></param>
    /// <param name="waitTime"></param>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator DelayedFadeInAndOut(Light light, float waitTime, float initialDelay) {

        yield return new WaitForSeconds(initialDelay);

        WaitForSeconds waitForX = new WaitForSeconds(waitTime);

        while (_IsBuildingSomething) {

            // Fade out
            yield return FadeLerp(light, false);

            // Wait
            yield return waitForX;

            // Fade in
            yield return FadeLerp(light, true);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="light"></param>
    /// <param name="fadeIn"></param>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator FadeLerp(Light light, bool fadeIn) {

        // Restart timer for lerp
        float counter = 0f;

        // Set luminosity targets 
        float a, b;
        if (fadeIn) {

            a = MinLuminosity;
            b = MaxLuminosity;
        }
        else {

            a = MaxLuminosity;
            b = MinLuminosity;
        }
        float current = light.intensity;

        // Lerp through the target intensities
        while (counter < LightUpDuration) {

            counter += Time.deltaTime;
            light.intensity = Mathf.Lerp(a, b, counter / LightUpDuration);
            yield return null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
