using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 22/10/2018
//
//******************************

public class Ability_BaseHealth : Abstraction {

    private GameObject _ReferenceObject;
    private IronHealth _IronHealth;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {
        
        _IronHealth = new IronHealth();
        _ReferenceObject = new GameObject();
        // Deselect the building slot
        buildingSlot.SetIsSelected(false);

        // Get player reference
        Player plyr = GameManager.Instance.Players[0];
     
        //Set references for the instantiated object.
        _IronHealth = _ReferenceObject.AddComponent(typeof(IronHealth)) as IronHealth;
        _IronHealth.SetBase(buildingSlot.AttachedBase);
        _IronHealth.SetBuildings(buildingSlot.AttachedBase.GetBuildingList());

        // Check if the player has enough resources to build the object
        if ((plyr.SuppliesCount >= this.CostSupplies) && (plyr.PowerCount >= this.CostPower)) {

            buildingSlot.AttachedBase.SetCanBeDamaged(false);
            for(int i = 0; i < buildingSlot.AttachedBase.GetBuildingList().Count; ++i)
            {
                buildingSlot.AttachedBase.GetBuildingList()[i].SetCanBeDamaged(false);
            }
            Debug.Log("Iron health activated! ");
            
            // DO SOME SHIT BRO
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
