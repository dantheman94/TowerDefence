using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 11/6/2018
//
//******************************

public class UnitBuildingCounter : MonoBehaviour {

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
    private WorldObject _ObjectAttached = null;
    private Vector3 _UnitPosition;
    private Vector3 _PanelPosition;
    private RectTransform _RectTransform;
    private Text _TextComponent;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
    /// </summary>
    private void Start() {

        // Get component references
        _RectTransform = GetComponent<RectTransform>();
        _TextComponent = GetComponentInChildren<Text>();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update() {

        if (_ObjectAttached != null && _CameraAttached != null) {

            // Only show widget if the object is currently being built
            if (_ObjectAttached.getObjectState() == WorldObject.WorldObjectStates.Building) {

                // Update text to show how much time is remaining in the build
                int time = (int)_ObjectAttached.getCurrentBuildTimeRemaining();
                string healthString = time.ToString();
                _TextComponent.text = healthString;

                // Convert world position to screen space
                _UnitPosition = _CameraAttached.WorldToViewportPoint(_ObjectAttached.transform.position);

                // Set widget above the unit
                _PanelPosition = new Vector3(_UnitPosition.x + _HorizontalOffset, _UnitPosition.y + _VerticalOffset, _UnitPosition.z);
                _RectTransform.anchorMin = _PanelPosition;
                _RectTransform.anchorMax = _PanelPosition;
            }

            // Destroy prefab instance as we no longer need it anymore
            else { Destroy(this.gameObject); }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    public void setObjectAttached(WorldObject obj) { _ObjectAttached = obj; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cam"></param>
    public void setCameraAttached(Camera cam) { _CameraAttached = cam; }
    
}