using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 25/7/2018
//
//******************************

public class UI_PlatoonUnits : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PANELS")]
    [Space]
    public List<UI_UnitInfoPanel> UnitInfoPanels;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _PlayerAttached;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    private void Start() {

        // Get component references
        _PlayerAttached = GameManager.Instance.Players[0];
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        for (int i = 0; i < UnitInfoPanels.Count; i++) {

            // Update platoon size
            UnitInfoPanels[i].AmountCounter.text = _PlayerAttached.GetPlatoon(i)._Size.ToString();
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}