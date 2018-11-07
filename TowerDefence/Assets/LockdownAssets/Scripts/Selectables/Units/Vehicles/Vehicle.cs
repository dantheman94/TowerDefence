using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/9/2018
//
//******************************

public class Vehicle : Unit {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BASE VEHICLE PROPERTIES")]
    [Space]
    public GameObject WeaponObject = null;
    [Space]
    public bool ClampRotation = false;
    public float RotationClampMinY = -180f;
    public float RotationClampMaxY = 180f;
    public float RotationClampMinZ = -180f;
    public float RotationClampMaxZ = 180f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" BASE VEHICLE MOVEMENT")]
    [Space]
    public float Acceleration = 5f;
    public float Deceleration = 2f;
    [Space]
    public float MaxSpeed = 20f;
    public float MaxReverseSpeed = -10f;
    [Space]
    public float BaseRotationSpeed = 45f;
    public float WeaponRotationSpeed = 50f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" VEHICLE SOUNDS")]
    [Space]
    public string EngineSoundPath = "SFX/_SFX_[SOUND PREFAB HERE]";

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected float _CurrentSpeed = 0f;
    protected float _BaseRotation = 0f;
    protected float _WeaponRotation = 0f;
    protected CharacterController _Controller = null;
    protected Vector3 _DirectionToTarget = Vector3.zero;
    protected Quaternion _WeaponLookRotation = Quaternion.identity;

    protected AudioSource _EngineSound = null;

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

        // Get component references
        _Controller = GetComponent<CharacterController>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        InfantryMovementSpeed = MaxSpeed;
        _Agent.speed = MaxSpeed;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    protected override void Init() {
        base.Init();

        // Get new engine sound reference
        ///_EngineSound = SoundManager.Instance.PlaySoundAtLocation(EngineSoundPath, transform.position, 0.5f, 1.5f);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Move character controller forward / backwards based on current movement speed
        if (_Controller != null) {

            if (_Controller.enabled) { _Controller.Move(transform.forward * _CurrentSpeed * Time.deltaTime); }
        }

        // The unit is active in the world
        if (_ObjectState == WorldObjectStates.Active) {

            // Update the sound location for the engine sound
            if (_EngineSound != null) { _EngineSound.transform.position = transform.position; }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the unit 'dies'.
    /// </summary>
    public override void OnDeath(WorldObject instigator) {
        base.OnDeath(instigator);

        // Stop the engine sound
        if (_EngineSound != null) { ObjectPooling.Despawn(_EngineSound.gameObject); }
    }    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected override void UpdatePlayerControlledMovement() {

        _Agent.enabled = false;
        if (_ObjectState == WorldObjectStates.Active) {
            
            // Update base rotation
            _BaseRotation = Input.GetAxis("Horizontal") * BaseRotationSpeed * Time.deltaTime;
            if (_BaseRotation != 0) { transform.eulerAngles += new Vector3(0f, _BaseRotation, 0f); }

            // Forward input
            if (Input.GetAxis("Vertical") > 0f) {

                // Apply forward acceleration
                _CurrentSpeed += Acceleration;

                // Clamp to top speed
                if (_CurrentSpeed > MaxSpeed) { _CurrentSpeed = MaxSpeed; }
            }

            // Reverse input
            else if (Input.GetAxis("Vertical") < 0f) {

                // Apply deceleration
                _CurrentSpeed -= Deceleration;

                // Clamp to top reverse speed
                if (_CurrentSpeed < MaxReverseSpeed) { _CurrentSpeed = MaxReverseSpeed; }
            }

            // No input
            else {

                // Slowly decelerate to a stop
                if (_CurrentSpeed > 0.01) { _CurrentSpeed -= Deceleration * 0.5f; }
                else if (_CurrentSpeed < -0.01) { _CurrentSpeed += Deceleration * 0.5f; }
                else { _CurrentSpeed = 0f; }
            }

            // Update weapon rotation
            _WeaponRotation = Input.GetAxis("Mouse X") * WeaponRotationSpeed * Time.deltaTime;
            if (WeaponObject && _WeaponRotation != 0) { WeaponObject.transform.eulerAngles += new Vector3(0f, _WeaponRotation, 0f); }

            // Check for weapon firing input
            if (Input.GetMouseButtonDown(0)) {

                // If theres a weapon attached
                if (PrimaryWeapon) {

                    // Fire the weapon if possible
                    if (PrimaryWeapon.CanFire()) { PrimaryWeapon.FireWeapon(); }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected override void UpdateAIControllerMovement() {
        base.UpdateAIControllerMovement();

        if (_ObjectState == WorldObjectStates.Active) {

            _Agent.enabled = true;

            // Update agent movement characteristics
            _Agent.autoBraking = true;
            _Agent.autoRepath = true;
            _Agent.acceleration = Acceleration;
            _Agent.angularSpeed = BaseRotationSpeed;
            _CurrentSpeed = Vector3.Project(_Agent.desiredVelocity, transform.forward).magnitude * Time.deltaTime;
        }
        else { _Agent.enabled = false; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected override void SightLineCheck() {

        // Look at attack target
        _IsAttacking = true;
        ///if (_IsAttacking) { LookAt(_AttackTargetObject.transform.position); }

        // Fire raycast to confirm valid line of sight to target
        int def = 1 << LayerMask.NameToLayer("Default");
        int units = 1 << LayerMask.NameToLayer("Units");
        LayerMask mask = def | units;
        RaycastHit hit;
        if (WeaponObject != null) {

            // Raycast from the weapon barrel
            if (Physics.Raycast(MuzzleLaunchPoints[0].transform.position, WeaponObject.transform.forward, out hit, MaxAttackingRange, mask)) {

                // There is a line of sight to the target, fire the weapon (if possible)
                if (PrimaryWeapon.CanFire()) { PrimaryWeapon.FireWeapon(); }

                Debug.DrawRay(MuzzleLaunchPoints[0].transform.position, WeaponObject.transform.forward * MaxAttackingRange, Color.green);
            }
            else { Debug.DrawRay(MuzzleLaunchPoints[0].transform.position, WeaponObject.transform.forward * MaxAttackingRange, Color.red); }
        }
        else {

            // Raycast from the face
            if (Physics.Raycast(MuzzleLaunchPoints[0].transform.position, MuzzleLaunchPoints[0].transform.forward, out hit, MaxAttackingRange, mask)) {

                // There is a line of sight to the target, fire the weapon (if possible)
                if (PrimaryWeapon.CanFire()) { PrimaryWeapon.FireWeapon(); }

                Debug.DrawRay(MuzzleLaunchPoints[0].transform.position, MuzzleLaunchPoints[0].transform.forward * MaxAttackingRange, Color.green);
            }
            else { Debug.DrawRay(MuzzleLaunchPoints[0].transform.position, MuzzleLaunchPoints[0].transform.forward * MaxAttackingRange, Color.red); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected override void LookAtLerp(Vector3 position) {

        // Rotate the vehicle's weapon to face the target
        if (WeaponObject) {

            // Find the vector pointing from our position to the target
            _DirectionToTarget = (position - WeaponObject.transform.position).normalized;

            // Create the rotation we need to be in to look at the target
            _WeaponLookRotation = Quaternion.LookRotation(_DirectionToTarget);

            // Clamp the rotation
            if (ClampRotation) {  _WeaponLookRotation.eulerAngles = new Vector3(_WeaponLookRotation.eulerAngles.x,
                                                                                Mathf.Clamp(_WeaponLookRotation.y, RotationClampMinY, RotationClampMaxY),
                                                                                Mathf.Clamp(_WeaponLookRotation.z, RotationClampMinZ, RotationClampMaxZ));
            }

            // Rotate us over time according to speed until we are in the required rotation
            WeaponObject.transform.rotation = Quaternion.LerpUnclamped(WeaponObject.transform.rotation, _WeaponLookRotation, Time.deltaTime * 2);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Snaps the agent's transform to the position specified.
    /// </summary>
    protected override void LookAtSnap(Vector3 position) {

        // Use the vehicles attached "gunner object" for the lookAt function
        if (WeaponObject) { WeaponObject.transform.LookAt(position); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="delayTime"></param>
    protected override IEnumerator ResetWeaponPosition(int delayTime) {

        yield return new WaitForSeconds(delayTime);

        // Delay complete so reset position
        if (_AttackTarget == null) {

            // Face forward
            ///if (WeaponObject != null) { LookAtLerp(ActiveState.transform.forward * 100f); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}