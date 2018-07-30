using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 24/7/2018
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
    public Difficulties CurrentDifficulty = Difficulties.Normal;

    [Space]
    [Header("-----------------------------------")]
    [Header(" EASY DIFFICULTY")]
    [Space]
    [Header("  HEALTH")]
    public HealthModifier EasyEnemyHealthModifier;
    [Space]
    public HealthModifier EasyFriendlyHealthModifier;
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
    public HealthModifier NormalEnemyHealthModifier;
    [Space]
    public HealthModifier NormalFriendlyHealthModifier;
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
    public HealthModifier HardEnemyHealthModifier;
    [Space]
    public HealthModifier HardFriendlyHealthModifier;
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
    public HealthModifier ImpossibleEnemyHealthModifier;
    [Space]
    public HealthModifier ImpossibleFriendlyHealthModifier;
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

    public enum Difficulties { Easy, Normal, Hard, Impossible }
    
    //******************************************************************************************************************************
    //
    //      CLASSES
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [System.Serializable]
    public class HealthModifier {

        public float HealthCoreMarine = 1f;
        public float HealthAntiInfantryMarine = 1f;
        public float HealthHero = 1f;

        public float HealthCoreVehicle = 1f;
        public float HealthAntiAirVehicle = 1f;
        public float HealthMobileArtillery = 1f;
        public float HealthBattleTank = 1f;

        public float HealthCoreAirship = 1f;
        public float HealthSupportAirship = 1f;
        public float HealthHeavyAirship = 1f;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    [System.Serializable]
    public class FiringRateModifier {

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

}