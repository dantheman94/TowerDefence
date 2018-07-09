using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 26/5/2018
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
    private Building _BuildingAttached = null;
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
    //  
    /// </summary>
    private void Update() {
        
        if (_WorldObject != null && _CameraAttached != null) {

            // Display text if the object is a building
            if (_BuildingAttached != null) {

                _TextComponent.enabled = true;
                _TextComponent.text = _WorldObject.ObjectName;
            }

            // Don't show the text component if the object is NOT a building
            else { _TextComponent.enabled = false; }

            // Object is alive
            if (_WorldObject.isActiveInWorld()) {
                
                // Set world space position
                Vector3 pos = _WorldObject.transform.position + Offsetting;
                pos.y = pos.y + _WorldObject.GetObjectHeight();
                transform.position = pos;

                // Constantly face the widget towards the camera
                transform.LookAt(2 * transform.position - _CameraAttached.transform.position);
            }

            // Object is dead/destroyed
            else {  ObjectPooling.Despawn(this.gameObject); }
        } 
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="obj"></param>
    public void setObjectAttached(WorldObject obj) {

        // Set localized reference of world object attached
        _WorldObject = obj;
        _BuildingAttached = _WorldObject.GetComponent<Building>();

        // Set object's health bar reference
        _WorldObject.setHealthBar(this);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="cam"></param>
    public void setCameraAttached(Camera cam) { _CameraAttached = cam; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
