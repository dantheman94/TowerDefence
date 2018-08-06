using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 4/8/2018
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
    [Header(" BASE UNIT PROPERTIES")]
    [Space]
    public EUnitType UnitType = EUnitType.Undefined;
    [Tooltip("The movement/walking speed of this unit." +
            "\n\nNOTE: ONLY APPLIES TO GROUND INFANTRY, VEHICLES DO NOT USE THIS VALUE.")]
    public float InfantryMovementSpeed = 10f;
    public bool CanBePlayerControlled = false;
    [Tooltip("When this unit is killed, the speed in which it shrinks down until it is no longer visible " +
            "before being sent back to the object pool.")]
    public float ShrinkSpeed = 0.1f;
    
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

    protected CharacterController _CharacterController = null;
    protected NavMeshAgent _Agent = null;
    protected Squad _SquadAttached = null;
    protected bool _IsSeeking = false;
    protected GameObject _SeekWaypoint = null;
    protected WorldObject _AttackTarget = null;
    protected List<WorldObject> _PotentialTargets;
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
        _CharacterController = GetComponent<CharacterController>();

        _ObjectHeight = _Agent.height;
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

        // Initialize lists
        _PotentialTargets = new List<WorldObject>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();
        BoxSelection();

        // Hide the unit UI widgets if it is building
        if (_ObjectState == WorldObjectStates.Building) {

            // Hide the healthbar
            if (_HealthBar != null) { _HealthBar.gameObject.SetActive(false); }

            // Despawn build counter widget (it is unused)
            if (_BuildingProgressCounter != null) { ObjectPooling.Despawn(_BuildingProgressCounter.gameObject); }
        }

        // Force the unit to skip the deployable state and go straight to being active in the world
        else if (_ObjectState == WorldObjectStates.Deployable) { _ObjectState = WorldObjectStates.Active; }

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

        // Update movement
        if (_IsBeingPlayerControlled) {

            _IsCurrentlySelected = true;
            _Agent.enabled = false;
            UpdatePlayerControlledMovement();
        }
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
            if (_AttackTarget != null) {

                if (_AttackTarget.IsAlive()) {

                    _DistanceToTarget = Vector3.Distance(transform.position, _AttackTarget.transform.position);

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
                    Vector3 FireAtPos = _AttackTarget.transform.position;
                    FireAtPos.y = FireAtPos.y + _AttackTarget.GetObjectHeight() / 2;
                    LookAt(FireAtPos);
                }
                else {

                    /// GET NEW ATTACK TARGET
                }
            }
        }

        // Gradually shrink the character then despawn it once its dead
        if (_StartShrinking && !IsAlive()) {

            transform.localScale -= Vector3.one * ShrinkSpeed * Time.deltaTime;
            if (transform.localScale.x < 0.1f) {

                _StartShrinking = false;
                ObjectPooling.Despawn(this.gameObject);
            }
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

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.AttachedBase.UnitSpawnTransform.transform.position;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.AttachedBase.UnitSpawnTransform.transform.rotation;
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
    //  
    /// </summary>
    public override void OnDeath() {
        base.OnDeath();

        _StartShrinking = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnSpawn() {

        // Enable components
        _Agent.enabled = true;
        _CharacterController.enabled = true;

        SetObjectState(WorldObjectStates.Active);
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="seekTarget"></param>
    public void AgentSeekPosition(Vector3 seekTarget, bool displayWaypoint = true) {

        if (_Agent.isOnNavMesh) {

            // Set agent's new goto target
            _Agent.destination = seekTarget;
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
    //  Sets the attack target to the worldObject passed through, as well as
    //  seek pathfinding to the position
    /// </summary>
    /// <param name="attackTarget"></param>
    public void AgentAttackObject(WorldObject attackTarget, Vector3 seekPosition) {

        _AttackTarget = attackTarget;
        AgentSeekPosition(seekPosition);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public virtual void AgentPerformAbility(Vector3 hitPoint) {

        // Does the unit have an ability?
        if (SecondaryWeapon != null) {

            // Fire secondary weapon (its used to be its ability)
            if (SecondaryWeapon.CanFire()) { SecondaryWeapon.FireWeapon(); }
        }
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
    /// Checks if unit is selected by click & drag box
    /// </summary>
    private void BoxSelection()
    {
        // Precautions
        if (_Player != null) {

            if(!KeyboardInput.MouseIsDown)
            {

         
            //if (Input.GetMouseButton(0)) {

                Vector3 camPos = _Player.PlayerCamera.WorldToScreenPoint(transform.position);
                camPos.y = KeyboardInput.InvertMouseY(camPos.y);

                if (KeyboardInput.Selection.Contains(camPos)) {

                    _IsCurrentlySelected = true;
                    if (IsInASquad())
                    {

                        if (GetSquadAttached().GetObjectState() == WorldObject.WorldObjectStates.Active)
                        {
                            _Player.SelectedWorldObjects.Add(GetSquadAttached());
                            GetSquadAttached().SetPlayer(_Player);
                            GetSquadAttached().SetIsSelected(true);
                        }

                    }
                    else
                    {
                        if (this.GetObjectState() == WorldObject.WorldObjectStates.Active)
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
    public WorldObject GetAttackTarget() { return _AttackTarget; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //   
    /// </summary>
    /// <returns></returns>
    public bool IsBeingPlayerControlled() { return _IsBeingPlayerControlled; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds a WorldObject to the weighted target list
    /// </summary>
    /// <param name="target"></param>
    public void AddPotentialTarget(WorldObject target) { _PotentialTargets.Add(target); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Checks if the WorldObject is contained in the weighted 
    //  target list & removes if it found.
    /// </summary>
    /// <param name="target"></param>
    public void RemovePotentialTarget(WorldObject target) {

        // Look for match
        for (int i = 0; i < _PotentialTargets.Count; i++) {

            // Match found
            if (_PotentialTargets[i] == target) {

                _PotentialTargets.Remove(target);
                break;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}