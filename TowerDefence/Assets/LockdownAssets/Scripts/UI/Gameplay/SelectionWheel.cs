using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/11/2018
//
//******************************

public class SelectionWheel : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" COLOURS")]
    [Space]
    public Color ColorAvailiable = Color.white;
    public Color ColorExpensive = Color.grey;
    public Color ColorLocked = Color.grey;

    [Space]
    [Header("-----------------------------------")]
    [Header(" ITEM HIGHLIGHT CENTER PANEL")]
    [Space]
    public Text CenterHighlightTitle;
    public Text CenterSupplyText;
    public Text CenterPowerText;
    public Text CenterPopulationText;
    public Text CenterTechLevelText;

    [Space]
    [Header("-----------------------------------")]
    [Header(" ITEM HIGHLIGHT DETAIL WINDOW")]
    [Space]
    public GameObject ItemPurchaseInfoPanel;
    public GameObject RadialDefaultCenterPanel;
    [Space]
    public Text DetailedHighlightTitle;
    public Text DetailedHighlightDescriptionShort;
    public Text DetailedHighlightDescriptionLong;

    [Space]
    [Header("-----------------------------------")]
    [Header(" GAMEPAD PROPERTIES")]
    [Space]
    public RectTransform SelectionMarker;

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUTTONS ")]
    [Space]
    public Button MasterButton;
    [Space]
    public List<Button> _WheelButtons;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    [HideInInspector]
    public List<Abstraction> _BuildingList;

    private Abstraction _AbstractionInDefaultFocus = null;
    private string _DefaultFocusString = "";

    private BuildingSlot _BuildingSlotInstigator = null;

    private Player _Player = null;
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called in the next frame (1 frame delay)
    /// </summary>
    private void LateUpdate() {

        if (MasterButton != null && MasterButton.gameObject.activeInHierarchy == true) { MasterButton.enabled = true; }

        if (_Player == null) { _Player = GameManager.Instance.Players[0]; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectables"></param>
    public void UpdateListWithBuildings(List<Abstraction> selectables, BuildingSlot buildingSlotInFocus) {

        _BuildingSlotInstigator = buildingSlotInFocus;

        // Reset button click events for all buttons
        foreach (var button in _WheelButtons) {

            // Clear
            button.onClick.RemoveAllListeners();

            // Add defaults
            button.onClick.AddListener(() => HideSelectionWheel());
        }

        // Clear list & update
        _BuildingList.Clear();
        int i = 0;
        foreach (var obj in selectables) {

            if (obj != null) {

                // Update list with new slots
                Building building = obj.GetComponent<Building>();
                if (building != null) {

                    _BuildingList.Add(building);

                    // Update reference unit
                    SelectionWheelUnitRef unitRef = _WheelButtons[i].GetComponent<SelectionWheelUnitRef>();
                    unitRef.AbstractRef = building;

                    // Update button click event
                    _WheelButtons[i].onClick.AddListener(delegate { unitRef.AbstractRef.OnWheelSelect(buildingSlotInFocus); });
                }
            }
            ++i;
        }
        RefreshButtons(selectables);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectable"></param>
    public void UpdateListWithBuildables(List<Abstraction> selectable, BuildingSlot buildingSlotInFocus) {

        _BuildingSlotInstigator = buildingSlotInFocus;

        // Reset button click events for all buttons
        foreach (var button in _WheelButtons) {

            // Clear
            button.onClick.RemoveAllListeners();

            // Add defaults
            button.onClick.AddListener(() => RefreshButtons(selectable));
        }

        // Clear list & update
        _BuildingList.Clear();
        int i = 0;
        foreach (var obj in selectable) {

            if (obj != null) {

                // If slot is an upgrade
                UpgradeTree upgrade = obj.GetComponent<UpgradeTree>();
                if (upgrade != null) {

                    // Dont update wheel with this button since its the upgrade is maxed out
                    if (upgrade.HasMaxUpgrade()) { continue; }
                }

                // Update list with new slots
                _BuildingList.Add(obj);

                // Update reference unit
                SelectionWheelUnitRef unitRef = _WheelButtons[i].GetComponent<SelectionWheelUnitRef>();
                unitRef.AbstractRef = obj.GetComponent<Abstraction>();

                // Update button click event
                _WheelButtons[i].onClick.AddListener(delegate { unitRef.AbstractRef.OnWheelSelect(buildingSlotInFocus); });
            }
            ++i;
        }
        RefreshButtons(selectable);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void UpdateButtonStates() {

        if (_BuildingSlotInstigator != null) {

            if (_BuildingSlotInstigator.GetBuildingOnSlot() != null) {

                RefreshButtons(_BuildingSlotInstigator.GetBuildingOnSlot().Selectables);
            }
            else { RefreshButtons(_BuildingSlotInstigator.GetBuildingsAsAbstraction()); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    public void RefreshButtons(List<Abstraction> list) {

        // Update radial wheel building queue item counters
        if (_BuildingSlotInstigator != null) {

            Building building = _BuildingSlotInstigator.GetBuildingOnSlot();
            if (building != null) {

                if (building.GetBuildingQueueUI() != null) { building.GetBuildingQueueUI().UpdateSelectionWheelItemCounters(); }
            }
        }
        
        int i = 0;
        foreach (var button in _WheelButtons) {

            // If valid
            if (list.Count > i) {

                if (list[i] != null) {

                    // Update button
                    SelectionWheelUnitRef unitRef = button.GetComponent<SelectionWheelUnitRef>();
                    if (unitRef.AbstractRef != null) {

                        Player player = GameManager.Instance.Players[0];
                        Abstraction item = unitRef.AbstractRef;

                        // Update button image
                        unitRef.UpdateUnitRefLogo();
                        
                        // Check whether the button should be interactable or not
                        bool unlock = player.Level >= item.CostTechLevel;
                        bool purchasable = player.SuppliesCount >= item.CostSupplies &&
                                           player.PowerCount >= item.CostPower &&
                                           (player.MaxPopulation - player.PopulationCount) >= item.CostPopulation;
                        button.interactable = unlock && purchasable;

                        // Update locked icon for the element
                        unitRef.LockedImage.enabled = !unlock;
                                                
                        // Update image colour based on state
                        if (!unlock)            { button.image.color = ColorLocked; }
                        else if (!purchasable)  { button.image.color = ColorExpensive; }
                        else                    { button.image.color = ColorAvailiable; }
                    }

                    // If theres no unit reference in the button, just disable it by default
                    else { button.interactable = false; }

                    // Update item visibility
                    ButtonVisibility(button.gameObject, true);
                }

                else { ButtonVisibility(button.gameObject, false); }

                ++i;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buttonItem"></param>
    /// <param name="visibile"></param>
    public void ButtonVisibility(GameObject buttonItem, bool visibile) {

        Image imgComp = buttonItem.GetComponent<Image>();
        Text txtComp = buttonItem.GetComponentInChildren<Text>();
        if (imgComp) {
            
            // Show / hide the button
            imgComp.gameObject.SetActive(visibile);
            
            if (buttonItem.GetComponent<Button>().interactable) { imgComp.color = new Color(148, 108, 63, 1); }
            else                                                { imgComp.color = new Color(0, 0, 0, 0.5f); }
        }
        if (txtComp) {
            
            // Change the text colour depending if the button is locked or not
            if (buttonItem.GetComponent<Button>().interactable) { txtComp.color = new Color(0, 0, 0, 1); }
            else                                                { txtComp.color = new Color(1, 1, 1, 1); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    public void ShowSelectionWheel() {

        GameManager.Instance.SelectionWheel.SetActive(true);
        MasterButton.enabled = false;

        HideItemPurchaseInfoPanel();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private IEnumerator WaitHide()
    {
        yield return new WaitForSeconds(0.05f);

        // Deselect all objects
        foreach (var selectable in GameManager.Instance.Selectables) { selectable.SetIsSelected(false); }

        // Play sound
        SoundManager.Instance.PlaySound("SFX/_SFX_Woosh1", 1f, 1f);

        // Hide widget
        GameManager.Instance.SelectionWheel.SetActive(false);
        MasterButton.enabled = false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void HideSelectionWheel() {
        StartCoroutine(WaitHide());
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void ShowItemPurchaseInfoPanel() {

        // Show info panel
        if (ItemPurchaseInfoPanel != null) { ItemPurchaseInfoPanel.gameObject.SetActive(true); }

        // Hide center default panel
        if (RadialDefaultCenterPanel != null) { RadialDefaultCenterPanel.gameObject.SetActive(false); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void HideItemPurchaseInfoPanel() {

        // Hide info panel
        if (ItemPurchaseInfoPanel != null) { ItemPurchaseInfoPanel.gameObject.SetActive(false); }

        // Show center default panel
        if (RadialDefaultCenterPanel != null) {

            RadialDefaultCenterPanel.gameObject.SetActive(true);
            DetailedHighlightTitle.text = _AbstractionInDefaultFocus.ObjectName.ToUpper();
            DetailedHighlightDescriptionShort.text = _AbstractionInDefaultFocus.ObjectDescriptionShort.ToUpper();
            DetailedHighlightDescriptionLong.text = _AbstractionInDefaultFocus.ObjectDescriptionLong;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="abs"></param>
    public void SetDefaultAbstraction(Abstraction abs) {

        // Get reference
        _AbstractionInDefaultFocus = abs;
        _DefaultFocusString = _AbstractionInDefaultFocus.ObjectName;

        // Update text
        RadialDefaultCenterPanel.GetComponentInChildren<Text>().text = _DefaultFocusString;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  BuildingSlot
    /// </returns>
    public BuildingSlot GetBuildingSlotInstigator() { return _BuildingSlotInstigator; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="abs"></param>
    public void UpdateLogoSliders(Abstraction abs) {

        for (int i = 0; i < _WheelButtons.Count; i++) {

            // Find matching slider
            SelectionWheelUnitRef unitRef = _WheelButtons[i].GetComponent<SelectionWheelUnitRef>();
            if (unitRef.AbstractRef != null) {

                if (unitRef.AbstractRef.GetType() == abs.GetType()) {

                    // Match found - update the slider attached to this button
                    unitRef.SetCurrentBuildProgress(abs.GetBuildPercentage());
                }
                else { unitRef.SetCurrentBuildProgress(0f); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
