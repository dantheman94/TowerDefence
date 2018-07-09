﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 7/7/2018
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
    [Header(" START OF MATCH")]
    [Space]
    public int StartingSupplyCount;
    public int StartingPowerCount;
    public int StartingPlayerLevel;
    public int StartingMaxPopulation;

    [Space]
    [Header("-----------------------------------")]
    [Header(" HUD")]
    [Space]
    public GameObject SelectionWheel;
    public GameObject AbilityWheel;
    public GameObject ConfirmRecycleScreen;
    public GameObject UnitHealthBar;
    public GameObject BuildingInProgressPanel;
    public GameObject RecycleBuilding;
    public GameObject ObjectSelected;
    public GameObject AgentSeekObject;
    public Canvas WorldSpaceCanvas;
    public Canvas ScreenSpaceCanvas;

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
    public void RebakeNavmesh() {

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