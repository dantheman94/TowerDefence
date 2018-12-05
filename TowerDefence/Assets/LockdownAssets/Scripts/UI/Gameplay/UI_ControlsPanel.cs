using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/12/2018
//
//******************************

public class UI_ControlsPanel : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" CONTROLLERS")]
    [Space]
    public GameObject PC_Controls = null;
    public GameObject Xbox_Controls = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called once each frame.
    /// </summary>
    private void Update() {

        if (_Player == null) { _Player = GameManager.Instance.Players[0]; }   
        if (_Player != null) {
            
            if (PC_Controls != null) {

                // Show pc controls
                PC_Controls.SetActive(_Player._KeyboardInputManager.IsPrimaryController);
            }

            if (Xbox_Controls != null) {

                // Show xbox controls
                Xbox_Controls.SetActive(_Player._XboxGamepadInputManager.IsPrimaryController);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
