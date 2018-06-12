using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/6/2018
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
    [Header(" ITEM HIGHLIGHT DETAIL WINDOW")]
    [Space]
    public Text DetailedHighlightTitle;
    public Text DetailedHighlightDescription;

    [Space]
    [Header("-----------------------------------")]
    [Header(" ITEM HIGHLIGHT CENTER PANEL")]
    [Space]
    public Text CenterHighlightTitle;
    public Text CenterSupplyText;
    public Text CenterPowerText;
    public Text CenterPlayerLevelText;

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUTTONS ")]
    [Space]
    public List<Button> _WheelButtons;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    [HideInInspector]
    public List<Abstraction> _BuildingList;
    private BuildingSlot _BuildingSlotInFocus = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectables"></param>
    public void UpdateListWithBuildings(List<Abstraction> selectables) {

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
                _BuildingList.Add(building);
                
                // Update reference unit
                SelectionWheelUnitRef unitRef = _WheelButtons[i].GetComponent<SelectionWheelUnitRef>();
                unitRef.Unit = building.GetComponent<WorldObject>();

                // Update button click event
                _WheelButtons[i].onClick.AddListener(delegate { building.OnWheelSelect(_BuildingSlotInFocus); });
            }
            ++i;
        }
        RefreshButtons(selectables);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectable"></param>
    public void UpdateListWithBuildables(List<Abstraction> selectable) {

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
        foreach (var obj in selectable) {

            if (obj != null) {

                // If slot is an upgrade
                Upgrade upgrade = obj.GetComponent<Upgrade>();
                if (upgrade != null) {

                    // Dont update wheel with this button since its the upgrade is maxed out
                    if (upgrade.HasMaxUpgrade()) { continue; }
                }

                // Update list with new slots
                _BuildingList.Add(obj);

                // Update reference unit
                SelectionWheelUnitRef unitRef = _WheelButtons[i].GetComponent<SelectionWheelUnitRef>();
                unitRef.Unit = obj.GetComponent<WorldObject>();

                // Update button click event
                _WheelButtons[i].onClick.AddListener(delegate { obj.GetComponent<WorldObject>().OnWheelSelect(_BuildingSlotInFocus); });
            }
            ++i;
        }
        RefreshButtons(selectable);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    public void RefreshButtons(List<Abstraction> list) {

        int i = 0;
        foreach (var button in _WheelButtons) {

            // If valid
            if (list.Count > i) {

                if (list[i] != null) {

                    // Update button text
                    Text txtComp = button.GetComponentInChildren<Text>();
                    txtComp.text = list[i].ObjectName;

                    // Update button interactibility state
                    SelectionWheelUnitRef unitRef = button.GetComponent<SelectionWheelUnitRef>();
                    if (unitRef.Unit != null) {

                        bool unlock = GameManager.Instance.Players[0].Level >= unitRef.Unit.CostPlayerLevel;
                        button.interactable = unlock;
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
            
            if (buttonItem.GetComponent<Button>().interactable) { imgComp.color = new Color(0, 0, 0, 1); }
            else                                                { imgComp.color = new Color(0, 0, 0, 0.5f); }
        }
        if (txtComp) {
            
            // Change the text colour depending if the button is locked or not
            if (buttonItem.GetComponent<Button>().interactable) { txtComp.color = new Color(1, 1, 1, 1); }
            else                                                { txtComp.color = new Color(0, 0, 0, 1); }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowSelectionWheel() { GameManager.Instance.SelectionWheel.SetActive(true); }

    /// <summary>
    /// 
    /// </summary>
    public void HideSelectionWheel() {

        // Deselect all objects
        foreach (var selectable in GameManager.Instance.Selectables) { selectable._IsCurrentlySelected = false; }

        // Hide widget
        GameManager.Instance.SelectionWheel.SetActive(false);
    }

    public void setBuildingSlotInFocus(BuildingSlot slot) { _BuildingSlotInFocus = slot; }

}
