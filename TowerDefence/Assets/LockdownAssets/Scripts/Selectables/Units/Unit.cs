﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/8/2018
//
//******************************

public class Unit : Ai {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UNIT PROPERTIES")]
    [Space]
    public EUnitType UnitType = EUnitType.Undefined;
    public ENavmeshType NavmeshType = ENavmeshType.Ground;
    [Tooltip("The movement/walking speed of this unit." +
            "\n\nNOTE: ONLY APPLIES TO GROUND INFANTRY, VEHICLES DO NOT USE THIS VALUE.")]
    public float InfantryMovementSpeed = 10f;
    public bool CanBePlayerControlled = false;

    [Space]
    [Header("-----------------------------------")]
    [Header(" COMBAT/WEAPON PROPERTIES")]
    [Space]
    public Weapon PrimaryWeapon = null;
    public Weapon SecondaryWeapon = null;
    [Space]
    public GameObject MuzzleLaunchPoint = null;
    public float AttackingRange = 100f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum EUnitType { Undefined, CoreMarine, AntiInfantryMarine, Hero, CoreVehicle, AntiAirVehicle, MobileArtillery, BattleTank, CoreAirship, SupportShip, HeavyAirship }
    public enum ENavmeshType { Ground, Air }

    protected CharacterController _CharacterController = null;
    protected NavMeshAgent _Agent = null;
    protected Squad _SquadAttached = null;
    protected GameObject _SeekWaypoint = null;
    protected float _DistanceToTarget = 0f;
    protected bool _IsBeingPlayerControlled = false;
    private float SnapLookAtRange = 0f;
    private BuildingSlot _BuildingSlotInstigator = null;

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

        SnapLookAtRange = AttackingRange / 2;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
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

        // Get component references
        _CharacterController = GetComponent<CharacterController>();
        _Agent = GetComponent<NavMeshAgent>();
        _ObjectHeight = _Agent.height;

        // Initialize lists
        _PotentialTargets = new List<WorldObject>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Selecting the unit via drag selection
        UpdateBoxSelection();

        // Hide the unit UI widgets if it is building
        if (_ObjectState == WorldObjectStates.Building) {

            // Disable components
            _CharacterController.enabled = false;
            _Agent.enabled = false;

            // Hide the healthbar
            if (_HealthBar != null) { _HealthBar.gameObject.SetActive(false); }

            // Despawn build counter widget (it is unused)
            if (_BuildingProgressCounter != null) { ObjectPooling.Despawn(_BuildingProgressCounter.gameObject); }

            // Force the position of the object to be at the attached buildings vector
            if (_BuildingSlotInstigator != null) {
                
                switch (NavmeshType) {

                    case ENavmeshType.Ground: {

                            _Agent.transform.position = _BuildingSlotInstigator.AttachedBase.GroundUnitSpawnTransform.transform.position;
                            _Agent.transform.rotation = _BuildingSlotInstigator.AttachedBase.GroundUnitSpawnTransform.transform.rotation;
                            break;
                        }

                    case ENavmeshType.Air: {

                            _Agent.transform.position = _BuildingSlotInstigator.AttachedBase.AirUnitSpawnTransform.transform.position;
                            _Agent.transform.rotation = _BuildingSlotInstigator.AttachedBase.AirUnitSpawnTransform.transform.rotation;
                            break;
                        }

                    default: break;
                }
            }
        }

        // Force the unit to skip the deployable state and go straight to being active in the world
        else if (_ObjectState == WorldObjectStates.Deployable) { OnSpawn(); }

