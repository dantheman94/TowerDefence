using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/10/2018
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
    public List<Platoon> PlatoonInfoPanels;

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

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        if (_PlayerAttached != null) {

            for (int i = 0; i < PlatoonInfoPanels.Count; i++) {

                // Update platoon size
                PlatoonInfoPanels[i].PlatoonCounter.text = _PlayerAttached.GetPlatoon(i).GetAi().Count.ToString();
            }
        }

        // Get component references
        else { _PlayerAttached = GameManager.Instance.Players[0]; }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}