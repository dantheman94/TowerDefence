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
    public bool TrackingProjectile = false;
    public float TrackingStrength = 1f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" MUZZLE EFFECTS")]
    [Space]
    public GameObject MuzzleEffectWrapper = null;
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
    [Header(" RAYCAST PROPERTIES")]
    [Space]
    public bool SpreadRaycasts = false;
    public int SpreadAmount = 1;
    public float SpreadAngle = 1f;
    [Space]
    public bool VisualizeHitscanTrace = false;
    public float HitScanDrawTime = 1f;
    [Space]
    public Color HitScanStartColour = Color.white;
    public float HitScanStartWidth = 1f;
    [Space]
    public Color HitScanEndColour = Color.white;
    public float HitScanEndWidth = 1f;
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" IMPACT DAMAGES")]
    [Space]
    public ParticleSystem DefaultImpactEffect = null;
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
                    
                    // Start the projectile
                    proj.Init(this);
                    proj.transform.rotation = rot;
                    proj.GetComponent<ParabolicArc>().Init(_TowerAttached.GetAttackTarget().transform);
                }
            }            
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Fires a hitscan based projectile.
    /// </summary>
    private void ProjectileRaycastSpread() {

        // Weapon is attached to a unit
        if (_UnitAttached != null) {

            // The first projectile has already been shot so the next
            // iterator(s) fire off in an additive angle by the previous iterator
            for (int i = 0; i < SpreadAmount; i++) {

                RaycastHit hit;
                Vector3 attackPos = _UnitAttached.GetAttackTarget().transform.position;
                attackPos.y = attackPos.y + _UnitAttached.GetAttackTarget().GetObjectHeight() / 2;
                Vector3 attackDir = attackPos - _UnitAttached.MuzzleLaunchPoints[0].transform.position;
                attackDir.Normalize();
                attackDir = Quaternion.AngleAxis(SpreadAngle * i, Vector3.up) * attackDir;
                ///attackDir.Normalize();

                // Raycast hit something
                if (Physics.Raycast(_UnitAttached.MuzzleLaunchPoints[0].transform.position, attackDir, out hit, _UnitAttached.MaxAttackingRange)) {

                    // Render line
                    Debug.DrawLine(_UnitAttached.MuzzleLaunchPoints[0].transform.position, hit.point, Color.blue);
                    GenerateHitScanVisualizer(_UnitAttached.MuzzleLaunchPoints[0].transform.position, hit.point);

                    // Damage target
                    WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                    else if (worldObj != null) {

                        // Play impacted object's particle effect
                        Vector3 position = hit.point;
                        Quaternion rotation = Quaternion.FromToRotation(hit.point, _UnitAttached.MuzzleLaunchPoints[0].transform.position);
                        if (worldObj.ProjectileImpactEffect != null) {

                            ParticleSystem impact = ObjectPooling.Spawn(worldObj.ProjectileImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                            impact.Play();

                            // Despawn particle system once it has finished its cycle
                            float effectDuration = impact.duration + impact.startLifetime;
                            StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                        }

                        // Play default impact effect
                        else {

                            if (DefaultImpactEffect != null) {

                                ParticleSystem impact = ObjectPooling.Spawn(DefaultImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                                impact.Play();

                                // Despawn particle system once it has finished its cycle
                                float effectDuration = impact.duration + impact.startLifetime;
                                StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                            }
                        }

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

                                    case Unit.EUnitType.Undefined: { unitObj.Damage(((Damages.DamageDefault * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.DwfSoldier: { unitObj.Damage(((Damages.DamageCoreInfantry * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.DwfSpecialistInfantry: { unitObj.Damage(((Damages.DamageAntiInfantryMarine * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.DwfSpecialistVehicle: { unitObj.Damage(((Damages.DamageHero * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.Grumblebuster: { unitObj.Damage(((Damages.DamageCoreVehicle * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.Skylancer: { unitObj.Damage(((Damages.DamageAntiAirVehicle * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.Catapult: { unitObj.Damage(((Damages.DamageMobileArtillery * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.SiegeEngine: { unitObj.Damage(((Damages.DamageBattleTank * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.LightAirship: { unitObj.Damage(((Damages.DamageCoreAirship * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.SupportShip: { unitObj.Damage(((Damages.DamageSupportShip * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    case Unit.EUnitType.HeavyAirship: { unitObj.Damage(((Damages.DamageHeavyAirship * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                    default: break;
                                }
                            }
                        }

                        // Damage the object (its not a unit so use the default damage value)
                        else { worldObj.Damage((Damages.DamageDefault * wm.GetWaveDamageModifier(worldObj)) * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod), _UnitAttached); }
                    }
                }
                else {

                    Vector3 dir = _UnitAttached.MuzzleLaunchPoints[0].transform.forward;
                    dir = Quaternion.AngleAxis(SpreadAngle * i, Vector3.up) * dir;

                    // No raycast hit but still draw a hitscan visualizer
                    GenerateHitScanVisualizer(_UnitAttached.MuzzleLaunchPoints[0].transform.position, dir * 1000f);
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

                // Render line
                Debug.DrawLine(_TowerAttached.MuzzleLaunchPoints[0].transform.position, hit.point, Color.blue);
                GenerateHitScanVisualizer(_TowerAttached.MuzzleLaunchPoints[0].transform.position, hit.point);

                // Damage target
                WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                else if (worldObj != null) {

                    // Play impacted object's particle effect
                    Vector3 position = hit.point;
                    Quaternion rotation = Quaternion.FromToRotation(hit.point, _TowerAttached.MuzzleLaunchPoints[0].transform.position);
                    if (worldObj.ProjectileImpactEffect != null) {

                        ParticleSystem impact = ObjectPooling.Spawn(worldObj.ProjectileImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                        impact.Play();

                        // Despawn particle system once it has finished its cycle
                        float effectDuration = impact.duration + impact.startLifetime;
                        StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                    }

                    // Play default impact effect
                    else {

                        ParticleSystem impact = ObjectPooling.Spawn(DefaultImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                        impact.Play();

                        // Despawn particle system once it has finished its cycle
                        float effectDuration = impact.duration + impact.startLifetime;
                        StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                    }

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

                                case Unit.EUnitType.Undefined: { unitObj.Damage((Damages.DamageDefault * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.DwfSoldier: { unitObj.Damage((Damages.DamageCoreInfantry * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.DwfSpecialistInfantry: { unitObj.Damage((Damages.DamageAntiInfantryMarine * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.DwfSpecialistVehicle: { unitObj.Damage((Damages.DamageHero * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.Grumblebuster: { unitObj.Damage((Damages.DamageCoreVehicle * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.Skylancer: { unitObj.Damage((Damages.DamageAntiAirVehicle * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.Catapult: { unitObj.Damage((Damages.DamageMobileArtillery * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.SiegeEngine: { unitObj.Damage((Damages.DamageBattleTank * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.LightAirship: { unitObj.Damage((Damages.DamageCoreAirship * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.SupportShip: { unitObj.Damage((Damages.DamageSupportShip * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.HeavyAirship: { unitObj.Damage((Damages.DamageHeavyAirship * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                default: break;
                            }
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObj.Damage((Damages.DamageDefault * wm.GetWaveDamageModifier(worldObj)) * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod), _TowerAttached); }
                }
            }
            else {

                // No raycast hit but still draw a hitscan visualizer
                GenerateHitScanVisualizer(_TowerAttached.MuzzleLaunchPoints[0].transform.position, _TowerAttached.MuzzleLaunchPoints[0].transform.forward * 1000f);
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

                // Render line
                Debug.DrawLine(_GunnerAttached.MuzzleLaunchPoints[0].transform.position, hit.point, Color.blue);
                GenerateHitScanVisualizer(_GunnerAttached.MuzzleLaunchPoints[0].transform.position, hit.point);

                // Damage target
                WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                else if (worldObj != null) {

                    // Play impacted object's particle effect
                    Vector3 position = hit.point;
                    Quaternion rotation = Quaternion.FromToRotation(hit.point, _GunnerAttached.MuzzleLaunchPoints[0].transform.position);
                    if (worldObj.ProjectileImpactEffect != null) {

                        ParticleSystem impact = ObjectPooling.Spawn(worldObj.ProjectileImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                        impact.Play();

                        // Despawn particle system once it has finished its cycle
                        float effectDuration = impact.duration + impact.startLifetime;
                        StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                    }

                    // Play default impact effect
                    else {

                        ParticleSystem impact = ObjectPooling.Spawn(DefaultImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                        impact.Play();

                        // Despawn particle system once it has finished its cycle
                        float effectDuration = impact.duration + impact.startLifetime;
                        StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                    }

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

                                case Unit.EUnitType.Undefined: { unitObj.Damage(((Damages.DamageDefault * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.DwfSoldier: { unitObj.Damage(((Damages.DamageCoreInfantry * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.DwfSpecialistInfantry: { unitObj.Damage(((Damages.DamageAntiInfantryMarine * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.DwfSpecialistVehicle: { unitObj.Damage(((Damages.DamageHero * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.Grumblebuster: { unitObj.Damage(((Damages.DamageCoreVehicle * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.Skylancer: { unitObj.Damage(((Damages.DamageAntiAirVehicle * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.Catapult: { unitObj.Damage(((Damages.DamageMobileArtillery * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.SiegeEngine: { unitObj.Damage(((Damages.DamageBattleTank * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.LightAirship: { unitObj.Damage(((Damages.DamageCoreAirship * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.SupportShip: { unitObj.Damage(((Damages.DamageSupportShip * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.HeavyAirship: { unitObj.Damage(((Damages.DamageHeavyAirship * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                default: break;
                            }
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObj.Damage((Damages.DamageDefault * wm.GetWaveDamageModifier(worldObj)) * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod), _GunnerAttached._VehicleAttached); }
                }
            }
            else {

                // No raycast hit but still draw a hitscan visualizer
                GenerateHitScanVisualizer(_GunnerAttached.MuzzleLaunchPoints[0].transform.position, _GunnerAttached.MuzzleLaunchPoints[0].transform.forward * 1000f);
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

                // Render line
                Debug.DrawLine(_UnitAttached.MuzzleLaunchPoints[0].transform.position, hit.point, Color.blue);
                GenerateHitScanVisualizer(_UnitAttached.MuzzleLaunchPoints[0].transform.position, hit.point);

                // Damage target
                WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                else if (worldObj != null) {

                    // Play impacted object's particle effect
                    Vector3 position = hit.point;
                    Quaternion rotation = Quaternion.FromToRotation(hit.point, _UnitAttached.MuzzleLaunchPoints[0].transform.position);
                    if (worldObj.ProjectileImpactEffect != null) {

                        ParticleSystem impact = ObjectPooling.Spawn(worldObj.ProjectileImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                        impact.Play();

                        // Despawn particle system once it has finished its cycle
                        float effectDuration = impact.duration + impact.startLifetime;
                        StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                    }

                    // Play default impact effect
                    else {

                        // Validity check
                        if (DefaultImpactEffect != null) {

                            // Object pool the effect
                            ParticleSystem impact = ObjectPooling.Spawn(DefaultImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                            impact.Play();

                            // Despawn particle system once it has finished its cycle
                            float effectDuration = impact.duration + impact.startLifetime;
                            StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                        }
                    }

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
                                case Unit.EUnitType.DwfSoldier:         { unitObj.Damage(((Damages.DamageCoreInfantry * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.DwfSpecialistInfantry: { unitObj.Damage(((Damages.DamageAntiInfantryMarine * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.DwfSpecialistVehicle:               { unitObj.Damage(((Damages.DamageHero * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.Grumblebuster:        { unitObj.Damage(((Damages.DamageCoreVehicle * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.Skylancer:     { unitObj.Damage(((Damages.DamageAntiAirVehicle * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.Catapult:    { unitObj.Damage(((Damages.DamageMobileArtillery * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.SiegeEngine:         { unitObj.Damage(((Damages.DamageBattleTank * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
                                case Unit.EUnitType.LightAirship:        { unitObj.Damage(((Damages.DamageCoreAirship * _UnitAttached.VetDamages[_UnitAttached.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _UnitAttached); break; }
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
            else {

                // No raycast hit but still draw a hitscan visualizer
                GenerateHitScanVisualizer(_UnitAttached.MuzzleLaunchPoints[0].transform.position, _UnitAttached.MuzzleLaunchPoints[0].transform.forward * _UnitAttached.MaxAttackingRange);
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

                // Render line
                Debug.DrawLine(_TowerAttached.MuzzleLaunchPoints[0].transform.position, hit.point, Color.blue);
                GenerateHitScanVisualizer(_TowerAttached.MuzzleLaunchPoints[0].transform.position, hit.point);

                // Damage target
                WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                else if (worldObj != null) {

                    // Play impacted object's particle effect
                    Vector3 position = hit.point;
                    Quaternion rotation = Quaternion.FromToRotation(hit.point, _TowerAttached.MuzzleLaunchPoints[0].transform.position);
                    if (worldObj.ProjectileImpactEffect != null) {

                        ParticleSystem impact = ObjectPooling.Spawn(worldObj.ProjectileImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                        impact.Play();

                        // Despawn particle system once it has finished its cycle
                        float effectDuration = impact.duration + impact.startLifetime;
                        StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                    }

                    // Play default impact effect
                    else {

                        ParticleSystem impact = ObjectPooling.Spawn(DefaultImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                        impact.Play();

                        // Despawn particle system once it has finished its cycle
                        float effectDuration = impact.duration + impact.startLifetime;
                        StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                    }

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
                                case Unit.EUnitType.DwfSoldier:         { unitObj.Damage((Damages.DamageCoreInfantry * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.DwfSpecialistInfantry: { unitObj.Damage((Damages.DamageAntiInfantryMarine * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.DwfSpecialistVehicle:               { unitObj.Damage((Damages.DamageHero * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.Grumblebuster:        { unitObj.Damage((Damages.DamageCoreVehicle * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.Skylancer:     { unitObj.Damage((Damages.DamageAntiAirVehicle * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.Catapult:    { unitObj.Damage((Damages.DamageMobileArtillery * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.SiegeEngine:         { unitObj.Damage((Damages.DamageBattleTank * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
                                case Unit.EUnitType.LightAirship:        { unitObj.Damage((Damages.DamageCoreAirship * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), _TowerAttached); break; }
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
            else {

                // No raycast hit but still draw a hitscan visualizer
                GenerateHitScanVisualizer(_TowerAttached.MuzzleLaunchPoints[0].transform.position, _TowerAttached.MuzzleLaunchPoints[0].transform.forward * _TowerAttached.MaxAttackingRange);
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

                // Render line
                Debug.DrawLine(_GunnerAttached.MuzzleLaunchPoints[0].transform.position, hit.point, Color.blue);
                GenerateHitScanVisualizer(_GunnerAttached.MuzzleLaunchPoints[0].transform.position, hit.point);

                // Damage target
                WorldObject worldObj = hit.transform.GetComponentInParent<WorldObject>();

                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast")) { return; }

                else if (worldObj != null) {

                    // Play impacted object's particle effect
                    Vector3 position = hit.point;
                    Quaternion rotation = Quaternion.FromToRotation(hit.point, _GunnerAttached.MuzzleLaunchPoints[0].transform.position);
                    if (worldObj.ProjectileImpactEffect != null) {

                        ParticleSystem impact = ObjectPooling.Spawn(worldObj.ProjectileImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                        impact.Play();

                        // Despawn particle system once it has finished its cycle
                        float effectDuration = impact.duration + impact.startLifetime;
                        StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                    }

                    // Play default impact effect
                    else {

                        ParticleSystem impact = ObjectPooling.Spawn(DefaultImpactEffect.gameObject, position, rotation).GetComponent<ParticleSystem>();
                        impact.Play();

                        // Despawn particle system once it has finished its cycle
                        float effectDuration = impact.duration + impact.startLifetime;
                        StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(impact, effectDuration));
                    }

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

                                case Unit.EUnitType.Undefined: { unitObj.Damage(((Damages.DamageDefault * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.DwfSoldier: { unitObj.Damage(((Damages.DamageCoreInfantry * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.DwfSpecialistInfantry: { unitObj.Damage(((Damages.DamageAntiInfantryMarine * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.DwfSpecialistVehicle: { unitObj.Damage(((Damages.DamageHero * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.Grumblebuster: { unitObj.Damage(((Damages.DamageCoreVehicle * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.Skylancer: { unitObj.Damage(((Damages.DamageAntiAirVehicle * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.Catapult: { unitObj.Damage(((Damages.DamageMobileArtillery * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.SiegeEngine: { unitObj.Damage(((Damages.DamageBattleTank * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.LightAirship: { unitObj.Damage(((Damages.DamageCoreAirship * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.SupportShip: { unitObj.Damage(((Damages.DamageSupportShip * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                case Unit.EUnitType.HeavyAirship: { unitObj.Damage(((Damages.DamageHeavyAirship * vehicleA.VetDamages[vehicleA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), vehicleA); break; }
                                default: break;
                            }
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObj.Damage((Damages.DamageDefault * wm.GetWaveDamageModifier(worldObj)) * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObj.Team == GameManager.Team.Defending, mod), _GunnerAttached._VehicleAttached); }
                }
            }
            else {

                // No raycast hit but still draw a hitscan visualizer
                GenerateHitScanVisualizer(_GunnerAttached.MuzzleLaunchPoints[0].transform.position, _GunnerAttached.MuzzleLaunchPoints[0].transform.forward * _GunnerAttached.MaxAttackRange);
            }
        }

        // Fire raycasts in a spread
        if (SpreadRaycasts && SpreadAmount > 1) { ProjectileRaycastSpread(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Creates a line renderer to visualize 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void GenerateHitScanVisualizer(Vector3 start, Vector3 end) {

        if (VisualizeHitscanTrace) {

            // Create line renderer object
            GameObject lineStencil = GameManager.Instance.HitScanVisualizer;
            LineRenderer line = ObjectPooling.Spawn(lineStencil.gameObject, Vector3.zero).GetComponent<LineRenderer>();

            Vector3[] positions = new Vector3[2];
            positions[0] = start;
            positions[1] = end;

            // Set line properties
            line.positionCount = 2;
            line.SetPositions(positions);
            line.material = new Material(Shader.Find("Particles/Additive"));
            line.startColor = HitScanStartColour;
            line.endColor = HitScanEndColour;
            line.startWidth = HitScanStartWidth;
            line.endWidth = HitScanEndWidth;

            // Delayed despawn
            StartCoroutine(HitscanRenderDespawn(line, HitScanDrawTime));
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits for the time specified in seconds, then despawns the line renderer gameobject
    //  that is passed through.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="delay"></param>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator HitscanRenderDespawn(LineRenderer line, float delay) {

        // Delay
        yield return new WaitForSeconds(delay);

        // Despawn
        ObjectPooling.Despawn(line.gameObject);
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
            StartCoroutine(ParticleDespawner.Instance.ParticleDespawn(effect, effectDuration));
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
        if (MuzzleEffectWrapper != null) {

            // Spawn
            ParticleSystem effect = ObjectPooling.Spawn(MuzzleEffectWrapper.gameObject).GetComponentInChildren<ParticleSystem>();

            // Muzzle iterator should be set already coz the firing weapon mechanism has already been set this frame
            // (in the switch statement just above)
            effect.transform.parent.position = _MuzzleLaunchPoints[_MuzzleIterator].position;
            effect.transform.parent.rotation = _MuzzleLaunchPoints[_MuzzleIterator].rotation;

            // Play
            effect.Play();

            // Despawn particle system once it has finished its cycle
            float effectDuration = effect.duration + effect.startLifetime;
            StartCoroutine(ParticleDespawner.Instance.ParticleParentDespawn(effect, effectDuration));
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