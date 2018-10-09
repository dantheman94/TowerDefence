using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 28/9/2018
//
//******************************

[System.Serializable]
public class Abstraction : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header(" ABSTRACTION PROPERTIES")]
    [Header("-----------------------------------")]
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
    [Space]
    [Tooltip("The current game state of the object. " +
        "\n\nDEFAULT = Any disabled objects that are going to be instantiated at runtime " +
        "\n\nBUILDING = The object is active in the world, but is still being built. " +
        "\n\nDEPLOYABLE = The object has been built but is locked/hidden in its building (IE: AI Units). " +
        "\n\nACTIVE = The object is active/interactable within the game world.")]
    public WorldObjectStates _ObjectState = WorldObjectStates.Default;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum WorldObjectStates { Default, InQueue, Building, Deployable, Active, Destroyed, ENUM_COUNT }

    protected float _CurrentBuildTime = 0f;

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

    /// <summary>
    //  Starts the construction process of this abstraction.
    /// </summary>
    public virtual void StartBuildingObject(BuildingSlot buildingSlot = null) {

        _ObjectState = WorldObjectStates.Building;

        if (buildingSlot != null) { buildingSlot.GetBuildingOnSlot().SetBuildingStarted(true); }

        // Send message to match feed
        MatchFeed.Instance.AddMessage(string.Concat(ObjectName, " started construction."));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetCurrentBuildTimeRemaining() { return BuildingTime - _CurrentBuildTime; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  float
    /// </returns>
    public float GetBuildPercentage() { return _CurrentBuildTime / BuildingTime; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}