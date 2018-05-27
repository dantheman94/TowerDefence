using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/26/2018
//
//******************************

public class UnitHealthBar : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header("OFFSETS")]
    [Range(-0.1f, 0.1f)]
    public float _VerticalOffset = 0.04f;
    [Range(-0.1f, 0.1f)]
    public float _HorizontalOffset = 0f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Camera _CameraAttached = null;
    private WorldObject _WorldObject;
    private Vector3 _UnitPosition;
    private Vector3 _PanelPosition;
    private RectTransform _RectTransform;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    private void Start() {

        // Get component references
        _RectTransform = GetComponent<RectTransform>();
    }

    private void Update() {
        
        if (_WorldObject != null && _CameraAttached != null) {

            // Unit is alive
            if (_WorldObject.isActiveInWorld()) {

                // Convert world position to screen space
                _UnitPosition = _CameraAttached.WorldToViewportPoint(_WorldObject.transform.position);

                // Set widget above the unit
                _PanelPosition = new Vector3(_UnitPosition.x + _HorizontalOffset, _UnitPosition.y + _VerticalOffset, _UnitPosition.z);
                _RectTransform.anchorMin = _PanelPosition;
                _RectTransform.anchorMax = _PanelPosition;
            }

            // Unit is dead/destroyed
            else {

            }
        } 
    }

    public void setObjectAttached(WorldObject obj) {

        // Set localized reference of world object attached
        _WorldObject = obj;

        // Set object's health bar reference
        _WorldObject.setHealthBar(this);
    }

    public void setCameraAttached(Camera cam) { _CameraAttached = cam; }

}
