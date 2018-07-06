using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/6/2018
//
//******************************

public class Platoon : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PLATOON PROPERTIES")]
    [Space]
    public int KeybindingID;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private List<Squad> _PlatoonSquads;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
    /// </summary>
    private void Start() {

        _PlatoonSquads = new List<Squad>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="squadsToAdd"></param>
    public void AddToPlatooon(List<Squad> squadsToAdd) {

        // Loop through the list and add it to the platoon
        foreach (var squad in squadsToAdd) { _PlatoonSquads.Add(squad); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="replacementSquads"></param>
    public void ReplacePlatoon(List<Squad> replacementSquads) {

        // Clear the current platoon
        _PlatoonSquads.Clear();

        // Loop through the list and add it to the platoon
        foreach (var squad in replacementSquads) { _PlatoonSquads.Add(squad); }
    }

}