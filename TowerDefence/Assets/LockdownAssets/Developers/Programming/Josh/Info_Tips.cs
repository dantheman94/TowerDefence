using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 6/08/2018
//
//******************************

public class Info_Tips : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" LOADING MENU TIP SLOTS")]
    [Space]

    [Tooltip("First tip slot.")]
    public Text TipSlot1;
    [Tooltip("Second tip slot.")]
    public Text TipSlot2;
    [Tooltip("Third tip slot.")]
    public Text TipSlot3;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public List<string> _TipsList = new List<string>();

    int _TipDecider1;
    int _TipDecider2;
    int _TipDecider3;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start () {

        // Set tip deciders to random values
        _TipDecider1 = Random.Range(1, _TipsList.Count);
        _TipDecider2 = Random.Range(1, _TipsList.Count);
        _TipDecider3 = Random.Range(1, _TipsList.Count);

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update () {

        // Give it a new value if it's the same as _TipDecider1 or 2
        if (_TipDecider2 == _TipDecider1 || _TipDecider2 == _TipDecider3) {
            _TipDecider2 = Random.Range(1, _TipsList.Count);
        }

        // Give it a new value if it's the same as _TipDecider2 or 3
        if (_TipDecider3 == _TipDecider1 || _TipDecider3 == _TipDecider2) {
            _TipDecider3 = Random.Range(1, _TipsList.Count);
        }

        // Assign tips to text
        TipSlot1.text = _TipsList[_TipDecider1];
        TipSlot2.text = _TipsList[_TipDecider2];
        TipSlot3.text = _TipsList[_TipDecider3];
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////