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
//  Last edited on: 6/12/2018
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
    public Image UpdateImageOnHover = null;
    public Sprite TextureSpriteToUpdateWith = null;
    [Space]
    public bool UpdateTextOnHover = false;
    public Text TextToUpdate;
    public string TextString = " ";
    [Space]
    public Image SelectorBar = null;
    public Image FillImage = null;
    [Space]
    public Color Default_TextColour = Color.white;
    public Color Default_OutlineColour = Color.black;
    public bool Default_OutlineEnabled = true;
    [Space]
    public bool Default_ShowSelectorBar = false;
    public Color Default_SelectorBarColour = Color.white;
    public bool Default_ShowFill = false;
    [Space]
    public int Default_FontSize = 26;
    [Space]
    [Space]
    public Color Hover_TextColour = Color.black;
    public Color Hover_OutlineColour = Color.white;
    public bool Hover_OutlineEnabled = false;
    [Space]
    public bool Hover_ShowSelectorBar = true;
    public Color Hover_SelectorBarColour = Color.white;
    public bool Hover_ShowFill = true;
    [Space]
    public int Hover_FontSize = 30;
    [Space]
    [Space]
    public Color Clicked_TextColour = Color.white;
    public Color Clicked_OutlineColour = Color.black;
    public bool Clicked_OutlineEnabled = true;
    [Space]
    public bool Clicked_ShowSelectorBar = true;
    public Color Clicked_SelectorBarColour = Color.grey;
    public bool Clicked_ShowFill = true;
    [Space]
    public int Clicked_FontSize = 26;
    [Space]
    [Space]
    public Color Down_TextColour = Color.white;
    public Color Down_OutlineColour = Color.black;
    public bool Down_OutlineEnabled = true;
    [Space]
    public int Down_FontSize = 26;
    [Space]
    public bool Down_ShowSelectorBar = true;
    public Color Down_SelectorBarColour = Color.grey;
    public bool Down_ShowFill = true;
    [Space]
    [Space]
    public Color Up_TextColour = Color.white;
    public Color Up_OutlineColour = Color.black;
    public bool Up_OutlineEnabled = true;
    [Space]
    public int Up_FontSize = 26;
    [Space]
    public bool Up_ShowSelectorBar = true;
    public Color Up_SelectorBarColour = Color.grey;
    public bool Up_ShowFill = true;

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
    //  Called when the button this script is attached to, is hovered.
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerEnter(PointerEventData eventdata) {

        if (TextAttached != null && ButtonAttached != null) {
            
            // Update selectorbar
            if (SelectorBar) {

                SelectorBar.gameObject.SetActive(Hover_ShowSelectorBar);
                SelectorBar.color = Hover_SelectorBarColour;
            }

            // Update imageFill
            if (FillImage) { FillImage.gameObject.SetActive(Hover_ShowFill); }

            // Update imageHover
            if (UpdateImageOnHover && TextureSpriteToUpdateWith) { UpdateImageOnHover.sprite = TextureSpriteToUpdateWith; }

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Hover_TextColour;
                TextAttached.fontSize = Hover_FontSize;

                // Update outline colour
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Hover_OutlineEnabled;
                    o.OutlineColor = Hover_OutlineColour;
                }

                // Update hover text
                if (UpdateTextOnHover && TextToUpdate != null) { TextToUpdate.text = TextString; }

                // Play hover button sound
                SoundManager.Instance.PlaySound("SFX/_SFX_Back_Alt", 1f, 1f, false);
            }
            // Update hover text
            else {
                if (UpdateTextOnHover && TextToUpdate != null) { TextToUpdate.text = "Changing difficulty is disabled in the tutorial level."; }
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

            // Update selectorbar
            if (SelectorBar) {

                SelectorBar.gameObject.SetActive(Default_ShowSelectorBar);
                SelectorBar.color = Default_SelectorBarColour;
            }

            // Update imageFill
            if (FillImage) { FillImage.gameObject.SetActive(Default_ShowFill); }

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Default_TextColour;
                TextAttached.fontSize = Default_FontSize;

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

            // Update selectorbar
            if (SelectorBar) {

                SelectorBar.gameObject.SetActive(Clicked_ShowSelectorBar);
                SelectorBar.color = Clicked_SelectorBarColour;
            }

            // Update imageFill
            if (FillImage) { FillImage.gameObject.SetActive(Clicked_ShowFill); }

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Clicked_TextColour;
                TextAttached.fontSize = Clicked_FontSize;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Clicked_OutlineEnabled;
                    o.OutlineColor = Clicked_OutlineColour;
                }

                // Play button press sound
                SoundManager.Instance.PlaySound("SFX/_SFX_Woosh1", 1f, 1f, false);
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

            // Update selectorbar
            if (SelectorBar) {

                SelectorBar.gameObject.SetActive(Down_ShowSelectorBar);
                SelectorBar.color = Down_SelectorBarColour;
            }

            // Update imageFill
            if (FillImage) { FillImage.gameObject.SetActive(Down_ShowFill); }

            // Update imageHover
            if (UpdateImageOnHover && TextureSpriteToUpdateWith) { UpdateImageOnHover.sprite = TextureSpriteToUpdateWith; }

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Down_TextColour;
                TextAttached.fontSize = Down_FontSize;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Down_OutlineEnabled;
                    o.OutlineColor = Down_OutlineColour;
                }

                // Update hover text
                if (UpdateTextOnHover && TextToUpdate != null) { TextToUpdate.text = TextString; }

                // Play hover button sound
                SoundManager.Instance.PlaySound("SFX/_SFX_Back_Alt", 1f, 1f, false);
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

            // Update selectorbar
            if (SelectorBar) {

                SelectorBar.gameObject.SetActive(Up_ShowSelectorBar);
                SelectorBar.color = Up_SelectorBarColour;
            }

            // Update imageFill
            if (FillImage) { FillImage.gameObject.SetActive(Up_ShowFill); }

            // Update imageHover
            if (UpdateImageOnHover && TextureSpriteToUpdateWith) { UpdateImageOnHover.sprite = TextureSpriteToUpdateWith; }

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Up_TextColour;
                TextAttached.fontSize = Up_FontSize;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Up_OutlineEnabled;
                    o.OutlineColor = Up_OutlineColour;
                }

                // Update hover text
                if (UpdateTextOnHover && TextToUpdate != null) { TextToUpdate.text = TextString; }

                // Play hover button sound
                SoundManager.Instance.PlaySound("SFX/_SFX_Back_Alt", 1f, 1f, false);
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

            // Update imageHover
            if (UpdateImageOnHover && TextureSpriteToUpdateWith) { UpdateImageOnHover.sprite = TextureSpriteToUpdateWith; }

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Down_TextColour;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Down_OutlineEnabled;
                    o.OutlineColor = Down_OutlineColour;
                }

                // Update selectorbar
                if (SelectorBar) {

                    SelectorBar.gameObject.SetActive(Down_ShowSelectorBar);
                    SelectorBar.color = Down_SelectorBarColour;
                }

                // Update imageFill
                if (FillImage) { FillImage.gameObject.SetActive(Down_ShowFill); }

                // Update hover text
                if (UpdateTextOnHover && TextToUpdate != null) { TextToUpdate.text = TextString; }

                // Play hover button sound
                SoundManager.Instance.PlaySound("SFX/_SFX_Back_Alt", 1f, 1f, false);
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

            // Update imageHover
            if (UpdateImageOnHover && TextureSpriteToUpdateWith) { UpdateImageOnHover.sprite = TextureSpriteToUpdateWith; }

            if (ButtonAttached.interactable) {

                // Update text colour
                TextAttached.color = Up_TextColour;

                // Update outline
                Outline o = TextAttached.GetComponent<Outline>();
                if (o != null) {

                    o.enabled = Up_OutlineEnabled;
                    o.OutlineColor = Up_OutlineColour;
                }

                // Update selectorbar
                if (SelectorBar) {

                    SelectorBar.gameObject.SetActive(Up_ShowSelectorBar);
                    SelectorBar.color = Up_SelectorBarColour;
                }

                // Update imageFill
                if (FillImage) { FillImage.gameObject.SetActive(Up_ShowFill); }

                // Update hover text
                if (UpdateTextOnHover && TextToUpdate != null) { TextToUpdate.text = TextString; }

                // Play hover button sound
                SoundManager.Instance.PlaySound("SFX/_SFX_Back_Alt", 1f, 1f, false);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}