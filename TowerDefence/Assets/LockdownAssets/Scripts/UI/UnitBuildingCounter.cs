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
    [Header(" OFFSETS")]
    [Space]
    public Vector3 Offsetting = Vector3.zero;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Camera _CameraAttached = null;
    private WorldObject _WorldObject = null;
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

        if (_WorldObject != null && _CameraAttached != null) {

            // Only show widget if the object is currently being built
            if (_WorldObject.getObjectState() == WorldObject.WorldObjectStates.Building) {

                // Update text to show how much time is remaining in the build
                int time = (int)_WorldObject.getCurrentBuildTimeRemaining();
                string healthString = time.ToString();
                _TextComponent.text = healthString;

                // Set world space position
                Vector3 pos = _WorldObject.transform.position + Offsetting;
                pos.y = pos.y + _WorldObject.GetObjectHeight();
                transform.position = pos;

                // Constantly face the widget towards the camera
                transform.LookAt(2 * transform.position - _CameraAttached.transform.position);
            }

            // Destroy prefab instance as we no longer need it anymore
            else { ObjectPooling.Despawn(this.gameObject); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    public void setObjectAttached(WorldObject obj) { _WorldObject = obj; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cam"></param>
    public void setCameraAttached(Camera cam) { _CameraAttached = cam; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}