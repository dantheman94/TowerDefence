using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/7/2018
//
//******************************

public class Base : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MINIMAP PROPERTIES")]
    [Space]
    public GameObject MinimapQuad = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" BASE PROPERTIES")]
    [Space]
    public eBaseType BaseType;
    public int TechLevelWhenBuilt;
    public GameObject GroundUnitSpawnTransform;
    public GameObject AirUnitSpawnTransform;
    [Space]
    public List<BuildingSlot> DepotSlots;
    public List<BuildingSlot> TowerSlots;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum eBaseType { Outpost, Station, CommandCenter, Headquarters, Minibase }
    protected List<Building> _BuildingList;
    protected Renderer _MinimapRenderer;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        // Get component references
        if (MinimapQuad != null) { _MinimapRenderer = MinimapQuad.GetComponent<Renderer>(); }
        _BuildingList = new List<Building>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Change minimap colour based on attacking/defending & team colour
        if (_MinimapRenderer != null) {

            // Attacking team colour
            if (Team == GameManager.Team.Attacking) { _MinimapRenderer.material.color = WaveManager.Instance.AttackingTeamColour; }
            
            // Defending team
            else if (Team == GameManager.Team.Defending) {

                // Use individual player colour
                if (_Player) { _MinimapRenderer.material.color = _Player.TeamColor; }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object
    //  linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {

        // Gets reference to the original base (before the upgrade)
        Base originalBase = null;
        if (buildingSlot != null) {

            if (buildingSlot.AttachedBase != null) { originalBase = buildingSlot.AttachedBase; }
        }

        // Remove old healthbar (if valid)
        float hitpoints = MaxHitPoints;
        if (originalBase != null) {

            hitpoints = originalBase._HitPoints;
            if (originalBase._HealthBar != null) { ObjectPooling.Despawn(originalBase._HealthBar.gameObject); }

        }

        // Start building process
        base.OnWheelSelect(buildingSlot);

        // Only proceed if there was a previous building and we are upgrading from that
        if (originalBase != null) {

            // Update player ref
            _ClonedWorldObject._Player = originalBase._Player;
                        
            // Set the new bases building state object to be the currently active base
            _ClonedWorldObject.BuildingState = originalBase.gameObject;

            // Get references to all the buildings that were in the previous base & allocate them to the new one
            Base newBase = _ClonedWorldObject.GetComponent<Base>();

            // Depot/Generator slots
            for (int i = 0; i < newBase.DepotSlots.Count; i++) {

                // Only go as far as the previous bases slot size
                if (i < originalBase.DepotSlots.Count) {

                    // Reallocate building to new base (if there is one)
                    if (originalBase.DepotSlots[i] != null) {

                        // Send building to new base
                        newBase.DepotSlots[i].SetBuildingOnSlot(originalBase.DepotSlots[i].GetBuildingOnSlot());
                        originalBase.DepotSlots[i].SetBuildingOnSlot(null);
                    }
                    else { continue; }
                }
                else { break; }
            }

            // Tower slots
            for (int i = 0; i < newBase.TowerSlots.Count; i++) {

                // Only go as far as the previous bases slot size
                if (i < originalBase.TowerSlots.Count) {

                    // Reallocate tower to new base (if there is one)
                    if (originalBase.TowerSlots[i] != null) {

                        // Send tower to new base
                        newBase.TowerSlots[i].SetBuildingOnSlot(originalBase.TowerSlots[i].GetBuildingOnSlot());
                        originalBase.TowerSlots[i].SetBuildingOnSlot(null);
                    }
                    else { continue; }
                }
                else { break; }
            }
        }

        // Update attached base reference
        if (buildingSlot != null) { buildingSlot.AttachedBase = _ClonedWorldObject.GetComponent<Base>(); }

        // Update new bases health with the old bases health
        _ClonedWorldObject.SetHitPoints(hitpoints);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object's state switches to active
    /// </summary>
    protected override void OnBuilt() {
        base.OnBuilt();
        
        // Add tech level if required
        if (_Player != null) {

            if (_Player.Level < TechLevelWhenBuilt) { _Player.Level = TechLevelWhenBuilt; }
        }

        // Show any hidden base slots that are linked to the building slot
        if (AttachedBuildingSlot != null) { AttachedBuildingSlot.SetLinkedSlotsBase(this); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public override void OnDeath() {

        // Destroy all the attached buildings (towers, depots, etc...)
        for (int i = 0; i < _BuildingList.Count; i++) {

            float dmgHealth = _BuildingList[i].GetHitPoints();
            float dmgShield = _BuildingList[i].GetShieldPoints();
            _BuildingList[i].Damage((int)dmgHealth);
        }
        _BuildingList.Clear();

        // Now we can safely be destroyed
        base.OnDeath();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="building"></param>
    public void AddBuildingToList(Building building) { _BuildingList.Add(building); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}