using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 27/7/2018
//
//******************************

public class Platoon : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" WIDGET COMPONENTS")]
    [Space]
    public TMP_Text PlatoonCounter = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" GROUP BLOCK COLOURS ")]
    [Space]
    public Image BlockImage = null;
    public float BlockLerpDuration = 1f;
    [Space]
    public Color DefaultBlockColour = Color.white;
    public Color HighlightingBlockColour = Color.cyan;
    [Space]
    public Color DefaultBlockOutlineColour = Color.white;
    public Color HighlightingBlockOutlineColour = Color.cyan;

    [Header("-----------------------------------")]
    [Header(" GROUP COUNTER COLOURS ")]
    [Space]
    public GameObject GroupCounterObj = null;
    [Space]
    public float CounterLerpDuration = 1f;
    [Space]
    public Color DefaultCounterColour = Color.white;
    public Color HighlightingCounterColour = Color.cyan;
    [Space]
    public Color DefaultCounterOutlineColour = Color.white;
    public Color HighlightingCounterOutlineColour = Color.cyan;

    [Header("-----------------------------------")]
    [Header(" PC HOTKEY COLOURS ")]
    [Space]
    public GameObject PCHotkeyObj = null;
    [Space]
    public float HotkeyLerpDuration = 1f;
    [Space]
    public Color DefaultHotkeyColour = Color.white;
    public Color HighlightingHotkeyColour = Color.cyan;
    [Space]
    public Color DefaultHotkeyOutlineColour = Color.white;
    public Color HighlightingHotkeyOutlineColour = Color.cyan;
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private List<Unit> _PlatoonAi;

    private Player _Player = null;

    private Text _PCHotkeyTextComponent = null;
    private Outline _PCHotkeyOutline = null;

    private Text _CounterTextComponent = null;
    private Outline _CounterOutline = null;

    private float _CurrentHotKeyTime = 0f;
    private float _CurrentBlockTime = 0f;
    private float _CurrentCounterTime = 0f;

    private bool _Selected = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is created.
    /// </summary>
    private void Start() {

        // Create array for this platoon to store the units in
        _PlatoonAi = new List<Unit>();

        // Get component references
        if (PCHotkeyObj != null) {

            _PCHotkeyTextComponent = PCHotkeyObj.gameObject.GetComponent<Text>();
            _PCHotkeyOutline = PCHotkeyObj.gameObject.GetComponent<Outline>();
        }
        if (GroupCounterObj != null) {

            _CounterTextComponent = GroupCounterObj.gameObject.GetComponent<Text>();
            _CounterOutline = GroupCounterObj.gameObject.GetComponent<Outline>();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        
        if (_Selected) {

            // Light up the hotkey
            LightUpPCHotkeyText();
        }
        else {

            // Unlight the hotkey
            LightDownPCHotkeyText();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void Wipe() {

        // Clear everything
        if (PlatoonCounter != null) { PlatoonCounter.text = ""; }
        _PlatoonAi.Clear();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void SetSelected(bool value) { _Selected = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  List<Unit>
    /// </returns>
    public List<Unit> GetAi() { return _PlatoonAi; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame by keyboard input.cs
    /// </summary>
    public void LightUpBlock() {

        if (BlockImage != null) {

            // Outline
            BlockImage.color = Color.Lerp(DefaultBlockOutlineColour, HighlightingBlockOutlineColour, _CurrentBlockTime);

            // Text
            BlockImage.color = Color.Lerp(DefaultBlockColour, HighlightingBlockColour, _CurrentBlockTime);
            if (_CurrentBlockTime < 1) {

                _CurrentBlockTime += Time.deltaTime / BlockLerpDuration;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame by keyboard input.cs
    /// </summary>
    public void LightDownBlock() {

        if (BlockImage != null) {

            // Outline
            BlockImage.color = Color.Lerp(DefaultBlockOutlineColour, HighlightingBlockOutlineColour, _CurrentBlockTime);

            // Text
            BlockImage.color = Color.Lerp(DefaultBlockColour, HighlightingBlockColour, _CurrentBlockTime);
            if (_CurrentBlockTime > 0) {

                _CurrentBlockTime -= Time.deltaTime / BlockLerpDuration;
            }
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame by keyboard input.cs
    /// </summary>
    public void LightUpCounter() {

        if (_CounterTextComponent != null) {

            // Outline
            _CounterTextComponent.color = Color.Lerp(DefaultCounterOutlineColour, HighlightingCounterOutlineColour, _CurrentCounterTime);

            // Text
            _CounterTextComponent.color = Color.Lerp(DefaultCounterColour, HighlightingCounterColour, _CurrentCounterTime);
            if (_CurrentCounterTime < 1) {

                _CurrentCounterTime += Time.deltaTime / CounterLerpDuration;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame by keyboard input.cs
    /// </summary>
    public void LightDownCounter() {

        if (_CounterTextComponent != null) {

            // Outline
            _CounterTextComponent.color = Color.Lerp(DefaultCounterOutlineColour, HighlightingCounterOutlineColour, _CurrentCounterTime);

            // Text
            _CounterTextComponent.color = Color.Lerp(DefaultCounterColour, HighlightingCounterColour, _CurrentCounterTime);
            if (_CurrentCounterTime > 0) {

                _CurrentCounterTime -= Time.deltaTime / CounterLerpDuration;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame by keyboard input.cs
    /// </summary>
    public void LightUpPCHotkeyText() {

        if (_PCHotkeyTextComponent != null) {

            // Outline
            _PCHotkeyTextComponent.color = Color.Lerp(DefaultHotkeyOutlineColour, HighlightingHotkeyOutlineColour, _CurrentHotKeyTime);

            // Text
            _PCHotkeyTextComponent.color = Color.Lerp(DefaultHotkeyColour, HighlightingHotkeyColour, _CurrentHotKeyTime);
            if (_CurrentHotKeyTime < 1) {

                _CurrentHotKeyTime += Time.deltaTime / HotkeyLerpDuration;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame by keyboard input.cs
    /// </summary>
    public void LightDownPCHotkeyText() {

        if (_PCHotkeyTextComponent != null) {

            // Outline
            _PCHotkeyTextComponent.color = Color.Lerp(DefaultHotkeyOutlineColour, HighlightingHotkeyOutlineColour, _CurrentHotKeyTime);

            // Text
            _PCHotkeyTextComponent.color = Color.Lerp(DefaultHotkeyColour, HighlightingHotkeyColour, _CurrentHotKeyTime);
            if (_CurrentHotKeyTime > 0) {

                _CurrentHotKeyTime -= Time.deltaTime / HotkeyLerpDuration;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}