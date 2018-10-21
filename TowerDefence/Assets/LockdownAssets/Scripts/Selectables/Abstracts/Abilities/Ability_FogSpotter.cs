using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 17/10/2018
//
//******************************

public class Ability_FogSpotter : Abstraction {

    

    // Variables
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public GameObject FogSpotterObject;

    private FogUnit _FogUnit;
    private bool _ActivateSpotter = false;
    private int _SpotterLifetime = 10;
    private int VisionRadius = 60;
    public Rect BoxSelection = new Rect();
    
    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        // Deselect the building slot
        buildingSlot.SetIsSelected(false);
        Debug.Log("Fog ability!");
        // Get player reference
        Player plyr = GameManager.Instance.Players[0];

        // Check if the player has enough resources to build the object
        if ((plyr.SuppliesCount >= this.CostSupplies) && (plyr.PowerCount >= this.CostPower)) {

            Instantiate(FogSpotterObject, plyr._HUD.FindHitPoint(), new Quaternion());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    }

    private void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, 0.5f);
     //   GUI.DrawTexture(BoxSelection);
    }

}