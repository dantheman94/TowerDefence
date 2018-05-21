using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TowerDefence;

public class SelectionWheel : MonoBehaviour {

    //******************************************************************************************************************************
    // INSPECTOR

    public List<Button> _WheelButtons;

    //******************************************************************************************************************************
    // VARIABES

    [HideInInspector]
    public List<Building> _BuildingList;

    //******************************************************************************************************************************
    // FUNCTIONS
    
    public void UpdateList(List<Selectable> list) {

        // Reset button click events for all buttons
        foreach (var button in _WheelButtons) {

            // Clear
            button.onClick.RemoveAllListeners();

            // Add defaults
            button.onClick.AddListener(delegate { HideSelectionWheel(); });
        }

        // Clear list
        _BuildingList.Clear();
        int i = 0;
        foreach (var obj in list) {

            if (obj != null) {

                // Update list with new slots
                Building building = obj.GetComponent<Building>();
                _BuildingList.Add(building);

                // Update button click event
                _WheelButtons[i].onClick.AddListener(delegate { building.OnWheelSelect(); });

                // Update reference unit
                SelectionWheelUnitRef unitRef = _WheelButtons[i].gameObject.transform.parent.GetComponent<SelectionWheelUnitRef>();
                unitRef.Unit = obj.GetComponent<WorldObject>();
            }
            ++i;
        }

        RefreshButtons(list);
    }

    public void RefreshButtons(List<Selectable> list) {

        int i = 0;
        foreach (var button in _WheelButtons) {

            // If valid
            if (list[i] != null) {

                // Update button text
                Text txtComp = button.gameObject.transform.parent.GetComponentInChildren<Text>();
                txtComp.text = list[i].ObjectName;

                // Update button interactibility state
                SelectionWheelUnitRef unitRef = button.gameObject.transform.parent.GetComponent<SelectionWheelUnitRef>();
                bool unlock = unitRef.Unit.CostPlayerLevel >= GameManager.Instance.Players[0]._Level;
                button.interactable = unlock;

                // Update item visibility
                ButtonVisibility(button.transform.parent.gameObject, true);
            }

            else { ButtonVisibility(button.transform.parent.gameObject, false); }

            ++i;
        }
    }

    public void ButtonVisibility(GameObject buttonItem, bool visibile) {

        Image imgComp = buttonItem.GetComponent<Image>();

        if (visibile) {

            // Full alpha
            imgComp.color = new Color(0, 0, 0, 1);
        }

        else {
            
            // No alpha
            imgComp.color = new Color(0, 0, 0, 0);
        }
    }

    public void ShowSelectionWheel() { GameManager.Instance.SelectionWheel.SetActive(true); }

    public void HideSelectionWheel() {

        // Deselect all objects
        foreach (var selectable in GameManager.Instance.Selectables) { selectable._IsCurrentlySelected = false; }

        // Hide widget
        GameManager.Instance.SelectionWheel.SetActive(false);
    }

}
