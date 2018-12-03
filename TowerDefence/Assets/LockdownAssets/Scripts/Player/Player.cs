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
//  Last edited on: 17/10/2018
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
    public Camera CameraAttached = null;
    public CameraPlayer CameraRTS = null;

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
    [Space]
    public Texture2D CursorDefault = null;
    public Color CursorColourDefault = Color.white;
    [Space]
    public Texture2D CursorFriendly = null;
    public Color CursorColourFriendly = Color.cyan;
    [Space]
    public Texture2D CursorEnemy = null;
    public Color CursorColourEnemy = Color.red;

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
    public static Rect SelectionScreen = new Rect(0, 0, Screen.width, Screen.height);
    public Texture2D _CurrentCursor { get; set; }

    // Economy
    public int MaxPopulation { get; set; }
    public int PopulationCount { get; set; }
    public float SuppliesCount { get; set; }
    public float MaxSupplyCount { get; set; }
    public float PowerCount { get; set; }
    public float MaxPowerCount { get; set; }
    public int Level { get; set; }
    private UpgradeManager _UpgradeManager;
    private ResourceManager _ResourceManager;

    // Leaderboard stats
    private int _Score = 0;
    private int _WavesSurvived = 0;
    private int _EnemiesKilled = 0;
    private int _UnitsProduced = 0;
    private int _UnitsLost = 0;
    private int _BuildingBuilt = 0;
    private int _BuildingsDestroyed = 0;
    public enum ScoreType { EnemyKilled, BaseDestroyed, BuildingDestroyed, SpireDestroyed, UpgradedBase, BuildingBuilt, WaveDefeated }
    
    // Army
    private List<Base> _Bases;
    private List<Unit> _Army;
    private List<Platoon> _Platoons;
    const int _PlatoonCount = 9;

    // Global rally point
    private bool _GlobalRallyPointActive = false;
    private Vector3 _GlobalRallyPointPosition = Vector3.zero;
    private GameObject _GlobalRallyFlag = null;
    
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
        Name = InstanceManager.Instance.PlayerName;

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
        _ResourceManager = GetComponent<ResourceManager>();

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
        for (int i = 0; i < _PlatoonCount; i++) { _Platoons.Add(GameManager.Instance.PlatoonUnitsHUD.PlatoonInfoPanels[i].GetComponent<Platoon>()); }

        // Setup camera
        if (CameraAttached != null) { CameraAttached.GetComponent<CameraPlayer>().SetPlayerAttached(this); }

        // Initialize starting base
        _Bases.Add(GameManager.Instance.StartingBase);
        GameManager.Instance.StartingBase.AttachedBuildingSlot.SetBuildingOnSlot(GameManager.Instance.StartingBase);

        // Set default cursor/crosshair
        _CurrentCursor = CursorDefault;
        Cursor.SetCursor(_CurrentCursor, Vector2.zero, CursorMode.Auto);
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

    /// <summary>
    //  Returns reference to the Resource Manager attached to this player.
    /// </summary>
    /// <returns>
    //  ResourceManager
    /// </returns>
    public ResourceManager GetResourceManager() { return _ResourceManager; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Re-enables the player's global rally point to overwrite all the base rally points.
    /// </summary>
    /// <param name="position"></param>
    public void UpdateGlobalRallyPoint(Vector3 position) {

        _GlobalRallyPointPosition = position;
        if (_GlobalRallyFlag != null) { }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds to the player's score & updates the HUD text component.
    /// </summary>
    /// <param name="add"></param>
    public void AddToScore(int add, ScoreType scoreType) {

        _Score += add;

        // Update text in HUD
        if (_HUD.PlayerScoreText != null) { _HUD.PlayerScoreText.text = _Score.ToString(); }

        // Notification in the match feed UI
        GameManager.Instance.WaveStatsHUD.ScoreMessage(true, add, scoreType);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Subtracts from the player's score & updates the HUD text component.
    /// </summary>
    /// <param name="subtract"></param>
    public void SubstractFromScore(int subtract, ScoreType scoreType) {

        _Score -= subtract;
        if (_Score < 0) { _Score = 0; }

        // Update text in HUD
        if (_HUD.PlayerScoreText != null) { _HUD.PlayerScoreText.text = _Score.ToString(); }

        // Notification in the match feed UI
        GameManager.Instance.WaveStatsHUD.ScoreMessage(false, subtract, scoreType);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds to the internal counter for the enemies that are killed (leaderboards stats).
    /// </summary>
    public void AddUnitsKilled() { _EnemiesKilled += 1; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds to the internal counter for the friendly units that are built (leaderboards stats).
    /// </summary>
    public void AddUnitsProduced() { _UnitsProduced += 1; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds to the internal counter for the friendly units that are killed (leaderboards stats).
    /// </summary>
    public void AddUnitsLost() { _UnitsLost += 1; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds to the internal counter for the enemies that are killed (leaderboards stats).
    /// </summary>
    public void AddBuildingsBuilt() { _BuildingBuilt += 1; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds to the internal counter for the enemies that are killed (leaderboards stats).
    /// </summary>
    public void AddBuildingsDestroyed() { _BuildingsDestroyed += 1; }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void UnselectAllPlatoons() {

        // Loop through and unselect
        for (int i = 0; i < _Platoons.Count; i++) { _Platoons[i].SetSelected(false); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnitFromAllPlatoons(Unit unit) {

        // Loop through all the platoons and try to remove the unit from them
        for (int i = 0; i < _Platoons.Count; i++) { _Platoons[i].TryToRemoveUnit(unit); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    // <param name="colour"></param>
    // <param name="texture"></param>
    public void ChangeCursorProperties(Color colour, Texture2D texture) {

        // Update texture
        _CurrentCursor = texture;

        // Update texture pixel colours
        Color[] pixels = _CurrentCursor.GetPixels();
        for (int i = 0; i < pixels.Length - 1; i++) { pixels[i] = colour; }
        _CurrentCursor.Apply();

        // Update cursor
        Cursor.SetCursor(_CurrentCursor, Vector2.zero, CursorMode.Auto);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}