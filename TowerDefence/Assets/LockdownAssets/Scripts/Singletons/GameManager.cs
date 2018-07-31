using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 27/7/2018
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
    [Header(" HEADS UP DISPLAY")]
    [Space]
    public GameObject HUDWrapper;
    public Canvas WorldSpaceCanvas;
    public Canvas ScreenSpaceCanvas;
    [Space]
    public CinematicBars CinematicBars;
    [Space]
    public bool _IsRadialMenu = false;
    public GameObject SelectionWheel;
    public GameObject selectionWindow;
    public GameObject ConfirmRecycleScreen;
    public GameObject AbilityWheel;
    [Space]
    public GameObject UnitHealthBar;
    public GameObject BuildingInProgressPanel;
    public GameObject CaptureProgressPanel;
    [Space]
    public GameObject RecycleBuilding;
    public GameObject ObjectSelected;
    public GameObject ObjectHighlighted;
    public GameObject AgentSeekObject;
    [Space]
    public UI_SelectedUnits SelectedUnitsHUD;
    public UI_PlatoonUnits PlatoonUnitsHUD;

    [Space]
    [Header("-----------------------------------")]
    [Header(" OBJECT PRE-LOADING")]
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

        // Initialize lists
        Selectables = new List<Selectable>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Get all player entities
        Players = new List<Player>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in objs) {

            Players.Add(item.GetComponent<Player>());
        }

        // Preload objects
        foreach (var pObj in PreloadWorldObjects)   { ObjectPooling.PreLoad(pObj.worldObject.gameObject, pObj.size); }
        foreach (var pProj in PreloadProjectiles)   { ObjectPooling.PreLoad(pProj.projectile.gameObject, pProj.size); }
        foreach (var pParticle in PreloadParticles) { ObjectPooling.PreLoad(pParticle.particle.gameObject, pParticle.size); }

        if (StartingBase != null) {

            // Set camera starting position behind the starting base's position
            Players[0].PlayerCamera.transform.position = new Vector3(StartingBase.transform.position.x, Settings.MaxCameraHeight, StartingBase.transform.position.z - 100);
            ///Players[0].PlayerCamera.transform.rotation = StartingBase.transform.rotation

            // Initialize starting base
            StartingBase.SetPlayer(Players[0]);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        // Update menu type
        if (!_IsRadialMenu) { SelectionWheel = selectionWindow; }
    }

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

}