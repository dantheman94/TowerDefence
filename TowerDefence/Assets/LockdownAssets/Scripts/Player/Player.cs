using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 3/08/2018
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

    //Game Settings
    [HideInInspector]
    public string Difficulty;
    [HideInInspector]
    public string Outcome;
    public string Name = "Player";
    

    // Input
    public enum InputController { Keyboard, PSGamepad, XboxGamepad }
    public KeyboardInput _KeyboardInputManager { get; private set; }
    public XboxGamepadInput _XboxGamepadInputManager { get; private set; }
    public CameraFollow _CameraFollow { get; private set; }
    public BuildingSlot SelectedBuildingSlot { get; set; }
    public List<Selectable> SelectedWorldObjects { get; set; }
    public List<Unit> SelectedUnits { get; set; }

    // Economy
    public int MaxPopulation { get; set; }
    public int PopulationCount { get; set; }
    public float SuppliesCount { get; set; }
    public float MaxSupplyCount { get; set; }
    public float PowerCount { get; set; }
    public float MaxPowerCount { get; set; }
    public int Level { get; set; }
    private int _Score = 0;
    private int _WavesSurvived = 0;
    private UpgradeManager _UpgradeManager;

    // Army
    private List<Base> _Bases;
    private List<Unit> _Army;
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
        _Score = 0;
        _WavesSurvived = 0;

        SuppliesCount = GameManager.Instance.StartingSupplyCount;
        PowerCount = GameManager.Instance.StartingPowerCount;
        MaxSupplyCount = GameManager.Instance.StartingMaxSupplyCount;
        MaxPowerCount = GameManager.Instance.StartingMaxPowerCount;
        Level = GameManager.Instance.StartingTechLevel;

        PopulationCount = 0;
        MaxPopulation = GameManager.Instance.StartingMaxPopulation;

        _UpgradeManager = GetComponent<UpgradeManager>();

        // Initialize controller
        switch (_CurrentController) {

            case InputController.Keyboard: { _KeyboardInputManager.IsPrimaryController = true; break; }

            case InputController.PSGamepad: { break; }

            case InputController.XboxGamepad: { _XboxGamepadInputManager.IsPrimaryController = true; break; }

            default: break;
        }

        // Create army
        SelectedWorldObjects = new List<Selectable>();
        SelectedUnits = new List<Unit>();
        _Army = new List<Unit>();
        _Bases = new List<Base>();

        // Create platoons
        _Platoons = new List<Platoon>();
        for (int i = 0; i < _PlatoonCount; i++) { _Platoons.Add(GameManager.Instance.PlatoonUnitsHUD.UnitInfoPanels[i].GetComponent<Platoon>()); }

        // Setup camera
        if (PlayerCamera != null) { PlayerCamera.GetComponent<CameraPlayer>().SetPlayerAttached(this); }

        // Initialize starting base
        _Bases.Add(GameManager.Instance.StartingBase);
        GameManager.Instance.StartingBase.AttachedBuildingSlot.SetBuildingOnSlot(GameManager.Instance.StartingBase);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        if (!GameManager.Instance._CinematicInProgress) {

            // If there are no bases in the player's list >> Lose the match
            if (_Bases.Count == 0) { GameManager.Instance.OnGameover(false); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void DeselectAllObjects() {

        // Set all objects to NOT selected
        foreach (var obj in SelectedWorldObjects) { obj.SetIsSelected(false); }
        foreach (var obj in SelectedUnits) { obj.SetIsSelected(false); }

        // Clear the list
        if (SelectedWorldObjects.Count > 0) { SelectedWorldObjects.Clear(); }
        if (SelectedUnits.Count > 0) { SelectedUnits.Clear(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="selectable"></param>
    public void RemoveFromSelection(Selectable selectable) {

        SelectedWorldObjects.Remove(selectable);
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
    //  Removes an army from the player's population
    /// </summary>
    /// <param name="ai"></param>
    public void RemoveFromArmy(Unit unit) {

        if (unit != null) {

            // Remove if from army array
            _Army.Remove(unit);

            // Remove it from any assigned groups
            for (int i = 0; i < _Platoons.Count; i++) { _Platoons[i].GetAi().Remove(unit); }

            // Deduct population cost
            PopulationCount -= unit.CostPopulation;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns></returns>
    public List<Unit> GetArmy() { return _Army; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //   
    /// </summary>
    /// <returns></returns>
    public Platoon GetPlatoon(int i) { return _Platoons[i]; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets waves survived
    /// </summary>
    /// <returns></returns>
    public int GetWavesSurvived() { return _WavesSurvived; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets score
    /// </summary>
    /// <returns></returns>
    public int GetScore() { return _Score; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="newBase"></param>
    public void AddBase(Base newBase) { _Bases.Add(newBase); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  List<Base>
    /// returns>
    public List<Base> GetBaseList() { return _Bases; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="list"></param>
    public void SetBaseList(List<Base> list) { _Bases = list; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Returns reference to the Upgrade Manager attached to this player.
    /// </summary>
    /// <returns>
    //  UpgradeManager
    /// </returns>
    public UpgradeManager GetUpgradeManager() { return _UpgradeManager; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}