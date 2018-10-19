using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 17/10/2018
//
//******************************

public class ButtonHover_SelectionWheel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" WHEELS ")]
    [Space]
    public SelectionWheel SelectionWheel;

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

    private SelectionWheelUnitRef _ObjectRefComponent = null;
    private Button _ButtonComponent = null;
    private TextMesh _TextComponent = null;
    private Color _PCHotkeyColour;
    private Color _OriginalColour;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this gameObject is created.
    /// </summary>
    private void Start() {

        // Get component references
        _ObjectRefComponent = GetComponent<SelectionWheelUnitRef>();
        _ButtonComponent = GetComponent<Button>();
        if (PCHotkeyObj != null) {

            _TextComponent = PCHotkeyObj.GetComponentInChildren<TextMesh>();

            // Store colours set by the inspector
            _PCHotkeyColour = _TextComponent.color;
            _OriginalColour = _PCHotkeyColour;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        
        // Update the PC hotkey text colour
        if (_ButtonComponent != null && PCHotkeyObj != null) {

            // Get colour for the text component
            if (_ButtonComponent.IsInteractable())  { _PCHotkeyColour = _ButtonComponent.colors.disabledColor; }
            else                                    { _PCHotkeyColour = _OriginalColour; }

            // Set text colour
            _TextComponent.color = _PCHotkeyColour;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerEnter(PointerEventData eventdata) {

        if (SelectionWheel != null && _ObjectRefComponent != null && _ButtonComponent != null) {

            // Update the highlight text in the selection wheel
            if (_ObjectRefComponent.AbstractRef != null) {

                // Get selection wheel reference
                SelectionWheel selectionWheel = null;
                if (GameManager.Instance._IsRadialMenu) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
                else { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }
                selectionWheel.ShowItemPurchaseInfoPanel();

                // Detail window
                SelectionWheel.DetailedHighlightTitle.text = _ObjectRefComponent.AbstractRef.ObjectName.ToUpper();
                SelectionWheel.DetailedHighlightDescriptionShort.text = _ObjectRefComponent.AbstractRef.ObjectDescriptionShort.ToUpper();
                SelectionWheel.DetailedHighlightDescriptionLong.text = _ObjectRefComponent.AbstractRef.ObjectDescriptionLong;

                // Center panel
                SelectionWheel.CenterHighlightTitle.text = _ObjectRefComponent.AbstractRef.ObjectName;
                SelectionWheel.CenterTechLevelText.text = _ObjectRefComponent.AbstractRef.CostTechLevel.ToString();
                SelectionWheel.CenterSupplyText.text = _ObjectRefComponent.AbstractRef.CostSupplies.ToString();
                SelectionWheel.CenterPowerText.text = _ObjectRefComponent.AbstractRef.CostPower.ToString();
                SelectionWheel.CenterPopulationText.text = _ObjectRefComponent.AbstractRef.CostPopulation.ToString();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData) {

        if (SelectionWheel != null && _ObjectRefComponent != null && _ButtonComponent != null) {

            // Get selection wheel reference
            SelectionWheel selectionWheel = null;
            if (GameManager.Instance._IsRadialMenu) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
            else { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }
            selectionWheel.HideItemPurchaseInfoPanel();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the button this script is attached to is clicked on by a mouse.
    //  RMB - Deducts this object from the queue.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData) {

        // Deduct item from queue if it exists
        if (eventData.button == PointerEventData.InputButton.Right && SelectionWheel != null) {

            // Check if the item exists in the building queue of the building
            // Deduct the last iterator of the abstraction type from the queue
            List<Abstraction> queue = SelectionWheel.GetBuildingSlotInstigator().GetBuildingOnSlot().GetBuildingQueue();
            for (int i = queue.Count - 1; i >= 0; i--) {
                
                // Matching type found
                if (queue[i].GetType() == _ObjectRefComponent.AbstractRef.GetType()) {

                    // Add resources back to the player
                    Player player = GameManager.Instance.Players[0];
                    player.PopulationCount -= _ObjectRefComponent.AbstractRef.CostPopulation;
                    player.SuppliesCount += _ObjectRefComponent.AbstractRef.CostSupplies;
                    player.PowerCount += _ObjectRefComponent.AbstractRef.CostPower;

                    // Remove it
                    queue.RemoveAt(i);
                    break;
                }
            }
            SelectionWheel.UpdateButtonStates();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}