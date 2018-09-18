using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 1/9/2018
//
//******************************

public class Projectile : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MOVEMENT")]
    [Space]
    public float MovementSpeed = 40f;
    public float MaxDistance = 1000f;
    [Space]
    public bool AffectedByGravity = false;
    public float GravityStrength = 1f;
    [Space]
    public bool HomingProjectile = false;
    public float TrackingStrength = 0f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" ON IMPACT")]
    [Space]
    public bool ExplodeOnImpact = false;
    public float ExplosionRadius = 20f;
    public float DamageFalloff = 0.5f;
    public ParticleSystem ExplosionEffect = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private float _DistanceTravelled  = 0f;
    private Vector3 _Velocity = Vector3.zero;
    private Vector3 _Downwards = Vector3.down;
    private Weapon _WeaponAttached = null;
    private Vector3 _OriginPosition = Vector3.zero;
    private Weapon.ObjectDamages _Damages;
    private WorldObject HomingTarget = null;
    private Vector3 _DirectionToTarget = Vector3.zero;
    private Quaternion _WeaponLookRotation = Quaternion.identity;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    private void Start () {

        // Set forward velocity
        _Velocity = transform.forward;
        _DistanceTravelled = 0f;

        // Downwards is up because the position subtracts this force
        // this avoids having a 'negative - negative = positive force'
        _Downwards = transform.up;
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Initializes the projectile, usually called 
    //  when it is being reused from an object pool
    /// </summary>
    public void Init(Weapon wep) {

        // Reset stats
        _DistanceTravelled = 0f;
        _WeaponAttached = wep;
        _Velocity = transform.forward;
        _OriginPosition = transform.position;
        _Damages = wep.Damages;

        // This should already be called when it is pulled from its object pool,
        // but this is just incase it somehow isn't
        gameObject.SetActive(true);

        // Set homing target for tracking
        if (HomingProjectile) {

            if (_WeaponAttached.GetUnitAttached() != null) { HomingTarget = _WeaponAttached.GetUnitAttached().GetAttackTarget(); }
            if (_WeaponAttached.GetTowerAttached() != null) { HomingTarget = _WeaponAttached.GetTowerAttached().GetAttackTarget(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update () {

        // Constantly move forward
        _Velocity = transform.forward;
        transform.position += _Velocity * MovementSpeed * Time.deltaTime;

        // Apply downward force if affected by gravity
        if (AffectedByGravity) { transform.position -= _Downwards * GravityStrength * Time.deltaTime; }

        // Re-pool projectile when it has reached max distance threshold
        if (_DistanceTravelled < MaxDistance) _DistanceTravelled = Vector3.Distance(_OriginPosition, transform.position);
        else { OnDestroy(); }

        // Track target if possible
        if (HomingProjectile && HomingTarget != null) {

            if (HomingTarget.IsAlive()) {

                // Find the vector pointing from our position to the target
                _DirectionToTarget = (HomingTarget.transform.position - transform.position).normalized;

                // Create the rotation we need to be in to look at the target
                _WeaponLookRotation = Quaternion.LookRotation(_DirectionToTarget);

                // Rotate us over time according to speed until we are in the required rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, _WeaponLookRotation, TrackingStrength * Time.deltaTime);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the projectile is destroyed, this is 
    //  usually called when the projectile collides with something
    /// </summary>
    protected void OnDestroy() {

        if (ExplodeOnImpact) {

            /*
                // DEBUGGING SPHERECAST
                // Uncomment LINE 273 (the ignore raycast line) otherwise this will give unintended results!
                GameObject child = new GameObject();
                child.transform.parent = gameObject.transform;
                SphereCollider debug = child.AddComponent<SphereCollider>();
                debug.radius = ExplosionRadius;
                debug.isTrigger = true;
                debug.transform.localPosition = new Vector3(0, 0, 0);
                child.layer = LayerMask.NameToLayer("Ignore Raycast");
            */

            // Use a spherical raycast for an AOE damage with falloff
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, ExplosionRadius, transform.forward, 0f);
            foreach (var rayHit in hits) {

                WorldObject worldObj = rayHit.transform.gameObject.GetComponentInParent<WorldObject>();
                if (worldObj != null) {

                    // Get reference to the instigator this projectile belongs to
                    WorldObject worldObjAttached = null;
                    if (_WeaponAttached.GetUnitAttached() != null) { worldObjAttached = _WeaponAttached.GetUnitAttached(); }
                    if (_WeaponAttached.GetTowerAttached() != null) { worldObjAttached = _WeaponAttached.GetTowerAttached(); }

                    // Friendly fire is OFF
                    if (worldObj.Team != worldObjAttached.Team) {

                        // Determine damage falloff
                        float distanceFromEpicenter = Vector3.Distance(transform.position, rayHit.point);
                        float damageMultiplier = (distanceFromEpicenter / ExplosionRadius) * DamageFalloff;

                        // Damage the object
                        worldObj.Damage(Mathf.FloorToInt(_Damages.DamageDefault * damageMultiplier));
                    }
                }
            }            

            // Play OnDeath effect
            if (ExplosionEffect != null) {

                // Play
                ParticleSystem effect = ObjectPooling.Spawn(ExplosionEffect.gameObject, transform.position, transform.rotation).GetComponent<ParticleSystem>();
                effect.Play();

                // Despawn particle system once it has finished its cycle
                float effectDuration = effect.duration + effect.startLifetime;
                StartCoroutine(ParticleDespawn(effect, effectDuration));
            }
        }

        gameObject.SetActive(false);
        ObjectPooling.Despawn(gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  When this collider/rigid body enters another collider/rigidbody 
    //  (there must be at least 1 rigidbody in the collision for this to work)
    /// </summary>
    /// <param name="collision"></param>
    protected void OnCollisionEnter(Collision collision) {

        // Check for terrain collision
        if (collision.gameObject.CompareTag("Ground")) { OnDestroy(); }

        // Get object type
        GameObject gameObj = collision.gameObject;
        WorldObject worldObj = gameObj.GetComponentInParent<WorldObject>();

        // Successful WorldObject cast
        if (worldObj != null) {

            DifficultyManager dm = DifficultyManager.Instance;
            DifficultyManager.EDifficultyModifiers mod = DifficultyManager.EDifficultyModifiers.Damage;
            WaveManager wm = WaveManager.Instance;

            // Check if object is of type unit
            Unit unitObj = worldObj.GetComponent<Unit>();
            if (unitObj != null) {

                if (_WeaponAttached != null) {

                    // Cant damage self
                    if (worldObj == _WeaponAttached.GetUnitAttached()) { return; }

                    // Friendly fire is OFF
                    if (unitObj.Team != _WeaponAttached.GetUnitAttached().Team) {

                        // Damage based on unit type
                        switch (unitObj.UnitType) {

                            case Unit.EUnitType.Undefined:          { unitObj.Damage(_Damages.DamageDefault * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.CoreMarine:         { unitObj.Damage(_Damages.DamageCoreInfantry * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(_Damages.DamageAntiInfantryMarine * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.Hero:               { unitObj.Damage(_Damages.DamageHero * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(_Damages.DamageCoreVehicle * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(_Damages.DamageAntiAirVehicle * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(_Damages.DamageMobileArtillery * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.BattleTank:         { unitObj.Damage(_Damages.DamageBattleTank * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.CoreAirship:        { unitObj.Damage(_Damages.DamageCoreAirship * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.SupportShip:        { unitObj.Damage(_Damages.DamageSupportShip * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(_Damages.DamageHeavyAirship * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                            default: break;
                        }
                    }
                }

                // Destroy projectile
                OnDestroy();
            }

            // Damage the world object
            else {

                // Destroy projectile
                worldObj.Damage(_Damages.DamageDefault * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod) * wm.GetWaveDamageModifier(worldObj));
                OnDestroy();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  When the trigger overlaps another collider (entry only)
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other) {

        // Check for terrain collision
        ///if (other.gameObject.CompareTag("Ground")) { OnDestroy(); }

        // Get object type
        GameObject gameObj = other.gameObject;
        WorldObject worldObj = gameObj.GetComponentInParent<WorldObject>();

        ///if (gameObj.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

        // Successful WorldObject cast
        if (worldObj != null) {

            DifficultyManager dm = DifficultyManager.Instance;
            DifficultyManager.EDifficultyModifiers mod = DifficultyManager.EDifficultyModifiers.Damage;
            WaveManager wm = WaveManager.Instance;

            // Check if object is of type unit
            Unit unitObj = worldObj.GetComponent<Unit>();
            if (unitObj != null) {

                if (_WeaponAttached != null) {
                    
                    // Does this projectile belong to a unit?
                    if (_WeaponAttached.GetUnitAttached() != null) {

                        // Cant damage self
                        if (worldObj == _WeaponAttached.GetUnitAttached()) { return; }

                        // Friendly fire is OFF
                        if (unitObj.Team != _WeaponAttached.GetUnitAttached().Team) {

                            // Damage based on unit type
                            switch (unitObj.UnitType) {

                                case Unit.EUnitType.Undefined:          { unitObj.Damage(_Damages.DamageDefault * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.CoreMarine:         { unitObj.Damage(_Damages.DamageCoreInfantry * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(_Damages.DamageAntiInfantryMarine * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.Hero:               { unitObj.Damage(_Damages.DamageHero * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(_Damages.DamageCoreVehicle * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(_Damages.DamageAntiAirVehicle * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(_Damages.DamageMobileArtillery * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.BattleTank:         { unitObj.Damage(_Damages.DamageBattleTank * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.CoreAirship:        { unitObj.Damage(_Damages.DamageCoreAirship * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.SupportShip:        { unitObj.Damage(_Damages.DamageSupportShip * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(_Damages.DamageHeavyAirship * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetUnitAttached()); break; }
                                default: break;
                            }
                        }
                    }

                    // Does this projectile belong to a tower?
                    if (_WeaponAttached.GetTowerAttached() != null) {

                        // Cant damage self
                        if (worldObj == _WeaponAttached.GetTowerAttached()) { return; }

                        // Friendly fire is OFF
                        if (unitObj.Team != _WeaponAttached.GetTowerAttached().Team) {

                            // Damage based on unit type
                            switch (unitObj.UnitType) {

                                case Unit.EUnitType.Undefined:          { unitObj.Damage(_Damages.DamageDefault * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.CoreMarine:         { unitObj.Damage(_Damages.DamageCoreInfantry * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(_Damages.DamageAntiInfantryMarine * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.Hero:               { unitObj.Damage(_Damages.DamageHero * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(_Damages.DamageCoreVehicle * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(_Damages.DamageAntiAirVehicle * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(_Damages.DamageMobileArtillery * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.BattleTank:         { unitObj.Damage(_Damages.DamageBattleTank * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.CoreAirship:        { unitObj.Damage(_Damages.DamageCoreAirship * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.SupportShip:        { unitObj.Damage(_Damages.DamageSupportShip * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(_Damages.DamageHeavyAirship * dm.GetDifficultyModifier(unitObj, mod) * wm.GetWaveDamageModifier(unitObj), _WeaponAttached.GetTowerAttached()); break; }
                                default: break;
                            }
                        }
                    }
                }

                // Destroy projectile
                OnDestroy();
            }

            // Damage the world object
            else {

                // Destroy projectile
                worldObj.Damage(_Damages.DamageDefault * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod) * wm.GetWaveDamageModifier(worldObj));
                OnDestroy();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits for the seconds specified then attempts to re-pool
    //  the particle effect (or destroyed entirely if re-pooling isn't possible)
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}