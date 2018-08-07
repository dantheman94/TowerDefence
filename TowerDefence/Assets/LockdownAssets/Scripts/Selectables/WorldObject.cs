using System.Collections;
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

public class WorldObject : Selectable {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" WORLD OBJECT STATES")]
    [Space]
    [Tooltip("Reference to the gameobject that represents this object when its in its 'Building' state. " +
            "\n\nNOTE:  Upgraded versions of the base object get this variable referenced at runtime and will overwrite this for you.")]
    public GameObject BuildingState;
    [Tooltip("Reference to the gameobject that represents this object when its in its 'active' state.")]
    public GameObject ActiveState;
    [Tooltip("Reference to the gameobject that represents this object when its in its 'destroyed' state.")]
    public GameObject DestroyedState;
    [Space]
    [Header("-----------------------------------")]
    [Header(" WORLD OBJECT PROPERTIES")]
    [Space]
    [Tooltip("The amount of time in SECONDS that this object takes to build.")]
    public int BuildTime = 20;
    [Space]
    [Tooltip("How much supply resource are required to build this object.")]
    public int CostSupplies = 0;
    [Tooltip("How much power resource are required to build this object.")]
    public int CostPower = 0;
    [Tooltip("Minimum player tech level required to build this object.")]
    public int CostTechLevel = 1;
    [Tooltip("The army population size needed for this object to be built.")]
    public int CostPopulation = 0;
    [Space]
    [Tooltip("How much supply resources are returned when this object is recycled?")]
    public int RecycleSupplies = 0;
    [Tooltip("How much power resources are returned when this object is recycled?")]
    public int RecyclePower = 0;
    [Space]
    public int MaxHitPoints = 100;
    public int MaxShieldPoints = 0;
    [Space]
    [Tooltip("Can this object be garrisoned by ground infantry units?")]
    public bool Garrisonable = false;
    [Tooltip("How much army population can this object hold? " +
            "\n\nNOTE: Only valid if 'Garrisonable' is TRUE.")]
    public int MaxGarrisonPopulation = 0;
    [Space]
    [Tooltip("Can this object be selected at the same time as other world objects?")]
    public bool MultiSelectable = true;
    [Tooltip("Uh just leave this variable empty for now. May be obsolete, but not yet... (IDK, Dan's fault :/)")]
    public float _OffsetY;
    [Tooltip("The current game state of the object. " +
        "\n\nDEFAULT = Any disabled objects that are going to be instantiated at runtime " +
        "\n\nBUILDING = The object is active in the world, but is still being built. " +
        "\n\nDEPLOYABLE = The object has been built but is locked/hidden in its building (IE: AI Units). " +
        "\n\nACTIVE = The object is active/interactable within the game world.")]
    public WorldObjectStates _ObjectState = WorldObjectStates.Default;
    [Space]
    [Tooltip("The 'width' of the RectTransform that represents the healthbar tied to this object.")]
    public float _WidgetHealthbarScaleX = 100f;
    [Tooltip("The 'height' of the RectTransform that represents the healthbar tied to this object.")]
    public float _WidgetHealthbarScaleY = 15f;
    [Tooltip("The 'Pos Y' of the RectTransform that represents the healthbar tied to this object.")]
    public float _WidgetHealthbarOffset = 15f;
    [Space]
    [Tooltip("The 'width' of the RectTransform that represents the shieldbar tied to this object.")]
    public float _WidgetShieldbarScaleX = 100f;
    [Tooltip("The 'height' of the RectTransform that represents the shieldbar tied to this object.")]
    public float _WidgetShieldbarScaleY = 15f;
    [Tooltip("The 'Pos Y' of the RectTransform that represents the shieldbar tied to this object.")]
    public float _WidgetShieldbarOffset = 22f;
    [Space]
    public bool ShowBuildingQueueUI;
    [Space]
    [Tooltip("When this unit is killed, the speed in which it shrinks down until it is no longer visible " +
            "before being sent back to the object pool.")]
    public float ShrinkSpeed = 0.2f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum WorldObjectStates { Default, Building, Deployable, Active, Destroyed, ENUM_COUNT }

    protected bool _ReadyForDeployment = false;
    protected float _CurrentBuildTime = 0f;
    protected UnitHealthBar _HealthBar = null;
    protected UnitBuildingCounter _BuildingProgressCounter = null;
    protected WorldObject _ClonedWorldObject = null;
    protected float _Health;
    protected int _CurrentGarrisonPopulation = 0;
    protected float _ObjectHeight = 0f;
    protected float _HitPoints;
    protected int _ShieldPoints;
    protected float _Shield;
    protected bool _StartShrinking = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        // Initialize health
        _HitPoints = MaxHitPoints;
        _ShieldPoints = MaxShieldPoints;

        // Get vertical offset based off the prefab template
        _OffsetY = transform.position.y;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        switch (_ObjectState) {

            case WorldObjectStates.Default: {

                // Hide meshes
                if (BuildingState) { BuildingState.SetActive(false); }
                if (ActiveState) { ActiveState.SetActive(false); }
                if (DestroyedState) { DestroyedState.SetActive(false); }
                break;
            }

            case WorldObjectStates.Building: {

                // Is unit building complete?
                _ReadyForDeployment = _CurrentBuildTime >= BuildTime;
                if (!_ReadyForDeployment) {

                    // Add to building timer
                    if (_CurrentBuildTime < BuildTime) { _CurrentBuildTime += Time.deltaTime; }
                } else {

                    // Object has finished building
                    OnBuilt();
                    _ObjectState = WorldObjectStates.Deployable;
                }

                // Show building state object
                if (BuildingState) { BuildingState.SetActive(true); }
                if (ActiveState) { ActiveState.SetActive(false); }
                if (DestroyedState) { DestroyedState.SetActive(false); }
                break;
            }

            case WorldObjectStates.Deployable: { break; }

            case WorldObjectStates.Active: {

                // Show active state object
                if (BuildingState) { BuildingState.SetActive(false); }
                if (ActiveState) { ActiveState.SetActive(true); }
                if (DestroyedState) { DestroyedState.SetActive(false); }
                break;
            }

            case WorldObjectStates.Destroyed: {


                // Show destroyed state object
                if (BuildingState) { BuildingState.SetActive(false); }
                if (ActiveState) { ActiveState.SetActive(false); }
                if (DestroyedState) { DestroyedState.SetActive(true); }
                break;
            }
                                
            default: break;
        }

        // Update shield to be a normalized range of the object's shield-points
        if (_ShieldPoints > 0) { _Shield = _ShieldPoints / MaxShieldPoints; } else {

            // Clamp the shield
            _ShieldPoints = 0;
            _Shield = 0f;
        }

        // Update health to be a normalized range of the object's hitpoints
        if (_HitPoints > 0) { _Health = _HitPoints / MaxHitPoints; }
        else {

            // Clamping health
            _HitPoints = 0;
            _Health = 0f;

            if (_ObjectState != WorldObjectStates.Default) {

                // Kill the object
                ///OnDeath();
            }
        }
        
        // Gradually shrink the character then despawn it once its dead
        UpdateDeathShrinker();
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called every frame. Scales down the unit's transform if its dead &
    //  Despawn when finished.
    /// </summary>
    private void UpdateDeathShrinker() {
        
        // Check if the unit should be shrinking
        if (_StartShrinking && !IsAlive()) {

            // Get in the cold ass water
            transform.localScale -= Vector3.one * ShrinkSpeed * Time.deltaTime;
            if (transform.localScale.x < 0.01f) {

                // MAXIMUM shrinkage
                _StartShrinking = false;

                // This is so that the next time the object is spawned - it is at its default state already
                _ObjectState = WorldObjectStates.Default;

                // Despawn the object
                transform.localScale = new Vector3(1f, 1f, 1f);
                ObjectPooling.Despawn(gameObject);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void DrawSelectionWheel() {

        // Get selection wheel reference
        SelectionWheel selectionWheel = null;
        if (GameManager.Instance._IsRadialMenu) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
        else { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }

        // Set building queue UI visiblity
        selectionWheel.BuildingQueue.gameObject.SetActive(ShowBuildingQueueUI);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public override void CalculateBounds() {
        base.CalculateBounds();

        selectionBounds = new Bounds(transform.position, Vector3.zero);

        foreach (Renderer r in GetComponentsInChildren<Renderer>()) {

            selectionBounds.Encapsulate(r.bounds);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is "clicked on" and the selection wheel appears.
    /// </summary>
    public virtual void OnSelectionWheel() {

        // Get selection wheel reference
        SelectionWheel selectionWheel = null;
        if (GameManager.Instance._IsRadialMenu) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
        else                                    { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }

        // Set building queue UI visiblity
        selectionWheel.BuildingQueue.gameObject.SetActive(ShowBuildingQueueUI);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public virtual void OnWheelSelect(BuildingSlot buildingSlot) {

        // Deselect
        buildingSlot._IsCurrentlySelected = false;
        this._IsCurrentlySelected = false;

        // Get player reference
        Player plyr;
        if (_Player) { plyr = _Player; } else { plyr = GameManager.Instance.Players[0]; }

        // Check if the player has enough resources to build the object
        if ((plyr.SuppliesCount >= this.CostSupplies) && (plyr.PowerCount >= this.CostPower)) {

            // Start building object on the selected building slot
            _ClonedWorldObject = ObjectPooling.Spawn(gameObject, Vector3.zero, Quaternion.identity).GetComponent<WorldObject>();
            _ClonedWorldObject.SetBuildingPosition(buildingSlot);
            _ClonedWorldObject.gameObject.SetActive(true);
            _ClonedWorldObject._ObjectState = WorldObjectStates.Building;

            // Create healthbar
            CreateHealthBar(_ClonedWorldObject, plyr.PlayerCamera);

            // Create building progress panel & allocate it to the unit
            GameObject buildProgressObj = ObjectPooling.Spawn(GameManager.Instance.BuildingInProgressPanel.gameObject);
            _BuildingProgressCounter = buildProgressObj.GetComponent<UnitBuildingCounter>();
            _BuildingProgressCounter.setObjectAttached(_ClonedWorldObject);
            _BuildingProgressCounter.setCameraAttached(plyr.PlayerCamera);
            buildProgressObj.gameObject.SetActive(true);
            buildProgressObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);

            // Deduct resources from player
            _ClonedWorldObject.SetPlayer(plyr);
            plyr.SuppliesCount -= _ClonedWorldObject.CostSupplies;
            plyr.PowerCount -= _ClonedWorldObject.CostPower;

            // Set object's properties
            _ClonedWorldObject.Team = plyr.Team;
            _ClonedWorldObject._IsCurrentlySelected = false;
            _CurrentBuildTime = BuildTime;

            if (ShowBuildingQueueUI) {

                // Add to building queue UI
                bool radialWheeel = GameManager.Instance._IsRadialMenu;
                SelectionWheel selectionWheel = null;
                if (radialWheeel) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
                else { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }
                selectionWheel.BuildingQueue.AddToQueue(_ClonedWorldObject);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object's state switches to active (Only once)
    /// </summary>
    protected virtual void OnBuilt() {

        // Send message to match feed
        MatchFeed.Instance.AddMessage(string.Concat(ObjectName, " built."));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Damages the object by a set amount.
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage) {

        // Damage object & kill it if theres no health left
        _HitPoints -= damage;
        if (_HitPoints <= 0) { OnDeath(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is killed/destroyed
    /// </summary>
    public virtual void OnDeath() {

        // Set object state
        _ObjectState = WorldObjectStates.Destroyed;

        // Destroy healthbar
        ObjectPooling.Despawn(_HealthBar.gameObject);

        // Clamping health
        _HitPoints = 0;
        _Health = 0f;

        // Delay then despawn
        StartCoroutine(DelayedShrinking(3f));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits for the seconds specified then
    //  starts the shrinking process before despawning.
    /// </summary>
    /// <param name="delay"></param>
    protected IEnumerator DelayedShrinking(float delay) {

        // Delay
        yield return new WaitForSeconds(delay);
        
        // Start shrinking
        _StartShrinking = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns TRUE if the object's health is greater than 0f.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsAlive() { return _HitPoints > 0f && _Health > 0f; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the position of the object based on what building slot has been passed through. 
    /// </summary>
    /// <param name="buildingSlot"><
    //  The building slot that is being used as reference for positioning.
    /// /param>
    protected void SetBuildingPosition(BuildingSlot buildingSlot) {

        // Initial transform update
        transform.SetPositionAndRotation(buildingSlot.transform.position, buildingSlot.transform.rotation);

        // Add offset
        transform.position = new Vector3(transform.position.x, _OffsetY, transform.position.z);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void CreateHealthBar(WorldObject thisObject, Camera camera) {

        // Create a health bar and allocate it to the unit
        GameObject healthBarObj = ObjectPooling.Spawn(GameManager.Instance.UnitHealthBar.gameObject);
        _HealthBar = healthBarObj.GetComponent<UnitHealthBar>();
        _HealthBar.SetObjectAttached(thisObject);
        _HealthBar.SetCameraAttached(camera);
        healthBarObj.gameObject.SetActive(true);
        healthBarObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);

        // Set healthbar widget size & anchoring
        RectTransform healthRectTransform = _HealthBar._HealthSlider.GetComponent<RectTransform>();
        healthRectTransform.sizeDelta = new Vector2(_WidgetHealthbarScaleX, _WidgetHealthbarScaleY);
        healthRectTransform.anchoredPosition = new Vector2(0, _WidgetHealthbarOffset);
        RectTransform shieldRectTransform = _HealthBar._ShieldSlider.GetComponent<RectTransform>();
        shieldRectTransform.sizeDelta = new Vector2(_WidgetShieldbarScaleX, _WidgetShieldbarScaleY);
        shieldRectTransform.anchoredPosition = new Vector2(0, _WidgetShieldbarOffset);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="healthBar"></param>
    public void SetHealthBar(UnitHealthBar healthBar) { _HealthBar = healthBar; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns TRUE if the object is either building or active in the game world.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsInWorld() { return IsAlive() && (_ObjectState == WorldObjectStates.Active || _ObjectState == WorldObjectStates.Building); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns TRUE if the object is alive/active in the game world.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsBuilt() { return IsAlive() && _ObjectState == WorldObjectStates.Active; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="value"></param>
    public void SetHitPoints(float value) { _HitPoints = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the object's current hitpoints as a raw value.
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public float GetHitPoints() { return _HitPoints; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Adds argument value to current health.
    /// </summary>
    ///<param name = "additionalHealth"</param>
    public void AddHitPoints(float additionalHealth) { _HitPoints += additionalHealth; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Subs argument value from current health.
    /// </summary>
    /// <param name="health"></param>
    public void SubHitPoints(float health) { _HitPoints -= health; }

    /// <summary>
    //  Returns the hitpoints as a normalized value. (0.0f - 1.0f [ % ])
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetHealth() { return _Health; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the object's current shield-points as a raw value.
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int GetShieldPoints() { return _ShieldPoints; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the shield-points as a normalized value. (0.0f - 1.0f [ % ])
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetShield() { return _Shield; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetCurrentBuildTimeRemaining() { return BuildTime - _CurrentBuildTime; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the current object state (Ie: Building, Deployable, Active).
    /// </summary>
    /// <returns>
    //  ENUM: WorldObjectState
    /// </returns>
    public WorldObjectStates GetObjectState() { return _ObjectState; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the new current object state (Ie: Building, Deployable, Active).
    /// </summary>
    /// <param name="newState"></param>
    public void SetObjectState(WorldObjectStates newState) { _ObjectState = newState; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int GetCurrentGarrisonCount() { return _CurrentGarrisonPopulation; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the current height value of the object (used for AI attacking offsets
    //  so they don't shoot at the target's position but actually at the target's 'chest').
    //
    //  (For buildings, the height should be '0' by default & units auto assign this 
    //  value based off their specified agent height).
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetObjectHeight() { return _ObjectHeight; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}