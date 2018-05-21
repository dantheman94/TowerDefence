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

        // Clear button click events on the wheel
        foreach (var button in _WheelButtons) {

            button.onClick.RemoveAllListeners();
        }

        // Clear list
        _BuildingList.Clear();
        int i = 0;
        foreach (var obj in list) {

            // Update list with new slots
            Building building = obj.GetComponent<Building>();
            _BuildingList.Add(building);

            // Update button click event
            _WheelButtons[i].onClick.AddListener(delegate { building.OnWheelSelect(); });

            // Update reference unit
            SelectionWheelUnitRef unitRef = _WheelButtons[i].gameObject.transform.parent.GetComponent<SelectionWheelUnitRef>();
            unitRef.Unit = obj.GetComponent<WorldObject>();

            ++i;
        }

        RefreshButtons();
    }

    public void RefreshButtons() {

        int i = 0;
        foreach (var button in _WheelButtons) {

            // If valid
            if (_BuildingList.Count > i) {

                // Update button text
                Text txtComp = button.gameObject.transform.parent.GetComponentInChildren<Text>();
                txtComp.text = _BuildingList[i].ObjectName;

                // Update button interactibility state
                SelectionWheelUnitRef unitRef = button.gameObject.transform.parent.GetComponent<SelectionWheelUnitRef>();
                bool unlock = unitRef.Unit.CostPlayerLevel >= GameManager.Instance.Players[0]._Level;
                button.interactable = unlock;

                // Update item visibility
                ButtonVisibility(button.transform.parent.gameObject, true);

                ++i;
            }

            else { ButtonVisibility(button.transform.parent.gameObject, false); }
        }
    }

    public void ButtonVisibility(GameObject buttonItem, bool visibile) {

        Image imgComp = buttonItem.GetComponent<Image>();
        Text txtComp = buttonItem.GetComponentInChildren<Text>();

        if (visibile) {

            imgComp.color = new Color(0, 0, 0, 1);
            ///txtComp.gameObject.SetActive(true);
        }

        else {
            
            imgComp.color = new Color(0, 0, 0, 0);
            ///txtComp.gameObject.SetActive(false);
        }
    }

}
