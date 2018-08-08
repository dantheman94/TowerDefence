using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 08/08/2018
//
//******************************

public class ButtonText : MonoBehaviour, ISelectHandler, IDeselectHandler {

    private Button TargetButton;
    private Text ButtonTexts;

    // Use this for initialization
    void Start()
    {
        TargetButton = GetComponent<Button>();
        ButtonTexts = GetComponentInChildren<Text>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(ButtonTexts != null)
        ButtonTexts.color = Color.black;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (ButtonTexts != null)
            ButtonTexts.color = Color.white;
    }
}
