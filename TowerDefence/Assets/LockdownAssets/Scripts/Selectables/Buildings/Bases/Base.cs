using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 20/8/2018
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
    [Space]
    public GameObject GroundUnitSpawnTransform;
    public GameObject AirUnitSpawnTransform;
    public GameObject RallyPointDefaultTransform;
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
    private Base _PreviousBase = null;

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
        if (buildingSlot != null) {

            if (buildingSlot.AttachedBase != null) { _PreviousBase = buildingSlot.AttachedBase; }
        }
        
        // Start building process
        base.OnWheelSelect(buildingSlot);
        _ClonedWorldObject.SetClonedObject(_ClonedWorldObject);
        _ClonedWorldObject.GetComponent<Base>()._PreviousBase = _PreviousBase;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="buildingSlot"></param>
    public override void StartBuildingObject(BuildingSlot buildingSlot) {
        base.StartBuildingObject();

        float hitpoints = MaxHitPoints;

        // Only proceed if there was a previous building and we are upgrading from that
        if (_ClonedWorldObject != null) {

            Base newBase = _ClonedWorldObject.GetComponent<Base>();
            if (newBase._PreviousBase != null) {

                // Remove old healthbar (if valid)
                if (newBase._PreviousBase != null) {

                    hitpoints = _PreviousBase._HitPoints;
                    if (newBase._PreviousBase._HealthBar != null) { ObjectPooling.Despawn(newBase._PreviousBase._HealthBar.gameObject); }
                }

                // Update player ref
                _ClonedWorldObject._Player = newBase._PreviousBase._Player;

                // Set the new bases queued/building state object to be the currently active base
                _ClonedWorldObject.InQueueState = newBase._PreviousBase.gameObject;
                _ClonedWorldObject.BuildingState = newBase._PreviousBase.gameObject;

                // Get references to all the buildings that were in the previous base & allocate them to the new one
                // Depot/Generator slots
                for (int i = 0; i < newBase.DepotSlots.Count; i++) {

                    // Only go as far as the previous bases slot size
                    if (i < newBase._PreviousBase.DepotSlots.Count) {

                        // Reallocate building to new base (if there is one)
                        if (newBase._PreviousBase.DepotSlots[i] != null) {

                            // Send building to new base
                            newBase.DepotSlots[i].SetBuildingOnSlot(newBase._PreviousBase.DepotSlots[i].GetBuildingOnSlot());
                            newBase._PreviousBase.DepotSlots[i].SetBuildingOnSlot(null);
                        }
                        else { continue; }
                    }
                    else { break; }
                }

                // Tower slots
                for (int i = 0; i < newBase.TowerSlots.Count; i++) {

                    // Only go as far as the previous bases slot size
                    if (i < newBase._PreviousBase.TowerSlots.Count) {

                        // Reallocate tower to new base (if there is one)
                        if (newBase._PreviousBase.TowerSlots[i] != null) {

                            // Send tower to new base
                            newBase.TowerSlots[i].SetBuildingOnSlot(_PreviousBase.TowerSlots[i].GetBuildingOnSlot());
                            newBase._PreviousBase.TowerSlots[i].SetBuildingOnSlot(null);
                        }
                        else { continue; }
                    }
                    else { break; }
                }

                // Pass the building queue to the new building
                for (int i = 0; i < newBase._PreviousBase.GetBuildingQueue().Count; i++) {

                    newBase.AddToQueue(newBase._PreviousBase.GetBuildingQueue()[i]);
                }
            }

            // Update attached base reference
            if (buildingSlot != null) { buildingSlot.AttachedBase = _ClonedWorldObject.GetComponent<Base>(); }

            // Update new bases health with the old bases health
            _ClonedWorldObject.SetHitPoints(hitpoints);
        }
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

        // Create a rally point
        if (GameManager.Instance.RallyPointObject != null && RallyPointDefaultTransform != null) {

            // Set rally point
            _Rallypoint = ObjectPooling.Spawn(GameManager.Instance.RallyPointObject, RallyPointDefaultTransform.transform.position);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  d
    /// </summary>
    public override void OnDeath() {
        
        // This is an enemy base >> remove its building slot from the enemy slot array in the wave manager
        if (Team == GameManager.Team.Attacking) { WaveManager.Instance.EnemyBaseDestroyed(AttachedBuildingSlot.AttachedBase); }

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

    /// <summary>
    //  
    /// </summary>
    /// <param name="building"></param>
    public void RemoveFromList(Building building) {

        // look for match
        for (int i = 0; i < _BuildingList.Count; i++) {

            // Check for reference match
            if (_BuildingList[i] == building) {

                // Remove from list
                _BuildingList.RemoveAt(i);
                break;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Create a rally point
    /// </summary>
    public void CreateRallyPoint() {

        // Check for null reference
        if (GameManager.Instance.RallyPointObject != null && RallyPointDefaultTransform != null) {

            // Set rally point
            _Rallypoint = ObjectPooling.Spawn(GameManager.Instance.RallyPointObject, RallyPointDefaultTransform.transform.position);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}