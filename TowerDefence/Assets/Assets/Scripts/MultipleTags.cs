using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class MultipleTags : MonoBehaviour {

    //***************************************************************
    // VARIABLES

    [Space]
    [Header("-----------------------------------")]
    [Header(" TAG LIST")]
    public List<string> TagArray;

    //***************************************************************
    // SET & GET
    
    public bool ContainsTag(string tag) {

        bool foundTag = false;

        // Loop through all the tags
        foreach (var stringTag in TagArray) {

            foundTag = true;
            break;
        }
        return foundTag;
    }
}