        // Unit is active in the world
        else if (_ObjectState == WorldObjectStates.Active && IsAlive()) {

            // And isn't part of a squad
            if (_SquadAttached == null) {

                // Show the healthbar
                if (_HealthBar != null) { _HealthBar.gameObject.SetActive(true); }

                // Create a healthbar if the unit doesn't have one linked to it
                else {

                    GameObject healthBarObj = ObjectPooling.Spawn(GameManager.Instance.UnitHealthBar.gameObject);
                    _HealthBar = healthBarObj.GetComponent<UnitHealthBar>();
                    _HealthBar.SetObjectAttached(this);
                    healthBarObj.gameObject.SetActive(true);
                    healthBarObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);

                    // Assigning player reference
                    if (_Player == null) {

                        Player plyr = GameManager.Instance.Players[0];
                        _HealthBar.SetCameraAttached(plyr.PlayerCamera);
                    }
                    else { _HealthBar.SetCameraAttached(_Player.PlayerCamera); }
                }
            }
        }

        // Is the unit currently being player controlled? (manually)
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
                    _Player._KeyboardInputManager.CreateCenterPoint();

                    // Reset mouse to center of the screen
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
        else { _IsBeingPlayerControlled = false; }

        // Update player controller movement
        if (_IsBeingPlayerControlled && IsAlive()) {

            _IsCurrentlySelected = true;
            _Agent.enabled = false;
            UpdatePlayerControlledMovement();
        }

        UpdateAIControllerMovement();
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

            Unit unit = _ClonedWorldObject.GetComponent<Unit>();
            unit._BuildingSlotInstigator = buildingSlot;

            // Despawn build counter widget
            if (_BuildingProgressCounter != null) { ObjectPooling.Despawn(_BuildingProgressCounter.gameObject); }

            // Let the building attached know that it is "building" something
            if (buildingSlot.GetBuildingOnSlot() != null) {

                buildingSlot.GetBuildingOnSlot().SetIsBuildingSomething(true);
                buildingSlot.GetBuildingOnSlot().SetObjectBeingBuilt(_ClonedWorldObject);
            }

            // Set position to be at the bases spawn vector while it is building
            // (the gameobject should be hidden completely until its deployed)
            if (buildingSlot.AttachedBase != null) {

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.AttachedBase.GroundUnitSpawnTransform.transform.position;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.AttachedBase.GroundUnitSpawnTransform.transform.rotation;
            }

            // No base attached
            else {

                // Set position to be at the buildings spawn vector while it is building
                // (the gameobject should be hidden completely until its deployed)
                _ClonedWorldObject.gameObject.transform.position = buildingSlot.transform.position + buildingSlot.transform.forward * 50.0f;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.transform.rotation;
            }

            // Add to list of AI(army)
            _ClonedWorldObject._Player.AddToPopulation(_ClonedWorldObject as Unit);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void UpdateChasingEnemy() {
        base.UpdateChasingEnemy();

        // Not in a squad? (squad handles their own chasing mechanics for the units!)
        if (!IsInASquad()) {

            if (_AttackTarget != null) { AgentAttackObject(_AttackTarget); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    public override void OnDeath() {
        base.OnDeath();

        // Remove from squad
        if (IsInASquad()) { _SquadAttached.RemoveUnitFromSquad(this); }

        // Play ragdoll stuff here
        _Agent.enabled = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called only once, when the unit transitions to an active state.
    /// </summary>
    public virtual void OnSpawn() {

        ResetHealth();

        // Enable components
        if (_Agent == null) { _Agent = GetComponent<NavMeshAgent>(); }
        _Agent.enabled = true;
        if (_CharacterController == null) { _CharacterController = GetComponent<CharacterController>(); ; }
        _CharacterController.enabled = true;

        SetObjectState(WorldObjectStates.Active);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    protected virtual void UpdateAIControllerMovement() {
        
        // Is the unit currently AI controlled?
        if (!_IsBeingPlayerControlled && _Agent.enabled && IsAlive()) {

            // Update agent seeking status
            _IsSeeking = _Agent.remainingDistance > 20f;
            if (_IsSeeking) {

                // Look at seek point
                ///_Agent.transform.LookAt(_Agent.destination);
            }
            else {

                _IsSeeking = false;
                _IsReturningToOrigin = false;
            }

            // Update seeking waypoint visibility
            if (_SeekWaypoint) {

                if (_IsSeeking && (_IsCurrentlySelected || _IsCurrentlyHighlighted)) { _SeekWaypoint.SetActive(true); }
                else { _SeekWaypoint.SetActive(false); }
            }

            // Update distance to attacking target
            if (_AttackTarget != null) {

                if (_AttackTarget.IsAlive()) {

                    // Check if the target is within attacking range
                    _DistanceToTarget = Vector3.Distance(transform.position, _AttackTarget.transform.position);
                    if (_DistanceToTarget <= AttackingRange && PrimaryWeapon != null) {

                        // Look at attack target
                        _IsAttacking = true;
                        ///if (_IsAttacking) { LookAt(_AttackTargetObject.transform.position); }

                        // If possible, fire weapon
                        if (PrimaryWeapon.CanFire()) { PrimaryWeapon.FireWeapon(); }
                    }

                    // Target is too far away or we have no primary weapon
                    else { _IsAttacking = false; }

                    // Constantly face the attacking target
                    Vector3 FireAtPos = _AttackTarget.transform.position;
                    FireAtPos.y = FireAtPos.y + _AttackTarget.GetObjectHeight() * 0.3f;

                    // Determine if we should snap to target or lerp rotation
                    if (_DistanceToTarget <= SnapLookAtRange) { LookAtSnap(FireAtPos); }
                    else { LookAtLerp(FireAtPos); }

                }

                // Attack target is now dead
                else {

                    // Get new attack target if possible
                    DetermineWeightedTargetFromList();
                }
            }

            // There is no current attack target
            else { /// _AttackTarget == null

                _IsAttacking = false;

                // Get new attack target if possible
                DetermineWeightedTargetFromList();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void UpdatePlayerControlledMovement() { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void LookAtLerp(Vector3 position) { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Snaps the agent's transform to the position specified.
    /// </summary>
    protected virtual void LookAtSnap(Vector3 position) {

        _Agent.transform.LookAt(position);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the agent's destination aswell as updating a waypoint gameObject
    //  to show the player where the agent is trying to go to.
    /// </summary>
    /// <param name="seekTarget"></param>
    public void AgentSeekPosition(Vector3 seekTarget, bool displayWaypoint = true) {

        if (_Agent.isOnNavMesh) {

            // Set agent's new goto target
            _SeekTarget = seekTarget;
            _Agent.destination = _SeekTarget;
            _Agent.speed = InfantryMovementSpeed;

            // Show seeking waypoint
            if (displayWaypoint) {

                // Create waypoint
                if (_SeekWaypoint == null) { _SeekWaypoint = ObjectPooling.Spawn(GameManager.Instance.AgentSeekObject, Vector3.zero, Quaternion.identity); }
                if (_SeekWaypoint != null) {

                    // Display waypoint if not already being displayed
                    if (_SeekWaypoint.activeInHierarchy != true) { _SeekWaypoint.SetActive(true); }

                    // Update waypoint position
                    _SeekWaypoint.transform.position = seekTarget;
                    _SeekWaypoint.transform.position += Vector3.up;
                }
            }
            _IsSeeking = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the attack target to the worldObject passed through, 
    //  as well as pathfinding to the position passed in.
    /// </summary>
    /// <param name="attackTarget"></param>
    public void AgentAttackObject(WorldObject attackTarget, Vector3 seekPosition) {

        // Set target
        AddPotentialTarget(attackTarget);
        _AttackTarget = attackTarget;

        // Seek
        AgentSeekPosition(seekPosition);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the attack target to the worldObject passed through &
    //  creates a seek position internally.
    /// </summary>
    /// <param name="attackTarget"></param>
    public void AgentAttackObject(WorldObject attackTarget) {

        // Add target to list and make it the current target
        AddPotentialTarget(attackTarget);
        _AttackTarget = attackTarget;

        // Get attacking position close to the target and seek to it
        Vector3 seekPos = GetAttackingPositionAtObject(_AttackTarget, 20);
        AgentSeekPosition(seekPos);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="hitPoint"></param>
    public virtual void AgentPerformAbility(Vector3 hitPoint) {

        // Does the unit have an ability?
        if (SecondaryWeapon != null) {

            // Fire secondary weapon (its used to be its ability)
            if (SecondaryWeapon.CanFire()) { SecondaryWeapon.FireWeapon(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Determines where the agent should seek to when it attacks the WorldObject passed in.
    /// </summary>
    /// <param name="worldObject"></param>
    /// <returns>
    //  Vector3
    /// </returns>
    public Vector3 GetAttackingPositionAtObject(WorldObject worldObject, float distanceFrom) {

        // Create transform and create a position in the direction of the target * distance specified
        Transform trans = new GameObject().transform;
        trans.position = worldObject.transform.position;
        trans.LookAt(transform.position);
        trans.position += trans.forward * distanceFrom;

        // Destroy obsolete transform and return the new attacking position
        Vector3 position = trans.position;
        Destroy(trans.gameObject);
        return position;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Determines where the agent should seek to when it attacks the WorldObject passed in.
    /// </summary>
    /// <param name="worldObject"></param>
    /// <returns>
    //  Vector3
    /// </returns>
    public Vector3 GetAttackingPositionAtObject(WorldObject worldObject) {

        // Create transform and create a position in the direction of the target * distance specified
        Transform trans = new GameObject().transform;
        trans.position = worldObject.transform.position;
        trans.LookAt(transform.position);
        trans.position += trans.forward * AttackingRange / 2;

        // Destroy obsolete transform and return the new attacking position
        Vector3 position = trans.position;
        Destroy(trans.gameObject);
        return position;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Makes the agent return back to its origin position.
    //  (Usually the position is where the unit was before it was pursuing a target).
    /// </summary>
    protected override void ResetToOriginPosition() {
        base.ResetToOriginPosition();

        // Not in s squad? (squad handles their own reset mechanics for the units!)
        if (!IsInASquad()) {

            PlayerSeekOverride();
            AgentSeekPosition(_ChaseOriginPosition);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Checks if unit is selected by click & drag box
    /// </summary>
    private void UpdateBoxSelection()
    {
        // Precautions
        if (_Player != null) {

            if(!KeyboardInput.MouseIsDown)
            {

         
            //if (Input.GetMouseButton(0)) {

                Vector3 camPos = _Player.PlayerCamera.WorldToScreenPoint(transform.position);
                camPos.y = KeyboardInput.InvertMouseY(camPos.y);

                if (KeyboardInput.Selection.Contains(camPos)) {

                 //   _IsCurrentlySelected = true;
                    if (IsInASquad())
                    {

                        if (GetSquadAttached().GetObjectState() == WorldObject.WorldObjectStates.Active && !_IsCurrentlySelected)
                        {
                            _Player.SelectedWorldObjects.Add(GetSquadAttached());
                            GetSquadAttached().SetPlayer(_Player);
                            GetSquadAttached().SetIsSelected(true);
                        }

                    }
                    else
                    {
                        if (this.GetObjectState() == WorldObject.WorldObjectStates.Active && !_IsCurrentlySelected)
                        {
                            _Player.SelectedWorldObjects.Add(this);
                            this.SetPlayer(_Player);
                            this.SetIsSelected(true);
                        }

                    }
                }

            }

            // }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference of the NavMeshAgent component attached to this unit.
    /// </summary>
    /// <returns>
    //  NavMeshAgent
    /// </returns>
    public NavMeshAgent GetAgent() { return _Agent; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns whether this unit is a part of a squad or not.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsInASquad() { return _SquadAttached != null; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the reference of the squad that is to be attached with this unit.
    /// </summary>
    /// <param name="squad"></param>
    public void SetSquadAttached(Squad squad) { _SquadAttached = squad; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference of the squad attached to this unit.
    //  (null if the unit isnt part of a squad).
    /// </summary>
    /// <returns>
    //  Squad
    /// </returns>
    public Squad GetSquadAttached() { return _SquadAttached; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //   
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsBeingPlayerControlled() { return _IsBeingPlayerControlled; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Set's the worldobject's hitpoints & shieldpoints to the current maximum value.
    /// </summary>
    public void ResetHealth() {
        
        _HitPoints = MaxHitPoints;
        _ShieldPoints = MaxShieldPoints;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}