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
//  Last edited on: 10/6/2018
//
//******************************

public class ButtonHover : MonoBehaviour, IPointerEnterHandler {

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

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private SelectionWheelUnitRef _UnitRefComponent;
    private Button _ButtonComponent;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
    /// </summary>
    private void Start() {

        // Get component references
        _UnitRefComponent = GetComponent<SelectionWheelUnitRef>();
        _ButtonComponent = GetComponent<Button>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerEnter(PointerEventData eventdata) {

        if (SelectionWheel != null && _UnitRefComponent != null && _ButtonComponent != null) {

            // Button is interactable
            if (/*_ButtonComponent.interactable == */true) {

                // Update the highlight text in the selection wheel

                /// Detail window
                SelectionWheel.DetailedHighlightTitle.text = _UnitRefComponent.Unit.ObjectName;
                SelectionWheel.DetailedHighlightDescription.text = _UnitRefComponent.Unit.ObjectDescription;

                /// Center panel
                SelectionWheel.CenterHighlightTitle.text = _UnitRefComponent.Unit.ObjectName;
                SelectionWheel.CenterPlayerLevelText.text = _UnitRefComponent.Unit.CostPlayerLevel.ToString();
                SelectionWheel.CenterSupplyText.text = _UnitRefComponent.Unit.CostSupplies.ToString();
                SelectionWheel.CenterPowerText.text = _UnitRefComponent.Unit.CostPower.ToString();
            }
        }
    }
    
}