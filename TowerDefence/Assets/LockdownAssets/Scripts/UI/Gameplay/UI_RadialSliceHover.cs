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
//  Last edited on: 13/11/2018
//
//******************************

public class UI_RadialSliceHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PROPERTIES ")]
    [Space]
    public Button LinkedButton;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    //  
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerEnter(PointerEventData eventdata) {

        if (LinkedButton != null) { LinkedButton.GetComponent<ButtonHover_SelectionWheel>().OnPointerEnter(eventdata); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData) {

        if (LinkedButton != null) { LinkedButton.GetComponent<ButtonHover_SelectionWheel>().OnPointerExit(eventData); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the button this script is attached to is clicked on by a mouse.
    //  RMB - Deducts this object from the queue.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData) {

        // Deduct item from queue if it exists
        if (eventData.button == PointerEventData.InputButton.Right && LinkedButton != null) {

            LinkedButton.GetComponent<ButtonHover_SelectionWheel>().OnPointerClick(eventData);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
