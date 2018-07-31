using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Joshua Peake
//  Last edited on: 31/7/2018
//
//******************************

public class BaseTurret : Tower
{

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" TURRET PROPERTIES")]
    [Space]
    public bool UpgradedTurret = false;
    public GameObject TurretCannon;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public WorldObject _TargetObject;
    public Vector3 _TargetDirection;
    public float _TargetDistance = 0.0f;
    public bool _TargetAquired = false;
    public float _AttackRange = 50.0f;
    public Quaternion _LookAtRotation;
    public Quaternion _DefaultRotation = Quaternion.identity;
    public float _RotationSpeed = 5.0f;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object
    //  linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot)
    {

        // Gets reference to the original turret (before the upgrade)
        BaseTurret originalTurret = null;
        if (buildingSlot != null)
        {

            if (buildingSlot.GetBuildingOnSlot() != null) { originalTurret = buildingSlot.GetBuildingOnSlot().GetComponent<BaseTurret>(); }
        }

        // Remove old healthbar (if valid)
        float hitpoints = MaxHitPoints;
        if (originalTurret != null)
        {

            hitpoints = originalTurret.GetHitPoints();
            if (originalTurret._HealthBar != null) { ObjectPooling.Despawn(originalTurret._HealthBar.gameObject); }
        }

        // Start building process
        base.OnWheelSelect(buildingSlot);
        if (_ClonedWorldObject != null)
        {

            // Only proceed if there was a previous building & we're upgrading from that
            if (originalTurret != null)
            {

                // Update player ref
                _ClonedWorldObject.SetPlayer(originalTurret._Player);

                // Set the new base's building state object to be the currently active object
                _ClonedWorldObject.BuildingState = originalTurret.gameObject;
            }

            // Update attached building slot turret reference
            if (buildingSlot != null) { buildingSlot.SetBuildingOnSlot(_ClonedWorldObject.GetComponent<BaseTurret>()); }

            // Reset turret's health
            _ClonedWorldObject.SetHitPoints(_ClonedWorldObject.MaxHitPoints);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    protected override void Start()
    {
        base.Start();

        _DefaultRotation = transform.rotation;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Update()
    {

        if (_TargetObject != null)
        {

            Debug.Log("Target != null");

            if (_TargetObject.IsAlive())
            {

                // Calculate the _TargetDistance
                _TargetDistance = Vector3.Distance(_TargetObject.transform.position, transform.position);

                Debug.Log("Calculating Distance");

                if (_TargetAquired == true && (_TargetDistance <= _AttackRange))
                {

                    Debug.Log("Looking, within range");

                    // Calculate _TargetDirection and _LookAtRotation
                    _TargetDirection = (_TargetObject.transform.position - transform.position).normalized;

                    _LookAtRotation = (Quaternion.LookRotation(_TargetDirection));

                    // When _TargetAquired is true and they are within range, look at the target
                    TurretCannon.transform.rotation = Quaternion.Lerp(TurretCannon.transform.rotation, _LookAtRotation, Time.deltaTime * _RotationSpeed);
                }
            }
        }

        else
        {

            // Restore original rotatiom
            TurretCannon.transform.rotation = Quaternion.Lerp(TurretCannon.transform.rotation, _DefaultRotation, Time.deltaTime * _RotationSpeed);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnTriggerEnter(Collider other)
    {
        // Store the collision
        Transform contact = other.transform;

        if (contact.gameObject.CompareTag("Unit"))
        {

            Debug.Log("Tag Matches");

            _TargetObject = other.gameObject.transform.GetComponentInParent<WorldObject>();
            _TargetAquired = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Target has exceeded attack range");

        // When the _TargetDistance exceeds a certain limit, _TargetAquired becomes false
        _TargetAquired = false;

        // And null the _TargetObject
        _TargetObject = null;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}