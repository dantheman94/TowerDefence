using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/9/2018
//
//******************************

public class UnitVeterancyCounter : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" OFFSETS")]
    [Space]
    public Vector3 Offsetting = Vector3.zero;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Camera _CameraAttached = null;
    private Unit _UnitAttached = null;
    private Text _TextComponent;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Get component references
        _TextComponent = GetComponentInChildren<Text>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        if (_UnitAttached != null && _CameraAttached != null) {
            
            // Unit is alive - display the widget
            if (_UnitAttached.IsInWorld() && _UnitAttached.GetVeterancyLevel() > 0) {

                // Update text
                _TextComponent.text = _UnitAttached.GetVeterancyLevel().ToString();

                // Set world space position
                Vector3 pos = _UnitAttached.transform.position + Offsetting;
                pos.y = pos.y + _UnitAttached.GetObjectHeight();
                transform.position = pos;

                // Constantly face the widget towards the camera
                transform.LookAt(2 * transform.position - _CameraAttached.transform.position);
            }

            // Object is dead/destroyed
            else { ObjectPooling.Despawn(gameObject); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnitAttached(Unit unit) { _UnitAttached = unit; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cam"></param>
    public void SetCameraAttached(Camera cam) { _CameraAttached = cam; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
