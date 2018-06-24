using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/6/2018
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
    public float _OffsetY;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum WorldObjectStates { Default, Building, Deployable, Active, ENUM_COUNT }

    protected GameObject _SelectionObj = null;
    protected bool _ReadyForDeployment = false;
    protected float _CurrentBuildTime = 0f;
    protected WorldObjectStates _ObjectState = WorldObjectStates.Default;
    protected int _HitPoints;
    protected UnitHealthBar _HealthBar = null;
    protected WorldObject _ClonedWorldObject = null;
    protected float _Health;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
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

                    // Add to timer
                    if (_CurrentBuildTime < BuildTime) { _CurrentBuildTime += Time.deltaTime; }
                    ///Debug.Log("Building: " + ObjectName + " at " + _CurrentBuildTime + " / " + BuildTime);
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
    /// 
    /// </summary>
    /// <param name="selectObj"></param>
    protected override void ChangeSelection(Selectable selectObj) { base.ChangeSelection(selectObj);

        // This should be called by the following line, but there is an outside chance it will not
        SetSelection(false);

        // Clear the world objects selection list
        foreach (var obj in _Player.SelectedWorldObjects) { obj.SetSelection(false); }
        _Player.SelectedWorldObjects.Clear();

        // Add new selection to the list
        _Player.SelectedWorldObjects.Add(selectObj.GetComponent<WorldObject>());
        selectObj.SetSelection(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="draw"></param>
    protected override void DrawSelection(bool draw) { base.DrawSelection(draw);

        // Show selection
        if (draw) {

            // Show selection prefab at the bottom of the object
            if (_SelectionObj == null) { _SelectionObj = Instantiate(Settings.SelectBoxObjects); }
            if (_SelectionObj != null) {

                // Display prefab if not already being displayed
                if (_SelectionObj.activeInHierarchy != true) { _SelectionObj.SetActive(true); }

                // Update selection prefab position
                Vector3 pos = new Vector3();
                pos.x = transform.position.x;
                pos.y = 1.1f;
                pos.z = transform.position.z;
                _SelectionObj.transform.position = pos;
            }
        }

        // Hide selection
        else { if (_SelectionObj != null) { Destroy(_SelectionObj.gameObject); } }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hitObject"></param>
    /// <param name="hitPoint"></param>
    public override void MouseClick(GameObject hitObject, Vector3 hitPoint) { base.MouseClick(hitObject, hitPoint);

        // Only handle input if currently selected
        if (_IsCurrentlySelected && hitObject && hitObject.name != "Ground") {

            // Cast hit object to world object
            WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();

            if (worldObject)
                ChangeSelection(worldObject);
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

        // Check if the player has enough resources to build the object
        Player plyr = GameManager.Instance.Players[0];
        if ((plyr.SuppliesCount >= this.CostSupplies) && (plyr.PowerCount >= this.CostPower)) {

            // Start building object on the selected building slot
            _ClonedWorldObject = Instantiate(this);
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

        // Deselect
        buildingSlot._IsCurrentlySelected = false;
        this._IsCurrentlySelected = false;
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

}