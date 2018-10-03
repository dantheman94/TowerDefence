using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 7/8/2018
//
//******************************

[System.Serializable]
public class WorldObject : Selectable {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header(" WORLD OBJECT STATES")]
    [Header("-----------------------------------")]
    [Space]
    [Tooltip("Reference to the gameobject that represents this object when its in its 'queuing' state.")]
    public GameObject InQueueState;
    [Tooltip("Reference to the gameobject that represents this object when its in its 'Building' state. " +
            "\n\nNOTE:  Upgraded versions of the base object get this variable referenced at runtime and will overwrite this for you.")]
    public GameObject BuildingState;
    [Tooltip("Reference to the gameobject that represents this object when its in its 'active' state.")]
    public GameObject ActiveState;
    [Tooltip("Reference to the gameobject that represents this object when its in its 'destroyed' state.")]
    public GameObject DestroyedState;

    [Space]
    [Header(" WORLD OBJECT PROPERTIES")]
    [Header("-----------------------------------")]
    [Space]
    public bool Damagable = true;
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

    [Space]
    [Header(" USER INTERFACE")]
    [Header("-----------------------------------")]
    [Space]
    public GameObject QuadMinimap = null;
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
    [Header(" ON DEATH/DESTROYED")]
    [Header("-----------------------------------")]
    [Space]
    public List<ParticleSystem> OnDeathEffects;
    public bool ShrinkWhenDestroyed = true;
    [Tooltip("When this unit is killed, the speed in which it shrinks down until it is no longer visible " +
            "before being sent back to the object pool.")]
    public float ShrinkSpeed = 0.2f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************


    protected bool _ReadyForDeployment = false;

    protected UnitHealthBar _HealthBar = null;
    protected UnitBuildingCounter _BuildingProgressCounter = null;

    protected bool _StartShrinking = false;
    protected int _CurrentGarrisonPopulation = 0;
    protected float _ObjectHeight = 0f;

    protected float _HitPoints;
    protected float _Health;
    protected float _ShieldPoints;
    protected float _Shield;
    
    private int _RecycleSupplies = 0;
    private int _RecyclePower = 0;

    protected WorldObject _ClonedWorldObject = null;
    private Renderer _MinimapQuadRenderer;

    protected bool _ShowHealthbar = true;

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
        
        // Get component references
        if (QuadMinimap != null) { _MinimapQuadRenderer = QuadMinimap.GetComponent<Renderer>(); }

        Init();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Initializes the object.
    /// </summary>
    protected virtual void Init() {

        // Initialize health
        _HitPoints = MaxHitPoints;
        _ShieldPoints = MaxShieldPoints;

        // Get vertical offset based off the prefab template
        _OffsetY = transform.position.y;

        // Set recycle values
        _RecycleSupplies = CostSupplies / 2;
        _RecyclePower = CostPower / 2;
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
                if (InQueueState) { InQueueState.SetActive(false); }
                if (BuildingState) { BuildingState.SetActive(false); }
                if (ActiveState) { ActiveState.SetActive(false); }
                if (DestroyedState) { DestroyedState.SetActive(false); }
                break;
            }

            case WorldObjectStates.InQueue: {

                // Show inqueue state object
                if (BuildingState) { BuildingState.SetActive(false); }
                if (InQueueState) { InQueueState.SetActive(true); }
                if (ActiveState) { ActiveState.SetActive(false); }
                if (DestroyedState) { DestroyedState.SetActive(false); }
                break;
            }

            case WorldObjectStates.Building: {

                // Is unit building complete?
                _ReadyForDeployment = _CurrentBuildTime >= BuildingTime;
                if (!_ReadyForDeployment) {

                    // Add to building timer
                    if (_CurrentBuildTime < BuildingTime) { _CurrentBuildTime += Time.deltaTime; }
                } else {

                    // Object has finished building
                    OnBuilt();
                    _ObjectState = WorldObjectStates.Deployable;
                }

                // Show building state object
                if (InQueueState) { InQueueState.SetActive(false); }
                if (BuildingState) { BuildingState.SetActive(true); }
                if (ActiveState) { ActiveState.SetActive(false); }
                if (DestroyedState) { DestroyedState.SetActive(false); }
                break;
            }

            case WorldObjectStates.Deployable: { break; }

            case WorldObjectStates.Active: {

                // Show active state object
                if (InQueueState) { InQueueState.SetActive(false); }
                if (BuildingState) { BuildingState.SetActive(false); }
                if (ActiveState) { ActiveState.SetActive(true); }
                if (DestroyedState) { DestroyedState.SetActive(false); }
                break;
            }

            case WorldObjectStates.Destroyed: {
                    
                // Show destroyed state object
                if (InQueueState) { InQueueState.SetActive(false); }
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

        // Change minimap colour based on attacking/defending & team colour
        if (_MinimapQuadRenderer != null) {

            // Attacking team colour
            if (Team == GameManager.Team.Attacking) { _MinimapQuadRenderer.material.color = WaveManager.Instance.AttackingTeamColour; }

            // Defending team
            else if (Team == GameManager.Team.Defending) {

                // Use individual player colour
                if (_Player) { _MinimapQuadRenderer.material.color = _Player.TeamColor; }
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
        if (_StartShrinking && !IsAlive() && ShrinkWhenDestroyed) {

            // Get in the cold ass water
            transform.localScale -= Vector3.one * ShrinkSpeed * Time.deltaTime;
            if (transform.localScale.x < 0.01f) {

                // Deselect / De-highlight
                if (_Player) { _Player.RemoveFromSelection(this); }
                SetIsHighlighted(false);
                SetIsSelected(false);

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
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public override void CalculateBounds() {
        base.CalculateBounds();

        selectionBounds = new Bounds(transform.position, Vector3.zero);

        foreach (Renderer r in GetComponentsInChildren<Renderer>()) { selectionBounds.Encapsulate(r.bounds); }
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
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        // Deselect
        buildingSlot._IsCurrentlySelected = false;
        this._IsCurrentlySelected = false;

        // Get player reference
        Player plyr;
        if (_Player) { plyr = _Player; } else { plyr = GameManager.Instance.Players[0]; }

        // Check if the player has enough resources to build the object
        if ((plyr.SuppliesCount >= this.CostSupplies) && (plyr.PowerCount >= this.CostPower)) {

            // Add to building queue on the selected building slot
            _ClonedWorldObject = ObjectPooling.Spawn(gameObject, Vector3.zero, Quaternion.identity).GetComponent<WorldObject>();
            _ClonedWorldObject.SetBuildingPosition(buildingSlot);
            _ClonedWorldObject.gameObject.SetActive(true);
            _ClonedWorldObject.Init();
            
            // Create healthbar
            CreateHealthBar(_ClonedWorldObject, plyr.PlayerCamera);

            // Create building progress panel & allocate it to the unit
            GameObject buildProgressObj = ObjectPooling.Spawn(GameManager.Instance.BuildingInProgressPanel.gameObject);
            _BuildingProgressCounter = buildProgressObj.GetComponent<UnitBuildingCounter>();
            _BuildingProgressCounter.SetObjectAttached(_ClonedWorldObject);
            _BuildingProgressCounter.SetCameraAttached(plyr.PlayerCamera);
            buildProgressObj.gameObject.SetActive(true);
            buildProgressObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);

            // Add to building queue
            _ClonedWorldObject._ObjectState = WorldObjectStates.InQueue;
            if (buildingSlot.GetBuildingOnSlot() != null) { buildingSlot.GetBuildingOnSlot().AddToQueue(_ClonedWorldObject); }
            else {

                // Add to base queue
                if (buildingSlot.AttachedBase != null) { buildingSlot.AttachedBase.AddToQueue(_ClonedWorldObject); }
            }

            // Update queue widget
            UI_BuildingQueueWrapper.Instance.UpdateQueuePositions();

            // Deduct resources from player
            _ClonedWorldObject.SetPlayer(plyr);
            plyr.SuppliesCount -= _ClonedWorldObject.CostSupplies;
            plyr.PowerCount -= _ClonedWorldObject.CostPower;

            // Set object's properties
            _ClonedWorldObject.Team = plyr.Team;
            _ClonedWorldObject._IsCurrentlySelected = false;
            _CurrentBuildTime = BuildingTime;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void OnActiveState() { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object's state switches to active (Only once)
    /// </summary>
    protected virtual void OnBuilt() {

        // Send message to match feed
        MatchFeed.Instance.AddMessage(string.Concat(ObjectName, " constructed."));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Damages the object by a set amount.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void Damage(float damage, WorldObject instigator = null) {

        // Only proceed if were meant to be killable
        if (Damagable) {

            // Cant damage if were already destroyed
            if (_ObjectState != WorldObjectStates.Destroyed) {

                // Damage object & kill it if theres no health left
                _HitPoints -= damage;
                if (_HitPoints <= 0) { OnDeath(instigator); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is killed/destroyed
    /// </summary>
    public virtual void OnDeath(WorldObject instigator) {

        // Force deselect
        SetIsSelected(false);

        // Set object state
        _ObjectState = WorldObjectStates.Destroyed;

        // Destroy healthbar
        if (_HealthBar != null) {

            // Null the bitch
            _HealthBar.SetObjectAttached(null);
            ObjectPooling.Despawn(_HealthBar.gameObject);
        }

        // Clamping health
        _HitPoints = 0;
        _Health = 0f;

        // Play OnDeath(s) effect
        for (int i = 0; i < OnDeathEffects.Count; i++) {
                    
            // Play
            ParticleSystem effect = ObjectPooling.Spawn(OnDeathEffects[i].gameObject, transform.position, transform.rotation).GetComponent<ParticleSystem>();
            effect.Play();

            // Despawn particle system once it has finished its cycle
            float effectDuration = effect.duration + effect.startLifetime;
            StartCoroutine(ParticleDespawn(effect, effectDuration));
        }

        // Send message to match feed
        MatchFeed.Instance.AddMessage(string.Concat(ObjectName, " destroyed."));

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
        transform.position = new Vector3(transform.position.x, transform.position.y + _OffsetY, transform.position.z);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Creates a healthbar widget for the object. (This also attachs it to the object afterwards).
    /// </summary>
    /// <param name="thisObject"></param>
    /// <param name="camera"></param>
    public void CreateHealthBar(WorldObject thisObject, Camera camera) {

        // Create a health bar and allocate it to the unit
        GameObject healthBarObj = ObjectPooling.Spawn(GameManager.Instance.UnitHealthBar.gameObject);
        _HealthBar = healthBarObj.GetComponent<UnitHealthBar>();
        _HealthBar.SetObjectAttached(thisObject);
        _HealthBar.SetCameraAttached(camera);
        healthBarObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);
        healthBarObj.gameObject.SetActive(true);

        // Set healthbar widget size & anchoring
        RectTransform healthRectTransform = _HealthBar.HealthSlider.GetComponent<RectTransform>();
        healthRectTransform.sizeDelta = new Vector2(_WidgetHealthbarScaleX, _WidgetHealthbarScaleY);
        healthRectTransform.anchoredPosition = new Vector2(0, _WidgetHealthbarOffset);
        RectTransform shieldRectTransform = _HealthBar.ShieldSlider.GetComponent<RectTransform>();
        shieldRectTransform.sizeDelta = new Vector2(_WidgetShieldbarScaleX, _WidgetShieldbarScaleY);
        shieldRectTransform.anchoredPosition = new Vector2(0, _WidgetShieldbarOffset);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets reference to the object's health bar widget.
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
    public virtual bool IsInWorld() { return IsAlive() && (_ObjectState == WorldObjectStates.Active || _ObjectState == WorldObjectStates.Building); }

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
    //  Sets the current hitpoints of this object by the amount specified.
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
    public float GetShieldPoints() { return _ShieldPoints; }

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
    public virtual void SetObjectState(WorldObjectStates newState) { _ObjectState = newState; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference to the current population of garrisoned infantry in this object.s
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int GetCurrentGarrisonCount() { return _CurrentGarrisonPopulation; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the current height value of the object (used for AI attacking offsets
    //  so they don't shoot at the target's position but actually at the target's 'chest').
    //  (For buildings, the height should be '0' by default & units auto assign this 
    //  value based off their specified agent height).
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetObjectHeight() { return _ObjectHeight; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference to the amount of SUPPLIES that is to be given
    //  to the owning player controller when the building is recycled.
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int GetRecycleSuppliesAmount() { return _RecycleSupplies; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference to the amount of POWER that is to be given
    //  to the owning player controller when the building is recycled.
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int GetRecyclePowerAmount() { return _RecyclePower; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Sets reference to the "Cloned" object that was created from this prefab.
    /// </summary>
    /// <param name="clone"></param>
    public void SetClonedObject(WorldObject clone) { _ClonedWorldObject = clone; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets whether the object attached should have a healthbar allocated to it.
    //  (This is used in the unit showcase)
    /// </summary>
    /// <param name="value"></param>
    public void SetShowHealthBar(bool value) { _ShowHealthbar = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits for the seconds specified then attempts to re-pool
    //  the particle effect (or destroyed entirely if re-pooling isn't possible)
    /// </summary>
    /// <param name="particleEffect"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator ParticleDespawn(ParticleSystem particleEffect, float delay) {

        // Delay
        yield return new WaitForSeconds(delay);

        // Despawn the system
        ObjectPooling.Despawn(particleEffect.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}