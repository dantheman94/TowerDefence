using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 17/10/2018
//
//******************************

public class AntiInfantryMarine : Humanoid {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" INFANTRY SPECIALIST DEATH PROPERTIES")]
    [Space]
    [Range(0f, 1f)]
    public float ExplodeOnDeathChance = 0.5f;
    [Space]
    public ParticleSystem CannisterLeakStencil = null;
    public Transform CannisterLeakTransform = null;
    public ParticleSystem ExplosionEffectStencil = null;
    public Transform ExplosionEffectTransform = null;
    [Space]
    public float CannisterLeakTime = 2f;
    public bool ExplosionCanDamageUnits = false;
    public float ExplosionRadius = 20f;
    public float DamageFalloff = 0.5f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private ParticleSystem _CannisterLeakEffect = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is killed/destroyed
    /// </summary>
    public override void OnDeath(WorldObject instigator) {

        // Determine whether the character should explode or not
        float xplode = Random.Range(0f, 1f);
        if (xplode <= ExplodeOnDeathChance) {

            _Agent.enabled = false;

            // Cannister leak then explode
            _CannisterLeakEffect = ObjectPooling.Spawn(CannisterLeakStencil.gameObject, CannisterLeakTransform.position, CannisterLeakTransform.rotation).GetComponent<ParticleSystem>();
            _CannisterLeakEffect.Play();

            StartCoroutine(DelayedExplosion());

            // If were in the wave manager's enemies array - remove it
            if (WaveManager.Instance.GetCurrentWaveEnemies().Contains(this)) { WaveManager.Instance.GetCurrentWaveEnemies().Remove(this); }
            if (Team == GameManager.Team.Attacking) { GameManager.Instance.WaveStatsHUD.DeductLifeFromCurrentPopulation(); }

            // Remove from player's population counter
            else if (Team == GameManager.Team.Defending) { _Player.RemoveFromArmy(this); }

            // Destroy waypoint
            if (_SeekWaypoint != null) { ObjectPooling.Despawn(_SeekWaypoint.gameObject); }

            // Add xp to the instigator (if valid)
            if (instigator != null) {

                // Only units can gain XP from kills
                Unit unit = instigator.GetComponent<Unit>();
                if (unit != null) { unit.AddVeterancyXP(XPGrantedOnDeath); }
            }

            // Force deselect
            SetIsSelected(false);

            // Despawn any damaged threshold particles in play
            for (int i = 0; i < _DamagedParticles.Count; i++) {

                ParticleSystem effect = _DamagedParticles[i];
                effect.Stop(true);
                ObjectPooling.Despawn(effect.gameObject);
            }
            if (_DamagedParticles.Count > 0) { _DamagedParticles.Clear(); }

            // Set object state
            _ObjectState = WorldObjectStates.Destroyed;

            // Destroy healthbar
            if (_HealthBar != null) {

                // Null the bitch
                _HealthBar.SetObjectAttached(null);
                ObjectPooling.Despawn(_HealthBar.gameObject);
            }

            // Clamping health
            _HitPoints = 0;
            _Health = 0f;

            // Camera shake if neccessary
            if (_Player == null) { _Player = GameManager.Instance.Players[0]; }
            if (CameraShakeOnDeath && _Player != null) { _Player.CameraRTS.ExplosionShake(transform.position, DeathExplosionRadius); }

            // Send message to match feed
            if (Team == GameManager.Team.Defending) {

                MatchFeed.Instance.AddMessage(string.Concat(ObjectName, " has been killed."));
                _Player.AddUnitsLost();
            }

            // Add to enemies killed stats
            else if (Team == GameManager.Team.Attacking) { _Player.AddUnitsKilled(); }
        }

        // Die normally
        else { base.OnDeath(instigator); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator DelayedExplosion() {

        // Delay
        float normalizedTime = 0f;
        while (normalizedTime <= 1f) {

            normalizedTime += Time.deltaTime / CannisterLeakTime;
            yield return null;
        }

        // Stop leaking & explode
        if (_CannisterLeakEffect != null) { _CannisterLeakEffect.Stop(); }
        ExplosionEffectStencil = ObjectPooling.Spawn(ExplosionEffectStencil.gameObject, ExplosionEffectTransform.position, ExplosionEffectTransform.rotation).GetComponent<ParticleSystem>();
        ExplosionEffectStencil.Play();

        // Camera shake
        if (_Player == null) { _Player = GameManager.Instance.Players[0]; }
        if (_Player != null) { _Player.CameraRTS.ExplosionShake(ExplosionEffectTransform.position, ExplosionRadius); }

        // Hide/despawn the unit
        _ObjectState = WorldObjectStates.Default;
        ObjectPooling.Despawn(gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
