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
    [Header(" BUILDABLES ")]
    public List<UnityEvent> UpgradePath;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    protected int _CurrentUpgradeLevel = 0;
    protected string _UpgradeName;

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
    /// <param name="buildingSlot"></param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void UpgradeOne(UpgradeCosts costs) {

        // Check if the player can afford the upgrade
        bool affordable = ((GameManager.Instance.Players[0].Level >= costs.PlayerLevel) && (GameManager.Instance.Players[0].SuppliesCount >= costs.SupplyCost) && (GameManager.Instance.Players[0].PowerCount >= costs.PowerCost));
        if (affordable) { 

            // Increase upgrade level
            _CurrentUpgradeLevel += 1;

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

            // Increase upgrade level
            _CurrentUpgradeLevel += 1;

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

            // Increase upgrade level
            _CurrentUpgradeLevel += 1;

            // Deduct cost from player
            GameManager.Instance.Players[0].SuppliesCount -= costs.SupplyCost;
            GameManager.Instance.Players[0].PowerCount -= costs.PowerCost;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void Update() {

        // Get roman numerial from current upgrade level + 1
        string num = " ";
        for (int i = -1; i < _CurrentUpgradeLevel; i++) {

            num = num + "I";
        }

        // Update name to include current upgrade level
        ObjectName = _UpgradeName + num;
    }

}