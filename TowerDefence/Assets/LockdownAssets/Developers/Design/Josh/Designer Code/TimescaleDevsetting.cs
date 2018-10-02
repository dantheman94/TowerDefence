﻿using System.Collections;
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
        "F11 to return to normal speed";

    public float timeScale = 1.0f;
    public GameObject textobj;
    private Text texttoset;

    void Start()
    {
        texttoset = textobj.GetComponent<Text>();
    }

    void Update () {

        /// <summary>
        //  Wait for key presses to change Time Scale speed.
        /// </summary>
        /// 
        if (Input.GetKeyDown(KeyCode.F12)) {
            Time.timeScale = timeScale;
            texttoset.text = "Game is "+ timeScale + "x speed" ;
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Time.timeScale = 1.0f;
            texttoset.text = " ";
        }
    }
}
