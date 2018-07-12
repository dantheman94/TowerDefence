using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 9/7/2018
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
    public float PerWaveIncrementDamage = 0.01f;
    public float PerWaveIncrementHealth = 0.01f;
    [Space]
    public float PerBossWaveIncrementDamage = 0.15f;
    public float PerBossWaveIncrementHealth = 0.2f;
    [Space]
    public int BossWaveInterval = 10;
    [Space]
    public int StartingWaveTimer = 30;
    public int TimeAddedPerWave = 5;

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
        [Space]
        public List<WaveEnemyInfo> Enemies;
    }

    [System.Serializable]
    public class WaveEnemyInfo {

        public string Name;
        public WorldObject EnemyReference;
        public int PerSpawn;
        public int WaveMax;
        [HideInInspector]
        public int CurrentLives;
    }

    public static WaveManager Instance;

    private int _CurrentWave = 0;
    private int _WaveInterval = 0;
    private int _BossWaveCount = 0;
    private bool _BossWave = false;

    private float _CurrentDamageModifier;
    private float _CurrentHealthModifier;

    private WaveInfo _CurrentWaveInfo;
    private List<WorldObject> _CurrentWaveEnemies;
    private List<WorldObject> _PreviousWavesEnemies;

    private bool _WaveInProgress = false;
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

        _CurrentWaveTime = 0f;
        _NextWaveTimer = _TimeTillNextWave;

        // Add to wave count
        _CurrentWave++;
        _WaveInterval++;

        // Check if current interval is boss wave
        if (_WaveInterval == BossWaveInterval) { _BossWave = true; }
        else { _BossWave = false; }
        
        // Determine wave type based off the (boss|wave) intervals
        switch (_BossWaveCount) {

            // Boss wave 0
            case 0: {
                
                switch (_WaveInterval) {

                    // Wave interval 1
                    case 1: {     
                            
                        int id = Random.Range(0, (int)WaveType.ENUM_COUNT - 2);
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.SuperLight, (WaveType)id);
                        break;
                    }

                    // Wave interval 2
                    case 2: {
   
                        int id = Random.Range(0, (int)WaveType.ENUM_COUNT - 2);
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.SuperLight, (WaveType)id);
                        break;
                    }

                    // Wave interval 3
                    case 3: {
   
                        int id = Random.Range(0, (int)WaveType.ENUM_COUNT - 2);
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.SuperLight, (WaveType)id);
                        break;
                    }

                    // Wave interval 4
                    case 4: {
                                
                        int id = Random.Range(0, (int)WaveType.ENUM_COUNT - 2);
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.SuperLight, (WaveType)id);
                        break;
                    }

                    // Wave interval 5
                    case 5: {
                                
                        int id = Random.Range(0, (int)WaveType.ENUM_COUNT - 2);
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.SuperLight, (WaveType)id);
                        break;
                    }

                    // Wave interval 6
                    case 6: {
                                
                        int id = Random.Range(0, (int)WaveType.ENUM_COUNT - 2);
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.SuperLight, (WaveType)id);
                        break;
                    }

                    // Wave interval 7
                    case 7: {
                                
                        int id = Random.Range(0, (int)WaveType.ENUM_COUNT - 2);
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.SuperLight, (WaveType)id);
                        break;
                    }

                    // Wave interval 8
                    case 8: {
                                
                        int id = Random.Range(0, (int)WaveType.ENUM_COUNT - 2);
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.SuperLight, (WaveType)id);
                        break;
                    }

                    // Wave interval 9
                    case 9: {
                                
                        int id = Random.Range(0, (int)WaveType.ENUM_COUNT - 2);
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.SuperLight, (WaveType)id);
                        break;
                    }

                    // Wave interval 10 (boss wave)
                    case 10: {
                                
                        _CurrentWaveInfo = GetWaveInfo(WaveSeverity.Light, WaveType.Boss);
                        _BossWave = true;
                        break;
                    }

                    default: break;
                }                
                break;
            }

            default: break;
        }

        // Get array of new wave enemies
        _CurrentWaveEnemies = GetWaveEnemies(_CurrentWaveInfo.Severity, _CurrentWaveInfo.Type);
        _WaveInProgress = true;
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

        // Update timers
        _WaveInProgress = false;
        _CurrentWaveTime = 0f;
        _TimeTillNextWave += TimeAddedPerWave;

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
    /// <param name="severity"></param>
    /// <param name="type"></param>
    /// <returns>
    //  WaveInfo
    /// </returns>
    private WaveInfo GetWaveInfo(WaveSeverity severity, WaveType type) {

        WaveInfo info = new WaveInfo();
        bool match = false;

        // Loop through wave lists
        for (int i = 0; i < Waves.Count; i++) {
            
            // Matching severity
            if (Waves[i].Severity == severity) {

                bool typeMatch = false;

                // Matching wave type
                if (Waves[i].Type == type) { typeMatch = true; }

                // Keep looping through the wave lists if the type doesn't match
                if (!typeMatch) { continue; }
                else {

                    // Found wave with matching info
                    info = Waves[i];
                    match = true;
                    break;
                }
            }

            // Found a match
            if (match) { break; }
        }

        // Assert error if a match hasn't been found
        if (!match) {  Debug.Assert(match == false, "ERROR: WaveInfo type match was not found. Have you created a wave with these settings?" + severity + " : " + type); }

        return info;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="severity"></param>
    /// <param name="type"></param>
    /// <returns>
    //  List<WorldObject>
    /// </returns>
    private List<WorldObject> GetWaveEnemies(WaveSeverity severity, WaveType type) {

        // Array of all the enemies in the wave
        List<WorldObject> wave = new List<WorldObject>();

        // Loop through wave lists
        for (int i = 0; i < Waves.Count; i++) {

            bool match = false;

            // Matching severity
            if (Waves[i].Severity == severity) {

                bool typeMatch = false;

                // Matching wave type
                if (Waves[i].Type == type) { typeMatch = true; }

                // Keep looping through the wave lists if the type doesn't match
                if (!typeMatch) { continue; }
                else {

                    // Add all the enemy types to the wave enemies array
                    for (int j = 0; j < Waves[i].Enemies.Count; j++) { wave.Add(Waves[i].Enemies[j].EnemyReference); }
                    match = true;
                    break;
                }
            }

            // Found a match
            if (match) { break; }
        }

        return wave;
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