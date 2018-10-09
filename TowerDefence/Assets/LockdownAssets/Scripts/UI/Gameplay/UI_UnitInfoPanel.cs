using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Joshua D'Agostino
//  Last edited on: 09/10/2018
//
//******************************

public class UI_UnitInfoPanel : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" COMPONENTS")]
    [Space]
    public Image LogoComponent = null;
    public TMP_Text UnitName = null;
    public TMP_Text AmountCounter = null;
    public TMP_Text AbilityText = null;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void Wipe() {

        // Clear everything
        if (LogoComponent != null) { LogoComponent.sprite = null; }
        if (UnitName != null) { UnitName.text = ""; }
        if (AmountCounter != null) { AmountCounter.text = ""; }
        if (AbilityText != null) { AmountCounter.text = ""; }

        // Hide the gameObject
        gameObject.SetActive(false);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}