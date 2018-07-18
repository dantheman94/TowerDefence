using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 18/7/2018
//
//******************************

public class StorageSuppliesUpgrades : Upgrade {

    //******************************************************************************************************************************
    //
    //      EVENTS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {
        base.OnWheelSelect(buildingSlot);

        // Set ref to which storage vault we are upgrading
        _WorldObjectAttached = buildingSlot.GetBuildingOnSlot();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="costs"></param>
    public override void UpgradeOne(UpgradeValues value) {
        base.UpgradeOne(value);

        // Increase max cap for supplies only
        if (_WorldObjectAttached) {

            Storage storage = _WorldObjectAttached.GetComponent<Storage>();
            storage.AddToCap((int)value.UpgradeValue, 0);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="costs"></param>
    public override void UpgradeTwo(UpgradeValues value) {
        base.UpgradeTwo(value);

        // Increase max cap for supplies only
        if (_WorldObjectAttached) {

            Storage storage = _WorldObjectAttached.GetComponent<Storage>();
            storage.AddToCap((int)value.UpgradeValue, 0);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="costs"></param>
    public override void UpgradeThree(UpgradeValues value) {
        base.UpgradeThree(value);

        // Increase max cap for supplies only
        if (_WorldObjectAttached) {

            Storage storage = _WorldObjectAttached.GetComponent<Storage>();
            storage.AddToCap((int)value.UpgradeValue, 0);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="costs"></param>
    public override void UpgradeFour(UpgradeValues value) {
        base.UpgradeFour(value);

        // Increase max cap for supplies only
        if (_WorldObjectAttached) {

            Storage storage = _WorldObjectAttached.GetComponent<Storage>();
            storage.AddToCap((int)value.UpgradeValue, 0);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="costs"></param>
    public override void UpgradeFive(UpgradeValues value) {
        base.UpgradeFive(value);

        // Increase max cap for supplies only
        if (_WorldObjectAttached) {

            Storage storage = _WorldObjectAttached.GetComponent<Storage>();
            storage.AddToCap((int)value.UpgradeValue, 0);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}