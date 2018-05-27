using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TowerDefence;

public class SelectionWheel : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    public List<Button> _WheelButtons;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [HideInInspector]
    public List<Building> _BuildingList;
    private BuildingSlot _BuildingSlotInFocus = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    public void UpdateList(List<Abstraction> list) {

        // Reset button click events for all buttons
        foreach (var button in _WheelButtons) {

            // Clear
            button.onClick.RemoveAllListeners();

            // Add defaults
            button.onClick.AddListener( () => HideSelectionWheel());
        }

        // Clear list
        _BuildingList.Clear();
        int i = 0;
        foreach (var obj in list) {

            if (obj != null) {

                // Update list with new slots
                Building building = obj.GetComponent<Building>();
                _BuildingList.Add(building);
                
                // Update reference unit
                SelectionWheelUnitRef unitRef = _WheelButtons[i].GetComponent<SelectionWheelUnitRef>();
                unitRef.Unit = obj.GetComponent<WorldObject>();

                // Update button click event
                _WheelButtons[i].onClick.AddListener(delegate { building.OnWheelSelect(_BuildingSlotInFocus); });
            }
            ++i;
        }

        RefreshButtons(list);
    }

    public void RefreshButtons(List<Abstraction> list) {

        int i = 0;
        foreach (var button in _WheelButtons) {

            // If valid
            if (list[i] != null) {

                // Update button text
                Text txtComp = button.GetComponentInChildren<Text>();
                txtComp.text = list[i].ObjectName;

                // Update button interactibility state
                SelectionWheelUnitRef unitRef = button.GetComponent<SelectionWheelUnitRef>();
                bool unlock = GameManager.Instance.Players[0]._Level >= unitRef.Unit.CostPlayerLevel;
                button.interactable = unlock;

                // Update item visibility
                ButtonVisibility(button.gameObject, true);
            }

            else { ButtonVisibility(button.gameObject, false); }

            ++i;
        }
    }

    public void ButtonVisibility(GameObject buttonItem, bool visibile) {

        Image imgComp = buttonItem.GetComponent<Image>();
        if (imgComp) {
            
            // Show / hide the button
            imgComp.gameObject.SetActive(visibile);
            
            if (buttonItem.GetComponent<Button>().interactable) { imgComp.color = new Color(0, 0, 0, 1); }
            else                                                { imgComp.color = new Color(0, 0, 0, 0.75f); }
        }
    }

    public void ShowSelectionWheel() { GameManager.Instance.SelectionWheel.SetActive(true); }

    public void HideSelectionWheel() {

        // Deselect all objects
        foreach (var selectable in GameManager.Instance.Selectables) { selectable._IsCurrentlySelected = false; }

        // Hide widget
        GameManager.Instance.SelectionWheel.SetActive(false);
    }

    public void setBuildingSlotInFocus(BuildingSlot slot) { _BuildingSlotInFocus = slot; }

}
