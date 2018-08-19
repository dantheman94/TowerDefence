using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/8/2018
//
//******************************

public class Upgrade : Abstraction {
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the upgrading is complete and needs to be applied.
    /// </summary>
    public virtual void OnUpgrade() {

        Debug.Log("Upgrade Applied.");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}