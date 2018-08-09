using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 7/8/2018
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
    public EProjectileType _ProjectileType;
    public Projectile ProjectileClass = null;
    public ParticleSystem FiringEffect = null;
    [Space]
    public ParticleSystem MuzzleEffect = null;
    [Space]
    public FireType FiringType = FireType.FullAuto;
    public int ProjectilesPerShot = 1;
    public float FiringDelay = 0.5f;
    [Space]
    public ObjectDamages Damages;

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

    public enum EProjectileType { Object, Raycast, Particle }
    public enum FireType { FullAuto, Spread }

    [System.Serializable]
    public struct ObjectDamages {

        public int DamageDefault;
        public int DamageCoreInfantry;
        public int DamageAntiInfantryMarine;
        public int DamageHero;
        public int DamageCoreVehicle;
        public int DamageAntiAirVehicle;
        public int DamageMobileArtillery;
        public int DamageBattleTank;
        public int DamageCoreAirship;
        public int DamageSupportShip;
        public int DamageHeavyAirship;
    }

    private int _CurrentMagazineCount;
    private float _FireDelayTimer = 0f;
    private bool _IsReloading = false;
    private float _ReloadTimer = 0f;
    private Unit _UnitAttached = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Initiate magazine count to max size
        _CurrentMagazineCount = MagazineSize;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Fires a 3D projectile object.
    /// </summary>
    private void ProjectileObject() {

        if (ProjectileClass && _UnitAttached != null) {

            // Create projectile facing forward from the muzzle
            Projectile proj = ObjectPooling.Spawn(ProjectileClass.gameObject, _UnitAttached.MuzzleLaunchPoint.transform.position).GetComponent<Projectile>();
            proj.transform.rotation = _UnitAttached.MuzzleLaunchPoint.transform.rotation;

            // Start the projectile
            proj.Init(this);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Fires a hitscan based projectile.
    /// </summary>
    private void ProjectileRaycast() {

        if (_UnitAttached != null) { 

            RaycastHit hit;
            Vector3 attackPos = _UnitAttached.GetAttackTarget().transform.position;
            attackPos.y = attackPos.y + _UnitAttached.GetAttackTarget().GetObjectHeight() / 2;
            Vector3 attackDir = attackPos - _UnitAttached.MuzzleLaunchPoint.transform.position;
            attackDir.Normalize();

            if (Physics.Raycast(_UnitAttached.MuzzleLaunchPoint.transform.position, attackDir, out hit, _UnitAttached.MaxAttackingRange)) {

                Debug.DrawLine(_UnitAttached.MuzzleLaunchPoint.transform.position, hit.point, Color.red);

                // Damage target
                WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                else if (worldObj != null) {

                    // Check if object is of type unit
                    Unit unitObj = worldObj.GetComponent<Unit>();
                    if (unitObj != null) {

                        // Cannot damage self
                        if (unitObj.Team != _UnitAttached.Team) {

                            // Damage based on unit type
                            switch (unitObj.UnitType) {

                                case Unit.EUnitType.Undefined:          { unitObj.Damage(Damages.DamageDefault, _UnitAttached); break; }
                                case Unit.EUnitType.CoreMarine:         { unitObj.Damage(Damages.DamageCoreInfantry, _UnitAttached); break; }
                                case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(Damages.DamageAntiInfantryMarine, _UnitAttached); break; }
                                case Unit.EUnitType.Hero:               { unitObj.Damage(Damages.DamageHero, _UnitAttached); break; }
                                case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(Damages.DamageCoreVehicle, _UnitAttached); break; }
                                case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(Damages.DamageAntiAirVehicle, _UnitAttached); break; }
                                case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(Damages.DamageMobileArtillery, _UnitAttached); break; }
                                case Unit.EUnitType.BattleTank:         { unitObj.Damage(Damages.DamageBattleTank, _UnitAttached); break; }
                                case Unit.EUnitType.CoreAirship:        { unitObj.Damage(Damages.DamageCoreAirship, _UnitAttached); break; }
                                case Unit.EUnitType.SupportShip:        { unitObj.Damage(Damages.DamageSupportShip, _UnitAttached); break; }
                                case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(Damages.DamageHeavyAirship, _UnitAttached); break; }
                                default: break;
                            }
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObj.Damage(Damages.DamageDefault); }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Fires a particle effect that uses particle collision to invoke damage.
    /// </summary>
    private void ProjectileParticle() {
        
        // Play firing effect
        if (FiringEffect != null) {

            // Play
            ParticleSystem effect = ObjectPooling.Spawn(FiringEffect.gameObject, 
                                                        _UnitAttached.MuzzleLaunchPoint.transform.position, 
                                                        _UnitAttached.MuzzleLaunchPoint.transform.rotation).GetComponent<ParticleSystem>();
            effect.GetComponent<ParticleBasedDamage>().SetWeaponAttached(this);
            effect.Play();

            // Despawn particle system once it has finished its cycle
            float effectDuration = effect.duration + effect.startLifetime;
            StartCoroutine(ParticleDespawn(effect, effectDuration));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Fires either a hitscan or 3D projectile object 
    /// </summary>
    public virtual void FireWeapon() {

        // If the weapon can be fired
        if (_CurrentMagazineCount > 0 && CanFire()) {

            _FireDelayTimer = FiringDelay;

            // Deduct ammo (if not bottomless clip)
            if (!BottomlessClip) { _CurrentMagazineCount--; }

            // Fire trace (or projectile)
            // Determine damage type
            switch (_ProjectileType) {

                case EProjectileType.Object:    { ProjectileObject(); break; }
                case EProjectileType.Raycast:   { ProjectileRaycast(); break; }
                case EProjectileType.Particle:  { ProjectileParticle(); break; }
                default: { break; }
            }

            // Play muzzle firing effect
            if (MuzzleEffect != null) {

                // Play
                ParticleSystem effect = ObjectPooling.Spawn(MuzzleEffect.gameObject, 
                                                            _UnitAttached.MuzzleLaunchPoint.transform.position,
                                                            _UnitAttached.MuzzleLaunchPoint.transform.rotation).GetComponent<ParticleSystem>();
                effect.Play();

                // Despawn particle system once it has finished its cycle
                float effectDuration = effect.duration + effect.startLifetime;
                StartCoroutine(ParticleDespawn(effect, effectDuration));
            }
        }

        // Reloading if theres no ammo in the mag left
        else if (_CurrentMagazineCount <= 0) { Reload(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits for the seconds specified then attempts to repool
    //  the particle effect (or destroyed entirely if re-pooling isn't possible)
    /// </summary>
    /// <param name="particleEffect"></param>
    /// <param name="delay"></param>
    IEnumerator ParticleDespawn(ParticleSystem particleEffect, float delay) {

        // Delay
        yield return new WaitForSeconds(delay);

        // Despawn the system
        ObjectPooling.Despawn(particleEffect.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnitAttached(Unit unit) { _UnitAttached = unit; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public Unit GetUnitAttached() { return _UnitAttached; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public bool IsReloading() { return _IsReloading; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns TRUE if the weapon's firing delay timer is complete
    //  and the weapon isn't currently reloading.
    /// </summary>
    /// <returns></returns>
    public bool CanFire() { return _FireDelayTimer <= 0 && !_IsReloading; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}