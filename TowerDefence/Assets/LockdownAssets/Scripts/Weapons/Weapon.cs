using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 31/8/2018
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
    public int ProjectilesPerShot = 1;
    public float FiringDelay = 0.5f;
    [Space]
    public EOffsetType AngularOffsetType = EOffsetType.Alternate;
    public Vector3 AngularOffset = Vector3.zero;
    [Space]
    public EMuzzleFiringPatternType MuzzlePatternType = EMuzzleFiringPatternType.Consective;
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
    public enum EOffsetType { Alternate, Consecutive, Random }
    public enum EMuzzleFiringPatternType { Consective, Random }

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
    private bool _IsFiring = false;
    private float _FireDelayTimer = 0f;
    private bool _IsReloading = false;
    private float _ReloadTimer = 0f;
    private Unit _UnitAttached = null;
    private Tower _TowerAttached = null;

    private float _CurrentOffsetMultiplier = 1f;

    private int _MuzzleIterator = 0;
    private List<Transform> _MuzzleLaunchPoints = null;
    private List<int> _UnusedLaunchPoints = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called before Start().
    /// </summary>
    protected void Awake() {


        // Create lists
        _MuzzleLaunchPoints = new List<Transform>();
        _UnusedLaunchPoints = new List<int>();
    }

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
        if (_FireDelayTimer > 0f) {

            ///_IsFiring = false;
            _FireDelayTimer -= Time.deltaTime;
        }

        // Bottomless clip setup
        if (BottomlessClip) {

            _ReloadTimer = 0;
            _IsReloading = false;
        }

        // Reloading timer
        if (_ReloadTimer > 0f) { _ReloadTimer -= Time.deltaTime; }
        else { _IsReloading = false; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Fires a 3D projectile object.
    /// </summary>
    private void ProjectileObject() {

        if (ProjectileClass) {

            // Create projectile facing forward * offset from the muzzle 
            Projectile proj = ObjectPooling.Spawn(ProjectileClass.gameObject).GetComponent<Projectile>();
            Quaternion rot = Quaternion.identity;
            
            // Determine muzzle firing pattern & projectile starting position
            switch (MuzzlePatternType) {

                // Consecutive pattern
                case EMuzzleFiringPatternType.Consective: {
                                            
                    // Use the first point in the list
                    _MuzzleIterator = _UnusedLaunchPoints[0];
                    _UnusedLaunchPoints.RemoveAt(0);
                    ///_MuzzleIterator++;
                    break;
                }

                // Random pattern
                case EMuzzleFiringPatternType.Random: {

                    // Get an unused launch point
                    int i = UnityEngine.Random.Range(0, _UnusedLaunchPoints.Count - 1);
                    _MuzzleIterator = _UnusedLaunchPoints[i];

                    // Remove launch point from availibility
                    _UnusedLaunchPoints.RemoveAt(i);
                    break;
                }

                default: break;
            }

            proj.transform.position = _MuzzleLaunchPoints[_MuzzleIterator].position;
            rot = _MuzzleLaunchPoints[_MuzzleIterator].rotation;
            
            // Apply offset pattern
            switch (AngularOffsetType) {

                case EOffsetType.Alternate: {

                    // Apply alternating pattern
                    _CurrentOffsetMultiplier *= -1;

                    AngularOffset = AngularOffset * _CurrentOffsetMultiplier;
                    break;
                }

                case EOffsetType.Consecutive: {

                    // Apply alternating pattern
                    _CurrentOffsetMultiplier *= -1;

                    AngularOffset = AngularOffset * _CurrentOffsetMultiplier;
                    break;
                }

                case EOffsetType.Random: {

                    // Random offset
                    int i = UnityEngine.Random.Range(-1, 1);
                    if (i == 0) { i = 1; }
                    AngularOffset = AngularOffset * i;
                    break;
                }

                default: break;
            }
            
            // Set rotation by offset
            rot.eulerAngles += AngularOffset;
            proj.transform.rotation = rot;

            // Start the projectile
            proj.Init(this);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Fires a hitscan based projectile.
    /// </summary>
    private void ProjectileRaycast() {
        /*
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
                            DifficultyManager dM = DifficultyManager.Instance;
                            DifficultyManager.EDifficultyModifiers mod = DifficultyManager.EDifficultyModifiers.Damage;
                            switch (unitObj.UnitType) {

                                case Unit.EUnitType.Undefined:          { unitObj.Damage(Damages.DamageDefault * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.CoreMarine:         { unitObj.Damage(Damages.DamageCoreInfantry * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(Damages.DamageAntiInfantryMarine * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.Hero:               { unitObj.Damage(Damages.DamageHero * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(Damages.DamageCoreVehicle * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(Damages.DamageAntiAirVehicle * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(Damages.DamageMobileArtillery * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.BattleTank:         { unitObj.Damage(Damages.DamageBattleTank * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.CoreAirship:        { unitObj.Damage(Damages.DamageCoreAirship * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.SupportShip:        { unitObj.Damage(Damages.DamageSupportShip * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(Damages.DamageHeavyAirship * dM.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                default: break;
                            }
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObj.Damage(Damages.DamageDefault); }
                }
            }
        }*/
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Fires a particle effect that uses particle collision to invoke damage.
    /// </summary>
    private void ProjectileParticle() {
        
        // Play firing effect
        if (FiringEffect != null) {
            
            // Spawn
            ParticleSystem effect = ObjectPooling.Spawn(FiringEffect.gameObject).GetComponent<ParticleSystem>();
            effect.GetComponent<ParticleBasedDamage>().SetWeaponAttached(this);
            
            // Determine muzzle firing pattern & projectile starting position
            switch (MuzzlePatternType) {

                // Consecutive pattern
                case EMuzzleFiringPatternType.Consective: {

                    // Use the first point in the list
                    _MuzzleIterator = _UnusedLaunchPoints[0];

                    // Pop front of the unused list
                    _UnusedLaunchPoints.RemoveAt(0);
                    break;
                }

                // Random pattern
                case EMuzzleFiringPatternType.Random: {

                    // Get an unused launch point
                    int i = UnityEngine.Random.Range(0, _UnusedLaunchPoints.Count - 1);
                    _MuzzleIterator = _UnusedLaunchPoints[i];

                    // Remove launch point from availibility
                    _UnusedLaunchPoints.RemoveAt(i);
                    break;
                }

                default: break;
            }

            effect.transform.position = _MuzzleLaunchPoints[_MuzzleIterator].position;
            effect.transform.rotation = _MuzzleLaunchPoints[_MuzzleIterator].rotation;

            // Play
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
        
            _IsFiring = true;
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

                // Spawn
                ParticleSystem effect = ObjectPooling.Spawn(MuzzleEffect.gameObject).GetComponent<ParticleSystem>();
                
                // Muzzle iterator should be set already coz the firing weapon mechanism has already been set this frame
                // (in the switch statement just above)
                effect.transform.position = _MuzzleLaunchPoints[_MuzzleIterator].position;
                effect.transform.rotation = _MuzzleLaunchPoints[_MuzzleIterator].rotation;

                // Play
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
            
            _IsFiring = false;
            _ReloadTimer = ReloadTime;
            _IsReloading = true;
            _CurrentMagazineCount = MagazineSize;

            // Make all muzzle points available again
            _UnusedLaunchPoints.Clear();
            for (int i = 0; i < _MuzzleLaunchPoints.Count; i++) { _UnusedLaunchPoints.Add(i); }
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
    //  Sets references to the unit that this weapon is associated with &
    //  updates references to the muzzle launch point transforms for the weapon's firing mechanism.
    /// </summary>
    /// <param name="unit"></param>
    public void SetUnitAttached(Unit unit) {

        _UnitAttached = unit;

        // Get muzzle launch point transforms for the weapon
        _MuzzleLaunchPoints.Clear();
        for (int i = 0; i < unit.MuzzleLaunchPoints.Count; i++) { _MuzzleLaunchPoints.Add(unit.MuzzleLaunchPoints[i].transform); }

        // Make all muzzle points again
        _UnusedLaunchPoints.Clear();
        for (int i = 0; i < _MuzzleLaunchPoints.Count; i++) { _UnusedLaunchPoints.Add(i); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public Unit GetUnitAttached() { return _UnitAttached; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets references to the tower building that this weapon is associated with &
    //  updates references to the muzzle launch point transforms for the weapon's firing mechanism.
    /// </summary>
    /// <param name="unit"></param>
    public void SetTowerAttached(Tower tower) {

        _TowerAttached = tower;

        // Get muzzle launch point transforms for the weapon
        _MuzzleLaunchPoints.Clear();
        for (int i = 0; i < tower.MuzzleLaunchPoints.Count; i++) { _MuzzleLaunchPoints.Add(tower.MuzzleLaunchPoints[i].transform); }

        // Make all muzzle points again
        _UnusedLaunchPoints.Clear();
        for (int i = 0; i < _MuzzleLaunchPoints.Count; i++) { _UnusedLaunchPoints.Add(i); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public Tower GetTowerAttached() { return _TowerAttached; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public bool IsReloading() { return _IsReloading; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public bool IsFiring() { return _IsFiring; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns TRUE if the weapon's firing delay timer is complete
    //  and the weapon isn't currently reloading.
    /// </summary>
    /// <returns></returns>
    public bool CanFire() { return _FireDelayTimer <= 0 && !_IsReloading; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="value"></param>
    public void SetReload(bool value) {

        if (value) { Reload(); }
        else {

            _ReloadTimer = 0;
            _IsReloading = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}