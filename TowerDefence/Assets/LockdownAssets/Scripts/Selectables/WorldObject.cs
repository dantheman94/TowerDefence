﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 25/6/2018
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
    public GameObject BuildingState;
    public GameObject ActiveState;
    [Space]
    [Header("-----------------------------------")]
    [Header(" WORLD OBJECT PROPERTIES")]
    [Space]
    public Texture2D BuildImage;
    public int BuildTime = 20;
    [Space]
    public int CostSupplies = 0;
    public int CostPower = 0;
    public int CostTechLevel = 1;
    public int CostPopulation = 0;
    [Space]
    public int RecycleSupplies = 0;
    public int RecyclePower = 0;
    [Space]
    public int MaxHitPoints = 100;
    public int MaxShieldPoints = 0;
    [Space]
    public bool Garrisonable = false;
    public int MaxGarrisonPopulation = 0;
    [Space]
    public bool MultiSelectable = true;
    public float _OffsetY;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum WorldObjectStates { Default, Building, Deployable, Active, ENUM_COUNT }

    protected bool _ReadyForDeployment = false;
    protected float _CurrentBuildTime = 0f;
    public WorldObjectStates _ObjectState = WorldObjectStates.Default;
    protected UnitHealthBar _HealthBar = null;
    protected UnitBuildingCounter _BuildingProgressCounter = null;
    protected WorldObject _ClonedWorldObject = null;
    protected float _Health;
    protected int _CurrentGarrisonPopulation = 0;
    protected float _ObjectHeight = 0f;
    protected int _HitPoints;
    protected int _ShieldPoints;
    protected float _Shield;

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
                break;
            }

            case WorldObjectStates.Building: {

                // Is unit building complete?
                _ReadyForDeployment = _CurrentBuildTime >= BuildTime;
                if (!_ReadyForDeployment) {

                    // Add to building timer
                    if (_CurrentBuildTime < BuildTime) { _CurrentBuildTime += Time.deltaTime; }
                }

                else {

                    // Object has finished building
                    OnBuilt();
                    _ObjectState = WorldObjectStates.Deployable;
                }

                // Show building state object
                if (BuildingState) { BuildingState.SetActive(true); }
                if (ActiveState) { ActiveState.SetActive(false); }
                break;
            }

            case WorldObjectStates.Deployable: {

                break;
            }

            case WorldObjectStates.Active: {

                // Show active state object
                if (BuildingState) { BuildingState.SetActive(false); }
                if (ActiveState) { ActiveState.SetActive(true); }
                break;
            }

            default: break;
        }

        // Update shield to be a normalized range of the object's shield-points
        if (_ShieldPoints > 0) { _Shield = _ShieldPoints / MaxShieldPoints; }
        else {

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
                OnDeath();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void DrawSelectionWheel() {}

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

        // Check if the player has enough resources to build the object
        Player plyr;
        if (_Player) { plyr = _Player; }
        else { plyr = GameManager.Instance.Players[0]; }
        
        if ((plyr.SuppliesCount >= this.CostSupplies) && (plyr.PowerCount >= this.CostPower)) {

            // Start building object on the selected building slot
            _ClonedWorldObject = ObjectPooling.Spawn(gameObject, Vector3.zero, Quaternion.identity).GetComponent<WorldObject>();
            _ClonedWorldObject.SetBuildingPosition(buildingSlot);
            _ClonedWorldObject.gameObject.SetActive(true);
            _ClonedWorldObject._ObjectState = WorldObjectStates.Building;

            // Create a health bar and allocate it to the unit
            GameObject healthBarObj = ObjectPooling.Spawn(GameManager.Instance.UnitHealthBar.gameObject);
            _HealthBar = healthBarObj.GetComponent<UnitHealthBar>();
            _HealthBar.setObjectAttached(_ClonedWorldObject);
            _HealthBar.setCameraAttached(plyr.PlayerCamera);
            healthBarObj.gameObject.SetActive(true);
            healthBarObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);

            // Create building progress panel & allocate it to the unit
            GameObject buildProgressObj = ObjectPooling.Spawn(GameManager.Instance.BuildingInProgressPanel.gameObject);
            _BuildingProgressCounter = buildProgressObj.GetComponent<UnitBuildingCounter>();
            _BuildingProgressCounter.setObjectAttached(_ClonedWorldObject);
            _BuildingProgressCounter.setCameraAttached(plyr.PlayerCamera);
            buildProgressObj.gameObject.SetActive(true);
            buildProgressObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);

            // Deduct resources from player
            SetPlayer(plyr);
            plyr.SuppliesCount -= _ClonedWorldObject.CostSupplies;
            plyr.PowerCount -= _ClonedWorldObject.CostPower;

            // Set object's properties
            _ClonedWorldObject.Team = _Player.Team;
            _ClonedWorldObject._IsCurrentlySelected = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void OnBuilt() {

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
    //  
    /// </summary>
    public virtual void OnDeath() {

        // Clamping health
        _HitPoints = 0;
        _Health = 0f;

        // Delay then despawn
        DelayedDespawn(this, 3f);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits for the seconds specified then attempts to repool
    //  the particle effect (or destroyed entirely if re-pooling isn't possible)
    /// </summary>
    /// <param name="particleEffect"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator DelayedDespawn(WorldObject worldObject, float delay) {

        // Delay
        yield return new WaitForSeconds(delay);

        // This is so that the next time the object is spawned - it is at its default state already
        _ObjectState = WorldObjectStates.Default;
        
        // Despawn the object
        ObjectPooling.Despawn(worldObject.gameObject);
        transform.localScale = new Vector3(1f, 1f, 1f);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="healthBar"></param>
    public void setHealthBar(UnitHealthBar healthBar) { _HealthBar = healthBar; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns TRUE if the object is either building or active in the game world.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool isInWorld() { return IsAlive() && (_ObjectState == WorldObjectStates.Active || _ObjectState == WorldObjectStates.Building); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="value"></param>
    public void SetHitPoints(int value) { _HitPoints = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the object's current hitpoints as a raw value.
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int getHitPoints() { return _HitPoints; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the hitpoints as a normalized value. (0.0f - 1.0f [ % ])
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float getHealth() { return _Health; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the object's current shield-points as a raw value.
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int getShieldPoints() { return _ShieldPoints; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the shield-points as a normalized value. (0.0f - 1.0f [ % ])
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float getShield() { return _Shield; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float getCurrentBuildTimeRemaining() { return BuildTime - _CurrentBuildTime; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the current object state (Ie: Building, Deployable, Active).
    /// </summary>
    /// <returns>
    //  ENUM: WorldObjectState
    /// </returns>
    public WorldObjectStates getObjectState() { return _ObjectState; }

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