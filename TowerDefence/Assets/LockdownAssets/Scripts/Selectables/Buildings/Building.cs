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
    public ParticleSystem EffectPlayedWhenBuildingSelectable = null;
    public ParticleSystem EffectPlayedOnBuiltSelectable = null;
    public Transform BuiltSelectableEffectPosition;
    [Space]
    public List<Abstraction> Selectables;


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
    protected List<Abstraction> _BuildingQueue;
    protected UI_BuildingQueue _BuildingQueueUI = null;

    private ParticleSystem _SelectableBuildingEffect = null;
    private ParticleSystem _SelectableBuiltEffect = null;
    
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
                    UpgradeTree newTree = ObjectPooling.Spawn(tree.gameObject).GetComponent<UpgradeTree>();
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
            OnActiveState();
        }

        // Update building queue
        if (_BuildingQueue != null) {

            if (_BuildingQueue.Count > 0) {

                // Check if current building is complete
                _BuildingQueue[0]._ObjectState = WorldObjectStates.Building;
                if (_BuildingQueue[0].GetCurrentBuildTimeRemaining() <= 0f) {

                    // Play "on selectable built" particle effect if neccessary
                    if (EffectPlayedOnBuiltSelectable != null) {

                        // Assign internal effect & play
                        _SelectableBuiltEffect = ObjectPooling.Spawn(EffectPlayedOnBuiltSelectable.gameObject, BuiltSelectableEffectPosition.position, transform.rotation).GetComponent<ParticleSystem>();
                        _SelectableBuiltEffect.Play();

                        // Despawn particle effect when it has finished its cycle
                        float duration = _SelectableBuiltEffect.duration + _SelectableBuiltEffect.startLifetime;
                        StartCoroutine(ParticleDespawn(_SelectableBuiltEffect, duration));
                    }

                    // Remove from queue
                    _BuildingQueue.RemoveAt(0);

                    // Start building next item
                    if (_BuildingQueue.Count > 0) { _BuildingQueue[0].StartBuildingObject(); }

                    // Update building queue UI
                    _BuildingQueueUI.UpdateQueueItemList();
                }

                // Currently building something
                else {

                    // Play "on building selectable" particle effect if neccessary
                    if (EffectPlayedWhenBuildingSelectable != null && _SelectableBuildingEffect == null) {

                        // Assign internal effect & play
                        _SelectableBuildingEffect = ObjectPooling.Spawn(EffectPlayedWhenBuildingSelectable.gameObject, BuiltSelectableEffectPosition.position, transform.rotation).GetComponent<ParticleSystem>();
                        _SelectableBuildingEffect.Play();

                        // Despawn particle effect when it has finished its cycle
                        float duration = _BuildingQueue[0].BuildingTime;
                        StartCoroutine(ParticleDespawn(_SelectableBuildingEffect, duration));
                    }

                    if (_SelectableBuildingEffect != null) {

                        // Building particle is complete so null the reference (so next time its used - it repools a new instance)
                        if (!_SelectableBuildingEffect.isPlaying) { _SelectableBuildingEffect = null; }
                    }
                }
            }
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

            // Update building slot team for the outline colouring
            Building building = _ClonedWorldObject.GetComponent<Building>();
            building.AttachedBuildingSlot = buildingSlot;
            building.AttachedBuildingSlot.Team = _ClonedWorldObject.Team;

            // Update building slot ref with building
            buildingSlot.SetBuildingOnSlot(building);
            buildingSlot.SetIsSelected(false);
            buildingSlot.SetOutlineVisibility(false);

            Base attachedBase = buildingSlot.AttachedBase;
            Building attachedBuilding = buildingSlot.AttachedBuilding;

            // Add the building to the buildingslot's building's queue (i know, confusing right?)
            if (buildingSlot.ObjectsCreatedAreQueued) {

                // Add to attached base list (if valid)
                if (attachedBase != null) {

                    attachedBase.AddBuildingToList(building);
                    ///attachedBase.AddToQueue(building);

                    // Get rally point reference
                    if (attachedBase.GetRallyPoint() == null) { attachedBase.CreateRallyPoint(); }
                    building._Rallypoint = attachedBase.GetRallyPoint();

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
    //  Starts the construction process of this building.
    /// </summary>
    /// <param name="buildingSlot"></param>
    public override void StartBuildingObject(BuildingSlot buildingSlot = null) {
        base.StartBuildingObject(buildingSlot);

        // Determine build time
        if (_Player != null) {

            UpgradeManager upgradeManager = _Player.GetUpgradeManager();
            BuildingTime *= (int)upgradeManager._BuildingSpeedMultiplier;
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
            OnDeath(null);

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
    public override void OnDeath(WorldObject instigator) {
        base.OnDeath(instigator);
        
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
    //  A coroutine that waits for the seconds specified then attempts to re-pool
    //  the particle effect (or destroyed entirely if re-pooling isn't possible)
    /// </summary>
    /// <param name="particleEffect"></param>
    /// <param name="delay"></param>
    /// <returns>
    //  IEnumerator
    /// </returns>
    IEnumerator ParticleDespawn(ParticleSystem particleEffect, float delay) {

        // Delay
        yield return new WaitForSeconds(delay);

        // Despawn the system
        ObjectPooling.Despawn(particleEffect.gameObject);
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