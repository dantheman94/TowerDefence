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

    [Space]
    [Header("-----------------------------------")]
    [Header(" FLASHING TEXT")]
    [Space]
    public Text Text_Core = null;
    public Text Text_SpireA = null;
    public Text Text_SpireB = null;
    public Text Text_SpireC = null;
    [Space]
    public float FlashingTextSpeed = 1f;
    public Color FlashStartingColour = Color.black;
    public Color FlashEndColour = Color.red;
    [Space]
    public float FlashDuration = 2f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum ObjectFlashing { Core, SpireA, SpireB, SpireC }

    private bool _Flashing = false;
    private Color _CurrentFlashColour = Color.white;
    private bool _Core = false;
    private bool _SpireA = false;
    private bool _SpireB = false;
    private bool _SpireC = false;
    private float _TimerCore = 0f;
    private float _TimerSpireA = 0f;
    private float _TimerSpireB = 0f;
    private float _TimerSpireC = 0f;

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

        if (_Flashing) {

            // Lerp between Colour A & B
            _CurrentFlashColour = Color.Lerp(FlashStartingColour, FlashEndColour, (Mathf.Sin(Time.time * FlashingTextSpeed) + 1) / 2f);

            // Set the text component(s) colour
            if (_Core) {

                Text_Core.color = _CurrentFlashColour;
                _TimerCore += Time.deltaTime;
                if (_TimerCore >= FlashDuration) {

                    _Core = false;
                    _TimerCore = 0f;
                }
            }
            if (_SpireA) {

                Text_SpireA.color = _CurrentFlashColour;
                _TimerSpireA += Time.deltaTime;
                if (_TimerSpireA >= FlashDuration) {

                    _Core = false;
                    _TimerSpireA = 0f;
                }
            }
            if (_SpireB) {

                Text_SpireA.color = _CurrentFlashColour;
                _TimerSpireB += Time.deltaTime;
                if (_TimerSpireB >= FlashDuration) {

                    _Core = false;
                    _TimerSpireB = 0f;
                }
            }
            if (_SpireC) {

                Text_SpireC.color = _CurrentFlashColour;
                _TimerSpireC += Time.deltaTime;
                if (_TimerSpireC >= FlashDuration) {

                    _Core = false;
                    _TimerSpireC = 0f;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="flash"></param>
    public void SetFlashingText(ObjectFlashing obj, bool flash) {

        _Flashing = flash;

        // Flashing text
        if (flash) {

            switch (obj) {

                case ObjectFlashing.Core:
                    _Core = true;
                    break;

                case ObjectFlashing.SpireA:
                    _SpireA = true;
                    break;

                case ObjectFlashing.SpireB:
                    _SpireB = true;
                    break;

                case ObjectFlashing.SpireC:
                    _SpireC = true;
                    break;

                default: break;
            }
        }

        // No flashing text
        else {

            switch (obj) {

                case ObjectFlashing.Core:
                    _Core = false;
                    break;

                case ObjectFlashing.SpireA:
                    _SpireA = false;
                    break;

                case ObjectFlashing.SpireB:
                    _SpireB = false;
                    break;

                case ObjectFlashing.SpireC:
                    _SpireC = false;
                    break;

                default: break;
            }
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}