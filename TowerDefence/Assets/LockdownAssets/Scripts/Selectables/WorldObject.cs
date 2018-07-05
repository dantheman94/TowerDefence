﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

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
    public int CostSupplies = 0;
    public int CostPower = 0;
    public int CostPlayerLevel = 1;
    public int RecycleSupplies = 0;
    public int RecyclePower = 0;
    public int MaxHitPoints = 100;
    public int MaxShieldPoints = 0;
    public bool MultiSelectable = true;
    public int PopulationSize = 0;
    public bool Garrisonable = false;
    public int MaxGarrisonPopulation = 10;
    public float _OffsetY;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum WorldObjectStates { Default, Building, Deployable, Active, ENUM_COUNT }

    protected bool _ReadyForDeployment = false;
    protected float _CurrentBuildTime = 0f;
    protected WorldObjectStates _ObjectState = WorldObjectStates.Default;
    protected int _HitPoints;
    protected UnitHealthBar _HealthBar = null;
    protected WorldObject _ClonedWorldObject = null;
    protected float _Health;
    protected int _CurrentGarrisonPopulation = 0;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        // Initialize health
        _HitPoints = MaxHitPoints;

        // Get vertical offset based off the prefab template
        _OffsetY = transform.position.y;
    }

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();
        
        switch (_ObjectState) {

            case WorldObjectStates.Default: { break; }

            case WorldObjectStates.Building: {

                // Is unit building complete?
                _ReadyForDeployment = _CurrentBuildTime >= BuildTime;
                if (!_ReadyForDeployment) {

                    // Add to building timer
                    if (_CurrentBuildTime < BuildTime) { _CurrentBuildTime += Time.deltaTime; }
                }
                else { _ObjectState = WorldObjectStates.Deployable; }

                // Show building state object
                if (BuildingState) { BuildingState.SetActive(true); }
                if (ActiveState) { ActiveState.SetActive(false); }
                break;
            }

            case WorldObjectStates.Deployable: {

                break;
            }

            case WorldObjectStates.Active: {

                // Show building state object
                if (BuildingState) { BuildingState.SetActive(false); }
                if (ActiveState) { ActiveState.SetActive(true); }
                break;
            }

            default: break;
        }

        // Update health to be a normalized range of the object's hitpoints
        _Health = _HitPoints / MaxHitPoints;

        if (_HitPoints == 0) {


        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void DrawSelectionWheel() {}

    /// <summary>
    /// 
    /// </summary>
    public override void CalculateBounds() {
        base.CalculateBounds();

        selectionBounds = new Bounds(transform.position, Vector3.zero);

        foreach (Renderer r in GetComponentsInChildren<Renderer>()) {

            selectionBounds.Encapsulate(r.bounds);
        }
    }
        
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
        Player plyr = GameManager.Instance.Players[0];
        if ((plyr.SuppliesCount >= this.CostSupplies) && (plyr.PowerCount >= this.CostPower)) {

            // Start building object on the selected building slot
            ///_ClonedWorldObject = Instantiate(this);
            _ClonedWorldObject = ObjectPooling.Spawn(gameObject, Vector3.zero, Quaternion.identity).GetComponent<WorldObject>();
            _ClonedWorldObject.SetBuildingPosition(buildingSlot);
            _ClonedWorldObject.gameObject.SetActive(true);
            _ClonedWorldObject._ObjectState = WorldObjectStates.Building;

            // Create a health bar and allocate it to the unit
            GameObject healthBarObj = Instantiate(GameManager.Instance.UnitHealthBar);
            UnitHealthBar healthBar = healthBarObj.GetComponent<UnitHealthBar>();
            healthBar.setObjectAttached(_ClonedWorldObject);
            healthBarObj.gameObject.SetActive(true);

            // Create building progress panel & allocate it to the unit
            GameObject buildProgressObj = Instantiate(GameManager.Instance.BuildingInProgressPanel);
            UnitBuildingCounter buildCounter = buildProgressObj.GetComponent<UnitBuildingCounter>();
            buildCounter.setObjectAttached(_ClonedWorldObject);
            buildCounter.setCameraAttached(Camera.main);
            buildProgressObj.gameObject.SetActive(true);

            // Deduct resources from player
            SetPlayer(plyr);
            plyr.SuppliesCount -= _ClonedWorldObject.CostSupplies;
            plyr.PowerCount -= _ClonedWorldObject.CostPower;

            _ClonedWorldObject._IsCurrentlySelected = false;
        }
    }

    /// <summary>
    //  Damages the object by a set amount.
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage) {

        // Damage object & clamp health to 0 if it exceeds
        _HitPoints -= damage;
        if (_HitPoints < 0) { _HitPoints = 0; }
    }

    /// <summary>
    //  Returns TRUE if the object's health is greater than 0f.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsAlive() { return _HitPoints > 0f && _Health > 0f; }

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="healthBar"></param>
    public void setHealthBar(UnitHealthBar healthBar) { _HealthBar = healthBar; }

    /// <summary>
    //  Returns TRUE if the object is either building or active in the game world.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool isActiveInWorld() { return IsAlive() && (_ObjectState == WorldObjectStates.Active || _ObjectState == WorldObjectStates.Building); }

    /// <summary>
    //  Returns the object's current hitpoints as a raw value.
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int getHitPoints() { return _HitPoints; }

    /// <summary>
    //  Returns the hitpoints as a normalized value.
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float getHealth() { return _Health; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float getCurrentBuildTimeRemaining() { return BuildTime - _CurrentBuildTime; }

    /// <summary>
    //  Returns the current object state (Ie: Building, Deployable, Active).
    /// </summary>
    /// <returns>
    //  ENUM: WorldObjectState
    /// </returns>
    public WorldObjectStates getObjectState() { return _ObjectState; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newState"></param>
    public void SetObjectState(WorldObjectStates newState) { _ObjectState = newState; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetCurrentGarrisonCount() { return _CurrentGarrisonPopulation; }

}