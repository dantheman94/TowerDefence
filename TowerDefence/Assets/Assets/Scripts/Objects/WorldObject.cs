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
    // INSPECTOR

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

    //******************************************************************************************************************************
    // VARIABES

    public enum WorldObjectStates { Default, Building, Deployable, Active, ENUM_COUNT }

    protected GameObject _SelectionObj = null;
    protected bool _IsBeingBuilt = false;
    protected bool _ReadyForDeployment = false;
    protected float _CurrentBuildTime = 0f;
    protected WorldObjectStates _ObjectState = WorldObjectStates.Default;

    //******************************************************************************************************************************
    // FUNCTIONS

    protected override void Update() { base.Update();
        
        // Has unit building started?
        if (_IsBeingBuilt) {

            // Is unit building complete?
            _ReadyForDeployment = _CurrentBuildTime >= BuildTime;
            if (!_ReadyForDeployment) {
                
                // Add to timer
                if (_CurrentBuildTime < BuildTime) { _CurrentBuildTime += Time.deltaTime; }
                Debug.Log("Building");
            }
        }
    }

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

                // Update position
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

    public virtual void OnWheelSelect() {

        // Start building object
        _IsBeingBuilt = true;
        _ObjectState = WorldObjectStates.Building;

        // Hide selection wheel
        GameManager.Instance.SelectionWheel.SetActive(false);
    }

}