using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 27/5/2018
//
//******************************

public class Abstraction : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" ABSTRACTION PROPERTIES")]
    [Space]
    [Tooltip("The name that shows in the selection wheel/menu for this object.")]
    public string ObjectName;
    [Tooltip("The short description that shows in the selection wheel/menu for this object.")]
    public string ObjectDescriptionShort;
    [Tooltip("The detailed description that shows in the selection wheel/menu for this object.")]
    public string ObjectDescriptionLong;
    [Space]
    [Tooltip("The thumbnail image that represents this object in.")]
    public Texture2D Logo;
    [Tooltip("The amount of time in SECONDS that this object takes to build.")]
    public int BuildingTime = 20;
    [Space]
    [Tooltip("How much supply resource are required to build this object.")]
    public int CostSupplies = 0;
    [Tooltip("How much power resource are required to build this object.")]
    public int CostPower = 0;
    [Tooltip("Minimum player tech level required to build this object.")]
    public int CostTechLevel = 1;
    [Tooltip("The army population size needed for this object to be built.")]
    public int CostPopulation = 0;

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
    public virtual void OnWheelSelect(BuildingSlot buildingSlot) { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}