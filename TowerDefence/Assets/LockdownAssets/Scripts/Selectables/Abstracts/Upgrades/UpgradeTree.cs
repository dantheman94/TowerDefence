using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 18/9/2018
//
//******************************

public class UpgradeTree : Abstraction {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UPGRADE TREE ")]
    [Space]
    public List<Upgrade> UpgradeProperties;
    [Space]
    public List<UnityEvent> UpgradeEvents;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    private WorldObject _WorldObjectAttached = null;
    private Building _BuildingAttached = null;
    private int _CurrentUpgradeLevel = 0;
    private bool _HasMaxUpgrade = false;
    private bool _Upgrading = false;
    private float _UpgradeTimer = 0f;
    private float _UpgradeBuildTime = 0f;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this gameObject is created.
    /// </summary>
    protected void Start() {

        // Initialize
        this.ObjectName = UpgradeProperties[0].name;
        this.ObjectDescriptionShort = UpgradeProperties[0].ObjectDescriptionShort;
        this.ObjectDescriptionLong = UpgradeProperties[0].ObjectDescriptionLong;
        this.BuildingTime = UpgradeProperties[0].BuildingTime;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected void Update() {

        if (UpgradeProperties.Count > 0 && UpgradeEvents.Count > 0) {

            // Update name to include current upgrade level
            ObjectName = UpgradeProperties[_CurrentUpgradeLevel].ObjectName;
            ObjectDescriptionShort = UpgradeProperties[_CurrentUpgradeLevel].ObjectDescriptionShort;
            ObjectDescriptionLong = UpgradeProperties[_CurrentUpgradeLevel].ObjectDescriptionLong;

            // Update current costs for the next upgrade
            if (UpgradeProperties.Count > _CurrentUpgradeLevel + 1) {

                CostSupplies = UpgradeProperties[_CurrentUpgradeLevel + 1].CostSupplies;
                CostPower = UpgradeProperties[_CurrentUpgradeLevel + 1].CostPower;
            }

            // Update if reached max upgrade level
            _HasMaxUpgrade = (UpgradeProperties.Count <= _CurrentUpgradeLevel) || (UpgradeEvents.Count <= _CurrentUpgradeLevel);

            // Update upgrade timer
            if (_BuildingAttached != null) {

                // Checking if the object in the queue is a valid upgrade 'value' 
                if (_BuildingAttached.GetBuildingQueue().Count > 0) {

                    Upgrade upgradeVal = _BuildingAttached.GetBuildingQueue()[0].GetComponent<Upgrade>();
                    if (upgradeVal != null) {

                        // Does it match this upgrade's current upgrade 'value'
                        if (upgradeVal.GetType() == UpgradeProperties[_CurrentUpgradeLevel].GetType()) {

                            // Start upgrading
                            _Upgrading = true;
                        }
                    }
                }

                // Are we upgrading?
                if (_Upgrading) {

                    // Add to timer
                    _UpgradeTimer += Time.deltaTime;
                    if (_UpgradeTimer >= _UpgradeBuildTime) {

                        // Upgrading is complete
                        _CurrentUpgradeLevel += 1;
                        _Upgrading = false;
                        UpgradeProperties[_CurrentUpgradeLevel].OnUpgrade();

                        // Remove from the queue
                        _BuildingAttached.GetBuildingQueue().RemoveAt(0);
                    }
                }
            }
            else { _UpgradeTimer = 0f; }
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
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        // Set building attached reference
        _BuildingAttached = buildingSlot.GetBuildingOnSlot();

        // Not reached the maximum upgrade yet
        if (!_HasMaxUpgrade) {

            // If the method exists for the next upgrade, then call it
            if (UpgradeEvents.Count >= _CurrentUpgradeLevel + 1 && UpgradeProperties.Count >= _CurrentUpgradeLevel + 1) { UpgradeEvents[_CurrentUpgradeLevel].Invoke(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void QueueUpgrade(Upgrade costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.CostTechLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.CostSupplies) && (GameManager.Instance.Players[0].PowerCount >= costs.CostPower));
        if (affordable) {
            
            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.CostSupplies;
            GameManager.Instance.Players[0].PowerCount -= costs.CostPower;

            // Start upgrading (or add to the queue)
            _UpgradeTimer = 0f;
            _UpgradeBuildTime = costs.BuildingTime;
            _BuildingAttached.GetBuildingQueue().Add(costs.gameObject);

            // Create worldspace UI
            ///GameObject progressWidget = ObjectPooling.Spawn(GameManager.Instance.BuildingInProgressPanel.gameObject);
            GameObject progressWidget = Instantiate(GameManager.Instance.BuildingInProgressPanel.gameObject);
            UpgradeBuildingCounter upgradeCounter = progressWidget.GetComponent<UpgradeBuildingCounter>();
            upgradeCounter.SetUpgradeAttached(this);
            upgradeCounter.SetCameraAttached(_BuildingAttached._Player.PlayerCamera);
            progressWidget.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);
            progressWidget.gameObject.SetActive(true);

            // Add to building queue UI
            bool radialWheeel = GameManager.Instance._IsRadialMenu;
            SelectionWheel selectionWheel = null;
            if (radialWheeel)   { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
            else                { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }
            selectionWheel.BuildingQueue.AddToQueue(costs);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool HasMaxUpgrade() { return _HasMaxUpgrade; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsUpgrading() { return _Upgrading; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float UpgradeTimeRemaining() { return _UpgradeBuildTime - _UpgradeTimer; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Building
    /// </returns>
    public Building GetBuildingAttached() { return _BuildingAttached; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}