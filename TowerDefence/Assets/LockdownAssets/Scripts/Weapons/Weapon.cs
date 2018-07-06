using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 4/7/2018
//
//******************************

public class Weapon : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" FIRING PROPERTIES")]
    [Space]
    public float FiringDelay = 0.5f;
    public bool HitScanProjectile = true;
    public HitScanDamages RaycastDamages;
    [Space]
    public Projectile ProjectileClass = null;
    public ParticleSystem FiringEffect = null;
    public FireType FiringType = FireType.FullAuto;

    [Space]
    [Header("-----------------------------------")]
    [Header(" MAGAZINE PROPERTIES")]
    [Space]
    public bool BottomlessClip = false;
    public int MagazineSize = 30;
    public float ReloadTime = 2f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [System.Serializable]
    public struct HitScanDamages {

        public int DamageDefault;
        public int DamageCoreInfantry;
        public int DamageAntiInfantryMarine;
        public int DamageAntiVehicleMarine;
        public int DamageCoreVehicle;
        public int DamageAntiAirVehicle;
        public int DamageMobileArtillery;
        public int DamageBattleTank;
        public int DamageCoreAirship;
        public int DamageSupportShip;
        public int DamageBattleAirship;
    }
    public enum FireType { FullAuto, Spread }

    private int _CurrentMagazineCount;
    private float _FireDelayTimer = 0f;
    private bool _IsReloading = false;
    private float _ReloadTimer = 0f;
    private Unit _UnitAttached = null;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Initiate magazine count to max size
        _CurrentMagazineCount = MagazineSize;
    }

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        // Firing delay timer
        if (_FireDelayTimer > 0f) { _FireDelayTimer -= Time.deltaTime; }

        // Reloading timer
        if (_ReloadTimer > 0f) { _ReloadTimer -= Time.deltaTime; }
        else { _IsReloading = false; }
    }

    /// <summary>
    //  Fires a hitscan or 3D projectile object 
    /// </summary>
    public void FireWeapon() {

        // If the weapon can be fired
        if (_CurrentMagazineCount > 0 && CanFire()) {

            _FireDelayTimer = FiringDelay;

            // Deduct ammo (if not bottomless clip)
            if (!BottomlessClip) { _CurrentMagazineCount--; }

            // Fire trace (or projectile)
            if (HitScanProjectile && _UnitAttached != null) {

                RaycastHit hit;
                if (Physics.Raycast(_UnitAttached.MuzzleLaunchPoint.transform.position, _UnitAttached.GetAttackTarget().transform.position, out hit, _UnitAttached.AttackingRange)) {

                    // Damage target
                    WorldObject worldObj = hit.transform.GetComponent<WorldObject>();

                    // Check if object is of type unit
                    Unit unitObj = worldObj.GetComponent<Unit>();
                    if (unitObj != null) {

                        // Damage based on unit type
                        switch (unitObj.Type) {

                            case Unit.UnitType.Undefined:           { unitObj.Damage(RaycastDamages.DamageDefault); break; }
                            case Unit.UnitType.CoreMarine:          { unitObj.Damage(RaycastDamages.DamageCoreInfantry); break; }
                            case Unit.UnitType.AntiInfantryMarine:  { unitObj.Damage(RaycastDamages.DamageAntiInfantryMarine); break; }
                            case Unit.UnitType.AntiVehicleMarine:   { unitObj.Damage(RaycastDamages.DamageAntiVehicleMarine); break; }
                            case Unit.UnitType.CoreVehicle:         { unitObj.Damage(RaycastDamages.DamageCoreVehicle); break; }
                            case Unit.UnitType.AntiAirVehicle:      { unitObj.Damage(RaycastDamages.DamageAntiAirVehicle); break; }
                            case Unit.UnitType.MobileArtillery:     { unitObj.Damage(RaycastDamages.DamageMobileArtillery); break; }
                            case Unit.UnitType.BattleTank:          { unitObj.Damage(RaycastDamages.DamageBattleTank); break; }
                            case Unit.UnitType.CoreAirship:         { unitObj.Damage(RaycastDamages.DamageCoreAirship); break; }
                            case Unit.UnitType.SupportShip:         { unitObj.Damage(RaycastDamages.DamageSupportShip); break; }
                            case Unit.UnitType.BattleAirship:       { unitObj.Damage(RaycastDamages.DamageBattleAirship); break; }
                            default: break;
                        }
                    }

                    // Damage the object
                    else { worldObj.Damage(RaycastDamages.DamageDefault); }
                }

                // Hit max range without hitting anything
                else {

                    ///hit.transform.position = _UnitAttached.MuzzleLaunchPoint.transform.position + _UnitAttached.MuzzleLaunchPoint.transform.forward * _UnitAttached.AttackingRange;
                }
            }

            // 3D projectile
            else if (ProjectileClass && _UnitAttached != null) {

                // Create projectile
                Projectile proj = ObjectPooling.Spawn(ProjectileClass.gameObject, _UnitAttached.MuzzleLaunchPoint.transform.position).GetComponent<Projectile>();

                // Set projectile rotation based on controller
                if (_UnitAttached.IsBeingPlayerControlled()) {

                    // The position is already set so just need to set its forward direction
                    proj.transform.rotation = _UnitAttached.MuzzleLaunchPoint.transform.rotation;
                }

                else { proj.transform.LookAt(_UnitAttached.GetAttackTarget().transform.position/* + Vector3.up*/); }

                // Now start the projectile
                proj.Init(this);
            }

            // Play firing effect
            if (FiringEffect != null) {

                // Play
                ParticleSystem effect = ObjectPooling.Spawn(FiringEffect.gameObject, _UnitAttached.MuzzleLaunchPoint.transform.position, _UnitAttached.MuzzleLaunchPoint.transform.rotation).GetComponent<ParticleSystem>();
                effect.Play();

                // Despawn particle system once it has finished its cycle
                float effectDuration = effect.duration + effect.startLifetime;
                StartCoroutine(ParticleDespawn(effect, effectDuration));
            }
        }

        // Reloading if theres no ammo in the mag left
        else if (_CurrentMagazineCount <= 0) { Reload(); }
    }

    /// <summary>
    //  Sets the magazine size to max capacity, then triggers the reload timer. 
    /// </summary>
    public void Reload() {

        // Only reload if magazine ISN'T max size
        if (_CurrentMagazineCount < MagazineSize) {

            _ReloadTimer = ReloadTime;
            _IsReloading = true;
            _CurrentMagazineCount = MagazineSize;
        }
    }

    /// <summary>
    //  
    /// </summary>
    /// <param name="particleEffect"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator ParticleDespawn(ParticleSystem particleEffect, float delay) {

        // Delay
        yield return new WaitForSeconds(delay);

        // Despawn the system
        ObjectPooling.Despawn(particleEffect.gameObject);
    }

    /// <summary>
    //  
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnitAttached(Unit unit) { _UnitAttached = unit; }

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public Unit GetUnitAttached() { return _UnitAttached; }

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public bool IsReloading() { return _IsReloading; }

    /// <summary>
    //  Returns TRUE if the weapon's firing delay timer is complete
    //  and the weapon isn't currently reloading.
    /// </summary>
    /// <returns></returns>
    public bool CanFire() { return _FireDelayTimer <= 0 && !_IsReloading; }
    
}