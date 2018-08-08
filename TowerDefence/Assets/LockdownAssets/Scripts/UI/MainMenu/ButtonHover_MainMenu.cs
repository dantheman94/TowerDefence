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
//  Last edited on: 29/7/2018
//
//******************************

public class ButtonHover_MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler { 

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUTTON HOVER PROPERTIES")]
    [Space]
    public Text TextAttached = null;
    public Color Default_TextColour = Color.white;
    public Color Hover_TextColour = Color.black;
    public Button EnterButton = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Button _ButtonComponent;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    private void Start() {

        // Get component references
        _ButtonComponent = GetComponent<Button>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerEnter(PointerEventData eventdata) {
        
        if (TextAttached != null) {

            // Update text colour
            TextAttached.color = Hover_TextColour;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerExit(PointerEventData eventdata) {
        
        if (TextAttached != null) {

            // Update text colour
            TextAttached.color = Default_TextColour;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerClick(PointerEventData eventdata) {

        if (TextAttached != null) {

            // Update text colour
            TextAttached.color = Hover_TextColour;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}