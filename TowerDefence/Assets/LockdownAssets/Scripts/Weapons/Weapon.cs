using System;
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
    public float FiringDelay = 0.5f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" MUZZLE EFFECTS")]
    [Space]
    public ParticleSystem MuzzleEffect = null;
    public EMuzzleFiringPatternType MuzzlePatternType = EMuzzleFiringPatternType.Consective;
    public bool ReusableMuzzlePoints = true;

    [Space]
    [Header("-----------------------------------")]
    [Header(" CHARGE UP PROPERTIES")]
    [Space]
    public float ChargeUpTime = 0f;
    public float ChargeLightRange = 50f;
    public float ChargeLightIntensity = 1f;
    public Color ChargeUpColor = Color.white;

    [Space]
    [Header("-----------------------------------")]
    [Header(" ANGULAR OFFSET")]
    [Space]
    public EOffsetType AngularOffsetType = EOffsetType.Alternate;
    public Vector3 AngularOffset = Vector3.zero;

    [Space]
    [Header("-----------------------------------")]
    [Header(" IMPACT DAMAGES")]
    [Space]
    public GameObject SpawnOnImpact = null;
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
    private bool _IsChargingUp = false;
    private Unit _UnitAttached = null;
    private Tower _TowerAttached = null;
    private VehicleGunner _GunnerAttached = null;

    private float _CurrentOffsetMultiplier = 1f;
    
    private int _MuzzleIterator = 0;
    private List<Transform> _MuzzleLaunchPoints = null;
    private List<int> _UnusedLaunchPoints = null;

    private Light _ChargeLight = null;
    private float _ChargeLerptime = 0f;
    
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

        // Initialize charge up light
        if (ChargeUpTime > 0) {

            _ChargeLight = gameObject.AddComponent<Light>();
            _ChargeLight.type = LightType.Point;
            _ChargeLight.range = 0f;
            _ChargeLight.intensity = ChargeLightIntensity;
            _ChargeLight.color = ChargeUpColor;
            _ChargeLight.enabled = false;
        }
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
    //  Sets the position of the charge up light (which is attached to this gameobject) to the first iterator
    //  in the attached object's muzzle launch vector list.
    /// </summary>
    private void UpdateChargeLightPosition() {

        // Attached to a unit
        if (_UnitAttached != null) {

            // position should be at the unit's muzzle launch vector
            transform.position = _UnitAttached.MuzzleLaunchPoints[0].transform.position;
        }

        // Attached to a tower
        if (_TowerAttached != null) {

            // position should be at the tower's muzzle launch vector
            transform.position = _TowerAttached.MuzzleLaunchPoints[0].transform.position;
        }

        // Attached to a gunner seat
        if (_GunnerAttached != null) {

            // position should be at the gunner's muzzle launch vector
            transform.position = _GunnerAttached.MuzzleLaunchPoints[0].transform.position;
        }
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

                    // Remove launch point from availibility
                    if (!ReusableMuzzlePoints) { _UnusedLaunchPoints.RemoveAt(0); }
                    break;
                }

                // Random pattern
                case EMuzzleFiringPatternType.Random: {

                    // Get an unused launch point
                    int i = UnityEngine.Random.Range(0, _UnusedLaunchPoints.Count);
                    _MuzzleIterator = _UnusedLaunchPoints[i];

                    // Remove launch point from availibility
                    if (!ReusableMuzzlePoints) { _UnusedLaunchPoints.RemoveAt(i); }
                    break;
                }

                default: break;
            }

            proj.transform.position = _MuzzleLaunchPoints[_MuzzleIterator].position;
            rot = _MuzzleLaunchPoints[_MuzzleIterator].rotation;

            // Apply offset pattern
            if (!ProjectileClass.AffectedByGravity) {

                switch (AngularOffsetType) {

                    // Apply alternating pattern
                    case EOffsetType.Alternate: {

                            AngularOffset = AngularOffset * -1;
                            break;
                        }

                    // Apply consecutive pattern
                    case EOffsetType.Consecutive: {

                            _CurrentOffsetMultiplier *= -1;

                            AngularOffset = AngularOffset * _CurrentOffsetMultiplier;
                            break;
                        }

                    // Random offset
                    case EOffsetType.Random: {

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

            // Projectile is affected by gravity (arc formation)
            else {

                if (_UnitAttached != null) {
                    
                    // Start the projectile
                    proj.Init(this);
                    proj.transform.rotation = rot;
                    proj.GetComponent<ParabolicArc>().Init(_UnitAttached.GetAttackTarget().transform);
                }

                if (_TowerAttached != null) {

                    // Determine arc velocity
                    ///Vector3 velocity = Projectile.DetermineArcVelocity(proj.transform.position, _UnitAttached.GetAttackTarget().transform.position, proj.MovementSpeed);

                    // Start the projectile
                    proj.Init(this);
                    proj.transform.rotation = rot;
                    proj.GetComponent<ParabolicArc>().Init(_TowerAttached.GetAttackTarget().transform);
                    ///proj.SetVelocity(velocity);
                }
            }            
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Fires a hitscan based projectile.
    /// </summary>
    private void ProjectileRaycast() {
        
        // Weapon is attached to a unit
        if (_UnitAttached != null) { 

            RaycastHit hit;
            Vector3 attackPos = _UnitAttached.GetAttackTarget().transform.position;
            attackPos.y = attackPos.y + _UnitAttached.GetAttackTarget().GetObjectHeight() / 2;
            Vector3 attackDir = attackPos - _UnitAttached.MuzzleLaunchPoints[0].transform.position;
            attackDir.Normalize();

            if (Physics.Raycast(_UnitAttached.MuzzleLaunchPoints[0].transform.position, attackDir, out hit, _UnitAttached.MaxAttackingRange)) {

                Debug.DrawLine(_UnitAttached.MuzzleLaunchPoints[0].transform.position, hit.point, Color.red);

                // Damage target
                WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                else if (worldObj != null) {

                    DifficultyManager dm = DifficultyManager.Instance;
                    DifficultyManager.EDifficultyModifiers mod = DifficultyManager.EDifficultyModifiers.Damage;
                    WaveManager wm = WaveManager.Instance;

                    // Check if object is of type unit
                    Unit unitObj = worldObj.GetComponent<Unit>();
                    if (unitObj != null) {

                        // Cannot damage self
                        if (unitObj.Team != _UnitAttached.Team) {

                            // Damage based on unit type
                            switch (unitObj.UnitType) {

                                case Unit.EUnitType.Undefined:          { unitObj.Damage(((Damages.DamageDefault * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.CoreMarine:         { unitObj.Damage(((Damages.DamageCoreInfantry * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(((Damages.DamageAntiInfantryMarine * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.Hero:               { unitObj.Damage(((Damages.DamageHero * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(((Damages.DamageCoreVehicle * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(((Damages.DamageAntiAirVehicle * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(((Damages.DamageMobileArtillery * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.BattleTank:         { unitObj.Damage(((Damages.DamageBattleTank * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.CoreAirship:        { unitObj.Damage(((Damages.DamageCoreAirship * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.SupportShip:        { unitObj.Damage(((Damages.DamageSupportShip * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(((Damages.DamageHeavyAirship * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                default: break;
                            }
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObj.Damage((Damages.DamageDefault * wm.GetWaveDamageModifier(worldObj)) * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod), _UnitAttached); }
                }
            }
        }

        // Weapon is attached to a tower
        if (_TowerAttached != null) {

            RaycastHit hit;
            Vector3 attackPos = _TowerAttached.GetAttackTarget().transform.position;
            attackPos.y = attackPos.y + _TowerAttached.GetAttackTarget().GetObjectHeight() / 2;
            Vector3 attackDir = attackPos - _TowerAttached.MuzzleLaunchPoints[0].transform.position;
            attackDir.Normalize();

            if (Physics.Raycast(_TowerAttached.MuzzleLaunchPoints[0].transform.position, attackDir, out hit, _TowerAttached.MaxAttackingRange)) {

                Debug.DrawLine(_TowerAttached.MuzzleLaunchPoints[0].transform.position, hit.point, Color.red);

                // Damage target
                WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                else if (worldObj != null) {

                    DifficultyManager dm = DifficultyManager.Instance;
                    DifficultyManager.EDifficultyModifiers mod = DifficultyManager.EDifficultyModifiers.Damage;
                    WaveManager wm = WaveManager.Instance;

                    // Check if object is of type unit
                    Unit unitObj = worldObj.GetComponent<Unit>();
                    if (unitObj != null) {

                        // Cannot damage self
                        if (unitObj.Team != _TowerAttached.Team) {

                            // Damage based on unit type
                            switch (unitObj.UnitType) {

                                case Unit.EUnitType.Undefined:          { unitObj.Damage((Damages.DamageDefault * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.CoreMarine:         { unitObj.Damage((Damages.DamageCoreInfantry * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage((Damages.DamageAntiInfantryMarine * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.Hero:               { unitObj.Damage((Damages.DamageHero * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.CoreVehicle:        { unitObj.Damage((Damages.DamageCoreVehicle * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage((Damages.DamageAntiAirVehicle * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.MobileArtillery:    { unitObj.Damage((Damages.DamageMobileArtillery * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.BattleTank:         { unitObj.Damage((Damages.DamageBattleTank * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.CoreAirship:        { unitObj.Damage((Damages.DamageCoreAirship * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.SupportShip:        { unitObj.Damage((Damages.DamageSupportShip * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.HeavyAirship:       { unitObj.Damage((Damages.DamageHeavyAirship * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                default: break;
                            }
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObj.Damage((Damages.DamageDefault * wm.GetWaveDamageModifier(worldObj)) * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod), _TowerAttached); }
                }
            }
        }

        // Weapon is attached to a gunner seat
        if (_GunnerAttached != null) {

            RaycastHit hit;
            Vector3 attackPos = _GunnerAttached.GetAttackTarget().transform.position;
            attackPos.y = attackPos.y + _GunnerAttached.GetAttackTarget().GetObjectHeight() / 2;
            Vector3 attackDir = attackPos - _GunnerAttached.MuzzleLaunchPoints[0].transform.position;
            attackDir.Normalize();

            if (Physics.Raycast(_GunnerAttached.MuzzleLaunchPoints[0].transform.position, attackDir, out hit, _GunnerAttached.MaxAttackRange)) {

                Debug.DrawLine(_GunnerAttached.MuzzleLaunchPoints[0].transform.position, hit.point, Color.red);

                // Damage target
                WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                else if (worldObj != null) {

                    DifficultyManager dm = DifficultyManager.Instance;
                    DifficultyManager.EDifficultyModifiers mod = DifficultyManager.EDifficultyModifiers.Damage;
                    WaveManager wm = WaveManager.Instance;

                    // Check if object is of type unit
                    Unit unitObj = worldObj.GetComponent<Unit>();
                    if (unitObj != null) {

                        // Cannot damage self
                        if (unitObj.Team != _GunnerAttached._VehicleAttached.Team) {
                            Vehicle vehicleA = _GunnerAttached._VehicleAttached;

                            // Damage based on unit type
                            switch (unitObj.UnitType) {

                                case Unit.EUnitType.Undefined:          { unitObj.Damage(((Damages.DamageDefault * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.CoreMarine:         { unitObj.Damage(((Damages.DamageCoreInfantry * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(((Damages.DamageAntiInfantryMarine * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.Hero:               { unitObj.Damage(((Damages.DamageHero * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(((Damages.DamageCoreVehicle * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(((Damages.DamageAntiAirVehicle * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(((Damages.DamageMobileArtillery * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.BattleTank:         { unitObj.Damage(((Damages.DamageBattleTank * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.CoreAirship:        { unitObj.Damage(((Damages.DamageCoreAirship * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.SupportShip:        { unitObj.Damage(((Damages.DamageSupportShip * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(((Damages.DamageHeavyAirship * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                default: break;
                            }
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObj.Damage((Damages.DamageDefault * wm.GetWaveDamageModifier(worldObj)) * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod), _GunnerAttached._VehicleAttached); }
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
            
            // Spawn
            ParticleSystem effect = ObjectPooling.Spawn(FiringEffect.gameObject).GetComponent<ParticleSystem>();
            effect.GetComponent<ParticleBasedDamage>().SetWeaponAttached(this);
            
            // Determine muzzle firing pattern & projectile starting position
            switch (MuzzlePatternType) {

                // Consecutive pattern
                case EMuzzleFiringPatternType.Consective: {

                    // Use the first point in the list
                    _MuzzleIterator = _UnusedLaunchPoints[0];

                    // Remove launch point from availibility
                    if (!ReusableMuzzlePoints) { _UnusedLaunchPoints.RemoveAt(0); }
                    break;
                }

                // Random pattern
                case EMuzzleFiringPatternType.Random: {

                    // Get an unused launch point
                    int i = UnityEngine.Random.Range(0, _UnusedLaunchPoints.Count - 1);
                    _MuzzleIterator = _UnusedLaunchPoints[i];

                    // Remove launch point from availibility
                    if (!ReusableMuzzlePoints) { _UnusedLaunchPoints.RemoveAt(i); }
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

            // Does the weapon need to be charged before firing?
            if (ChargeUpTime > 0) { StartCoroutine(ChargedFire()); }

            // Weapon does not need a chargeup, fire immediately
            else { ShootWeapon(); }
        }

        // Reloading if theres no ammo in the mag left
        else if (_CurrentMagazineCount <= 0) { Reload(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Waits the charge up time specified then fires the weapon.
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator ChargedFire() {

        _IsChargingUp = true;
        StartCoroutine(ChargeUpLight());

        yield return new WaitForSeconds(ChargeUpTime);

        ShootWeapon();

        // Reset light
        _ChargeLight.enabled = false;
        _ChargeLerptime = 0f;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Increases the light range over time (charge up time)
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator ChargeUpLight() {
        
        while (_ChargeLerptime < ChargeUpTime) {

            // Position the charge light at the correct position
            UpdateChargeLightPosition();

            // Make sure the light component is enabled
            _ChargeLight.enabled = true;

            // Increase light properties over multiple frames
            float percent = _ChargeLerptime / ChargeUpTime;
            _ChargeLight.range = Mathf.Lerp(0f, ChargeLightRange, percent);
            _ChargeLerptime += Time.deltaTime;
            
            yield return null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Determines the projectile type required for the weapon and fires its projectile.
    /// </summary>
    private void ShootWeapon() {

        _IsChargingUp = false;
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
    //  Sets references to the vehicle gunner that this weapon is associated with &
    //  updates references to the muzzle launch point transforms for the weapon's firing mechanism.
    /// </summary>
    /// <param name="unit"></param>
    public void SetGunnerAttached(VehicleGunner gunner) {

        _GunnerAttached = gunner;

        // Get muzzle launch point transforms for the weapon
        _MuzzleLaunchPoints.Clear();
        for (int i = 0; i < gunner.MuzzleLaunchPoints.Count; i++) { _MuzzleLaunchPoints.Add(gunner.MuzzleLaunchPoints[i].transform); }

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
    
}