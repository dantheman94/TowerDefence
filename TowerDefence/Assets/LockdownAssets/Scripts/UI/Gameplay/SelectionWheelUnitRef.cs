using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 17/10/2018
//
//******************************

public class SelectionWheelUnitRef : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MOVEMENT")]
    [Space]
    public Text QueueCounter = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [HideInInspector]
    public Abstraction AbstractRef { get; set; }

    private int _UnitCounter = 0;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        
        // Update queue counter text string
        if (_UnitCounter > 0 && QueueCounter != null) {

            QueueCounter.gameObject.SetActive(true);
            QueueCounter.text = _UnitCounter.ToString();
        }
        else if (QueueCounter != null) { QueueCounter.gameObject.SetActive(false); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="count"></param>
    public void SetQueueCounter(int count) { _UnitCounter = count; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}