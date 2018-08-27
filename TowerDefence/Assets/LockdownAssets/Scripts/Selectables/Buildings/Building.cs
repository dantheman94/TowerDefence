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
//  Last edited on: 21/8/2018
//
//******************************

[System.Serializable]
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
    public bool HasABuildingQueue = true;
    [Space]
    public List<Abstraction> Selectables;

    [Space]
    [Header("-----------------------------------")]
    [Header(" MINIMAP PROPERTIES")]
    [Space]
    public GameObject QuadMinimap = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected RecycleBuilding _RecycleOption;
    protected GameObject _Rallypoint = null;

    protected bool _IsBuildingSomething = false;
    protected WorldObject _ObjectBeingBuilt = null;
    protected bool _IsInBuildingQueue = false;
    private List<Abstraction> _BuildingQueue;
    private UI_BuildingQueue _BuildingQueueUI = null;

    private bool _RebuildNavmesh = false;

    private Renderer _MinimapQuadRenderer;

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
        if (HasABuildingQueue) { _BuildingQueue = new List<Abstraction>(); }

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

        // Get component references
        if (QuadMinimap != null) { _MinimapQuadRenderer = QuadMinimap.GetComponent<Renderer>(); }
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

        // Update building queue
        if (_BuildingQueue != null) {

            if (_BuildingQueue.Count > 0) {

                // Check if current building is complete
                _BuildingQueue[0]._ObjectState = WorldObjectStates.Building;
                if (_BuildingQueue[0].GetCurrentBuildTimeRemaining() <= 0f) {

                    // Remove from queue
                    _BuildingQueue.RemoveAt(0);

                    // Start building next item
                    if (_BuildingQueue.Count > 0) { _BuildingQueue[0].StartBuildingObject(); }

                    // Update building queue UI
                    _BuildingQueueUI.UpdateQueueItemList();
                }
            }
        }

        // Change minimap colour based on attacking/defending & team colour
        if (_MinimapQuadRenderer != null) {

            // Attacking team colour
            if (Team == GameManager.Team.Attacking) { _MinimapQuadRenderer.material.color = WaveManager.Instance.AttackingTeamColour; }

            // Defending team
            else if (Team == GameManager.Team.Defending) {

                // Use individual player colour
                if (_Player) { _MinimapQuadRenderer.material.color = _Player.TeamColor; }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Is called during the next frame. (1 frame delay)
    /// </summary>
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
        else if (_ObjectState == WorldObjectStates.Building || _ObjectState == WorldObjectStates.InQueue) {

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

        // Update building queue UI
        ///Base attachBase = AttachedBuildingSlot.AttachedBase;
        ///if (attachBase != null) {
        ///
        ///    if (attachBase._BuildingQueueUI != null) { attachBase._BuildingQueueUI.UpdateQueueItemList(); }
        ///}
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

            Base attachedBase = buildingSlot.AttachedBase;
            Building attachedBuilding = buildingSlot.AttachedBuilding;

            if (buildingSlot.ObjectsCreatedAreQueued) {

                // Add to attached base list (if valid)
                if (attachedBase != null) {

                    attachedBase.AddBuildingToList(building);
                    ///attachedBase.AddToQueue(building);

                    // Add to queue wrapper
                    if (building._BuildingQueueUI != null) {

                        if (!UI_BuildingQueueWrapper.Instance.ContainsQueue(building._BuildingQueueUI)) { UI_BuildingQueueWrapper.Instance.AddNewQueue(building._BuildingQueueUI); }
                        building._BuildingQueueUI.transform.SetParent(UI_BuildingQueueWrapper.Instance.QueueListTransform);
                        building._BuildingQueueUI.gameObject.SetActive(true);
                    }
                    else { building.CreateQueueWidget(); }
                }

                // Add to attached building list (if valid)
                if (attachedBuilding != null && attachedBase == null) {

                    if (!attachedBuilding.RemoveFromQueue(building)) {

                        attachedBuilding.AddToQueue(building);
                    }

                    // Add to queue wrapper
                    if (building._BuildingQueueUI != null) {

                        if (!UI_BuildingQueueWrapper.Instance.ContainsQueue(building._BuildingQueueUI)) { UI_BuildingQueueWrapper.Instance.AddNewQueue(building._BuildingQueueUI); }
                        building._BuildingQueueUI.transform.SetParent(UI_BuildingQueueWrapper.Instance.QueueListTransform);
                        building._BuildingQueueUI.gameObject.SetActive(true);
                    }
                    else { building.CreateQueueWidget(); }
                }

                // Send message to match feed
                MatchFeed.Instance.AddMessage(string.Concat(ObjectName, " added to queue."));
            }
            
            // Skip the queue process and start building the object immediately
            else { /// (!buildingSlot.ObjectsCreatedAreQueued)

                // Remove it from the base/building's queue (WorldObject class adds it at the start!)
                if (attachedBase != null)       { attachedBase.RemoveFromQueue(building); }
                if (attachedBuilding != null)   { attachedBuilding.RemoveFromQueue(building); }

                // Start building the object
                building.StartBuildingObject();
            }

            // Set rally point
            if (buildingSlot.AttachedBase != null) { _Rallypoint = buildingSlot.AttachedBase.GetRallyPoint(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Creates the building's personal queue widget 
    //  (the widget will not display anything until there are items in the queue.
    /// </summary>
    public void CreateQueueWidget() {

        // Create queue widget
        if (_BuildingQueueUI == null) {

            _BuildingQueueUI = Instantiate(UI_BuildingQueueWrapper.Instance.BuildingQueueStencil);
            _BuildingQueueUI.SetBuildingAttached(this);
        }

        // Add to wrapper singleton list
        if (!UI_BuildingQueueWrapper.Instance.ContainsQueue(_BuildingQueueUI)) { UI_BuildingQueueWrapper.Instance.AddNewQueue(_BuildingQueueUI); }
        _BuildingQueueUI.transform.SetParent(UI_BuildingQueueWrapper.Instance.QueueListTransform);
        _BuildingQueueUI.gameObject.SetActive(true);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Destroys the building & returns some resources back to the player who owns the building.
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

            // Remove from queues/lists
            // Bases
            if (AttachedBuildingSlot.AttachedBase != null) {

                AttachedBuildingSlot.AttachedBase.RemoveFromQueue(AttachedBuildingSlot.GetBuildingOnSlot());
                AttachedBuildingSlot.AttachedBase.RemoveFromList(AttachedBuildingSlot.GetBuildingOnSlot());

                if (UI_BuildingQueueWrapper.Instance.ContainsQueue(_BuildingQueueUI)) {

                    UI_BuildingQueueWrapper.Instance.RemoveFromQueue(_BuildingQueueUI);
                    Destroy(_BuildingQueueUI);
                }
            }
            // Buildings
            if (AttachedBuildingSlot.AttachedBuilding != null && AttachedBuildingSlot.AttachedBase == null) {

                AttachedBuildingSlot.AttachedBuilding.RemoveFromQueue(AttachedBuildingSlot.GetBuildingOnSlot());

                if (UI_BuildingQueueWrapper.Instance.ContainsQueue(_BuildingQueueUI)) {

                    UI_BuildingQueueWrapper.Instance.RemoveFromQueue(_BuildingQueueUI);
                    Destroy(_BuildingQueueUI);
                }
            }

            // Destroy Building
            if (_BuildingQueueUI != null) { UI_BuildingQueueWrapper.Instance.RemoveFromQueue(_BuildingQueueUI); }
            ObjectPooling.Despawn(AttachedBuildingSlot.GetBuildingOnSlot().gameObject);

            // Send message to match feed
            MatchFeed.Instance.AddMessage(string.Concat(ObjectName, " recycled."));
        }

        // Make building slot available again
        AttachedBuildingSlot.SetBuildingOnSlot(null);
        AttachedBuildingSlot.gameObject.SetActive(true);

        // Update building queue UI
        if (AttachedBuildingSlot.AttachedBase != null) { AttachedBuildingSlot.AttachedBase._BuildingQueueUI.UpdateQueueItemList(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is killed/destroyed
    /// </summary>
    public override void OnDeath() {
        base.OnDeath();
        
        // Detach from any bases & buildings
        if (AttachedBuildingSlot != null) {

            Base b = AttachedBuildingSlot.AttachedBase;
            if (b != null) {

                // Detach from base
                b.RemoveFromList(this);
                b.RemoveFromQueue(this);
            }

            // Detach from slot
            AttachedBuildingSlot.SetBuildingOnSlot(null);
        }
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
    //  Adds an abstraction object to the building's personal queue.
    /// </summary>
    /// <param name="queueObject"></param>
    public void AddToQueue(Abstraction queueObject) {

        _BuildingQueue.Add(queueObject);

        if (_BuildingQueueUI == null) { CreateQueueWidget(); }
        _BuildingQueueUI.UpdateQueueItemList();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference to the building's personal queue.
    /// </summary>
    /// <returns>
    //  List<WorldObject>
    /// </returns>
    public List<Abstraction> GetBuildingQueue() { return _BuildingQueue; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns TRUE if the object is either inQueue/building/active in the game world.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public override bool IsInWorld() { return base.IsInWorld() || _ObjectState == WorldObjectStates.InQueue; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Checks to see if THIS building is in the building's queue 
    //  & removes it from the list if there is a match.
    /// </summary>
    /// <param name="queueBuilding"></param>
    /// <returns>
    //  bool
    /// returns>
    public bool RemoveFromQueue(Building buildingToRemove) {

        if (GetBuildingQueue() != null) {

            // Loop through the queue
            for (int i = 0; i < GetBuildingQueue().Count; i++) {

                // Reference match?
                if (GetBuildingQueue()[i] == buildingToRemove) {

                    // Remove from the queue
                    GetBuildingQueue().RemoveAt(i);
                    _BuildingQueueUI.UpdateQueueItemList();
                    return true;
                }
            }
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  GameObject
    /// </returns>
    public GameObject GetRallyPoint() { return _Rallypoint; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}