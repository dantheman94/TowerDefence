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

public class Info_Level : MonoBehaviour, IPointerEnterHandler {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" OBJECT INFORMATION")]
    [Space]
    public int LevelIndex;
    [Space]
    public string LevelName = "";
    public string LevelDescription = "";
    public Sprite LevelThumbnailSprite = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUTTON HOVER PROPERTIES")]
    [Space]
    public Text HoverText = null;
    private Info_Level _Level;
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnPointerEnter(PointerEventData eventdata) {

        // Update hovered description text 
        if (HoverText != null) { HoverText.text = LevelDescription; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}