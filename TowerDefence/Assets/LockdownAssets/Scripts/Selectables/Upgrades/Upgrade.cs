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

    protected WorldObject _WorldObjectAttached = null;
    protected int _CurrentUpgradeLevel = 0;
    protected string _UpgradeName;
    protected bool _HasMaxUpgrade = false;
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
    //  Called before Star().
    /// </summary>
    protected override void Awake() {

        _UpgradeName = ObjectName;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        
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
        if (_Upgrading) {

            // Add to timer
            _UpgradeTimer += Time.deltaTime;
            if (_UpgradeTimer >= _UpgradeBuildTime) {
                
                // Upgrade complete
                _CurrentUpgradeLevel += 1;
                _Upgrading = false;
            }
        }
        else { _UpgradeTimer = 0f; }
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

        // Not reached the maximum upgrade yet
        if (!_HasMaxUpgrade) {

            // If the method exists for the next upgrade, then call it
            if (UpgradeEvents.Count >= _CurrentUpgradeLevel + 1 && UpgradeProperties.Count >= _CurrentUpgradeLevel + 1) { UpgradeEvents[_CurrentUpgradeLevel + 1].Invoke(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public virtual void QueueUpgrade(UpgradeValues costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {
            
            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;

            // Start upgrading
            _UpgradeTimer = 0f;
            _UpgradeBuildTime = costs.BuildTime;
            _Upgrading = true;
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

}