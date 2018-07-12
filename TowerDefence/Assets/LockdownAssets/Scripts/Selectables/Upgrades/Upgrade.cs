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
    public List<UpgradeCosts> UpgradeCosts;
    [Space]
    public List<UnityEvent> UpgradeInvokables;

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

        // Get roman numeral from current upgrade level + 1 in the form of 'I's.
        string num = " ";
        for (int i = (-1); i < _CurrentUpgradeLevel; i++) { num = num + "I"; }

        // Update name to include current upgrade level
        ///ObjectName = _UpgradeName + num;
        ObjectName = UpgradeCosts[_CurrentUpgradeLevel].ObjectName;
        ObjectDescriptionShort = UpgradeCosts[_CurrentUpgradeLevel].ObjectDescriptionShort;
        ObjectDescriptionLong = UpgradeCosts[_CurrentUpgradeLevel].ObjectDescriptionLong;

        // Update current costs for the next upgrade
        if (UpgradeCosts.Count > _CurrentUpgradeLevel + 1) {

            CostSupplies = UpgradeCosts[_CurrentUpgradeLevel + 1].SupplyCost;
            CostPower = UpgradeCosts[_CurrentUpgradeLevel + 1].PowerCost;
        }

        // Update if reached max upgrade level
        _HasMaxUpgrade = (UpgradeCosts.Count <= _CurrentUpgradeLevel) || (UpgradeInvokables.Count <= _CurrentUpgradeLevel);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="buildingSlot"></param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        // Not reached the maximum upgrade yet
        if (!_HasMaxUpgrade) {

            // If the method exists for the next upgrade, then call it
            if (UpgradeInvokables.Count >= _CurrentUpgradeLevel + 1 && UpgradeCosts.Count >= _CurrentUpgradeLevel + 1) { UpgradeInvokables[_CurrentUpgradeLevel + 1].Invoke(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public virtual void UpgradeOne(UpgradeCosts costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {

            // Increase upgrade level (if theres a level to go to next)
            if (UpgradeInvokables.Count > _CurrentUpgradeLevel && UpgradeCosts.Count > _CurrentUpgradeLevel) { _CurrentUpgradeLevel += 1; }
            else { _HasMaxUpgrade = true; }

            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public virtual void UpgradeTwo(UpgradeCosts costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {

            // Increase upgrade level (if theres a level to go to next)
            if (UpgradeInvokables.Count > _CurrentUpgradeLevel && UpgradeCosts.Count > _CurrentUpgradeLevel) { _CurrentUpgradeLevel += 1; }
            else { _HasMaxUpgrade = true; }

            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public virtual void UpgradeThree(UpgradeCosts costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {

            // Increase upgrade level (if theres a level to go to next)
            if (UpgradeInvokables.Count > _CurrentUpgradeLevel && UpgradeCosts.Count > _CurrentUpgradeLevel) { _CurrentUpgradeLevel += 1; }
            else { _HasMaxUpgrade = true; }

            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public virtual void UpgradeFour(UpgradeCosts costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {

            // Increase upgrade level (if theres a level to go to next)
            if (UpgradeInvokables.Count > _CurrentUpgradeLevel && UpgradeCosts.Count > _CurrentUpgradeLevel) { _CurrentUpgradeLevel += 1; }
            else { _HasMaxUpgrade = true; }

            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public virtual void UpgradeFive(UpgradeCosts costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) {

            // Increase upgrade level (if theres a level to go to next)
            if (UpgradeInvokables.Count > _CurrentUpgradeLevel && UpgradeCosts.Count > _CurrentUpgradeLevel) { _CurrentUpgradeLevel += 1; }
            else { _HasMaxUpgrade = true; }

            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;
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