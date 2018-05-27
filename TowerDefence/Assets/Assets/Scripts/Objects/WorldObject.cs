using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/10/2018
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
    [Header(" WORLD OBJECT PROPERTIES")]
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
    public bool _Deployed = false;
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

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    protected override void Start() { base.Start();

        // Initialize health
        _HitPoints = MaxHitPoints;

        // Get vertical offset based off the prefab template
        _OffsetY = transform.position.y;
    }

    protected override void Update() { base.Update();

        // Has unit building started?
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
                break;
            }

            case WorldObjectStates.Deployable: {

                break;
            }

            case WorldObjectStates.Active: {

                break;
            }

            default: break;
        }
    }

    protected virtual void DrawSelectionWheel() { }

    public override void CalculateBounds() { base.CalculateBounds();

        selectionBounds = new Bounds(transform.position, Vector3.zero);

        foreach (Renderer r in GetComponentsInChildren<Renderer>()) {

            selectionBounds.Encapsulate(r.bounds);
        }
    }

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

    protected override void DrawSelection(bool draw) { base.DrawSelection(draw);

        // Show selection
        if (draw) {

            // Show selection prefab at the bottom of the object
            if (_SelectionObj == null) { _SelectionObj = Instantiate(ResourceManager.SelectBoxObjects); }
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
        else { if (_SelectionObj != null) { _SelectionObj.SetActive(false); } }
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint) { base.MouseClick(hitObject, hitPoint);

        // Only handle input if currently selected
        if (_IsCurrentlySelected && hitObject && hitObject.name != "Ground") {

            // Cast hit object to world object
            WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();

            if (worldObject)
                ChangeSelection(worldObject);
        }
    }

    public virtual void OnWheelSelect(BuildingSlot buildingSlot) {

        // Start building object on the selected building slot
        WorldObject clone = Instantiate(this);
        clone.SetBuildingPosition(buildingSlot);
        clone.gameObject.SetActive(true);
        clone._ObjectState = WorldObjectStates.Building;

        // Create healthbar and allocate it to the unit
        GameObject healthBarObj = Instantiate(GameManager.Instance.UnitHealthBar);
        UnitHealthBar healthBar = healthBarObj.GetComponent<UnitHealthBar>();
        healthBar.setObjectAttached(clone);
    }

    public void Damage(int damage) {

        // Damage object & clamp health to 0 if it exceeds
        _HitPoints -= damage;
        if (_HitPoints < 0) { _HitPoints = 0; }
    }

    public bool IsAlive() { return _HitPoints > 0f; }

    private void SetBuildingPosition(BuildingSlot buildingSlot) {

        // Initial transform update
        transform.SetPositionAndRotation(buildingSlot.transform.position, buildingSlot.transform.rotation);

        // Add offset
        transform.position = new Vector3(transform.position.x, _OffsetY, transform.position.z);
    }

    public void setHealthBar(UnitHealthBar healthBar) { _HealthBar = healthBar; }

    public bool isActiveInWorld() { return IsAlive() && (_ObjectState == WorldObjectStates.Active || _ObjectState == WorldObjectStates.Building); }

}