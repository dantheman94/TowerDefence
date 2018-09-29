using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 29/8/2018
//
//******************************

public class UP_UnitBuilding : Upgrade {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UPGRADE PROPERTIES ")]
    [Space]
    [Range(0f, 1f)]
    public float PercentageRemoved = 0.1f;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the upgrading is complete and needs to be applied.
    /// </summary>
    public override void OnUpgrade() {
        base.OnUpgrade();

        Player player = GameManager.Instance.Players[0];
        UpgradeManager upgradeManager = player.GetUpgradeManager();

        upgradeManager._UnitBuildingSpeedMultiplier -= PercentageRemoved;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
