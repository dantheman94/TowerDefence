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
//  Last edited on: 27/7/2018
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

    [Space]
    [Header("-----------------------------------")]
    [Header(" CONTROLLER SETUP")]
    [Space]
    public InputController _CurrentController = InputController.Keyboard;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    // Input
    public enum InputController { Keyboard, PSGamepad, XboxGamepad }
    public KeyboardInput _KeyboardInputManager { get; private set; }
    public XboxGamepadInput _XboxGamepadInputManager { get; private set; }
    public CameraFollow _CameraFollow { get; private set; }
    public BuildingSlot SelectedBuildingSlot { get; set; }
    public List<Selectable> SelectedWorldObjects { get; set; }

    // Economy
    public int MaxPopulation { get; set; }
    public int PopulationCount { get; set; }
    public float SuppliesCount { get; set; }
    public float MaxSupplyCount { get; set; }
    public float PowerCount { get; set; }
    public float MaxPowerCount { get; set; }
    public int Level { get; set; }
    private int Score = 0;
    private int WavesSurvived = 0;

    // Army
    private List<WorldObject> _Army;
    private List<Platoon> _Platoons;
    const int _PlatoonCount = 10;
    
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
        _KeyboardInputManager = GetComponent<KeyboardInput>();
        _XboxGamepadInputManager = GetComponent<XboxGamepadInput>();
        _HUD = GetComponent<HUD>();
        _CameraFollow = GetComponent<CameraFollow>();

        // Initialize new player entity
        Score = 0;
        WavesSurvived = 0;

        SuppliesCount = GameManager.Instance.StartingSupplyCount;
        PowerCount = GameManager.Instance.StartingPowerCount;
        MaxSupplyCount = GameManager.Instance.StartingMaxSupplyCount;
        MaxPowerCount = GameManager.Instance.StartingMaxPowerCount;
        Level = GameManager.Instance.StartingTechLevel;

        PopulationCount = 0;
        MaxPopulation = GameManager.Instance.StartingMaxPopulation;
        SelectedWorldObjects = new List<Selectable>();

        // Initialize controller
        switch (_CurrentController) {

            case InputController.Keyboard: { _KeyboardInputManager.IsPrimaryController = true; break; }

            case InputController.PSGamepad: { break; }

            case InputController.XboxGamepad: { _XboxGamepadInputManager.IsPrimaryController = true; break; }

            default: break;
        }

        // Create army
        _Army = new List<WorldObject>();

        // Create platoons
        _Platoons = new List<Platoon>();
        for (int i = 0; i < _PlatoonCount; i++) { _Platoons.Add(GameManager.Instance.PlatoonUnitsHUD.UnitInfoPanels[i].GetComponent<Platoon>()); }
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
        _Army.Add(squad);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unit"></param>
    public void AddToPopulation(Unit unit) {

        // Add to population
        PopulationCount += unit.CostPopulation;
        _Army.Add(unit);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<WorldObject> GetArmy() { return _Army; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //   
    /// </summary>
    /// <returns></returns>
    public Platoon GetPlatoon(int i) { return _Platoons[i]; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}