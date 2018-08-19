using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/8/2018
//
//******************************

public class WatchTower : Tower {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" WATCHTOWER PROPERTIES")]
    [Space]
    public GameObject SearchLightObject = null;
    [Space]
    public float TargetInaccuracyOffset = 3f;
    public float TargetGeneratorDistance = 20f;
    public float TargetGeneratorRadius = 30f;
    [Space]
    public float StareAtTargetPositionTimeMin = 2f;
    public float StareAtTargetPositionTimeMax = 4f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Vector3 _SearchLightTarget = Vector3.zero;

    private float _StareAtTargetTime = 0f;
    private float _StareAtTimer = 0f;

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

        // Initialize
        _StareAtTargetTime = Random.Range(StareAtTargetPositionTimeMin, StareAtTargetPositionTimeMax);
        _SearchLightTarget = CreateSearchLightTarget();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // When not in a combatative state >> do random light searching behaviours
        if (SearchLightObject != null && _CombatState == ETowerState.Idle) {

            // Lerp search light to target position
            Vector3 direction = (_SearchLightTarget - SearchLightObject.transform.position).normalized;
            Quaternion lookAtRot = Quaternion.LookRotation(direction);
            SearchLightObject.transform.rotation = Quaternion.LerpUnclamped(SearchLightObject.transform.rotation, lookAtRot, Time.deltaTime * WeaponAimingSpeed);

            // Constantly fire a raycast to check if the tower has reached its search target (non-combat state)
            RaycastHit hit;
            Physics.Raycast(SearchLightObject.transform.position, SearchLightObject.transform.forward * 1000, out hit);

            // Has raycast hit target position? (slight imperfection is accounted for)
            float dist = Vector3.Distance(hit.point, _SearchLightTarget);
            if (dist < TargetInaccuracyOffset) {

                // Add to timer
                _StareAtTimer += Time.deltaTime;
                if (_StareAtTimer >= _StareAtTargetTime) {

                    // Timer reached threshold - generator new search light target position
                    _StareAtTimer = 0f;
                    _StareAtTargetTime = Random.Range(StareAtTargetPositionTimeMin, StareAtTargetPositionTimeMax);
                    _SearchLightTarget = CreateSearchLightTarget();
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Vector3
    /// </returns>
    private Vector3 CreateSearchLightTarget() {

        // Create random position infront of the tower
        Vector3 center = transform.position + transform.forward * TargetGeneratorDistance;
        center.y = 0;
        return center + new Vector3((Random.insideUnitCircle * TargetGeneratorRadius).x, 0, (Random.insideUnitCircle * TargetGeneratorRadius).y);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}