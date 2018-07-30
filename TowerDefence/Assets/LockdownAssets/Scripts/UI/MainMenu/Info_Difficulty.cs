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

public class Info_Difficulty : MonoBehaviour, IPointerEnterHandler {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" OBJECT INFORMATION")]
    [Space]
    public DifficultyManager.Difficulties Difficulty;
    public string DifficultyDescription = "";
    public Sprite DifficultyThumbnailSprite = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUTTON HOVER PROPERTIES")]
    [Space]
    public Text HoverText = null;

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
        if (HoverText != null) { HoverText.text = DifficultyDescription; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  string
    /// </returns>
    public string GetUIEnumerator() {

        switch (Difficulty) {
            case DifficultyManager.Difficulties.Easy:     { return "Normal"; }
            case DifficultyManager.Difficulties.Normal:       { return "Hard"; }
            case DifficultyManager.Difficulties.Hard:   { return "Very Hard"; }
            case DifficultyManager.Difficulties.Impossible: { return "Impossible"; }
            default: break;
        }
        return "Undefined";
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}