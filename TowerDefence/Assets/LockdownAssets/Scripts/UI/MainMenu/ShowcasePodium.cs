using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/09/2018
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
    public GameObject ShowcaseObjectRef = null;
    [Space]
    public Selectable DefaultDisplayObject = null;
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Vector3 _ShowcasePosition = Vector3.zero;
    private Quaternion _ShowcaseRotation = Quaternion.identity;

    private Selectable _CurrentShowcaseObject = null;

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
        
        if (ShowcaseObjectRef != null) { _ShowcasePosition = ShowcaseObjectRef.transform.position; }
        _CurrentShowcaseObject = DefaultDisplayObject;
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
            _CurrentShowcaseObject.transform.rotation = _ShowcaseRotation;
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
        if (btnref != null) {

            Selectable select = btnref.ObjectReference;
            GameObject g = ObjectPooling.Spawn(select.gameObject, _ShowcasePosition, _ShowcaseRotation);
            g.gameObject.layer = LayerMask.NameToLayer("Showcase");
            _CurrentShowcaseObject = g.GetComponent<Selectable>();
            _CurrentShowcaseObject._ObjectState = Abstraction.WorldObjectStates.Active;
        }

        
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
