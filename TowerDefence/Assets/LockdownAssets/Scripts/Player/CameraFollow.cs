using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/7/2018
//
//******************************

public class CameraFollow : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" CAMERA FOLLOW PROPERTIES")]
    [Space]
    public float Offset = 50f;
    public float ChaseSpeed = 0.3f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Camera _CameraAttached = null;
    private Selectable _FollowTarget = null;
    private bool _Following = false;
    private Vector3 _CurrentVelocity = Vector3.zero;
    private Vector3 _ChasePosition = Vector3.zero;
    private Transform _CameraOrigTransform;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************
    
    /// <summary>
    //   
    /// </summary>
    public void Init() {

        SetFollowing(true);
    }

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {
        
        // Valid object following execution
        if (_Following && _FollowTarget != null && _CameraAttached != null) {

            // Get chase target position
            Transform trans = new GameObject().transform;
            trans.position = new Vector3(_FollowTarget.transform.position.x, _CameraAttached.transform.position.y, _FollowTarget.transform.position.z);            
            trans.position -= _CameraOrigTransform.up * Offset;
            _ChasePosition = trans.position;

            // Clamp the chase position to mix / max heights
            if (_ChasePosition.y < Settings.MinCameraHeight) { _ChasePosition.y = Settings.MinCameraHeight; }
            if (_ChasePosition.y > Settings.MaxCameraHeight) { _ChasePosition.y = Settings.MaxCameraHeight; }

            // Smoothly move toward target position
            _CameraAttached.transform.position = Vector3.SmoothDamp(_CameraAttached.transform.position, _ChasePosition, ref _CurrentVelocity, ChaseSpeed);
            
            // Destroy obsolete gameobject
            Destroy(trans.gameObject);
        }
    }

    /// <summary>
    //  
    /// </summary>
    /// <param name="target"></param>
    public void SetFollowTarget(Selectable target) { _FollowTarget = target; }

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public Selectable GetFollowTarget() { return _FollowTarget; }

    /// <summary>
    //  
    /// </summary>
    /// <param name="value"></param>
    public void SetFollowing(bool value) {

        _Following = value;
        if (value == true) {

            _CameraOrigTransform = _CameraAttached.transform;
        }
    }

    /// <summary>
    //  
    /// </summary>
    /// <param name="cam"></param>
    public void SetCameraAttached(Camera cam) { _CameraAttached = cam; }

}