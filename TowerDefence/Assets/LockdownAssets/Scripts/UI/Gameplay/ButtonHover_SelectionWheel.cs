using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/11/2018
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
    private TMP_Text _TextComponent = null;
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

            _TextComponent = PCHotkeyObj.GetComponent<TMP_Text>();

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
            if (_ButtonComponent.IsInteractable())  { _PCHotkeyColour = _OriginalColour; }
            else                                    { _PCHotkeyColour = _ButtonComponent.colors.disabledColor; }

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
                
                Player player = GameManager.Instance.Players[0];
                SelectionWheelUnitRef unitRef = GetComponent<SelectionWheelUnitRef>();
                Abstraction item = unitRef.AbstractRef;

                bool unlock = player.Level >= item.CostTechLevel;
                bool purchasable = player.SuppliesCount >= item.CostSupplies &&
                                   player.PowerCount >= item.CostPower &&
                                   (player.MaxPopulation - player.PopulationCount) >= item.CostPopulation;
                
                // Update selection marker colours
                if (selectionWheel.SelectionMarkerImage) {

                    if (!unlock) { selectionWheel.SelectionMarkerImage.color = Color.red; }
                    if (!purchasable) { selectionWheel.SelectionMarkerImage.color = Color.red; }
                    else if (unlock) { selectionWheel.SelectionMarkerImage.color = Color.green; }
                    else { selectionWheel.SelectionMarkerImage.color = Color.grey; }
                }

                // Update cost texts colour based on state
                /// Tech level
                if (player.Level >= item.CostTechLevel) { selectionWheel.CenterTechLevelText.color = Color.black; }
                else { selectionWheel.CenterTechLevelText.color = Color.red; }

                /// Population
                if ((player.MaxPopulation - player.PopulationCount) >= item.CostPopulation) { selectionWheel.CenterPopulationText.color = Color.black; }
                else { selectionWheel.CenterPopulationText.color = Color.red; }

                /// Supplies
                if (player.SuppliesCount >= item.CostSupplies) { selectionWheel.CenterSupplyText.color = Color.black; }
                else { selectionWheel.CenterSupplyText.color = Color.red; }
                
                /// Power
                if (player.PowerCount >= item.CostPower) { selectionWheel.CenterPowerText.color = Color.black; }
                else { selectionWheel.CenterPowerText.color = Color.red; }

                // Play hover button sound
                SoundManager.Instance.PlaySound("SFX/_SFX_Back_Alt", 1f, 1f, false);
            }
            else {

                // Get selection wheel reference
                SelectionWheel selectionWheel = null;
                if (GameManager.Instance._IsRadialMenu) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); } else { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }
                selectionWheel.ShowItemPurchaseInfoPanel();

                // Detail window
                SelectionWheel.DetailedHighlightTitle.text = SelectionWheel.GetBuildingSlotInstigator().ObjectName.ToUpper();
                SelectionWheel.DetailedHighlightDescriptionShort.text = SelectionWheel.GetBuildingSlotInstigator().ObjectDescriptionShort.ToUpper();
                SelectionWheel.DetailedHighlightDescriptionLong.text = SelectionWheel.GetBuildingSlotInstigator().ObjectDescriptionLong;

                // Center panel
                SelectionWheel.CenterHighlightTitle.text = SelectionWheel.GetBuildingSlotInstigator().ObjectName;
                SelectionWheel.CenterTechLevelText.text = "1";
                SelectionWheel.CenterSupplyText.text = "0";
                SelectionWheel.CenterPowerText.text = "0";
                SelectionWheel.CenterPopulationText.text = "0";

                // Update selection marker colour
                selectionWheel.SelectionMarkerImage.color = Color.grey;
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