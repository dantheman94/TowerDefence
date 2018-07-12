using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/5/2018
//
//******************************

public class Player : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" CAMERA SETUP")]
    [Space]
    public Camera PlayerCamera = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" TEAM SETUP")]
    [Space]
    public PlayerIndex Index = PlayerIndex.One;
    public GameManager.Team Team = GameManager.Team.Defending;
    public Color TeamColor = Color.cyan;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    // Input
    public UserInput _Input { get; private set; }
    public CameraFollow _CameraFollow { get; private set; }
    public BuildingSlot SelectedBuildingSlot { get; set; }
    public List<Selectable> SelectedWorldObjects { get; set; }

    // Economy
    public int MaxPopulation { get; set; }
    public int PopulationCount { get; set; }
    public int SuppliesCount { get; set; }
    public int PowerCount { get; set; }
    public int Level { get; set; }
    private int Score = 0;
    private int WavesSurvived = 0;

    // Army
    private List<WorldObject> AiUnitObjects;
    private List<WorldObject> Platoon1Objects;
    private List<WorldObject> Platoon2Objects;
    private List<WorldObject> Platoon3Objects;
    private List<WorldObject> Platoon4Objects;
    private List<WorldObject> Platoon5Objects;
    private List<WorldObject> Platoon6Objects;
    private List<WorldObject> Platoon7Objects;
    private List<WorldObject> Platoon8Objects;
    private List<WorldObject> Platoon9Objects;
    private List<WorldObject> Platoon0Objects;

    // HUD
    public HUD _HUD { get; private set; }

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Get component references
        _Input = GetComponent<UserInput>();
        _HUD = GetComponent<HUD>();
        _CameraFollow = GetComponent<CameraFollow>();

        // Initialize new player entity
        Score = 0;
        WavesSurvived = 0;

        SuppliesCount = GameManager.Instance.StartingSupplyCount;
        PowerCount = GameManager.Instance.StartingPowerCount;
        Level = GameManager.Instance.StartingTechLevel;

        PopulationCount = 0;
        MaxPopulation = GameManager.Instance.StartingMaxPopulation;
        SelectedWorldObjects = new List<Selectable>();

        // Create armies
        AiUnitObjects = new List<WorldObject>();
        Platoon1Objects = new List<WorldObject>();
        Platoon2Objects = new List<WorldObject>();
        Platoon3Objects = new List<WorldObject>();
        Platoon4Objects = new List<WorldObject>();
        Platoon5Objects = new List<WorldObject>();
        Platoon6Objects = new List<WorldObject>();
        Platoon7Objects = new List<WorldObject>();
        Platoon8Objects = new List<WorldObject>();
        Platoon9Objects = new List<WorldObject>();
        Platoon0Objects = new List<WorldObject>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void DeselectAllObjects() {

        // Set all objects to NOT selected
        foreach (var obj in SelectedWorldObjects) { obj.SetIsSelected(false); }

        // Clear the list
        if (SelectedWorldObjects.Count > 0) { SelectedWorldObjects.Clear(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void AddToPopulation(Squad squad) {

        // Add to population
        PopulationCount += squad.CostPopulation;
        AiUnitObjects.Add(squad);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unit"></param>
    public void AddToPopulation(Unit unit) {

        // Add to population
        PopulationCount += unit.CostPopulation;
        AiUnitObjects.Add(unit);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetArmy() { return AiUnitObjects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //   
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon1() { return Platoon1Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon2() { return Platoon2Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon3() { return Platoon3Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon4() { return Platoon4Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon5() { return Platoon5Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon6() { return Platoon6Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon7() { return Platoon7Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon8() { return Platoon8Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon9() { return Platoon9Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetPlatoon10() { return Platoon0Objects; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
