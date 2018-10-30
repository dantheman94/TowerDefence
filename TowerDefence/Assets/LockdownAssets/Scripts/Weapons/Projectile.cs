using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 30/10/2018
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
    [Space]
    public bool TimedDetonation = false;
    public float TimeTillDetonation = 3f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" ON IMPACT")]
    [Space]
    public GameObject ImpactEffect = null;
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

    private Weapon.ObjectDamages _Damages;
    private Weapon _WeaponAttached = null;

    private Vector3 _Downwards = Vector3.down;
    private Vector3 _Velocity = Vector3.zero;

    private bool _HomingProjectile = false;
    private float _TrackingStrength = 0f;

    private float _DistanceTravelled = 0f;

    private float _DisableTrackDistance = 100000f;
    private WorldObject HomingTarget = null;
    private Vector3 _OriginPosition = Vector3.zero;
    private Vector3 _DirectionToTarget = Vector3.zero;
    private Quaternion _WeaponLookRotation = Quaternion.identity;

    private Vector3 _ArcTargetLocation = Vector3.zero;

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
        _HomingProjectile = wep.TrackingProjectile;
        _TrackingStrength = wep.TrackingStrength;
        if (_HomingProjectile) {

            if (_WeaponAttached.GetUnitAttached() != null)  { HomingTarget = _WeaponAttached.GetUnitAttached().GetAttackTarget(); }
            if (_WeaponAttached.GetTowerAttached() != null) { HomingTarget = _WeaponAttached.GetTowerAttached().GetAttackTarget(); }

            // Set distance for tracking unlock
            _DisableTrackDistance = Vector3.Distance(_OriginPosition, HomingTarget.transform.position);
        }
        else { _DisableTrackDistance = 100000f; }

        // Create parabolic arc component
        ParabolicArc oldArc = GetComponent<ParabolicArc>();
        if (oldArc != null) { Destroy(oldArc); }
        if (AffectedByGravity) { gameObject.AddComponent<ParabolicArc>(); }

        // Timed detonation
        if (TimedDetonation) { StartCoroutine(DetonatorTimer()); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update () {

        // Constantly move forward
        _Velocity = transform.forward;
        transform.position += _Velocity * MovementSpeed * Time.deltaTime;
        
        // Re-pool projectile when it has reached max distance threshold
        if (_DistanceTravelled < MaxDistance) _DistanceTravelled = Vector3.Distance(_OriginPosition, transform.position);
        else { OnDestroy(); }
        
        // Track target if possible
        if (_HomingProjectile && HomingTarget != null) {
            
            // Determine if we have passed our target & no longer track if so
            if (_DistanceTravelled >= _DisableTrackDistance) {

                HomingTarget = null;
                _HomingProjectile = false;
            }

            // Is the projectile still tracking
            if (HomingTarget != null) {

                if (HomingTarget.IsAlive()) {

                    // Find the vector pointing from our position to the target
                    Vector3 target = HomingTarget.transform.position;
                    target.y += HomingTarget.GetObjectHeight() * 0.75f;
                    if (HomingTarget.TargetPoint != null) { target = HomingTarget.TargetPoint.transform.position; }

                    _DirectionToTarget = (target - transform.position).normalized;

                    // Create the rotation we need to be in to look at the target
                    _WeaponLookRotation = Quaternion.LookRotation(_DirectionToTarget);

                    // Rotate us over time according to speed until we are in the required rotation
                    transform.rotation = Quaternion.Lerp(transform.rotation, _WeaponLookRotation, _TrackingStrength * Time.deltaTime);
                }
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
            }

            // Camera shake on explosion
            Player player = null;
            if (_WeaponAttached.GetUnitAttached() != null) { player = _WeaponAttached.GetUnitAttached()._Player; }
            if (_WeaponAttached.GetTowerAttached() != null) { player = _WeaponAttached.GetTowerAttached()._Player; }
            if (player != null) { player.CameraRTS.ExplosionShake(transform.position, ExplosionRadius); }
        }

        // Set the projectile's impact effect to match the weapon's impact effect (if set in the weapon)
        if (_WeaponAttached != null) {

            if (_WeaponAttached.DefaultImpactEffect != null) { ImpactEffect = _WeaponAttached.DefaultImpactEffect; }
        }

        // Play impact effect
        if (ImpactEffect != null) {

            // Play
            ParticleSystem effect = ObjectPooling.Spawn(ImpactEffect, transform.position, transform.rotation).GetComponentInChildren<ParticleSystem>();
            effect.Play();
        }

        // Play OnDeath effect (this is for when you dont want to use explosion damage properties, but have the effect play still)
        if (ExplosionEffect != null && !ExplodeOnImpact) {

            // Play
            ParticleSystem effect = ObjectPooling.Spawn(ExplosionEffect.gameObject, transform.position, transform.rotation).GetComponent<ParticleSystem>();
            effect.Play();
        }

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
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  When the trigger overlaps another collider (entry only)
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other) {

        // Check for terrain collision
        if (other.gameObject.CompareTag("Ground")) { OnDestroy(); }

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
                            Unit unitA = _WeaponAttached.GetUnitAttached();

                            // Damage based on unit type
                            switch (unitObj.UnitType) {
                                
                                case Unit.EUnitType.Undefined:          { unitObj.Damage(((_Damages.DamageDefault * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.DwfSoldier:         { unitObj.Damage(((_Damages.DamageCoreInfantry * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.DwfSpecialistInfantry: { unitObj.Damage(((_Damages.DamageAntiInfantryMarine * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.DwfSpecialistVehicle:               { unitObj.Damage(((_Damages.DamageHero * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.Grumblebuster:        { unitObj.Damage(((_Damages.DamageCoreVehicle * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.Skylancer:     { unitObj.Damage(((_Damages.DamageAntiAirVehicle * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.Catapult:    { unitObj.Damage(((_Damages.DamageMobileArtillery * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.SiegeEngine:         { unitObj.Damage(((_Damages.DamageBattleTank * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.LightAirship:        { unitObj.Damage(((_Damages.DamageCoreAirship * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.SupportShip:        { unitObj.Damage(((_Damages.DamageSupportShip * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(((_Damages.DamageHeavyAirship * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
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

                                case Unit.EUnitType.Undefined:              { unitObj.Damage((_Damages.DamageDefault * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.DwfSoldier:             { unitObj.Damage((_Damages.DamageCoreInfantry * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.DwfSpecialistInfantry:  { unitObj.Damage((_Damages.DamageAntiInfantryMarine * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.DwfSpecialistVehicle:   { unitObj.Damage((_Damages.DamageHero * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.Grumblebuster:          { unitObj.Damage((_Damages.DamageCoreVehicle * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.Skylancer:              { unitObj.Damage((_Damages.DamageAntiAirVehicle * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.Catapult:               { unitObj.Damage((_Damages.DamageMobileArtillery * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.SiegeEngine:            { unitObj.Damage((_Damages.DamageBattleTank * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.LightAirship:           { unitObj.Damage((_Damages.DamageCoreAirship * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.SupportShip:            { unitObj.Damage((_Damages.DamageSupportShip * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
                                case Unit.EUnitType.HeavyAirship:           { unitObj.Damage((_Damages.DamageHeavyAirship * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _WeaponAttached.GetTowerAttached()); break; }
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
                worldObj.Damage((_Damages.DamageDefault * wm.GetWaveDamageModifier(worldObj)) * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod));
                OnDestroy();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetonatorTimer() {

        // Delay
        float normalizedTime = 0f;
        while (normalizedTime <= 1f) {

            normalizedTime += Time.deltaTime / TimeTillDetonation;
            yield return null;
        }

        // Detonate
        OnDestroy();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}