using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/6/2018
//
//******************************

public class Building : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUILDING PROPERTIES ")]
    [Space]
    public BuildingSlot AttachedBuildingSlot = null;
    public float ObjectHeight = 15f;
    [Space]
    public List<Abstraction> Selectables;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected RecycleBuilding _RecycleOption;
    protected bool _IsBuildingSomething = false;
    protected WorldObject _ObjectBeingBuilt = null;
    private List<GameObject> _BuildingQueue;

    private bool _RebuildNavmesh = false;

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
        
        // Initialize
        _ObjectHeight = ObjectHeight;
        _BuildingQueue = new List<GameObject>();

        // Create upgrade instances & replace the selectable reference
        for (int i = 0; i < Selectables.Count; i++) {

            if (Selectables[i] != null) {

                // Check if the selectable option is an upgrade tree
                UpgradeTree tree = Selectables[i].GetComponent<UpgradeTree>();
                if (tree != null) {

                    // Replace reference with the runtime version
                    UpgradeTree newTree = Instantiate(tree.gameObject).GetComponent<UpgradeTree>();
                    Selectables[i] = newTree;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Force the building to skip the deployable state and go straight to being active in the world
        if (_ObjectState == WorldObjectStates.Deployable) {

            _ObjectState = WorldObjectStates.Active;
            _RebuildNavmesh = true;
            OnActiveState();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    protected void LateUpdate() {

        if (_RebuildNavmesh) {

            // Re-bake navMeshes
            ///GameManager.Instance.RebakeNavmesh();
            _RebuildNavmesh = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is "clicked on" and the selection wheel appears.
    /// </summary>
    public override void OnSelectionWheel() {
        base.OnSelectionWheel();

        // Show the building's options if its active in the world
        if (_ObjectState == WorldObjectStates.Active) {

            // Show building slot wheel
            if (_Player && Selectables.Count > 0) {

                // Update list then display on screen
                _Player._HUD.SelectionWheel.UpdateListWithBuildables(Selectables, AttachedBuildingSlot);

                // Get reference to the recycle building option
                if (Selectables[5] != null) { _RecycleOption = Selectables[5].GetComponent<RecycleBuilding>(); }

                // Show selection wheel
                if (ShowSelectionGUI) { GameManager.Instance.SelectionWheel.SetActive(true); }
            }
            _IsCurrentlySelected = true;
        }

        // Show a wheel with recycle only as the selection (so you can cancel building the world object)
        else if (_ObjectState == WorldObjectStates.Building) {

            if (_Player) {

                List<Abstraction> wheelOptions = new List<Abstraction>();
                for (int i = 0; i < 10; i++) {

                    // Recycle option
                    if (i == 5) {

                        if (_RecycleOption == null) {

                            _RecycleOption = ObjectPooling.Spawn(GameManager.Instance.RecycleBuilding).GetComponent<RecycleBuilding>();
                        }
                        _RecycleOption.SetBuildingToRecycle(this);
                        _RecycleOption.SetToBeDestroyed(true);
                        wheelOptions.Add(_RecycleOption);
                    }

                    // Empty option
                    else { wheelOptions.Add(null); }
                }

                // Update list then display on screen
                _Player._HUD.SelectionWheel.UpdateListWithBuildables(wheelOptions, AttachedBuildingSlot);

                // Show selection wheel
                if (ShowSelectionGUI) { GameManager.Instance.SelectionWheel.SetActive(true); }
            }
            _IsCurrentlySelected = true;
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
        base.OnWheelSelect(buildingSlot);

        // Get reference to the newly cloned building
        if (_ClonedWorldObject != null) {

            Building building = _ClonedWorldObject.GetComponent<Building>();
            building.AttachedBuildingSlot = buildingSlot;

            // Update building slot ref with building
            buildingSlot.SetBuildingOnSlot(building);

            // Disable building slot (is re-enabled when the building is recycled)
            buildingSlot.SetIsSelected(false);

            // Add to attached base list (if valid)
            Base attachedBase = buildingSlot.AttachedBase;
            if (attachedBase != null ) { attachedBase.AddBuildingToList(building); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    public void RecycleBuilding() {

        // Deselect all objects
        foreach (var selectable in GameManager.Instance.Selectables) { selectable.SetIsSelected(false); }

        // Add resources back to player
        if (_Player != null) {

            _Player.SuppliesCount += GetRecycleSuppliesAmount();
            _Player.PowerCount += GetRecyclePowerAmount();
        }

        // Destroy building
        if (_RecycleOption) {
            
            // Deselect self
            SetIsSelected(false);

            // Destroy Widgets
            if (AttachedBuildingSlot.GetBuildingOnSlot()._HealthBar) { ObjectPooling.Despawn(AttachedBuildingSlot.GetBuildingOnSlot()._HealthBar.gameObject); }
            if (AttachedBuildingSlot.GetBuildingOnSlot()._BuildingProgressCounter) { ObjectPooling.Despawn(AttachedBuildingSlot.GetBuildingOnSlot()._BuildingProgressCounter.gameObject); }

            // Destroy Building
            ObjectPooling.Despawn(AttachedBuildingSlot.GetBuildingOnSlot().gameObject);
        }

        // Make building slot available again
        AttachedBuildingSlot.SetBuildingOnSlot(null);
        AttachedBuildingSlot.gameObject.SetActive(true);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="value"></param>
    public void SetIsBuildingSomething(bool value) { _IsBuildingSomething = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="obj"></param>
    public void SetObjectBeingBuilt(WorldObject obj) { _ObjectBeingBuilt = obj; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="queueObject"></param>
    public void AddToQueue(GameObject queueObject) { _BuildingQueue.Add(queueObject); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  List<WorldObject>
    /// </returns>
    public List<GameObject> GetBuildingQueue() { return _BuildingQueue; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}