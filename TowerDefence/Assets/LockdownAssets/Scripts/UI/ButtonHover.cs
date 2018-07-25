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
//  Last edited on: 25/7/2018
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

    private SelectionWheelUnitRef _ObjectRefComponent;
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
        _ObjectRefComponent = GetComponent<SelectionWheelUnitRef>();
        _ButtonComponent = GetComponent<Button>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerEnter(PointerEventData eventdata) {

        if (SelectionWheel != null && _ObjectRefComponent != null && _ButtonComponent != null) {

            // Update the highlight text in the selection wheel

            // Detail window
            SelectionWheel.DetailedHighlightTitle.text = _ObjectRefComponent.Object.ObjectName.ToUpper();
            SelectionWheel.DetailedHighlightDescriptionShort.text = _ObjectRefComponent.Object.ObjectDescriptionShort.ToUpper();
            SelectionWheel.DetailedHighlightDescriptionLong.text = _ObjectRefComponent.Object.ObjectDescriptionLong;

            // Center panel
            SelectionWheel.CenterHighlightTitle.text = _ObjectRefComponent.Object.ObjectName;
            SelectionWheel.CenterTechLevelText.text = _ObjectRefComponent.Object.CostTechLevel.ToString();
            SelectionWheel.CenterSupplyText.text = _ObjectRefComponent.Object.CostSupplies.ToString();
            SelectionWheel.CenterPowerText.text = _ObjectRefComponent.Object.CostPower.ToString();
            SelectionWheel.CenterPopulationText.text = _ObjectRefComponent.Object.CostPopulation.ToString();
        }
    }
    
}