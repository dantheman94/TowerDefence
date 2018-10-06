using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 6/10/2018
//
//******************************

public class DetonateMines : Abstraction {

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
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

        // Detonate all known mines on the attached minefield
        MineField mineField = buildingSlot.GetBuildingOnSlot() as MineField;
        for (int i = 0; i < mineField.GetUndetonatedMines().Count; i++) {

            Mine mine = mineField.GetUndetonatedMines()[i];
            mine.DetonateMine();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}