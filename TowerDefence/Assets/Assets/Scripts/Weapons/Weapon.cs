using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class Weapon : MonoBehaviour {

    //***************************************************************
    // VARIABLES

    public enum EFiringType { NoShoot, SemiAuto, FullAuto, ChargeUp, ENUM_COUNT }

    [Space]
    [Header("Firing")]
    public EFiringType          _FiringType;
    public Projectile           _Projectile;
    [Space]
    [Header("Rate")]
    public float                _FiringDelay            = 0f;
    public float                _ChargeUpTime           = 0f;
    public float                _ChargedFireTime        = 0f;
    [Space]
    [Header("Ammo")]
    public int                  _MagazineSize           = 0;
    public int                  _ReloadsLeft            = 0;
    [Space]
    [Header("Transform")]
    public float                _RotationSpeed          = 0f;

    protected GroundVehicle     _Vehicle;
    protected Vector3           _Look                   = Vector3.zero;

    protected bool              _IsFiring               = false;
    protected bool              _IsChargeFiring         = false;
    protected float             _ChargeUpAmount         = 0;
    protected float             _ChargedFireAmount      = 0f;
    protected int               _Magazine               = 0;

    //***************************************************************
    // FUNCTIONS

    virtual protected void Start () {

        // Get component references
        _Vehicle = GetComponentInParent<GroundVehicle>();
	}

    virtual protected void Update () {

        UpdateValues();

        // Weapon is attached to a vehicle
        if (_Vehicle) {

            // Update movement based on controller type
            switch (_Vehicle.GetControllerType()) {

                case GroundVehicle.EControllerType.PlayerControlled: { UpdateMovement_PlayerControlled(); break; }
                case GroundVehicle.EControllerType.AiControlled: { UpdateMovement_AiControlled(); break; }
                default: break;
            }
        }
    }

    virtual protected void UpdateValues () { }

    private void UpdateMovement_PlayerControlled () {

        switch (_Vehicle.GetControllerScheme()) {

            // Swap on control scheme
            case 0: { _Vehicle.SetControllerScheme(1); break; }
            case 1: { PlayerControlled_UpdateFire_1(); break; }
            case 2: { PlayerControlled_UpdateFire_2(); break; }
            case 3: { PlayerControlled_UpdateFire_3(); break; }
            default: break;
        }
    }

    private void UpdateMovement_AiControlled () {

    }

    virtual protected void PlayerControlled_UpdateFire_1 () {


    }
                                                         
    virtual protected void PlayerControlled_UpdateFire_2 () {


    }
                                                         
    virtual protected void PlayerControlled_UpdateFire_3 () { PlayerControlled_UpdateFire_1(); }

    protected void CheckFire () {


    }

    //***************************************************************
    // SET & GET

    public Vector3 GetLook() { return _Look; }

    public void SetLook(Vector3 newLook) { _Look = newLook; }

    public float GetRotationSpeed() { return _RotationSpeed; }

}