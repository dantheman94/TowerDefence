﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 4/7/2018
//
//******************************

public class Unit : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UNIT PROPERTIES")]
    [Space]
    public UnitType Type = UnitType.Undefined;
    public float MovementSpeed = 10f;
    public bool CanBePlayerControlled = false;
    public float ScalingSpeed = 2f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" SENSORY PROPERTIES")]
    [Space]
    public ConeCollider SightCone = null;
    public SphereCollider HearingSphere = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" COMBAT PROPERTIES")]
    [Space]
    public GameObject MuzzleLaunchPoint = null;
    public Weapon PrimaryWeapon = null;
    public Weapon SecondaryWeapon = null;
    public float AttackingRange = 100f;
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum UnitType { Undefined, CoreMarine, AntiInfantryMarine, AntiVehicleMarine, CoreVehicle, AntiAirVehicle, MobileArtillery, BattleTank, CoreAirship, SupportShip, BattleAirship }

    protected NavMeshAgent _Agent = null;
    protected Squad _SquadAttached = null;
    protected bool _IsSeeking = false;
    protected GameObject _SeekWaypoint = null;
    protected WorldObject _AttackTargetObject = null;
    protected bool _IsAttacking = false;
    protected float _DistanceToTarget = 0f;
    protected bool _IsBeingPlayerControlled = false;
    protected bool _StartShrinking = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called before Start().
    /// </summary>
    protected override void Awake() {
        base.Awake();

        // Set recycle amount to the same as the cost amount
        RecycleSupplies = CostSupplies;
        RecyclePower = CostPower;

        // Get component references
        _Agent = GetComponent<NavMeshAgent>();

        _ObjectHeight = _Agent.height;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        // Create copy of weapons & re-assign them to replace the old ones
        if (PrimaryWeapon != null) {
            
            PrimaryWeapon = ObjectPooling.Spawn(PrimaryWeapon.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Weapon>();
            PrimaryWeapon.SetUnitAttached(this);
        }
        if (SecondaryWeapon != null) {
            
            PrimaryWeapon = ObjectPooling.Spawn(SecondaryWeapon.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Weapon>();
            SecondaryWeapon.SetUnitAttached(this);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        if (CanBePlayerControlled && _IsCurrentlySelected) {

            // Flip the player/AI controlled states
            if (Input.GetKeyDown(KeyCode.R)) {

                _IsBeingPlayerControlled = !_IsBeingPlayerControlled;

                if (_IsBeingPlayerControlled) {

                    GameManager.Instance.SetUnitControlling(true);

                    // Initialize camera follow script
                    _Player._CameraFollow.SetFollowTarget(this);
                    _Player._CameraFollow.SetCameraAttached(_Player.PlayerCamera);
                    _Player._CameraFollow.Init();

                    // Hide any seek points that are currently visible
                    if (_SeekWaypoint) { _SeekWaypoint.SetActive(false); }
                }
                else {

                    GameManager.Instance.SetUnitControlling(false);
                    _Player._CameraFollow.SetFollowing(false);
                    _Player._Input.CreateCenterPoint();

                    // Reset mouse to center of the screen
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        // Constantly select the unit if its being manually controlled
        if (_IsBeingPlayerControlled) { _IsCurrentlySelected = true; }

        // Force this unit to being AI controlled
        else { _IsBeingPlayerControlled = false; }

        // Force the unit to skip the deployable state and go straight to being active in the world
        if (_ObjectState == WorldObjectStates.Deployable) { _ObjectState = WorldObjectStates.Active; }

        // Is the unit currently AI controlled?
        if (!_IsBeingPlayerControlled && _Agent.enabled) {

            // Update agent seeking status
            _IsSeeking = _Agent.remainingDistance > 20f;
            if (_IsSeeking) {

                // Look at seek point
                ///_Agent.transform.LookAt(_Agent.destination);
            }

            // Update seeking waypoint visibility
            if (_SeekWaypoint && _IsCurrentlySelected) {

                if (_IsSeeking) { _SeekWaypoint.SetActive(true); }
                else            { _SeekWaypoint.SetActive(false); }
            }

            // Update distance to target
            if (_AttackTargetObject != null) {

                if (_AttackTargetObject.IsAlive()) {

                    _DistanceToTarget = Vector3.Distance(transform.position, _AttackTargetObject.transform.position);

                    // Fire primary weapon if within attacking range
                    if (_DistanceToTarget <= AttackingRange && PrimaryWeapon != null) {

                        // Look at attack target
                        _IsAttacking = true;
                        ///if (_IsAttacking) { LookAt(_AttackTargetObject.transform.position); }

                        // If possible, fire weapon
                        if (PrimaryWeapon.CanFire()) { PrimaryWeapon.FireWeapon(); }
                    }

                    else { _IsAttacking = false; }

                    // Constantly face the attacking target
                    Vector3 FireAtPos = _AttackTargetObject.transform.position;
                    FireAtPos.y = FireAtPos.y + _AttackTargetObject.GetObjectHeight() / 2;
                    LookAt(FireAtPos);
                }
                else {

                    /// GET NEW ATTACK TARGET
                }
            }
        }

        if (_StartShrinking && !IsAlive()) {

            transform.localScale -= new Vector3(Time.deltaTime * ScalingSpeed, Time.deltaTime * ScalingSpeed, Time.deltaTime * ScalingSpeed);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {
        base.OnWheelSelect(buildingSlot);

        // Get reference to the newly cloned unit
        if (_ClonedWorldObject != null) {

            // Set position to be at the spawn vector while it is building (it should be hidden until its deployed)
            if (buildingSlot.AttachedBase != null) {

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.AttachedBase.UnitSpawnTransform.transform.position;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.AttachedBase.UnitSpawnTransform.transform.rotation;

            }
            else {

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.transform.position + buildingSlot.transform.forward * 50.0f;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.transform.rotation;
            }

            // Add to list of AI
            _Player.AddToPopulation(_ClonedWorldObject as Unit);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public override void OnDeath() {
        base.OnDeath();

        _StartShrinking = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void LookAt(Vector3 position) {

        // Snap to target
        _Agent.transform.LookAt(position);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //   
    /// </summary>
    protected virtual void UpdateSight() {

    }

    /// <summary>
    //  
    /// </summary>
    /// <param name="seekTarget"></param>
    public void AgentSeekPosition(Vector3 seekTarget) {

        if (_Agent.isOnNavMesh) {

            // Set agent's new goto target
            _Agent.destination = seekTarget;
            _Agent.speed = MovementSpeed;

            // Show seeking waypoint
            if (_SeekWaypoint == null) { _SeekWaypoint = ObjectPooling.Spawn(GameManager.Instance.AgentSeekObject, Vector3.zero, Quaternion.identity); }
            if (_SeekWaypoint != null) {

                // Display waypoint if not already being displayed
                if (_SeekWaypoint.activeInHierarchy != true) { _SeekWaypoint.SetActive(true); }

                // Update waypoint position
                _SeekWaypoint.transform.position = seekTarget;
                _SeekWaypoint.transform.position += Vector3.up;
            }
            _IsSeeking = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the attack target to the worldObject passed through, as well as
    //  seek pathfinding to the position
    /// </summary>
    /// <param name="attackTarget"></param>
    public void AgentAttackObject(WorldObject attackTarget, Vector3 seekPosition) {

        _AttackTargetObject = attackTarget;
        AgentSeekPosition(seekPosition);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="worldObject"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public Vector3 GetAttackingPositionAtObject(WorldObject worldObject) {

        // Create transform and create a position within attacking range, in the direction of the target
        Transform trans = new GameObject().transform;
        trans.position = worldObject.transform.position;
        trans.LookAt(transform.position);
        trans.position += trans.forward * AttackingRange * 0.7f;

        // Destroy obsolete transform and return the new attacking position
        Vector3 position = trans.position;
        Destroy(trans.gameObject);
        return position;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public NavMeshAgent GetAgent() { return _Agent; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public bool IsInASquad() { return _SquadAttached != null; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="squad"></param>
    public void SetSquadAttached(Squad squad) { _SquadAttached = squad; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public Squad GetSquadAttached() { return _SquadAttached; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public WorldObject GetAttackTarget() { return _AttackTargetObject; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //   
    /// </summary>
    /// <returns></returns>
    public bool IsBeingPlayerControlled() { return _IsBeingPlayerControlled; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}