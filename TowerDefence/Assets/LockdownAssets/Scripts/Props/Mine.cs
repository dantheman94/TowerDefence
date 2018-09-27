using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 27/9/2018
//
//******************************

public class Mine : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MINE PROPERTIES")]
    [Space]
    public float ExplosionRadius = 20f;
    public float DamageFalloff = 0.5f;
    public ParticleSystem ExplosionEffect = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" DAMAGES")]
    [Space]
    public Weapon.ObjectDamages Damages;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private GameManager.Team _Team = GameManager.Team.Undefined;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        
        // Only units are damagable by this object
        if (other.CompareTag("Unit")) {

            Unit unit = other.GetComponent<Unit>();
            if (unit != null) {

                // Enemy units will 'trip' the mine
                if (unit.Team != _Team) { DetonateMine(); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Detonates the mine and damages any enemies within the blast radius.
    /// </summary>
    public void DetonateMine() {

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
                
                // Friendly fire is OFF
                if (worldObj.Team != _Team) {

                    // Determine damage falloff
                    float distanceFromEpicenter = Vector3.Distance(transform.position, rayHit.point);
                    float damageMultiplier = (distanceFromEpicenter / ExplosionRadius) * DamageFalloff;

                    // Damage the object
                    worldObj.Damage(Mathf.FloorToInt(Damages.DamageDefault * damageMultiplier));
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

    /// <summary>
    //  Sets the team for this mine (so that friendlies cannot be damaged by it).
    /// </summary>
    /// <param name="team"></param>
    public void SetTeam(GameManager.Team team) { _Team = team; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
