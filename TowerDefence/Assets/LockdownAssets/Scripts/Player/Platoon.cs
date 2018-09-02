using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 27/7/2018
//
//******************************

public class Platoon : MonoBehaviour {
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private List<Ai> _PlatoonAi;
    public int _Size { get; set; }

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is created.
    /// </summary>
    private void Start() {

        _PlatoonAi = new List<Ai>();
        _Size = 0;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        // Update platoon size
        _Size = _PlatoonAi.Count;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="squadsToAdd"></param>
    public void AddToPlatooon(List<Squad> squadsToAdd) {

        // Loop through the list and add it to the platoon
        foreach (var squad in squadsToAdd) { _PlatoonAi.Add(squad); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="replacementSquads"></param>
    public void ReplacePlatoon(List<Squad> replacementSquads) {

        // Clear the current platoon
        _PlatoonAi.Clear();
        _Size = 0;

        // Loop through the list and add it to the platoon
        foreach (var squad in replacementSquads) { _PlatoonAi.Add(squad); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public List<Ai> GetAi() { return _PlatoonAi; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}