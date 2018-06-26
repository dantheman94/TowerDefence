using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class VehicleBase : MonoBehaviour {

    //***************************************************************
    // INSPECTOR

    [Space]
    [Header("Movement")]
    public      float           _Acceleration           = 1f;
    public      float           _Decceleration          = 0.5f;
    public      float           _TopSpeed               = 20f;
    public      float           _TopReverseSpeed        = -10f;
    public      float           _BaseRotationSpeed      = 1.5f;

    //***************************************************************
    // VARIABLES

    public enum EControllerType { PlayerControlled, AiControlled, ENUM_COUNT }
    public enum EVehicleType { Car, Tank, StationaryTower, ENUM_COUNT }    

    protected   float           _CurrentSpeed           = 0f;
    protected   float           _BaseRotation           = 0f;
    protected   float           _ZOffset                { get; set; }
    protected   bool            _EnemyTeam              = false;

    protected   EVehicleType    _VehicleType;
    protected   EControllerType _ControllerType         = EControllerType.ENUM_COUNT;

    protected   MultipleTags    _TagList                = null;
    protected   Damagable       _DamageComponent        = null;
    protected   Tier            _TierComponent          = null;
    protected   FogOfWar        _FogOfWar               = null;
    protected   Player          _PlayerEntity           = null;
    protected   CharacterController _CharController     = null;

    //***************************************************************
    // FUNCTIONS

    virtual protected void Start () {

        // Get component references
        _ZOffset                = transform.position.y;
        _DamageComponent        = GetComponent<Damagable>();
        _TierComponent          = GetComponent<Tier>();
        _CharController         = GetComponentInParent<CharacterController>();
        _FogOfWar               = GetComponentInChildren<FogOfWar>();

        // Add to managers
    }

    virtual protected void Update () {

        UpdateValues();

        // Update movement based on controller type
        switch (_ControllerType) {

            case EControllerType.PlayerControlled:  { UpdateMovement_PlayerControlled();    break; }
            case EControllerType.AiControlled:      { UpdateMovement_AiControlled();        break; }
            case EControllerType.ENUM_COUNT:        { _ControllerType = EControllerType.AiControlled; break; }
            default: break;
        }

        // Move character controller forward / backwards based on current movement speed
        if (_CharController) { _CharController.Move(transform.right * _CurrentSpeed * Time.deltaTime); }
    }

    protected void UpdateMovement_PlayerControlled () {

        if (_PlayerEntity) {
            /*
            switch (_PlayerEntity.GetControlScheme()) {

                // Swap on control scheme
                case 0: { _PlayerEntity.SetControlScheme(1); break; }
                case 1: { PlayerControlled_UpdateMovement_1(); break; }
                case 2: { PlayerControlled_UpdateMovement_2(); break; }
                default: break;
            }*/
        }
    }

    protected void UpdateMovement_AiControlled () {

    }

    virtual protected void PlayerControlled_UpdateMovement_1 () {
        
        // Update base rotation
        _BaseRotation = _PlayerEntity._Input.GetLeftThumbstickXaxis() * _BaseRotationSpeed;
        if (_BaseRotation != 0) { transform.eulerAngles += new Vector3(0f, _BaseRotation, 0f); }

        // Update base movement
        if (_PlayerEntity._Input.GetLeftThumbstickYaxis() > 0.01f) {

            // Apply forward acceleration
            _CurrentSpeed += _Acceleration;

            // Clamp to top speed
            if (_CurrentSpeed > _TopSpeed) { _CurrentSpeed = _TopSpeed; }
        }
        else if (_PlayerEntity._Input.GetLeftThumbstickYaxis() < -0.01f) {

            // Apply backward acceleration
            _CurrentSpeed -= _Decceleration;

            // Clamp to reverse top speed
            if (_CurrentSpeed < _TopReverseSpeed) { _CurrentSpeed = _TopReverseSpeed; }
        }

        else {

            // Slowly decelerate to a stop
            if (_CurrentSpeed > 0.01) { _CurrentSpeed -= _Decceleration * 0.5f; }
            else if (_CurrentSpeed < -0.01) { _CurrentSpeed += _Decceleration * 0.5f; }
            else { _CurrentSpeed = 0f; }
        }        
    }

    virtual protected void PlayerControlled_UpdateMovement_2 () {

        // Update base rotation
        _BaseRotation = _PlayerEntity._Input.GetLeftThumbstickXaxis() * _BaseRotationSpeed;
        if (_BaseRotation != 0) { transform.eulerAngles += new Vector3(0f, _BaseRotation, 0f); }

        // Update base movement
        if (_PlayerEntity._Input.OnRightTrigger()) {

            // Apply forward acceleration
            _CurrentSpeed += _Acceleration;

            // Clamp to top speed
            if (_CurrentSpeed > _TopSpeed) { _CurrentSpeed = _TopSpeed; }
        }
        else if (_PlayerEntity._Input.OnLeftTrigger()) {

            // Apply backward acceleration
            _CurrentSpeed -= _Decceleration;

            // Clamp to reverse top speed
            if (_CurrentSpeed < _TopReverseSpeed) { _CurrentSpeed = _TopReverseSpeed; }
        }

        else {

            // Slowly decelerate to a stop
            if (_CurrentSpeed > 0.01) { _CurrentSpeed -= _Decceleration * 0.5f; }
            else if (_CurrentSpeed < -0.01) { _CurrentSpeed += _Decceleration * 0.5f; }
            else { _CurrentSpeed = 0f; }
        }
    }

    public void OnLevel2 () { UpdateTier2Stats(); }
                         
    public void OnLevel3 () { UpdateTier3Stats(); }
                         
    public void OnLevel4 () { UpdateTier4Stats(); }
                         
    public void OnLevel5 () { UpdateTier5Stats(); }

    protected void UpdateValues () {

        switch (_TierComponent.iTier) {
            
            case 2: { UpdateTier2Stats(); break; }
            case 3: { UpdateTier3Stats(); break; }
            case 4: { UpdateTier4Stats(); break; }
            case 5: { UpdateTier5Stats(); break; }
            default: break;
        }
    }
    
    protected void UpdateTier2Stats () {
        
        // Update tier 2
        _FogOfWar._Radius = _FogOfWar._FogOfWarTier2Radius;
    }
                                    
    protected void UpdateTier3Stats () {

        // Update tier 3
        _FogOfWar._Radius = _FogOfWar._FogOfWarTier3Radius;
    }
                                    
    protected void UpdateTier4Stats () {

        // Update tier 4
        _FogOfWar._Radius = _FogOfWar._FogOfWarTier4Radius;
    }
                                    
    protected void UpdateTier5Stats () {

        // Update tier 5
        _FogOfWar._Radius = _FogOfWar._FogOfWarTier5Radius;
    }

    //***************************************************************
    // SET & GET

    public void SetPlayerEntity(Player player) {

        _PlayerEntity = player;
    }

    public Player GetPlayerEntity() { return _PlayerEntity; }

    public void SetControllerType(EControllerType type) { _ControllerType = type; }

    public EControllerType GetControllerType() { return _ControllerType; }

    public MultipleTags GetTagList() { return _TagList; }

    public void SetIsEnemy(bool value) { _EnemyTeam = value; }

    public bool GetIsEnemy() { return _EnemyTeam; }

    public Tier GetTier() { return _TierComponent; }

}