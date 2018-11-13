using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//******************************
//
//  Created by: Joshua D'Agostino
//
//  Last edited by: Joshua D'Agostino
//  Last edited on: 14/11/2018
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
    //      VARIABLES
    //
    //******************************************************************************************************************************

        private Scene currentScene;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************


    /// <summary>
    //  Safety measure incase ResetScale is somehow not called
    /// </summary>
    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        if (currentScene.name ==  "_MasterScene") {
           transform.localScale = new Vector3(1, 1, 1);
          //  Debug.Log("Script should work");
        }  
    }
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
