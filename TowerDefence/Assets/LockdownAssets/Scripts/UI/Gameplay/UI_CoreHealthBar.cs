using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 1/9/2018
//
//******************************

public class UI_CoreHealthBar : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" CORE OBJECTS")]
    [Space]
    public Core CoreObject = null;
    public Spire SpireA = null;
    public Spire SpireB = null;
    public Spire SpireC = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" UI OBJECTS")]
    [Space]
    public Slider SliderBareCoreHealth = null;
    public Slider SliderBareCoreShield = null;
    public Slider SliderBareSpireAHealth = null;
    public Slider SliderBareSpireBHealth = null;
    public Slider SliderBareSpireCHealth = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

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

        // Update core health
        if (CoreObject != null && SliderBareCoreHealth != null) {

            float val = CoreObject.GetHealth();
            SliderBareCoreHealth.value = val;
        }

        // Update core shield
        if (CoreObject != null && SliderBareCoreShield != null) {

            float val = CoreObject.GetShield();
            SliderBareCoreShield.value = val;
        }

        // Update spireA health
        if (SpireA != null && SliderBareSpireAHealth != null) {

            float val = SpireA.GetHealth();
            SliderBareSpireAHealth.value = val;
        }

        // Update spireB health
        if (SpireB != null && SliderBareSpireBHealth != null) {

            float val = SpireB.GetHealth();
            SliderBareSpireBHealth.value = val;
        }

        // Update spireC health
        if (SpireC != null && SliderBareSpireCHealth != null) {

            float val = SpireC.GetHealth();
            SliderBareSpireCHealth.value = val;
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}