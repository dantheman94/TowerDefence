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
//  Last edited on: 7/7/2018
//
//******************************

public class Upgrade : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UPGRADE TREE ")]
    [Space]
    public List<UpgradeValues> UpgradeProperties;
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
    private string _UpgradeName;
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
    //  Called before Start().
    /// </summary>
    protected override void Awake() {

        // Initialize
        _UpgradeName = ObjectName;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {

        if (UpgradeProperties.Count > 0 && UpgradeEvents.Count > 0) {

            // Update name to include current upgrade level
            ObjectName = UpgradeProperties[_CurrentUpgradeLevel].ObjectName;
            ObjectDescriptionShort = UpgradeProperties[_CurrentUpgradeLevel].ObjectDescriptionShort;
            ObjectDescriptionLong = UpgradeProperties[_CurrentUpgradeLevel].ObjectDescriptionLong;

            // Update current costs for the next upgrade
            if (UpgradeProperties.Count > _CurrentUpgradeLevel + 1) {

                CostSupplies = UpgradeProperties[_CurrentUpgradeLevel + 1].SupplyCost;
                CostPower = UpgradeProperties[_CurrentUpgradeLevel + 1].PowerCost;
            }

            // Update if reached max upgrade level
            _HasMaxUpgrade = (UpgradeProperties.Count <= _CurrentUpgradeLevel) || (UpgradeEvents.Count <= _CurrentUpgradeLevel);

            // Update upgrade timer
            if (_BuildingAttached != null) {

                // Checking if the object in the queue is a valid upgrade 'value' 
                UpgradeValues upgradeVal = _BuildingAttached.GetBuildingQueue()[0].GetComponent<UpgradeValues>();
                if (upgradeVal != null) {

                    // Does it match this upgrade's current upgrade 'value'
                    if (upgradeVal.GetType() == UpgradeProperties[_CurrentUpgradeLevel].GetType()) {

                        // Start upgrading
                        _Upgrading = true;
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
    public void QueueUpgrade(UpgradeValues costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {
            
            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;

            // Start upgrading (or add to the queue)
            _UpgradeTimer = 0f;
            _UpgradeBuildTime = costs.BuildTime;
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