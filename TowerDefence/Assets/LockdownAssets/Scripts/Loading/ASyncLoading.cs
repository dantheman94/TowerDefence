using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 29/7/2018
//
//******************************

public class ASyncLoading : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public AsyncOperation Async { get; private set; }
    public static ASyncLoading Instance;

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

        // If the singleton has already been initialized yet
        if (Instance != null && Instance != this) {

            Destroy(this.gameObject);
            return;
        }

        // Set singleton
        Instance = this;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="index"></param>
    public void LoadLevel(int index) {

        Async = SceneManager.LoadSceneAsync(index);
        Async.allowSceneActivation = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void ActivateLevel() { Async.allowSceneActivation = true; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool LoadComplete() { return Async.isDone; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetSceneLoadProgress() { return Async.progress; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
