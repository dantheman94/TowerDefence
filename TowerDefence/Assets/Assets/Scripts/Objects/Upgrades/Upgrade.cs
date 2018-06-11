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
//  Last edited on: 6/7/2018
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
    public List<UpgradeCosts> UpgradeCosts;
    public List<UnityEvent> UpgradePath;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    protected int _CurrentUpgradeLevel = 0;
    protected string _UpgradeName;
    protected bool _HasMaxUpgrade = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake() {

        _UpgradeName = ObjectName;
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Update() {

        // Get roman numerial from current upgrade level + 1
        string num = " ";
        for (int i = (-1); i < _CurrentUpgradeLevel; i++) {

            num = num + "I";
        }

        // Update name to include current upgrade level
        ObjectName = _UpgradeName + num;

        // Update current costs for the next upgrade
        if (UpgradeCosts.Count > _CurrentUpgradeLevel + 1) {

            CostSupplies = UpgradeCosts[_CurrentUpgradeLevel + 1].SupplyCost;
            CostPower = UpgradeCosts[_CurrentUpgradeLevel + 1].PowerCost;
        }

        // Update if reached max upgrade level
        _HasMaxUpgrade = (UpgradeCosts.Count <= _CurrentUpgradeLevel) || (UpgradePath.Count <= _CurrentUpgradeLevel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buildingSlot"></param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        // If the method exists for the next upgrade, then call it
        if (UpgradePath.Count >= _CurrentUpgradeLevel + 1 && UpgradeCosts.Count >= _CurrentUpgradeLevel + 1) { UpgradePath[_CurrentUpgradeLevel + 1].Invoke(); }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void UpgradeOne(UpgradeCosts costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {

            // Increase upgrade level (if theres a level to go to next)
            if (UpgradePath.Count > _CurrentUpgradeLevel && UpgradeCosts.Count > _CurrentUpgradeLevel) { _CurrentUpgradeLevel += 1; }

            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void UpgradeTwo(UpgradeCosts costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {

            // Increase upgrade level (if theres a level to go to next)
            if (UpgradePath.Count > _CurrentUpgradeLevel && UpgradeCosts.Count > _CurrentUpgradeLevel) { _CurrentUpgradeLevel += 1; }

            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void UpgradeThree(UpgradeCosts costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {

            // Increase upgrade level (if theres a level to go to next)
            if (UpgradePath.Count > _CurrentUpgradeLevel && UpgradeCosts.Count > _CurrentUpgradeLevel) { _CurrentUpgradeLevel += 1; }

            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;
        }
    }

    public bool HasMaxUpgrade() { return _HasMaxUpgrade; }

}