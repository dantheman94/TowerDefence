using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Joshua D'Agostino
//  Last edited on: 29/10/2018
//
//******************************

public class ShowcasePodium : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PODIUM PROPERTIES")]
    [Space]
    public Transform StartingShowcaseTransform = null;
    public Selectable DefaultDisplayObject = null;
    [Space]
    public Camera ShowcaseRenderCamera = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public float rotateSpeed;

    private Vector3 _ShowcasePosition = Vector3.zero;
    private Quaternion _ShowcaseRotation = Quaternion.identity;

    private Selectable _CurrentShowcaseObject = null;
    private Renderer _CurrentShowcaseRendererer = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is created.
    /// </summary>
    private void Start() {
        
        // Update showcase transform
        if (StartingShowcaseTransform != null) { _ShowcasePosition = StartingShowcaseTransform.position; }
        if (StartingShowcaseTransform != null) { _ShowcaseRotation = StartingShowcaseTransform.rotation; }

        // Create default showcase object
        GameObject gObj = ObjectPooling.Spawn(DefaultDisplayObject.gameObject, _ShowcasePosition, _ShowcaseRotation);
        _CurrentShowcaseObject = gObj.GetComponent<Selectable>();
        _CurrentShowcaseObject._ObjectState = Abstraction.WorldObjectStates.Active;
        (_CurrentShowcaseObject as WorldObject).SetShowHealthBar(false);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        UpdateKeyboardInput();
        UpdateGamepadInput();

        // Update showcase object's transform
        if (_CurrentShowcaseObject != null) {

            _CurrentShowcaseObject.transform.position = _ShowcasePosition;
            _CurrentShowcaseObject.transform.Rotate(0, Time.deltaTime * rotateSpeed, 0);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void UpdateKeyboardInput() {


    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void UpdateGamepadInput() {

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Update the current showcase object reference.
    /// </summary>
    /// <param name="button"></param>
    public void ChangeSelection(Button button) {

        // Get button reference component
        ShowcaseButtonRef btnref = button.gameObject.GetComponent<ShowcaseButtonRef>();

        // Update showcase object reference
        if (btnref != null && _CurrentShowcaseObject != null) {

            // Destroy old showcase object
            ObjectPooling.Despawn(_CurrentShowcaseObject.gameObject);

            // Reset showcase transform
            if (StartingShowcaseTransform != null) { _ShowcasePosition = StartingShowcaseTransform.position; }
            if (StartingShowcaseTransform != null) { _ShowcaseRotation = StartingShowcaseTransform.rotation; }

            // Create new showcase object
            Selectable select = btnref.ObjectReference;
            GameObject gObj = ObjectPooling.Spawn(select.gameObject, _ShowcasePosition, _ShowcaseRotation);
            _CurrentShowcaseObject = gObj.GetComponent<Selectable>();
            _CurrentShowcaseObject._ObjectState = Abstraction.WorldObjectStates.Active;
            (_CurrentShowcaseObject as WorldObject).SetShowHealthBar(false);

            // Add offset to the showcase position
            _ShowcasePosition.y += _CurrentShowcaseObject.ShowcaseOffsetY;
            
            // Update camera properties
            if (ShowcaseRenderCamera != null) {

                ShowcaseRenderCamera.fieldOfView = _CurrentShowcaseObject.ShowcaseFOV;
            }
        }        
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
