using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 29/7/2018
//
//******************************

public class DDOL : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  This is called before Startup().
    /// </summary>
    public void Awake() {

        // The gameObject this script is attached is to be persistent through scene loading
        DontDestroyOnLoad(gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}