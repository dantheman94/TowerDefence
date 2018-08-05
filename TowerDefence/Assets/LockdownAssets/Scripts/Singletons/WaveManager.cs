﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/8/2018
//
//******************************

public class WaveManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MATCH START")]
    [Space]
    public float StartingDamageModifier = 0.75f;
    public float StartingHealthModifier = 0.7f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" WAVE PROPERTIES")]
    [Space]
    [Space]
    public float PerWaveIncrementDamage = 0.01f;
    public float PerWaveIncrementHealth = 0.01f;
    [Space]
    public float PerBossWaveIncrementDamage = 0.15f;
    public float PerBossWaveIncrementHealth = 0.2f;
    [Space]
    [Range(1, 10)]
    public int WavesPerInterval = 10;
    [Space]
    public int StartingWaveTimer = 30;
    public int TimeAddedPerWave = 5;
    public int TimeTillNextWaveAfterClear = 10;

    [Space]
    [Header("-----------------------------------")]
    [Header(" PROPS")]
    [Space]
    public Core CentralCore;
    [Space]
    public List<LockdownPad> LockdownPads;
    [Space]
    public List<Base> EnemyBases;

    [Space]
    [Header("-----------------------------------")]
    [Header(" WAVE TYPES")]
    [Space]
    public List<WaveInfo> Waves;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum WaveSeverity { SuperLight, Light, Small, Medium, Heavy, ENUM_COUNT }
    public enum WaveType { GroundTroops, GroundVehicles, MixedGround, AirVehicles, MixedVehicles, Mixed, Boss, ENUM_COUNT }

    [System.Serializable]
    public struct WaveInfo {

        public string Name;
        public WaveSeverity Severity;
        public WaveType Type;
        public WaveInterval WaveIntervals;
        public BossWaveInterval BossWaveIntervals;
        [Space]
        public int Subwaves;
        public List<WaveEnemyInfo> Enemies;
    }

    [System.Serializable]
    public class WaveEnemyInfo {

        public string Name;
        public WorldObject EnemyReference;
        public int PerSpawnInterval;
        public int WaveMax;
        [HideInInspector]
        public int CurrentLives;
    }

    [System.Serializable]
    public class WaveInterval {

        [Range(1, 9)]
        public int MinimumWaveInterval = 1;
        [Range(2, 9)]
        public int MaximumWaveInterval = 10;
    }

    [System.Serializable]
    public class BossWaveInterval {

        [Range(0, 10)]
        public int MinimumBossWaveInterval = 0;
        public bool UnlimitedBossWaveIntervals = true;
        [Range(0, 100)]
        public int MaximumBossWaveInterval = 0;
    }

    public static WaveManager Instance;

    private int _CurrentWave = 0;
    private int _CurrentSubwave = 1;
    private int _WaveInterval = 0;
    private int _BossWaveCount = 0;
    private bool _BossWave = false;
    private int _WaveArrayID;

    private float _CurrentDamageModifier;
    private float _CurrentHealthModifier;

    private WaveInfo _CurrentWaveInfo;
    private List<WorldObject> _CurrentWaveEnemies;
    private List<WorldObject> _PreviousWavesEnemies;
    private LockdownPad _CurrentLockdownPad;

    private bool _WaveInProgress = false;
    private float _CurrentSubwaveTime = 0f;
    private float _TimeTillNextSubwave = 0f;
    private float _CurrentWaveTime = 0f;
    private float _TimeTillNextWave = 0f;
    private float _NextWaveTimer = 0f;
    
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
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Initialize lists
        _CurrentWaveEnemies = new List<WorldObject>();
        _PreviousWavesEnemies = new List<WorldObject>();

        // Initialize enemy bases
        for (int i = 0; i < EnemyBases.Count; i++) {

            // Create healthbar
            EnemyBases[i].CreateHealthBar(EnemyBases[i], GameManager.Instance.Players[0].PlayerCamera);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        UpdateWave();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame via Update().
    /// </summary>
    private void UpdateWave() {

        if (_WaveInProgress) {

            // Update wave timers
            _CurrentWaveTime += Time.deltaTime;
            _NextWaveTimer -= Time.deltaTime;
            _CurrentSubwaveTime += Time.deltaTime;

            // Check for new subwave
            if (_CurrentSubwaveTime >= _TimeTillNextSubwave) {

                _CurrentSubwaveTime = 0f;
                SpawnSubwave(GetSubwave());
            }

            // Only non boss waves will start before the current wave is complete
            if (_NextWaveTimer <= 0f || _CurrentWaveTime >= _TimeTillNextWave) {

                if (!_BossWave) {

                    // Next wave timer complete
                    WaveComplete();
                    StartNewWave();
                }

                // Its a boss wave so force the timer to stay at zero when it's complete
                else { _NextWaveTimer = 0; }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Starts a new game at the specified starting wave amount.
    /// </summary>
    public void StartNewMatch() {

        // Initialize modifiers
        _CurrentDamageModifier = StartingDamageModifier;
        _CurrentHealthModifier = StartingHealthModifier;

        // Show HUD/hide cinematic bars
        GameManager.Instance.HUDWrapper.SetActive(true);
        CinematicBars.Instance.StartAnimation(CinematicBars.AnimationDirection.Exit);

        // Setup wave properties
        _TimeTillNextWave = StartingWaveTimer;

        // Start wave 1
        StartNewWave();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void StartNewWave() {

        _CurrentSubwave = 0;
        _CurrentWaveTime = 0f;
        _NextWaveTimer = _TimeTillNextWave;

        // Add to wave count
        _CurrentWave++;
        _WaveInterval++;

        // Check if current interval is boss wave
        if (_WaveInterval == WavesPerInterval) { _BossWave = true; }
        else { _BossWave = false; }
        
        // Get lockdown pad reference
        _CurrentLockdownPad = GetPadForSpawning();

        // Get array of new wave enemies
        _CurrentWaveInfo = GetWave();
        InitializeWave();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void BossWaveComplete() {

        // Reset wave interval
        _WaveInterval = 0;

        // Increase modifiers by boss amount
        _CurrentDamageModifier += PerBossWaveIncrementDamage;
        _CurrentHealthModifier += PerBossWaveIncrementHealth;

        _BossWave = false;
        _CurrentWaveTime = 0f;
        _WaveInProgress = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called whenever the wave timer reaches zero or the current wave is cleared out
    //  (whichever happens first).
    /// </summary>
    public void WaveComplete() {
        
        // Update next wave timer countdown
        if (_NextWaveTimer > TimeTillNextWaveAfterClear) { _NextWaveTimer = TimeTillNextWaveAfterClear; }

        // Update timers
        _WaveInProgress = false;
        _CurrentWaveTime = 0f;
        _TimeTillNextWave += TimeAddedPerWave;
        _CurrentSubwaveTime = 0f;

        // Clear the respawn counts for the previous wave
        if (_CurrentWaveInfo.Enemies != null) {

            for (int i = 0; i < _CurrentWaveInfo.Enemies.Count; i++) { _CurrentWaveInfo.Enemies[i].CurrentLives = 0; }
        }

        // Add current wave into the previous waves list
        for (int i = 0; i < _CurrentWaveEnemies.Count; i++) { _PreviousWavesEnemies.Add(_CurrentWaveEnemies[i]); }
        _CurrentWaveEnemies.Clear();

        // Update modifiers
        if (!_BossWave) {

            _CurrentDamageModifier += PerWaveIncrementDamage;
            _CurrentHealthModifier += PerWaveIncrementHealth;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="currentWaveEnemies"></param>
    private void InitializeWave() {

        // Determine the amount of subwave intervals (and their time)
        int subwaves = _CurrentWaveInfo.Subwaves;
        _TimeTillNextSubwave = _TimeTillNextWave / subwaves;
        
        SpawnSubwave(GetSubwave());
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  List<WorldObject>
    /// </returns>
    public List<WorldObject> GetSubwave() {

        List<WorldObject> subwave = new List<WorldObject>();

        // Loop through the current wave array
        for (int i = 0; i < _CurrentWaveInfo.Enemies.Count; i++) {

            /*
             *  ITS HERE THATS THE BUG, CURRENT LIVES IS 0
             */

            if (_CurrentWaveInfo.Enemies[i].CurrentLives >= _CurrentWaveInfo.Enemies[i].PerSpawnInterval) {

                // Add to subwave list
                subwave.Add(_CurrentWaveInfo.Enemies[i].EnemyReference);
            }
        }

        return subwave;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="subwaveEnemies"></param>
    private void SpawnSubwave(List<WorldObject> subwaveEnemies) {

        List<Vector3> spawnPositions = _CurrentLockdownPad.GetSpawnLocations(subwaveEnemies.Count);
        for (int i = 0; i < subwaveEnemies.Count; i++) {

            GameObject obj = ObjectPooling.Spawn(subwaveEnemies[i].gameObject, spawnPositions[i]);

            // Initialize the object as a squad
            Squad squad = obj.GetComponent<Squad>();
            if (squad != null) {

                squad.SpawnUnits();
                squad.SquadSeek(CentralCore.GetSeekPosition());
                squad.Team = GameManager.Team.Attacking;
                _CurrentWaveEnemies.Add(squad);
            }

            // Initialize the object as a unit
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null) {

                unit.OnSpawn();
                unit.AgentSeekPosition(CentralCore.GetSeekPosition());
                unit.Team = GameManager.Team.Attacking;
                _CurrentWaveEnemies.Add(unit);
            }
        }
        _CurrentSubwave++;
        _WaveInProgress = true;
        Debug.Log("Spawn subwave");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  LockdownPad
    /// </returns>
    private LockdownPad GetPadForSpawning() {

        int i = Random.Range(0, LockdownPads.Count - 1);
        return LockdownPads[i];
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  List<WorldObject>
    /// </returns>
    private WaveInfo GetWave() {

        // Array of all enemy infos
        List<WaveInfo> waveInfos = new List<WaveInfo>();

        // Loop through wave lists
        for (int i = 0; i < Waves.Count; i++) {

            // Valid boss interval check
            if (Waves[i].BossWaveIntervals.UnlimitedBossWaveIntervals ||
               (_BossWaveCount >= Waves[i].BossWaveIntervals.MinimumBossWaveInterval &&
                _BossWaveCount <= Waves[i].BossWaveIntervals.MaximumBossWaveInterval)) {

                // Valid wave interval check
                if (_WaveInterval >= Waves[i].WaveIntervals.MinimumWaveInterval &&
                    _WaveInterval <= Waves[i].WaveIntervals.MaximumWaveInterval) {

                    // Add to random wave pool
                    waveInfos.Add(Waves[i]);
                    continue;
                }
                else { continue; }
            }
        }

        // Return random wave from pool
        return GetRandomWaveFrom(waveInfos);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="waves"></param>
    /// <returns>
    //  List<WorldObject>
    /// </returns>
    private WaveInfo GetRandomWaveFrom(List<WaveInfo> waves) {
        
        int iRand = Random.Range(0, waves.Count);
        return Waves[iRand];
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the current counter value of the current wave
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int GetWaveCount() { return _CurrentWave; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  WaveInfo
    /// </returns>
    public WaveInfo GetCurrentWaveInfo() { return _CurrentWaveInfo; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetCurrentDamageModifer() { return _CurrentDamageModifier; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetCurrentHealthModifer() { return _CurrentHealthModifier; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetTimeTillNextWave() { return _NextWaveTimer; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsBossWave() { return _BossWave; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}