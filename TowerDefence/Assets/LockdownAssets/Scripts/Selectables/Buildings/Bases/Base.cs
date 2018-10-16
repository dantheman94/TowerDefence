using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    protected Base _PreviousBase = null;

    protected List<Unit> _EnemyUnits = null;

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
        _BuildingList = new List<Building>();
        _EnemyUnits = new List<Unit>();

        // Set matching team for all the building slots
        for (int i = 0; i < DepotSlots.Count; i++) {

            DepotSlots[i].Team = Team;
            DepotSlots[i]._Player = _Player;
        }
        for (int i = 0; i < TowerSlots.Count; i++) {

            TowerSlots[i].Team = Team;
            TowerSlots[i]._Player = _Player;
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
        
        // Create the base and initialize it
        base.OnWheelSelect(buildingSlot);
        _ClonedWorldObject.SetClonedObject(_ClonedWorldObject);
        _ClonedWorldObject.GetComponent<Base>()._PreviousBase = _PreviousBase;

        // Start building the base (if were meant to)
        Base attachedBase = buildingSlot.AttachedBase;
        if (attachedBase != null) {

            // This base is at the front of the queue
            if (attachedBase.GetBuildingQueue()[0] == _ClonedWorldObject) {

                // Start building it
                StartBuildingObject(buildingSlot);
            }
        }

        // Set matching team for all the building slots
        for (int i = 0; i < DepotSlots.Count; i++) {

            DepotSlots[i].Team = Team;
            DepotSlots[i]._Player = _Player;
        }
        for (int i = 0; i < TowerSlots.Count; i++) {

            TowerSlots[i].Team = Team;
            TowerSlots[i]._Player = _Player;
        }
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
                hitpoints = _PreviousBase._HitPoints;
                if (newBase._PreviousBase._HealthBar != null) { ObjectPooling.Despawn(newBase._PreviousBase._HealthBar.gameObject); }                

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
                            if (newBase._PreviousBase.DepotSlots[i].GetBuildingOnSlot() != null)
                            {
                                newBase._PreviousBase.DepotSlots[i].GetBuildingOnSlot().AttachedBuildingSlot = newBase.DepotSlots[i];
                                newBase.DepotSlots[i].SetBuildingOnSlot(newBase._PreviousBase.DepotSlots[i].GetBuildingOnSlot());
                                newBase._PreviousBase.DepotSlots[i].SetBuildingOnSlot(null);
                            }
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
                            if (newBase._PreviousBase.TowerSlots[i].GetBuildingOnSlot() != null)
                            {
                                newBase._PreviousBase.TowerSlots[i].GetBuildingOnSlot().AttachedBuildingSlot = newBase.TowerSlots[i];
                                newBase.TowerSlots[i].SetBuildingOnSlot(newBase._PreviousBase.TowerSlots[i].GetBuildingOnSlot());
                                newBase._PreviousBase.TowerSlots[i].SetBuildingOnSlot(null);
                            }
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
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object's state switches to active
    /// </summary>
    protected override void OnBuilt() {
        base.OnBuilt();
        
        if (_Player != null) {

            // Add tech level if required
            if (_Player.Level < TechLevelWhenBuilt) { _Player.Level = TechLevelWhenBuilt; }

            // Add to player's base list
            _Player.AddBase(_ClonedWorldObject as Base);
        }

        // Show any hidden base slots that are linked to the building slot
        if (AttachedBuildingSlot != null) { AttachedBuildingSlot.SetLinkedSlotsBase(this); }
        
        // Pass the building queue to the new building   
        Base newBase = _ClonedWorldObject.GetComponent<Base>();
        if (newBase._PreviousBase != null) {

            for (int i = 0; i < newBase._PreviousBase.GetBuildingQueue().Count; i++) {

                // BUT DONT ADD OURSELF TO THE QUEUE
                if (newBase._PreviousBase.GetBuildingQueue()[i] != _ClonedWorldObject) {

                    // Add to queue
                    newBase.AddToQueue(newBase._PreviousBase.GetBuildingQueue()[i]);
                }
            }

            // Clear/destroy the previous building's queue
            newBase._PreviousBase.GetBuildingQueue().Clear();
            if (UI_BuildingQueueWrapper.Instance.ContainsQueue(newBase._PreviousBase._BuildingQueueUI)) {

                UI_BuildingQueueWrapper.Instance.RemoveFromQueue(newBase._PreviousBase._BuildingQueueUI);
                Destroy(newBase._PreviousBase._BuildingQueueUI);
            }

            //  Passes the new bases's building slot to the old bases's building slots's building (confusing right?)
            for (int i = 0; i < newBase._PreviousBase.TowerSlots.Count; ++i)
            {
                // Tower slots
                ///BuildingSlot oldBaseSlot = newBase._PreviousBase.TowerSlots[i];
                ///BuildingSlot newBaseSlot = newBase.TowerSlots[i];
                ///if (oldBaseSlot.GetBuildingOnSlot() != null)
                ///{
                ///    oldBaseSlot.GetBuildingOnSlot().AttachedBuildingSlot = newBaseSlot;
                ///}                

                ///if(newBase._PreviousBase.TowerSlots[i].GetBuildingOnSlot() != null)
                ///{
                ///    newBase.TowerSlots[i].GetBuildingOnSlot().AttachedBuildingSlot = newBase._PreviousBase.TowerSlots[i];
                ///}
            }

            // Passes the new bases's building slot to the old bases's building slots's building (confusing right?)
            for (int i = 0; i < newBase._PreviousBase.DepotSlots.Count; ++i)
            {
                // Depot slots
                ///BuildingSlot oldBaseSlot = newBase._PreviousBase.DepotSlots[i];
                ///BuildingSlot newBaseSlot = newBase.DepotSlots[i];
                ///if (oldBaseSlot.GetBuildingOnSlot() != null)
                ///{
                ///    oldBaseSlot.GetBuildingOnSlot().AttachedBuildingSlot = newBaseSlot;
                ///}

                ///if (newBase._PreviousBase.DepotSlots[i].GetBuildingOnSlot() != null)
                ///{
                ///    newBase.TowerSlots[i].GetBuildingOnSlot().AttachedBuildingSlot = newBase._PreviousBase.DepotSlots[i];
                ///}
            }

        }

        // Create a rally point
        if (GameManager.Instance.RallyPointObject != null && RallyPointDefaultTransform != null) {

            // Set rally point
            _Rallypoint = ObjectPooling.Spawn(GameManager.Instance.RallyPointObject, RallyPointDefaultTransform.transform.position);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Damages the object by a set amount.
    /// </summary>
    /// <param name="damage"></param>
    public override void Damage(float damage, WorldObject instigator = null) {
        base.Damage(damage, instigator);

        // Check if the enemies attached to this base should retreat?
        if (_EnemyUnits.Count > 0 && Team == GameManager.Team.Attacking) {

            // Retreat check
            for (int i = 0; i < _EnemyUnits.Count; i++) { _EnemyUnits[i].RetreatToLockdownBaseCheck(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  d
    /// </summary>
    public override void OnDeath(WorldObject instigator) {
        
        // This is an enemy base >> remove its building slot from the enemy slot array in the wave manager
        if (Team == GameManager.Team.Attacking) { WaveManager.Instance.EnemyBaseDestroyed(AttachedBuildingSlot.AttachedBase); }
        else if (Team == GameManager.Team.Defending) { _Player.GetBaseList().Remove(this); }

        // Destroy all the attached buildings (towers, depots, etc...)
        for (int i = 0; i < _BuildingList.Count; i++) {

            float dmgHealth = _BuildingList[i].GetHitPoints();
            float dmgShield = _BuildingList[i].GetShieldPoints();
            _BuildingList[i].Damage((int)dmgHealth);
        }
        _BuildingList.Clear();

        // Now we can safely be destroyed
        base.OnDeath(instigator);
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
    //  Creates a rally point for newly spawned units to seek to when built.
    /// </summary>
    public void CreateRallyPoint() {

        // Check for null reference
        if (GameManager.Instance.RallyPointObject != null && RallyPointDefaultTransform != null) {

            // Set rally point
            _Rallypoint = ObjectPooling.Spawn(GameManager.Instance.RallyPointObject, RallyPointDefaultTransform.transform.position);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets reference to the _Previous base memeber variable.
    /// </summary>
    /// <param name="value"></param>
    public void SetPreviousBase(Base value) { _PreviousBase = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference to the _EnemyUnits array.
    /// </summary>
    /// <returns>
    //  List<Unit>
    /// </returns>
    public List<Unit> GetEnemyUnitList() { return _EnemyUnits; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Vector3
    /// </returns>
    public Vector3 GetPositionNearTheBase() {

        // Generate a sphere
        SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
        sphere.center = transform.position;
        sphere.radius = 50f;
        sphere.isTrigger = true;

        // Random position somewhere within the sphere
        float randX = Random.Range(sphere.center.x - sphere.radius, sphere.center.x + sphere.radius);
        float randZ = Random.Range(sphere.center.z - sphere.radius, sphere.center.z + sphere.radius);
        Vector3 pos = new Vector3(randX, gameObject.transform.position.y, randZ);
        return pos;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}