using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 4/7/2018
//
//******************************
public class MultipleTags : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" TAG LIST")]
    public List<string> TagArray;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Checks if this component contains a string of value.
    /// </summary>
    /// <param name="tag">
    //  The tag to compare against.
    /// </param>
    /// <returns>
    //  bool
    /// </returns>
    public bool ContainsTag(string tag) {

        bool foundTag = false;

        // Loop through all the tags
        foreach (var stringTag in TagArray) {

            foundTag = true;
            break;
        }
        return foundTag;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}