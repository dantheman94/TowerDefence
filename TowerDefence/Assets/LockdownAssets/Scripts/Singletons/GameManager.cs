﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 25/8/2018
//
//******************************

public class GameManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" LEVEL INFORMATION")]
    [Space]
    public Info_Level _Level;
    public GameObject FlyingNavMesh;
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" PLAYER STARTS")]
    [Space]
    public Base StartingBase;
    [Space]
    public int StartingSupplyCount;
    public int StartingPowerCount;
    public int StartingMaxSupplyCount;
    public int StartingMaxPowerCount;
    public int StartingTechLevel = 1;
    public int StartingMaxPopulation;
    [Space]
    public int StartingWave = 0;

    [Space]
    [Header("-----------------------------------")]
    [Header(" USER INTERFACE")]
    [Space]
    public GameObject HUDWrapper;
    public Canvas WorldSpaceCanvas;
    public Canvas ScreenSpaceCanvas;
    [Space]
    public GameObject AgentSeekObject;

    [Space]
    [Header("-----------------------------------")]
    [Header(" HUD")]
    [Space]
    public CinematicBars CinematicBars;
    public bool _IsRadialMenu = false;
    public GameObject SelectionWheel;
    public GameObject selectionWindow;
    public GameObject ConfirmRecycleScreen;
    public GameObject AbilityWheel;
    [Space]
    public UI_SelectedUnits SelectedUnitsHUD;
    public UI_PlatoonUnits PlatoonUnitsHUD;
    public UI_WaveStats WaveStatsHUD;
    public UI_BuildingQueueWrapper BuildingQueueHUD;

    [Space]
    [Header("-----------------------------------")]
    [Header(" PROGRESS BARS")]
    [Space]
    public GameObject UnitHealthBar;
    public GameObject BuildingInProgressPanel;
    public GameObject CaptureProgressPanel;

    [Space]
    [Header("-----------------------------------")]
    [Header(" MISCELLANEOUS")]
    [Space]
    public GameObject RecycleBuilding;
    public GameObject ObjectSelected;
    public GameObject ObjectHighlighted;
    public GameObject RallyPointObject = null;

    [Header("-----------------------------------")]
    [Header(" SCREENS")]
    [Space]
    public UI_PauseWidget PauseWidget;
    public UI_GameOver GameOverWidget;

    [Space]
    [Header("-----------------------------------")]
    [Header(" OBJECT PRE-LOADING")]
    [Space]
    public List<GameObjectPreloading> PreloadGameObjects;
    [Space]
    public List<WorldObjectPreloading> PreloadWorldObjects;
    [Space]
    public List<ProjectilesPreloading> PreloadProjectiles;
    [Space]
    public List<ParticlesPreloading> PreloadParticles;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum Team { Undefined, Defending, Attacking }

    [HideInInspector]
    public List<Player> Players { get; set; }

    [HideInInspector]
    public List<Selectable> Selectables { get; set; }

    [System.Serializable]
    public struct GameObjectPreloading {

        public GameObject gameObject;
        public int size;
    }
    [System.Serializable]
    public struct WorldObjectPreloading {

        public WorldObject worldObject;
        public int size;
    }

    [System.Serializable]
    public struct ProjectilesPreloading {

        public Projectile projectile;
        public int size;
    }

    [System.Serializable]
    public struct ParticlesPreloading {

        public ParticleSystem particle;
        public int size;
    }

    public static GameManager Instance;
    
    private int _LabratoryCount = 0;
    private bool _ManuallyControllingAUnit = false;
    private bool _GameOver = false;
    private bool _MatchVictory = false;
    private bool _GameIsPaused = false;
    private xb_gamepad _Gamepad;
    
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
        Input.mousePosition.Set(Screen.width / 2, Screen.height / 2,0);
        // Initialize singleton
        if (Instance != null && Instance != this) {

            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // Initialize lists
        Selectables = new List<Selectable>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {
        _Gamepad = GamepadManager.Instance.GetGamepad(1);
        // Get all player entities
        Players = new List<Player>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in objs) {

            Players.Add(item.GetComponent<Player>());
        }

        // Preload objects
        foreach (var pGameObj in PreloadGameObjects) { ObjectPooling.PreLoad(pGameObj.gameObject, pGameObj.size); }
        foreach (var pObj in PreloadWorldObjects)   { ObjectPooling.PreLoad(pObj.worldObject.gameObject, pObj.size); }
        foreach (var pProj in PreloadProjectiles)   { ObjectPooling.PreLoad(pProj.projectile.gameObject, pProj.size); }
        foreach (var pParticle in PreloadParticles) { ObjectPooling.PreLoad(pParticle.particle.gameObject, pParticle.size); }

        if (StartingBase != null) {

            // Set camera starting position behind the starting base's position
            Players[0].PlayerCamera.transform.position = new Vector3(StartingBase.transform.position.x, Settings.MaxCameraHeight, StartingBase.transform.position.z - 100);

            // Initialize starting base
            StartingBase.SetPlayer(Players[0]);
            StartingBase.CreateHealthBar(StartingBase, StartingBase._Player.PlayerCamera);
            StartingBase.CreateQueueWidget();
            StartingBase.CreateRallyPoint();
        }        
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {
        EnableControls();
        // Update menu type
        if (!_IsRadialMenu) { SelectionWheel = selectionWindow; }
    }

    private void EnableControls()
    {
        if(Input.anyKey)
        {
            Players[0]._KeyboardInputManager.enabled = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called once when the game is over
    /// </summary>
    public void OnGameover() {

        Time.timeScale = 0f;
        _GameOver = true;

        if (GameOverWidget != null) {

            // Show widget and play game over UI animation
            GameOverWidget.gameObject.SetActive(true);
            GameOverWidget.OnGameOver();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public xb_gamepad GetGamepad()
    {
        return _Gamepad;
    }

    /// <summary>
    //  
    /// </summary>
    public void OnPause() {

        Time.timeScale = 0f;
        _GameIsPaused = true;

        // Show pause widget
        if (PauseWidget != null) { PauseWidget.gameObject.SetActive(true); }

        // Hide ingame HUD
        if (HUDWrapper != null) { HUDWrapper.gameObject.SetActive(false); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnUnpause() {

        Time.timeScale = 1f;
        _GameIsPaused = false;

        // Hide pause widget
        if (PauseWidget != null) { PauseWidget.gameObject.SetActive(false); }

        // Show ingame HUD
        if (HUDWrapper != null) { HUDWrapper.gameObject.SetActive(true); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  
    /// </returns>
    public bool IsGamePause() { return _GameIsPaused; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void AddLabratoryActiveInWorld() { _LabratoryCount++; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void RemovedLabratoryFromWorld() { _LabratoryCount--; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public bool GetIsLabratoryActive() { return _LabratoryCount > 0; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void RebakeGroundNavmesh() {

        // Find a walkable surface, bake then repeat for next surface
        foreach (var surface in GameObject.FindGameObjectsWithTag("Ground")) { surface.GetComponent<NavMeshSurface>().BuildNavMesh(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="value"></param>
    public void SetUnitControlling(bool value) { _ManuallyControllingAUnit = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public bool GetIsUnitControlling() { return _ManuallyControllingAUnit; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetGameOverState() { return _GameOver; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetMatchVictory() { return _MatchVictory; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}