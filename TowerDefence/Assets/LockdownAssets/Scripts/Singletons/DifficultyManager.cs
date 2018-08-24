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
        bool friendly = unit.Team == unit._Player.Team;

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
                case Unit.EUnitType.CoreMarine:             { fMod = EasyFriendlyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = EasyFriendlyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = EasyFriendlyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = EasyFriendlyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = EasyFriendlyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = EasyFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = EasyFriendlyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = EasyFriendlyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = EasyFriendlyHealthModifier.DamageCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = EasyFriendlyHealthModifier.DamageSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = EasyFriendlyHealthModifier.DamageHeavyAirship; break; }
                default: break;
            }
        } else {

            // Enemy health
            switch (unitType) {

                // Easy difficulty
                case Unit.EUnitType.Undefined:              { fMod = EasyEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.CoreMarine:             { fMod = EasyEnemyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = EasyEnemyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = EasyEnemyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = EasyEnemyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = EasyEnemyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = EasyEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = EasyEnemyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = EasyEnemyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = EasyEnemyHealthModifier.DamageCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = NormalFriendlyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = NormalFriendlyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = NormalFriendlyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = NormalFriendlyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = NormalFriendlyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = NormalFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = NormalFriendlyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = NormalFriendlyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = NormalFriendlyHealthModifier.DamageCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = NormalEnemyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = NormalEnemyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = NormalEnemyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = NormalEnemyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = NormalEnemyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = NormalEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = NormalEnemyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = NormalEnemyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = NormalEnemyHealthModifier.DamageCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = HardFriendlyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = HardFriendlyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = HardFriendlyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = HardFriendlyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = HardFriendlyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = HardFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = HardFriendlyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = HardFriendlyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = HardFriendlyHealthModifier.DamageCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = HardEnemyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = HardEnemyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = HardEnemyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = HardEnemyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = HardEnemyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = HardEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = HardEnemyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = HardEnemyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = HardEnemyHealthModifier.DamageCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = ImpossibleFriendlyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = ImpossibleFriendlyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = ImpossibleFriendlyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = ImpossibleFriendlyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = ImpossibleFriendlyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = ImpossibleFriendlyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = ImpossibleFriendlyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = ImpossibleFriendlyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = ImpossibleFriendlyHealthModifier.DamageCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = ImpossibleEnemyHealthModifier.DamageCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = ImpossibleEnemyHealthModifier.DamageAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = ImpossibleEnemyHealthModifier.DamageHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = ImpossibleEnemyHealthModifier.DamageCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = ImpossibleEnemyHealthModifier.DamageAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = ImpossibleEnemyHealthModifier.DamageUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = ImpossibleEnemyHealthModifier.DamageMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = ImpossibleEnemyHealthModifier.DamageBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = ImpossibleEnemyHealthModifier.DamageCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = EasyFriendlyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = EasyFriendlyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = EasyFriendlyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = EasyFriendlyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = EasyFriendlyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = EasyFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = EasyFriendlyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = EasyFriendlyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = EasyFriendlyFiringRateModifier.FiringRateCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = EasyFriendlyFiringRateModifier.FiringRateSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = EasyFriendlyFiringRateModifier.FiringRateHeavyAirship; break; }
                default: break;
            }
        } else {

            // Enemy health
            switch (unitType) {

                // Easy difficulty
                case Unit.EUnitType.Undefined:              { fMod = EasyEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.CoreMarine:             { fMod = EasyEnemyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = EasyEnemyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = EasyEnemyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = EasyEnemyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = EasyEnemyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = EasyEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = EasyEnemyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = EasyEnemyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = EasyEnemyFiringRateModifier.FiringRateCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = NormalFriendlyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = NormalFriendlyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = NormalFriendlyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = NormalFriendlyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = NormalFriendlyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = NormalFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = NormalFriendlyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = NormalFriendlyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = NormalFriendlyFiringRateModifier.FiringRateCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = NormalEnemyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = NormalEnemyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = NormalEnemyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = NormalEnemyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = NormalEnemyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = NormalEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = NormalEnemyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = NormalEnemyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = NormalEnemyFiringRateModifier.FiringRateCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = HardFriendlyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = HardFriendlyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = HardFriendlyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = HardFriendlyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = HardFriendlyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = HardFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = HardFriendlyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = HardFriendlyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = HardFriendlyFiringRateModifier.FiringRateCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = HardEnemyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = HardEnemyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = HardEnemyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = HardEnemyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = HardEnemyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = HardEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = HardEnemyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = HardEnemyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = HardEnemyFiringRateModifier.FiringRateCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = ImpossibleFriendlyFiringRateModifier.FiringRateCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = ImpossibleEnemyFiringRateModifier.FiringRateCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = ImpossibleEnemyFiringRateModifier.FiringRateAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = ImpossibleEnemyFiringRateModifier.FiringRateHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = ImpossibleEnemyFiringRateModifier.FiringRateCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = ImpossibleEnemyFiringRateModifier.FiringRateAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = ImpossibleEnemyFiringRateModifier.FiringRateUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = ImpossibleEnemyFiringRateModifier.FiringRateMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = ImpossibleEnemyFiringRateModifier.FiringRateBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = ImpossibleEnemyFiringRateModifier.FiringRateCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = EasyFriendlyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        } else {

            // Enemy health
            switch (unitType) {

                // Easy difficulty
                case Unit.EUnitType.Undefined:              { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.CoreMarine:             { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = EasyEnemyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = NormalFriendlyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = NormalEnemyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = HardFriendlyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = HardEnemyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = HardEnemyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = HardEnemyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = HardEnemyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = HardEnemyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = HardEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = HardEnemyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = HardEnemyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = HardEnemyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = ImpossibleFriendlyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
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
                case Unit.EUnitType.CoreMarine:             { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedCoreMarine; break; }
                case Unit.EUnitType.AntiInfantryMarine:     { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedAntiInfantryMarine; break; }
                case Unit.EUnitType.Hero:                   { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedHero; break; }
                case Unit.EUnitType.CoreVehicle:            { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedCoreVehicle; break; }
                case Unit.EUnitType.AntiAirVehicle:         { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedAntiAirVehicle; break; }
                case Unit.EUnitType.AntiBuildingVehicle:    { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedUndefined; break; }
                case Unit.EUnitType.MobileArtillery:        { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedMobileArtillery; break; }
                case Unit.EUnitType.BattleTank:             { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedBattleTank; break; }
                case Unit.EUnitType.CoreAirship:            { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedCoreAirship; break; }
                case Unit.EUnitType.SupportShip:            { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedSupportAirship; break; }
                case Unit.EUnitType.HeavyAirship:           { fMod = ImpossibleEnemyMovementSpeedModifier.MovementSpeedHeavyAirship; break; }
                default: break;
            }
        }
        return fMod;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}