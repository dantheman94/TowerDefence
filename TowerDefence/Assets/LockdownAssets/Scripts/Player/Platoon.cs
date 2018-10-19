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
    [Header(" BLOCK COLOURS ")]
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
    [Header(" OULINE COLOURS ")]
    [Space]
    public float HotkeyLerpDuration = 1f;
    [Space]
    public Color DefaultHotkeyColour = Color.white;
    public Color HighlightingHotkeyColour = Color.cyan;
    [Space]
    public Color DefaultHotkeyOutlineColour = Color.white;
    public Color HighlightingHotkeyOutlineColour = Color.cyan;

    [Space]
    [Header("-----------------------------------")]
    [Header(" CONTROLLER ")]
    [Space]
    public GameObject PCHotkeyObj = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private List<Unit> _PlatoonAi;

    private Player _Player = null;

    private Text _PCHotkeyTextComponent = null;
    private Outline _PCHotkeyOutline = null;

    private float _CurrentTime = 0f;
    private float _CurrentBlockTime = 0f;

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

        // Initialize the gameobject
        _PlatoonAi = new List<Unit>();
        if (PCHotkeyObj != null) {

            _PCHotkeyTextComponent = PCHotkeyObj.gameObject.GetComponent<Text>();
            _PCHotkeyOutline = PCHotkeyObj.gameObject.GetComponent<Outline>();
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

    public List<Unit> GetAi() { return _PlatoonAi; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Called each frame by keyboard input.cs
    /// </summary>
    public void LightUpText() {

        if (_PCHotkeyTextComponent != null) {

            // Outline
            _PCHotkeyTextComponent.color = Color.Lerp(DefaultHotkeyOutlineColour, HighlightingHotkeyOutlineColour, _CurrentTime);

            // Text
            _PCHotkeyTextComponent.color = Color.Lerp(DefaultHotkeyColour, HighlightingHotkeyColour, _CurrentTime);
            if (_CurrentTime < 1) {

                _CurrentTime += Time.deltaTime / HotkeyLerpDuration;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame by keyboard input.cs
    /// </summary>
    public void LightDownText() {

        if (_PCHotkeyTextComponent != null) {

            // Outline
            _PCHotkeyTextComponent.color = Color.Lerp(DefaultHotkeyOutlineColour, HighlightingHotkeyOutlineColour, _CurrentTime);

            // Text
            _PCHotkeyTextComponent.color = Color.Lerp(DefaultHotkeyColour, HighlightingHotkeyColour, _CurrentTime);
            if (_CurrentTime > 0) {

                _CurrentTime -= Time.deltaTime / HotkeyLerpDuration;
            }
        }
    }

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

}