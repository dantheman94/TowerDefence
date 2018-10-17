using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 20/8/2018
//
//******************************

public class UI_BuildingQueueItem : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Text _TextComponent = null;

    private Abstraction _AbstractionAttached = null;
    private Slider _SliderComponent = null;
    private float _BuildPercentage = 0f;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this gameObject is created.
    /// </summary>
    private void Start() {

        // Get component references
        _SliderComponent = GetComponent<Slider>();
        _TextComponent = GetComponentInChildren<Text>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {
        
        // Update build percentage / slider value
        if (_SliderComponent != null && _AbstractionAttached != null) {

            _TextComponent.text = _AbstractionAttached.ObjectName;
            _BuildPercentage = _AbstractionAttached.GetBuildPercentage();
            _SliderComponent.value = _BuildPercentage;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="abs"></param>
    public void SetAbstractionAttached(Abstraction abs) { _AbstractionAttached = abs; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Abstraction
    /// </returns>
    public Abstraction GetAbstractionLinked() { return _AbstractionAttached; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}