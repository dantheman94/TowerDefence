using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 11/7/2018
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
    public Text UnitName = null;
    public Text AmountCounter = null;
    public Text AbilityText = null;

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