using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class VehicleWeapon : MonoBehaviour {

    //***************************************************************
    // INSPECTOR

    [Space]
    [Header("Firing")]
    public EFiringType FiringType;
    public Projectile Projectile;

    [Space]
    [Header("Controller Rumble")]
    public float RumbleMotorLeft;
    public float RumbleMotorRight;
    public float RumbleTime;

    [Space]
    [Header("Rate")]
    public float FireDelay = 0f;
    public float ChargeUpTime = 0f;
    public float ChargedFireTime = 0f;

    [Space]
    [Header("Ammo")]
    public int MagazineSize = 0;
    public int ReloadsLeft = 0;

    [Space]
    [Header("Transform")]
    public float RotationSpeed = 0f;

    //***************************************************************
    // VARIABLES

    public enum EFiringType { NoShoot, SemiAuto, FullAuto, ChargeUp, ENUM_COUNT }

    protected VehicleBase       _Vehicle;
    protected Vector3           _Look                   = Vector3.zero;
    protected Player      _PlayerEntity           = null;

    protected bool              _CanFire                = false;
    protected float             _FiringDelay            = 0f;
    protected bool              _CanTryToFire           = true;
    protected bool              _IsFiring               = false;
    protected bool              _IsChargingUp           = false;
    protected float             _ChargeUpAmount         = 0;
    protected float             _ChargedFireAmount      = 0f;
    protected int               _Magazine               = 0;

    //***************************************************************
    // FUNCTIONS

    virtual protected void Start () {

        // Get component references
        _Vehicle = transform.parent.GetComponentInChildren<VehicleBase>();
    }

    virtual protected void Update () {

        UpdateValues();
        CheckFire();

        // Update firing delay
        if (_FiringDelay > 0f) { _FiringDelay -= Time.deltaTime; }

        // Rotation input check
        if (_Vehicle) {

            // Update movement based on controller type
            switch (_Vehicle.GetControllerType()) {

                case VehicleBase.EControllerType.PlayerControlled: { UpdateRotation_PlayerControlled(); break; }
                case VehicleBase.EControllerType.AiControlled: { UpdateRotation_AiControlled(); break; }
                default: break;
            }
        }
    }

    virtual protected void UpdateValues () { }

    private void UpdateRotation_PlayerControlled () {

        // Rotate weapon
        if (_PlayerEntity._Input.GetRightThumbstickYaxis() != 0 || _PlayerEntity._Input.GetRightThumbstickXaxis() != 0) {

            Vector3 direction = _PlayerEntity._Input.GetRightThumbstickInput();
            Quaternion targetRotation = Quaternion.Euler(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateRotation_AiControlled () {

    }
    
    protected void CheckFire () {

        _CanFire = _FiringDelay <= 0f && _CanTryToFire;
        /*
        // Select firing button
        switch (_PlayerEntity.GetControlScheme()) {

            // Swap on control scheme
            case 0: { _PlayerEntity.SetControlScheme(1); break; }
            case 1: { PlayerControlled_UpdateFire_1(); break; }
            case 2: { PlayerControlled_UpdateFire_2(); break; }
            default: break;
        }*/
    }

    virtual protected void PlayerControlled_UpdateFire_1() {

        // On right trigger press
        if (_PlayerEntity._Input.OnRightTrigger() && _CanFire)
            Fire();
        
        // On right trigger release
        else if (!_PlayerEntity._Input.OnRightTrigger())
            StopFiring();
    }

    virtual protected void PlayerControlled_UpdateFire_2() {

        // On shoulder button press
        if ((_PlayerEntity._Input.GetRightShoulderClicked() || _PlayerEntity._Input.GetLeftShoulderClicked()) && _CanFire)
            Fire();
        
        // On shoulder button release 
        else if (!_PlayerEntity._Input.GetRightShoulderClicked() && !_PlayerEntity._Input.GetLeftShoulderClicked())
            StopFiring();
    }

    protected void Fire() {

        _IsFiring = true;
        _IsChargingUp = false;

        // Controller vibration
        _PlayerEntity._Input.StartRumble(1f, 1f, 0.2f);

        // Reset firing delay
        _FiringDelay = FireDelay;
        switch (FiringType) {

            case EFiringType.SemiAuto: {

                _CanTryToFire = false;
                break;
            }

            case EFiringType.FullAuto: {

                _CanTryToFire = true;
                break;
            }

            case EFiringType.ChargeUp: {

                break;
            }

            default: break;
        }
    }

    protected void StopFiring() {

        _CanTryToFire = true;
        _IsFiring = false;
    }

    //***************************************************************
    // SET & GET

    public void SetPlayerEntity(Player player) { _PlayerEntity = player; }

    public Player GetPlayerEntity() { return _PlayerEntity; }

    public void SetLook(Vector3 newLook) { _Look = newLook; }

    public Vector3 GetLook() { return _Look; }

    public float GetRotationSpeed() { return RotationSpeed; }

}