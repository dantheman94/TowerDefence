using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 25/8/2018
//
//******************************

public class DifficultyManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" STARTING DIFFICULTY")]
    [Space]
    public Info_Difficulty _Difficulty;

    [Space]
    [Header("-----------------------------------")]
    [Header(" EASY DIFFICULTY")]
    [Space]
    [Header("  HEALTH")]
    public DamageModifier EasyEnemyHealthModifier;
    [Space]
    public DamageModifier EasyFriendlyHealthModifier;
    [Space]
    [Header("  FIRING RATE")]
    public FiringRateModifier EasyEnemyFiringRateModifier;
    [Space]
    public FiringRateModifier EasyFriendlyFiringRateModifier;
    [Space]
    [Header("  MOVEMENT SPEED")]
    public MovementSpeedModifier EasyEnemyMovementSpeedModifier;
    [Space]
    public MovementSpeedModifier EasyFriendlyMovementSpeedModifier;

    [Space]
    [Header("-----------------------------------")]
    [Header(" NORMAL DIFFICULTY")]
    [Space]
    [Header("  HEALTH")]
    public DamageModifier NormalEnemyHealthModifier;
    [Space]
    public DamageModifier NormalFriendlyHealthModifier;
    [Space]
    [Header("  FIRING RATE")]
    public FiringRateModifier NormalEnemyFiringRateModifier;
    [Space]
    public FiringRateModifier NormalFriendlyFiringRateModifier;
    [Space]
    [Header("  MOVEMENT SPEED")]
    public MovementSpeedModifier NormalEnemyMovementSpeedModifier;
    [Space]
    public MovementSpeedModifier NormalFriendlyMovementSpeedModifier;

    [Space]
    [Header("-----------------------------------")]
    [Header(" HARD DIFFICULTY")]
    [Space]
    [Header("  HEALTH")]
    public DamageModifier HardEnemyHealthModifier;
    [Space]
    public DamageModifier HardFriendlyHealthModifier;
    [Space]
    [Header("  FIRING RATE")]
    public FiringRateModifier HardEnemyFiringRateModifier;
    [Space]
    public FiringRateModifier HardFriendlyFiringRateModifier;
    [Space]
    [Header("  MOVEMENT SPEED")]
    public MovementSpeedModifier HardEnemyMovementSpeedModifier;
    [Space]
    public MovementSpeedModifier HardFriendlyMovementSpeedModifier;

    [Space]
    [Header("----------------------------------")]
    [Header(" IMPOSSIBLE DIFFICULTY")]
    [Space]
    [Header("  HEALTH")]
    public DamageModifier ImpossibleEnemyHealthModifier;
    [Space]
    public DamageModifier ImpossibleFriendlyHealthModifier;
    [Space]
    [Header("  FIRING RATE")]
    public FiringRateModifier ImpossibleEnemyFiringRateModifier;
    [Space]
    public FiringRateModifier ImpossibleFriendlyFiringRateModifier;
    [Space]
    [Header("  MOVEMENT SPEED")]
    public MovementSpeedModifier ImpossibleEnemyMovementSpeedModifier;
    [Space]
    public MovementSpeedModifier ImpossibleFriendlyMovementSpeedModifier;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    public static DifficultyManager Instance;

    public enum EDifficultyModifiers { Damage, FiringRate, MovementSpeed }
    public enum Difficulties { Easy, Normal, Hard, Impossible }
    
    //******************************************************************************************************************************
    //
    //      CLASSES
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [System.Serializable]
    public class DamageModifier {

        public float DamageUndefined = 1f;

        public float DamageCoreMarine = 1f;
        public float DamageAntiInfantryMarine = 1f;
        public float DamageHero = 1f;

        public float DamageCoreVehicle = 1f;
        public float DamageAntiAirVehicle = 1f;
        public float DamageMobileArtillery = 1f;
        public float DamageBattleTank = 1f;

        public float DamageCoreAirship = 1f;
        public float DamageSupportAirship = 1f;
        public float DamageHeavyAirship = 1f;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    [System.Serializable]
    public class FiringRateModifier {

        public float FiringRateUndefined = 1f;

        public float FiringRateCoreMarine = 1f;
        public float FiringRateAntiInfantryMarine = 1f;
        public float FiringRateHero = 1f;
                     
        public float FiringRateCoreVehicle = 1f;
        public float FiringRateAntiAirVehicle = 1f;
        public float FiringRateMobileArtillery = 1f;
        public float FiringRateBattleTank = 1f;
                     
        public float FiringRateCoreAirship = 1f;
        public float FiringRateSupportAirship = 1f;
        public float FiringRateHeavyAirship = 1f;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    [System.Serializable]
    public class MovementSpeedModifier {

        public float MovementSpeedUndefined = 1f;

        public float MovementSpeedCoreMarine = 1f;
        public float MovementSpeedAntiInfantryMarine = 1f;
        public float MovementSpeedHero = 1f;
                     
        public float MovementSpeedCoreVehicle = 1f;
        public float MovementSpeedAntiAirVehicle = 1f;
        public float MovementSpeedMobileArtillery = 1f;
        public float MovementSpeedBattleTank = 1f;
                     
        public float MovementSpeedCoreAirship = 1f;
        public float MovementSpeedSupportAirship = 1f;
        public float MovementSpeedHeavyAirship = 1f;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  This is called before Startup().
    /// </summary>
    private void Awake() {

        // Initialize singleton
        if (Instance != null && Instance != this) {

            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <param name="modifier"></param>
    /// <returns>
    //  float
    /// </returns>
    public float GetDifficultyModifier(Unit unit, EDifficultyModifiers modifier) {

        Unit.EUnitType unitType = unit.UnitType;
        bool friendly = unit.Team == GameManager.Team.Defending;

        float fMod = 1f;
        switch (_Difficulty.Difficulty) {

            case Difficulties.Easy:         { fMod = GetEasyModifier(unitType, modifier, friendly); break; }
            case Difficulties.Normal:       { fMod = GetNormalModifier(unitType, modifier, friendly); break; }
            case Difficulties.Hard:         { fMod = GetHardModifier(unitType, modifier, friendly); break; }
            case Difficulties.Impossible:   { fMod = GetImpossibleModifier(unitType, modifier, friendly); break; }
            default: break;
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <param name="modifier"></param>
    /// <returns>
    //  float
    /// </returns>
    public float GetDifficultyModifier(Unit.EUnitType unitType, bool playerFriendly, EDifficultyModifiers modifier) {
        
        float fMod = 1f;
        switch (_Difficulty.Difficulty) {

            case Difficulties.Easy:         { fMod = GetEasyModifier(unitType, modifier, playerFriendly); break; }
            case Difficulties.Normal:       { fMod = GetNormalModifier(unitType, modifier, playerFriendly); break; }
            case Difficulties.Hard:         { fMod = GetHardModifier(unitType, modifier, playerFriendly); break; }
            case Difficulties.Impossible:   { fMod = GetImpossibleModifier(unitType, modifier, playerFriendly); break; }
            default: break;
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetEasyModifier(Unit.EUnitType unitType, EDifficultyModifiers modifier, bool friendly) {
        
        float fMod = 1f;
        switch (modifier) {

            case EDifficultyModifiers.Damage:           { fMod = GetEasyDamage(unitType, friendly); break; }
            case EDifficultyModifiers.FiringRate:       { fMod = GetEasyFiringRate(unitType, friendly); break; }
            case EDifficultyModifiers.MovementSpeed:    { fMod = GetEasyMovementSpeed(unitType, friendly); break; }
            default: break;
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetNormalModifier(Unit.EUnitType unitType, EDifficultyModifiers modifier, bool friendly) {

        float fMod = 1f;
        switch (modifier) {

            case EDifficultyModifiers.Damage:           { fMod = GetNormalDamage(unitType, friendly); break; }
            case EDifficultyModifiers.FiringRate:       { fMod = GetNormalFiringRate(unitType, friendly); break; }
            case EDifficultyModifiers.MovementSpeed:    { fMod = GetNormalMovementSpeed(unitType, friendly); break; }
            default: break;
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetHardModifier(Unit.EUnitType unitType, EDifficultyModifiers modifier, bool friendly) {

        float fMod = 1f;
        switch (modifier) {

            case EDifficultyModifiers.Damage:           { fMod = GetHardDamage(unitType, friendly); break; }
            case EDifficultyModifiers.FiringRate:       { fMod = GetHardFiringRate(unitType, friendly); break; }
            case EDifficultyModifiers.MovementSpeed:    { fMod = GetHardMovementSpeed(unitType, friendly); break; }
            default: break;
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetImpossibleModifier(Unit.EUnitType unitType, EDifficultyModifiers modifier, bool friendly) {

        float fMod = 1f;
        switch (modifier) {

            case EDifficultyModifiers.Damage:           { fMod = GetImpossibleDamage(unitType, friendly); break; }
            case EDifficultyModifiers.FiringRate:       { fMod = GetImpossibleFiringRate(unitType, friendly); break; }
            case EDifficultyModifiers.MovementSpeed:    { fMod = GetImpossibleMovementSpeed(unitType, friendly); break; }
            default: break;
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetEasyDamage(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly health
            switch (unitType) {

                // Easy difficulty
                case Unit.EUnitType.Undefined:              { fMod = EasyFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = EasyFriendlyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:  { fMod = EasyFriendlyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:   { fMod = EasyFriendlyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.Grumblebuster:          { fMod = EasyFriendlyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:              { fMod = EasyFriendlyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:               { fMod = EasyFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.Catapult:               { fMod = EasyFriendlyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:            { fMod = EasyFriendlyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.LightAirship:           { fMod = EasyFriendlyHealthModifier.DamageCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = EasyFriendlyHealthModifier.DamageSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = EasyFriendlyHealthModifier.DamageHeavyAirship; break; }
                default: break;
            }
        } else {

            // Enemy health
            switch (unitType) {

                // Easy difficulty
                case Unit.EUnitType.Undefined:              { fMod = EasyEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = EasyEnemyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:  { fMod = EasyEnemyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:   { fMod = EasyEnemyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.Grumblebuster:          { fMod = EasyEnemyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:              { fMod = EasyEnemyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:               { fMod = EasyEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.Catapult:               { fMod = EasyEnemyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:            { fMod = EasyEnemyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.LightAirship:           { fMod = EasyEnemyHealthModifier.DamageCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = EasyEnemyHealthModifier.DamageSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = EasyEnemyHealthModifier.DamageHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetNormalDamage(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly health
            switch (unitType) {

                // Normal difficulty
                case Unit.EUnitType.Undefined:              { fMod = NormalFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = NormalFriendlyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = NormalFriendlyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = NormalFriendlyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = NormalFriendlyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = NormalFriendlyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = NormalFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = NormalFriendlyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = NormalFriendlyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = NormalFriendlyHealthModifier.DamageCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = NormalFriendlyHealthModifier.DamageSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = NormalFriendlyHealthModifier.DamageHeavyAirship; break; }
                default: break;
            }
        }
        else {

            // Enemy health
            switch (unitType) {

                // Normal difficulty
                case Unit.EUnitType.Undefined:              { fMod = NormalEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = NormalEnemyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = NormalEnemyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = NormalEnemyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = NormalEnemyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = NormalEnemyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = NormalEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = NormalEnemyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = NormalEnemyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = NormalEnemyHealthModifier.DamageCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = NormalEnemyHealthModifier.DamageSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = NormalEnemyHealthModifier.DamageHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetHardDamage(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly health
            switch (unitType) {

                // Hard difficulty
                case Unit.EUnitType.Undefined:              { fMod = HardFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = HardFriendlyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = HardFriendlyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = HardFriendlyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = HardFriendlyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = HardFriendlyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = HardFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = HardFriendlyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = HardFriendlyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = HardFriendlyHealthModifier.DamageCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = HardFriendlyHealthModifier.DamageSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = HardFriendlyHealthModifier.DamageHeavyAirship; break; }
                default: break;
            }
        }
        else {

            // Enemy health
            switch (unitType) {

                // Hard difficulty
                case Unit.EUnitType.Undefined:              { fMod = HardEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = HardEnemyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = HardEnemyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = HardEnemyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = HardEnemyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = HardEnemyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = HardEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = HardEnemyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = HardEnemyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = HardEnemyHealthModifier.DamageCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = HardEnemyHealthModifier.DamageSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = HardEnemyHealthModifier.DamageHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetImpossibleDamage(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly health
            switch (unitType) {

                // Impossible difficulty
                case Unit.EUnitType.Undefined:              { fMod = ImpossibleFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = ImpossibleFriendlyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = ImpossibleFriendlyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = ImpossibleFriendlyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = ImpossibleFriendlyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = ImpossibleFriendlyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = ImpossibleFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = ImpossibleFriendlyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = ImpossibleFriendlyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = ImpossibleFriendlyHealthModifier.DamageCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = ImpossibleFriendlyHealthModifier.DamageSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = ImpossibleFriendlyHealthModifier.DamageHeavyAirship; break; }
                default: break;
            }
        }
        else {

            // Enemy health
            switch (unitType) {

                // Impossible difficulty
                case Unit.EUnitType.Undefined:              { fMod = ImpossibleEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = ImpossibleEnemyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = ImpossibleEnemyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = ImpossibleEnemyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = ImpossibleEnemyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = ImpossibleEnemyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = ImpossibleEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = ImpossibleEnemyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = ImpossibleEnemyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = ImpossibleEnemyHealthModifier.DamageCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = ImpossibleEnemyHealthModifier.DamageSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = ImpossibleEnemyHealthModifier.DamageHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetEasyFiringRate(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly health
            switch (unitType) {

                // Easy difficulty
                case Unit.EUnitType.Undefined:              { fMod = EasyFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = EasyFriendlyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = EasyFriendlyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = EasyFriendlyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = EasyFriendlyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = EasyFriendlyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = EasyFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = EasyFriendlyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = EasyFriendlyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = EasyFriendlyFiringRateModifier.FiringRateCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = EasyFriendlyFiringRateModifier.FiringRateSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = EasyFriendlyFiringRateModifier.FiringRateHeavyAirship; break; }
                default: break;
            }
        } else {

            // Enemy health
            switch (unitType) {

                // Easy difficulty
                case Unit.EUnitType.Undefined:              { fMod = EasyEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = EasyEnemyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = EasyEnemyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = EasyEnemyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = EasyEnemyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = EasyEnemyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = EasyEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = EasyEnemyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = EasyEnemyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = EasyEnemyFiringRateModifier.FiringRateCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = EasyEnemyFiringRateModifier.FiringRateSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = EasyEnemyFiringRateModifier.FiringRateHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetNormalFiringRate(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly firing rate
            switch (unitType) {

                // Normal difficulty
                case Unit.EUnitType.Undefined:              { fMod = NormalFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = NormalFriendlyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = NormalFriendlyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = NormalFriendlyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = NormalFriendlyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = NormalFriendlyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = NormalFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = NormalFriendlyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = NormalFriendlyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = NormalFriendlyFiringRateModifier.FiringRateCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = NormalFriendlyFiringRateModifier.FiringRateSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = NormalFriendlyFiringRateModifier.FiringRateHeavyAirship; break; }
                default: break;
            }
        }
        else {

            // Enemy firing rate
            switch (unitType) {

                // Normal difficulty
                case Unit.EUnitType.Undefined:              { fMod = NormalEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = NormalEnemyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = NormalEnemyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = NormalEnemyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = NormalEnemyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = NormalEnemyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = NormalEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = NormalEnemyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = NormalEnemyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = NormalEnemyFiringRateModifier.FiringRateCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = NormalEnemyFiringRateModifier.FiringRateSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = NormalEnemyFiringRateModifier.FiringRateHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetHardFiringRate(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly firing rate
            switch (unitType) {

                // Hard difficulty
                case Unit.EUnitType.Undefined:              { fMod = HardFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = HardFriendlyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = HardFriendlyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = HardFriendlyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = HardFriendlyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = HardFriendlyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = HardFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = HardFriendlyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = HardFriendlyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = HardFriendlyFiringRateModifier.FiringRateCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = HardFriendlyFiringRateModifier.FiringRateSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = HardFriendlyFiringRateModifier.FiringRateHeavyAirship; break; }
                default: break;
            }
        }
        else {

            // Enemy firing rate
            switch (unitType) {

                // Hard difficulty
                case Unit.EUnitType.Undefined:              { fMod = HardEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = HardEnemyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = HardEnemyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = HardEnemyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = HardEnemyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = HardEnemyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = HardEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = HardEnemyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = HardEnemyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = HardEnemyFiringRateModifier.FiringRateCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = HardEnemyFiringRateModifier.FiringRateSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = HardEnemyFiringRateModifier.FiringRateHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetImpossibleFiringRate(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly firing rate
            switch (unitType) {

                // Impossible difficulty
                case Unit.EUnitType.Undefined:              { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateHeavyAirship; break; }
                default: break;
            }
        }
        else {

            // Enemy firing rate
            switch (unitType) {

                // Impossible difficulty
                case Unit.EUnitType.Undefined:              { fMod = ImpossibleEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = ImpossibleEnemyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = ImpossibleEnemyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = ImpossibleEnemyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = ImpossibleEnemyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = ImpossibleEnemyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = ImpossibleEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = ImpossibleEnemyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = ImpossibleEnemyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = ImpossibleEnemyFiringRateModifier.FiringRateCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = ImpossibleEnemyFiringRateModifier.FiringRateSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = ImpossibleEnemyFiringRateModifier.FiringRateHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetEasyMovementSpeed(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly health
            switch (unitType) {

                // Easy difficulty
                case Unit.EUnitType.Undefined:              { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        } else {

            // Enemy health
            switch (unitType) {

                // Easy difficulty
                case Unit.EUnitType.Undefined:              { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetNormalMovementSpeed(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly firing rate
            switch (unitType) {

                // Normal difficulty
                case Unit.EUnitType.Undefined:              { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        }
        else {

            // Enemy firing rate
            switch (unitType) {

                // Normal difficulty
                case Unit.EUnitType.Undefined:              { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetHardMovementSpeed(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly firing rate
            switch (unitType) {

                // Hard difficulty
                case Unit.EUnitType.Undefined:              { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        }
        else {

            // Enemy firing rate
            switch (unitType) {

                // Hard difficulty
                case Unit.EUnitType.Undefined:              { fMod = HardEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = HardEnemyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = HardEnemyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = HardEnemyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = HardEnemyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = HardEnemyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = HardEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = HardEnemyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = HardEnemyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = HardEnemyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = HardEnemyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = HardEnemyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>
    //  float
    /// </returns>
    private float GetImpossibleMovementSpeed(Unit.EUnitType unitType, bool friendly) {

        float fMod = 1f;
        if (friendly) {

            // Friendly firing rate
            switch (unitType) {

                // Impossible difficulty
                case Unit.EUnitType.Undefined:              { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        }
        else {

            // Enemy firing rate
            switch (unitType) {

                // Impossible difficulty
                case Unit.EUnitType.Undefined:              { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.DwfSoldier:             { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.DwfSpecialistInfantry:     { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.DwfSpecialistVehicle:                   { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.Grumblebuster:            { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.Skylancer:         { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.Ballista:    { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.Catapult:        { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.SiegeEngine:             { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.LightAirship:            { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}