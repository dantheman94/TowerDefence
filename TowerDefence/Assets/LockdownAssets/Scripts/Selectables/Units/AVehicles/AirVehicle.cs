using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 21/7/2018
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
    public float MinimumFlyingHeight = 20f;
    public float MaximumFlyingHeight = 100f;
    public float IdealFlyingHeight = 40f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected bool _ForwardRayDetection = false;
    protected bool _DownwardRayDetection = false;
    protected bool _AngleRayDetection = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Update agent height
        UpdateHeight();

        // Clamp max movement speed
        if (_Agent.speed > MaxSpeed) { _Agent.speed = MaxSpeed; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. [ From Update() ]
    /// </summary>
    private void UpdateHeight() {

        // Get ships position with its offset from the ground
        Vector3 AgentPosition = transform.position;
        AgentPosition.y = AgentPosition.y + _ObjectHeight;

        // Fire a raycast forward
        RaycastHit hitForward;
        if (_ForwardRayDetection = Physics.Raycast(AgentPosition, transform.forward, out hitForward, ForwardAvoidanceRange)) {

            Debug.DrawRay(AgentPosition, transform.forward * ForwardAvoidanceRange, Color.green);

            // Slow the vehicle down
            float speed = _Agent.speed;
            speed -= Deceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Forward raycast MISS
        else {

            Debug.DrawRay(AgentPosition, transform.forward * ForwardAvoidanceRange, Color.red);

            // Speed the vehicle up
            float speed = _Agent.speed;
            speed += Acceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Fire a raycast downward
        RaycastHit hitDown;
        if (_DownwardRayDetection = Physics.Raycast(AgentPosition, -transform.up, out hitDown, DownwardsAvoidanceRange)) {

            Debug.DrawRay(AgentPosition, -transform.up * DownwardsAvoidanceRange, Color.green);
            if (hitDown.transform.gameObject.layer != 9) {
                // Push the air vehicle upwards
                _Agent.baseOffset += VerticalSpeed * Time.deltaTime;
            }


            // Slow the vehicle down
            float speed = _Agent.speed;
            speed -= Deceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Downward raycast MISS
        else {

            Debug.DrawRay(AgentPosition, -transform.up * DownwardsAvoidanceRange, Color.red);

            // Speed the vehicle up
            float speed = _Agent.speed;
            speed += Acceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Fire a raycast on a forward / down angle
        Vector3 angle = transform.forward + -transform.up;
        RaycastHit hitAngle;
        if (_AngleRayDetection = Physics.Raycast(AgentPosition, angle, out hitAngle, ForwardAvoidanceRange)) {

            Debug.DrawRay(AgentPosition, angle * ForwardAvoidanceRange, Color.green);

            // Push the air vehicle upwards
            _Agent.baseOffset += VerticalSpeed * Time.deltaTime;

            // Slow the vehicle down
            float speed = _Agent.speed;
            speed -= Deceleration * Time.deltaTime;
            _Agent.speed = speed;
        }

        // Angled raycast MISS
        else {

            Debug.DrawRay(AgentPosition, angle * ForwardAvoidanceRange, Color.red);

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
            if (transform.position.y < IdealFlyingHeight) {

                // Push the vehicle upwards
                _Agent.baseOffset += VerticalSpeed / 3 * Time.deltaTime;
            }

            // Move down check
            if (transform.position.y > IdealFlyingHeight) {

                // Push the vehicle downwards
                _Agent.baseOffset -= VerticalSpeed / 3 * Time.deltaTime;
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