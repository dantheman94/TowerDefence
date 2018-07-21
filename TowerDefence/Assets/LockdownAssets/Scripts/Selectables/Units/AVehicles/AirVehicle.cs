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
    public float MinimumFlyingHeight = 30f;
    public float MaximumFlyingHeight = 100f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Update agent height
        UpdateHeight();

        // Clamp flying heights
        if (transform.position.y < MinimumFlyingHeight) {

        }

        if (transform.position.y > MaximumFlyingHeight) {

        }
    }

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void UpdateHeight() {
        
        // Get ships position with its offset from the ground
        Vector3 AgentPosition = transform.position;
        ///AgentPosition.y = AgentPosition.y + _Agent.baseOffset;

        // Fire a raycast forward
        RaycastHit hitForward;
        if (Physics.Raycast(AgentPosition, transform.forward, out hitForward, ForwardAvoidanceRange)) {

            Debug.DrawRay(AgentPosition, transform.forward * ForwardAvoidanceRange, Color.green);

        }
        else { Debug.DrawRay(AgentPosition, transform.forward * ForwardAvoidanceRange, Color.red); }

        // Fire a raycast downward
        RaycastHit hitDown;
        if (Physics.Raycast(AgentPosition, -transform.up, out hitDown, DownwardsAvoidanceRange)) {
        
            Debug.DrawRay(AgentPosition, -transform.up * DownwardsAvoidanceRange, Color.blue);

            // Push the air vehicle upwards
            _Agent.baseOffset += VerticalSpeed * Time.deltaTime;            
        }
        else { Debug.DrawRay(AgentPosition, -transform.up * DownwardsAvoidanceRange, Color.magenta); }
    }

}