using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 22/10/18
//
//******************************

public class Ability_Veterancy : Abstraction {

    private GameObject _ReferenceObject;
    private VeteranUpgrade _VeteranUpgrade;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        _ReferenceObject = new GameObject();
        _VeteranUpgrade = new VeteranUpgrade();
      
        // Deselect the building slot
        buildingSlot.SetIsSelected(false);


        // Get player reference
        Player plyr = GameManager.Instance.Players[0];

     

        // Check if the player has enough resources to build the object
        if ((plyr.SuppliesCount >= this.CostSupplies) && (plyr.PowerCount >= this.CostPower)) {
            _VeteranUpgrade = _ReferenceObject.AddComponent(typeof(VeteranUpgrade)) as VeteranUpgrade;
            _VeteranUpgrade.SetPowerCost(CostPower);
            _VeteranUpgrade.SetSupplyCost(CostSupplies);
            _VeteranUpgrade.SetArea(350, 400);

            

            // DO SOME SHIT BRO
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
