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
//  Last edited on: 17/9/2018
//
//******************************

public class ButtonHover_MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUTTON HOVER PROPERTIES")]
    [Space]
    public bool UpdateTextOnHover = false;
    public Text TextToUpdate;
    public string TextString = " ";
    [Space]
    public Color Default_TextColour = Color.white;
    public Color Default_OutlineColour = Color.black;
    public bool Default_OutlineEnabled = true;
    [Space]
    public Color Hover_TextColour = Color.black;
    public Color Hover_OutlineColour = Color.white;
    public bool Hover_OutlineEnabled = false;
    [Space]
    public Color Clicked_TextColour = Color.white;
    public Color Clicked_OutlineColour = Color.black;
    public bool Clicked_OutlineEnabled = true;
    [Space]
    public Color Down_TextColour = Color.white;
    public Color Down_OutlineColour = Color.black;
    public bool Down_OutlineEnabled = true;
    [Space]
    public Color Up_TextColour = Color.white;
    public Color Up_OutlineColour = Color.black;
    public bool Up_OutlineEnabled = true;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Text TextAttached = null;
    private Button ButtonAttached = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is created.
    /// </summary>
    private void Start() {
        
        ButtonAttached = GetComponent<Button>();
        TextAttached = GetComponentInChildren<Text>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the button this script is attached to, is hovered.
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerEnter(PointerEventData eventdata) {

        if (TextAttached != null && ButtonAttached != null) {

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Hover_TextColour;

                // Update outline colour
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Hover_OutlineEnabled;
                    o.OutlineColor = Hover_OutlineColour;
                }

                // Update hover text
                if (UpdateTextOnHover && TextToUpdate != null) { TextToUpdate.text = TextString; }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the button this script is attached to, is unhovered.
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerExit(PointerEventData eventdata) {
        
        if (TextAttached != null && ButtonAttached != null) {

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Default_TextColour;

                // Update outline colour
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Default_OutlineEnabled;
                    o.OutlineColor = Default_OutlineColour;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the button this script is attached to, is clicked.
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerClick(PointerEventData eventdata) {

        if (TextAttached != null && ButtonAttached != null) {

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Clicked_TextColour;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Clicked_OutlineEnabled;
                    o.OutlineColor = Clicked_OutlineColour;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the button this script is attached to, is pressed down.
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerDown(PointerEventData eventdata) {

        if (TextAttached != null && ButtonAttached != null) {

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Down_TextColour;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Down_OutlineEnabled;
                    o.OutlineColor = Down_OutlineColour;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the button this script is attached to, is no longer pressed.
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerUp(PointerEventData eventdata) {

        if (TextAttached != null && ButtonAttached != null) {

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Up_TextColour;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Up_OutlineEnabled;
                    o.OutlineColor = Up_OutlineColour;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData) {

        if (TextAttached != null && ButtonAttached != null) {

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Down_TextColour;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Down_OutlineEnabled;
                    o.OutlineColor = Down_OutlineColour;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData) {

        if (TextAttached != null && ButtonAttached != null) {

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Up_TextColour;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Up_OutlineEnabled;
                    o.OutlineColor = Up_OutlineColour;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}