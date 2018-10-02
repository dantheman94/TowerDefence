using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Joshua D'Agostino
//
//  Last edited by: Joshua D'Agostino
//  Last edited on: 02/10/2018
//
//******************************
public class TimescaleDevsetting : MonoBehaviour {
    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [TextArea]
    //[Tooltip("Doesn't do anything. Just comments shown in inspector")]
    public string Notes = "F12 to speed up game by Time Scale amount. " + " " +
        "F11 to return to normal speed"+ " "+ "Game is unstable if speed is over 10, So it's capped";

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    [Tooltip("Speed can't be over 10, will round down if over")]
    public float timeScale = 1.0f;
    [Tooltip("Text obj is under Debugger > GameSpeedText")]
    public GameObject textObj;
    private Text textToSet;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        textToSet = textObj.GetComponent<Text>();

        if (timeScale >= 10)
        {
            timeScale = 10.0f;
        }
    }

    void Update () {

        /// <summary>
        //  Wait for key presses to change Time Scale speed.
        /// </summary>
        /// 
        if (Input.GetKeyDown(KeyCode.F12)) {
            if (timeScale >= 10) {
                timeScale = 10.0f;
            }
            Time.timeScale = timeScale;
            textToSet.text = "Game is "+ timeScale + "x speed" ;
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Time.timeScale = 1.0f;
            textToSet.text = " ";
        }
    }
}
