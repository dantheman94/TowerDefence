using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 6/8/2018
//
//******************************

public class AirVehicle : Vehicle {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" AIR-VEHICLE PROPERTIES")]
    [Space]
    public float ForwardAvoidanceRange = 20f;
    public float DownwardsAvoidanceRange = 20f;
    public float VerticalSpeed = 10f;
    [Space]
    public float MaximumFlyingHeight = 100f;
    public LayerMask AvoidanceLayerMask;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected bool _ForwardRayDetection = false;
    protected bool _DownwardRayDetection = false;
    protected bool _AngleRayDetection = false;

    private float MinimumFlyingHeight = 20f;
    private float _IdealFlyingHeightMin = 40f;
    private float _IdealFlyingHeightMax = 80f;
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {
        base.Start();
        
        // Initialize "ideal" flying heights based off game world
        MinimumFlyingHeight = GameManager.Instance.FlyingNavMesh.transform.position.y;
        _IdealFlyingHeightMin = GameManager.Instance.FlyingNavMesh.transform.position.y + _Agent.height;
        _IdealFlyingHeightMax = GameManager.Instance.FlyingNavMesh.transform.position.y + DownwardsAvoidanceRange + 5f;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Update agent height
        if (_ObjectState == WorldObjectStates.Active) { UpdateHeight(); }

        // Clamp max movement speed
        if (_Agent.speed > MaxSpeed) { _Agent.speed = MaxSpeed; }

        // Set the highlight & selected prefab heights to be at a matching height as the unit
        Vector3 pos = transform.position;
        pos.y = pos.y - _Agent.height / 2; /// THIS IS TEMPORARY, JUST SO THE PREFAB DOESNT INTERSECT THE ACTUAL UNIT AND LOOK FUNKY
        if (_SelectionObj != null) { _SelectionObj.transform.position = pos; }
        if (_HighlightObj != null) { _HighlightObj.transform.position = pos; }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called only once, when the unit transitions to an active state.
    /// </summary>
    public override void OnSpawn() {
        base.OnSpawn();

        // Reset agent base offset
        _Agent.baseOffset = Settings.MaxCameraHeight + 30f;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. [ From Update() ]
    /// </summary>
    private void UpdateHeight() {
        
        // Fire a raycast forward
        RaycastHit hitForward;
        if (_ForwardRayDetection = Physics.Raycast(transform.position, transform.forward, out hitForward, ForwardAvoidanceRange, AvoidanceLayerMask)) {

            Debug.DrawRay(transform.position, transform.forward * ForwardAvoidanceRange, Color.green);

            // Slow the vehicle down
            float speed = _Agent.speed;
            speed -= Deceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Forward raycast MISS
        else {

            Debug.DrawRay(transform.position, transform.forward * ForwardAvoidanceRange, Color.red);

            // Speed the vehicle up
            float speed = _Agent.speed;
            speed += Acceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Fire a raycast downward
        RaycastHit hitDown;
        if (_DownwardRayDetection = Physics.Raycast(transform.position, -transform.up, out hitDown, DownwardsAvoidanceRange, AvoidanceLayerMask)) {

            // Dont test raycast against self
            if (hitDown.transform.gameObject != gameObject) {

                Debug.DrawRay(transform.position, -transform.up * DownwardsAvoidanceRange, Color.green);
                if (hitDown.transform.gameObject.layer != 9) {
                    // Push the air vehicle upwards
                    _Agent.baseOffset += VerticalSpeed * Time.deltaTime;
                }

                // Slow the vehicle down
                float speed = _Agent.speed;
                speed -= Deceleration * Time.deltaTime;
                _Agent.speed = speed;
            }
        }

        // Downward raycast MISS
        else {

            Debug.DrawRay(transform.position, -transform.up * DownwardsAvoidanceRange, Color.red);

            // Speed the vehicle up
            float speed = _Agent.speed;
            speed += Acceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Fire a raycast on a forward / down angle
        Vector3 angle = transform.forward + -transform.up;
        RaycastHit hitAngle;
        if (_AngleRayDetection = Physics.Raycast(transform.position, angle, out hitAngle, ForwardAvoidanceRange, AvoidanceLayerMask)) {

            Debug.DrawRay(transform.position, angle * ForwardAvoidanceRange, Color.green);

            // Push the air vehicle upwards
            _Agent.baseOffset += VerticalSpeed * Time.deltaTime;

            // Slow the vehicle down
            float speed = _Agent.speed;
            speed -= Deceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Angled raycast MISS
        else {

            Debug.DrawRay(transform.position, angle * ForwardAvoidanceRange, Color.red);

            // Speed the vehicle up
            float speed = _Agent.speed;
            speed += Acceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Clamp flying heights
        if (transform.position.y < MinimumFlyingHeight) {

            // Push the vehicle upwards
            _Agent.baseOffset += VerticalSpeed * Time.deltaTime;
        }

        if (transform.position.y > MaximumFlyingHeight) {

            // Push the vehicle downwards
            _Agent.baseOffset -= VerticalSpeed * Time.deltaTime;
        }

        // Try to be at the 'ideal' height when possible
        if (!IsColliding()) {

            // Move up check
            if (transform.position.y < _IdealFlyingHeightMin) {

                // Push the vehicle upwards
                _Agent.baseOffset += VerticalSpeed * Time.deltaTime;
            }

            // Move down check
            if (transform.position.y > _IdealFlyingHeightMax) {

                // Push the vehicle downwards
                _Agent.baseOffset -= VerticalSpeed * Time.deltaTime;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns TRUE if any of the flying movement detection rays are true.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    private bool IsColliding() { return _ForwardRayDetection || _DownwardRayDetection || _AngleRayDetection; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}