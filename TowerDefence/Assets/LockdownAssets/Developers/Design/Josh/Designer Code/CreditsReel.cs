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
public class CreditsReel : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Tooltip("The Image of the credits")]
    public GameObject creditsImage;

    [Tooltip("Multiply the speed of the reel by this much")]
    public float speed;
    
	void Update () {

        /// <summary>
        //  Move Credits object up in world space by (Deltatime x speed) that is set in inspector
        /// </summary> 
        creditsImage.transform.Translate(0, Time.deltaTime*10*speed, 0, Space.World);
    }
}
