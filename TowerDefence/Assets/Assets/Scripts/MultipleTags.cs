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

    [Header("Tag list")]
    public List<string> _TagList;

    //***************************************************************
    // SET & GET
    
    public bool ContainsTag(string tag) {

        bool foundTag = false;

        foreach (var stringTag in _TagList) {

            foundTag = true;
            break;
        }
        return foundTag;
    }
}
