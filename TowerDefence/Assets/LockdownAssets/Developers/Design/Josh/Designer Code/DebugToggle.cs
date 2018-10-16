using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua D'Agostino
//
//  Last edited by: Joshua D'Agostino
//  Last edited on: 16/10/2018
//
//******************************

public class DebugToggle : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    public KeyCode keyToUse;

    public List<GameObject> objectsToToggle;

    private bool keyPressedToggle;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    void Update()
    {
        DoTheToggle();
    }


    /// <summary>
    /// Goes through each item in the list and enables them and disables them if the button is pressed again
    /// </summary>
    void DoTheToggle()
    {
        if (Input.GetKeyDown(keyToUse))
        {
            
            keyPressedToggle = !keyPressedToggle; //Toggles bool each time key is pressed

            if (keyPressedToggle)//Enable the objects
            {
                foreach (GameObject obj in objectsToToggle)
                {
                    obj.SetActive(true);
                }
            }
            else { //Disables the objects
                foreach (GameObject obj in objectsToToggle)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}

