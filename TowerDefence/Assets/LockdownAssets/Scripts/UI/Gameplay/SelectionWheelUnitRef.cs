using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 18/10/2018
//
//******************************

public class SelectionWheelUnitRef : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PROPERTIES")]
    [Space]
    public Text QueueCounterTextComponent = null;
    [Space]
    public KeyCode PCShortcutKey;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [HideInInspector]
    public Abstraction AbstractRef { get; set; }
    private int _UnitCounter = 0;

    private Image _ButtonItemLogoComponent = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Get component references
        _ButtonItemLogoComponent = GetComponent<Image>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        
        // Update queue counter text string
        if (_UnitCounter > 0 && QueueCounterTextComponent != null) {

            QueueCounterTextComponent.gameObject.SetActive(true);
            QueueCounterTextComponent.text = _UnitCounter.ToString();
        }
        else if (QueueCounterTextComponent != null) { QueueCounterTextComponent.gameObject.SetActive(false); }

        // PC hotkey input for this button
        if (Input.GetKeyDown(PCShortcutKey)) {

            // Check whether the button should be interactable or not
            Button button = GetComponent<Button>();
            Player player = GameManager.Instance.Players[0];
            bool unlock = player.Level >= AbstractRef.CostTechLevel;
            bool purchasable = player.SuppliesCount >= AbstractRef.CostSupplies &&
                               player.PowerCount >= AbstractRef.CostPower &&
                               (player.MaxPopulation - player.PopulationCount) >= AbstractRef.CostPopulation;
            button.interactable = unlock && purchasable;

            // 'Buy' the item if the button is interactable
            if (button.IsInteractable()) { button.onClick.Invoke(); }

            // Get selection wheel reference
            SelectionWheel selectionWheel = null;
            if (GameManager.Instance._IsRadialMenu) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
            else { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }
            selectionWheel.UpdateButtonStates();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="count"></param>
    public void SetQueueCounter(int count) { _UnitCounter = count; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    public void UpdateUnitRefLogo() {

        // Set the button's sprite to match the abstraction ref's logo sprite
        if (_ButtonItemLogoComponent != null) { _ButtonItemLogoComponent.sprite = AbstractRef.Logo; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}