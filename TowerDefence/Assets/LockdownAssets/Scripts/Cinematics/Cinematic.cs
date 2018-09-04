using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 2/9/2018
//
//******************************

public class Cinematic : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" CINEMATIC PROPERTIES")]
    [Space]
    public Camera ViewCamera = null;
    public float CameraLerpSpeed = 30f;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Starts the cinematic
    /// </summary>
    public virtual void StartCinematic() {

        // Show cinematic black bars
        GameManager.Instance.CinematicBars.gameObject.SetActive(true);
        CinematicBars.Instance.StartAnimation(CinematicBars.AnimationDirection.Enter);

        // Hide HUD
        GameManager.Instance.WorldSpaceCanvas.gameObject.SetActive(false);
        GameManager.Instance.HUDWrapper.gameObject.SetActive(false);

        // Hide mouse cursor
        Cursor.visible = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
