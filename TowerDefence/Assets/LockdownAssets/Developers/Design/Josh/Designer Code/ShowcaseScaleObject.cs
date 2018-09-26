using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//******************************
//
//  Created by: Joshua D'Agostino
//
//  Last edited by: Joshua D'Agostino
//  Last edited on: 27/09/2018
//
//******************************
public class ShowcaseScaleObject : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Tooltip ("The X,Y,Z Scale to set for the Asset")]
    public float scaleSize;

//******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
    
    /// <summary>
    //  Called when pointer enters button. Use with Event Trigger
    /// </summary>
    public void ScaleObject() {
        // Sets the local scale of the object by what the scaleSize is set to.
        transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
    }


    /// <summary>
    //  Called when pointer exits button. Use with Event Trigger
    /// </summary>
    public void ResetScale()
    {
        // Return the scale back to 1,1,1
        transform.localScale = new Vector3(1, 1, 1);

    }
}
