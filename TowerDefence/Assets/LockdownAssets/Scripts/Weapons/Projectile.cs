using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 22/6/2018
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
    public float _MovementSpeed = 40f;
    public float _MaxDistance = 1000f;
    public bool _AffectedByGravity = false;
    public float _GravityStrength = 1f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" DAMAGE")]
    [Space]
    public int DamageDefault = 1;
    public int DamageCoreInfantry = 1;
    public int DamageAntiInfantryMarine = 1;
    public int DamageHero = 1;
    public int DamageCoreVehicle = 1;
    public int DamageAntiAirVehicle = 1;
    public int DamageMobileArtillery = 1;
    public int DamageBattleTank = 1;
    public int DamageCoreAirship = 1;
    public int DamageSupportShip = 1;
    public int DamageHeavyAirship = 1;

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

        // This should already be called when it is pulled from its object pool,
        // but this is just incase it somehow isn't
        gameObject.SetActive(true);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update () {
        
        // Constantly move forward
        transform.position += _Velocity * _MovementSpeed * Time.deltaTime;

        // Apply downward force if affected by gravity
        if (_AffectedByGravity) { transform.position -= _Downwards * _GravityStrength *Time.deltaTime; }

        // Re-pool projectile when it has reached max distance threshold
        if (_DistanceTravelled < _MaxDistance) _DistanceTravelled = Vector3.Distance(_OriginPosition, transform.position);
        else { OnDestroy(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the projectile is destroyed, this is 
    //  usually called when the projectile collides with something
    /// </summary>
    protected void OnDestroy() {

        if (ExplodeOnImpact) {

            // Use a spherical raycast for an AOE damage with falloff
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, ExplosionRadius, transform.forward, 0f);
            foreach (var rayHit in hits) {

                WorldObject worldObj = rayHit.transform.gameObject.GetComponentInParent<WorldObject>();
                if (worldObj != null) {

                    // Friendly fire is OFF
                    if (worldObj.Team != _WeaponAttached.GetUnitAttached().Team) {

                        // Determine damage falloff
                        float distanceFromEpicenter = Vector3.Distance(transform.position, rayHit.point);
                        float damageMultiplier = 1 - (distanceFromEpicenter / ExplosionRadius);

                        // Damage the object
                        worldObj.Damage(Mathf.FloorToInt(DamageDefault * damageMultiplier));
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
        ObjectPooling.Despawn(this.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  When this collider/rigid body enters another collider/rigidbody 
    //  (there must be at least 1 rigidbody in the collision for this to work)
    /// </summary>
    /// <param name="collision"></param>
    protected void OnCollisionEnter(Collision collision) {

        // Get object type
        GameObject gameObj = collision.gameObject;
        WorldObject worldObj = gameObj.GetComponent<WorldObject>();

        // Successful WorldObject cast
        if (worldObj != null) {

            // Check if object is of type unit
            Unit unitObj = worldObj.GetComponent<Unit>();
            if (unitObj != null) {

                if (_WeaponAttached != null) {

                    // Friendly fire is OFF
                    if (unitObj.Team != _WeaponAttached.GetUnitAttached().Team) {

                        // Damage based on unit type
                        switch (unitObj.UnitType) {

                            case Unit.EUnitType.Undefined:          { unitObj.Damage(DamageDefault); break; }
                            case Unit.EUnitType.CoreMarine:         { unitObj.Damage(DamageCoreInfantry); break; }
                            case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(DamageAntiInfantryMarine); break; }
                            case Unit.EUnitType.Hero:               { unitObj.Damage(DamageHero); break; }
                            case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(DamageCoreVehicle); break; }
                            case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(DamageAntiAirVehicle); break; }
                            case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(DamageMobileArtillery); break; }
                            case Unit.EUnitType.BattleTank:         { unitObj.Damage(DamageBattleTank); break; }
                            case Unit.EUnitType.CoreAirship:        { unitObj.Damage(DamageCoreAirship); break; }
                            case Unit.EUnitType.SupportShip:        { unitObj.Damage(DamageSupportShip); break; }
                            case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(DamageHeavyAirship); break; }
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
                worldObj.Damage(DamageDefault);
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