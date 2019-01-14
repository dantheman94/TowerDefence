using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 12/11/2018
//
//******************************

public class Unit : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UNIT PROPERTIES")]
    [Space]
    public EUnitType UnitType = EUnitType.Undefined;
    public ECombatType CombatType = ECombatType.GroundInfantry;
    public ENavmeshType NavmeshType = ENavmeshType.Ground;
    [Tooltip("The movement/walking speed of this unit." +
            "\n\nNOTE: ONLY APPLIES TO GROUND INFANTRY, VEHICLES DO NOT USE THIS VALUE.")]
    public float InfantryMovementSpeed = 10f;
    [Space]
    public bool CanBePlayerControlled = false;
    [Space]
    public ConeCollider SightCone = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" WEIGHTED TARGETTING")]
    [Space]
    public TargetWeight[] TargetWeights = new TargetWeight[_WeightLength];
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" COMBAT/WEAPON PROPERTIES")]
    [Space]
    public Weapon PrimaryWeapon = null;
    public Weapon SecondaryWeapon = null;
    [Space]
    public List<GameObject> MuzzleLaunchPoints;
    [Space]
    public float MaxAttackingRange = 100f;
    public float IdealAttackRangeMax = 80f;
    public float IdealAttackRangeMin = 40f;
    [Space]
    public float ChasingDistance = 30f;
    [Space]
    public bool CanBeStunned = false;
    [Space]
    public float BarrierDetectionDistance = 20f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" VETERANCY PROPERTIES")]
    [Space]
    [Range(0, 3)]
    public int StartingVeterancyLevel = 0;
    public int XPGrantedOnDeath = 1;
    [Space]
    public int[] VetTargetXPs = new int[VeterancyLength] { 5, 8, 12 };
    [Space]
    public float[] VetDamages = new float[VeterancyLength + 1] { 1f, 1.15f, 1.3f, 1.45f };

    [Space]
    [Header("-----------------------------------")]
    [Header(" SCORE PROPERTIES")]
    [Space]
    public int ScoreGrantedEnemy = 100;
    public int ScoreDeductedFriendly = 100;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [System.Serializable]
    public struct TargetWeight {

        public EUnitType UnitType;
        public int Weight;
    }

    public const int _WeightLength = (int)EUnitType.ENUM_COUNT;

    public enum EUnitType { Undefined, DwfSoldier, DwfSpecialistInfantry, DwfSpecialistVehicle, Grumblebuster, Skylancer, Ballista, Catapult, SiegeEngine, LightAirship, SupportShip, HeavyAirship, ENUM_COUNT }
    public enum ECombatType { GroundInfantry, GroundVehicle, AirVehicle }
    public enum ENavmeshType { Ground, Air }

    protected WorldObject _AttackTarget = null;
    protected List<WorldObject> _PotentialTargets;

    protected bool _IsFollowingPlayerCommand = false;
    protected bool _IsAttacking;
    protected bool _IsChasing;
    protected bool _IsSeeking;
    protected bool _IsReturningToOrigin;
    protected bool _IsFiring = false;
    protected bool _IsGuarding = false;

    protected Vector3 _ChaseOriginPosition = Vector3.zero;
    protected Vector3 _SeekTarget = Vector3.zero;

    protected bool _PathInterupted = false;
    protected WorldObject _InteruptionInstigator = null;

    protected AttackPath _AttackPath = null;
    protected int _AttackPathIterator = 0;
    protected bool _AttackPathComplete = false;

    protected Building _AttachedBuilding;

    protected CharacterController _CharacterController = null;
    protected NavMeshAgent _Agent = null;
    protected GameObject _SeekWaypoint = null;
    private bool _DisplaySeekWaypoint = false;
    protected float _DistanceToTarget = 0f;
    protected bool _IsBeingPlayerControlled = false;

    private float SnapLookAtRange = 0f;
    private BuildingSlot _BuildingSlotInstigator = null;

    protected NavMeshPath _NavMeshPath;
    protected bool _ChaseCoroutineIsRunning = false;
    protected bool _ResetOriginCoroutineIsRunning = false;

    protected int _VeterancyLevel = 0;
    protected int _CurrentVetXP = 0;
    protected int _TargetVetXP = 3;

    private const int VeterancyLength = 3;
    private UnitVeterancyCounter _UnitVeterancyWidget = null;

    private SphereCollider _SightSphere = null;
    private SphereCollider _HearingSphere = null;

    private Base _LockdownBase = null;

    protected UnitDialogue _Dialogue = null;

    protected float _OriginalMovementSpeed = 0f;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the script is loaded or a value is changed in the inspector.
    /// </summary>
    private void OnValidate() {
        
        if (VetTargetXPs.Length != VeterancyLength) {

            Debug.LogWarning("Don't change the 'VetLevelTargetXP' field's array size!");
            System.Array.Resize(ref VetTargetXPs, VeterancyLength);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called before Start().
    /// </summary>
    protected override void Awake() {
        base.Awake();

        // Behavioural value precautions
        if (IdealAttackRangeMax < IdealAttackRangeMin) { IdealAttackRangeMax = IdealAttackRangeMin * 1.5f; }
        if (MaxAttackingRange < IdealAttackRangeMax)   { MaxAttackingRange = IdealAttackRangeMax; }
        if (MaxAttackingRange < IdealAttackRangeMax) { MaxAttackingRange = IdealAttackRangeMax; }
        SnapLookAtRange = IdealAttackRangeMin / 2;

        // Set fog vision radius
        if (_FogOfWarSight != null) { _FogOfWarSight.Radius = MaxAttackingRange * 1.5f; }

        // Get sensory collider references
        SphereCollider[] sphereCols = GetComponentsInChildren<SphereCollider>();
        for (int i = 0; i < sphereCols.Length; i++) {
            
            if (sphereCols[i].CompareTag("SightSphere"))    { _SightSphere = sphereCols[i]; continue; }
            if (sphereCols[i].CompareTag("HearingSphere"))  { _HearingSphere = sphereCols[i]; continue; }
        }

        // Set sensorary sizes
        if (_SightSphere != null && _FogOfWarSight != null) { _SightSphere.radius = _FogOfWarSight.Radius * 0.9f; }
        if (_HearingSphere != null && _FogOfWarSight != null) { _HearingSphere.radius = _FogOfWarSight.Radius * 1f; }
        if (SightCone != null && _FogOfWarSight != null) { SightCone.SetDistance(_FogOfWarSight.Radius * 0.9f); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        _PotentialTargets = new List<WorldObject>();

        // Create copy of weapons & re-assign them to replace the old ones
        if (PrimaryWeapon != null) {

            PrimaryWeapon = ObjectPooling.Spawn(PrimaryWeapon.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Weapon>();
            PrimaryWeapon.SetUnitAttached(this);
        }
        if (SecondaryWeapon != null) {

            PrimaryWeapon = ObjectPooling.Spawn(SecondaryWeapon.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Weapon>();
            SecondaryWeapon.SetUnitAttached(this);
        }

        // Get component references
        _Dialogue = GetComponent<UnitDialogue>();
        _CharacterController = GetComponent<CharacterController>();
        _Agent = GetComponent<NavMeshAgent>();
        _ObjectHeight = _Agent.height;
        _OriginalMovementSpeed = InfantryMovementSpeed;

        // Initialize veterancy
        _VeterancyLevel = StartingVeterancyLevel;
        _CurrentVetXP = 0;
        _TargetVetXP = VetTargetXPs[0];
        if (_VeterancyLevel > 0) {

            // Create a veterancy widget and allocate it to the unit
            if (_UnitVeterancyWidget == null && GameManager.Instance != null) { _UnitVeterancyWidget = ObjectPooling.Spawn(GameManager.Instance.UnitVeterancyPanel.gameObject).GetComponent<UnitVeterancyCounter>(); }
            if (_UnitVeterancyWidget != null && _Player != null) {

                _UnitVeterancyWidget.SetCameraAttached(_Player.CameraAttached);
                _UnitVeterancyWidget.SetUnitAttached(this);
                _UnitVeterancyWidget.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);
                _UnitVeterancyWidget.gameObject.SetActive(true);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Initializes the object.
    /// </summary>
    protected override void Init() {
        base.Init();

        // Reset veterancy
        _VeterancyLevel = 0;
        _CurrentVetXP = 0;
        _TargetVetXP = VetTargetXPs[0];
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();  

        // Update chasing enemy
        if (_IsChasing) { UpdateChasingEnemy(); }
        else { _ChaseOriginPosition = transform.position; }

        // Check for origin return 
        ///if (_IsReturningToOrigin && !_ResetOriginCoroutineIsRunning) { StartCoroutine(ResetToOrigin()); }

        // Update attack path
        UpdateAttackPath();

        // Selecting the unit via drag selection
        UpdateSquadSelection();
        UpdateBoxSelection();

        // Hide the unit UI widgets if it is building
        if (_ObjectState == WorldObjectStates.Building) {

            // Disable components
            if (_CharacterController != null) { _CharacterController.enabled = false; }
            _Agent.enabled = false;

            // Hide the healthbar
            if (_HealthBar != null) { _HealthBar.gameObject.SetActive(false); }

            // Despawn build counter widget (it is unused)
            if (_BuildingProgressCounter != null) { ObjectPooling.Despawn(_BuildingProgressCounter.gameObject); }

            // Force the position of the object to be at the attached buildings vector
            if (_BuildingSlotInstigator != null) {

                switch (NavmeshType) {

                    case ENavmeshType.Ground: {

                        _Agent.transform.position = _BuildingSlotInstigator.AttachedBase.GroundUnitSpawnTransform.transform.position;
                        _Agent.transform.rotation = _BuildingSlotInstigator.AttachedBase.GroundUnitSpawnTransform.transform.rotation;
                        break;
                    }

                    case ENavmeshType.Air: {

                        _Agent.transform.position = _BuildingSlotInstigator.AttachedBase.AirUnitSpawnTransform.transform.position;
                        _Agent.transform.rotation = _BuildingSlotInstigator.AttachedBase.AirUnitSpawnTransform.transform.rotation;
                        break;
                    }

                    default: break;
                }
            }
        }

        // Force the unit to skip the deployable state and go straight to being active in the world
        else if (_ObjectState == WorldObjectStates.Deployable) { OnSpawn(); }

        // Unit is active in the world
        else if (_ObjectState == WorldObjectStates.Active && IsAlive()) {

            // Show the healthbar
            if (_HealthBar != null) { _HealthBar.gameObject.SetActive(true); }

            // Create a healthbar if the unit doesn't have one linked to it
            else {

                if (_Player == null && GameManager.Instance != null) { _Player = GameManager.Instance.Players[0]; }
                if (_Player != null && _ShowHealthbar) { CreateHealthBar(this, _Player.CameraAttached); }
            }            
        }

        // Is the unit currently being player controlled? (manually)
        if (CanBePlayerControlled && _IsCurrentlySelected) {

            // Flip the player/AI controlled states
            if (Input.GetKeyDown(KeyCode.R)) {

                _IsBeingPlayerControlled = !_IsBeingPlayerControlled;

                if (_IsBeingPlayerControlled) {

                    GameManager.Instance.SetUnitControlling(true);

                    // Initialize camera follow script
                    _Player._CameraFollow.SetFollowTarget(this);
                    _Player._CameraFollow.SetCameraAttached(_Player.CameraAttached);
                    _Player._CameraFollow.Init();

                    // Hide any seek points that are currently visible
                    if (_SeekWaypoint) { _SeekWaypoint.SetActive(false); }
                }
                else {

                    GameManager.Instance.SetUnitControlling(false);
                    _Player._CameraFollow.SetFollowing(false);
                    _Player._KeyboardInputManager.CreateCenterPoint();

                    // Reset mouse to center of the screen
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
        else { _IsBeingPlayerControlled = false; }

        // Update player controller movement
        if (_IsBeingPlayerControlled && IsAlive()) {

            _IsCurrentlySelected = true;
            _Agent.enabled = false;
            UpdatePlayerControlledMovement();
        }

        // Update Ai controller movement
        else if (IsAlive()) { UpdateAIControllerMovement(); }

        // Destroying the unit manually
        if (_IsCurrentlySelected && Input.GetKeyDown(KeyCode.Delete)) {

            // Play OnSelectSound for the first unit that was selected
            if (_Player.SelectedUnits[0].GetDialogue() != null)
                _Player.SelectedUnits[0].GetDialogue().PlayDeleteSound();
            
            OnDeath(null);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected void LateUpdate() {

        ///_IsFiring = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="highlight"></param>
    public override void DrawSelection(bool draw) {
        base.DrawSelection(draw);

        // Highlight our attack target
        if (_AttackTarget != null) {

            if (_AttackTarget.Team != GameManager.Team.Defending) { _AttackTarget.SetForceSelectOutline(draw); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="highlight"></param>
    public override void DrawHighlight(bool highlight) {
        base.DrawHighlight(highlight);

        // highlight our attack target
        if (_AttackTarget != null) {

            if (_AttackTarget.Team != GameManager.Team.Defending) { _AttackTarget.SetForceHighlightOutline(highlight); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. Moves through the attack paths nodes for the unit and seeks to them.
    /// </summary>
    private void UpdateAttackPath() {

        if (_AttackPath != null && _AttackTarget == null) {

            // Check if the attack path is complete
            if (_AttackPathIterator + 1 < _AttackPath.GetNodePositions().Count) { _AttackPathComplete = false; }
            else { _AttackPathComplete = true; }

            // Calculate distance from path node
            float dist = Vector3.Distance(transform.position, _AttackPath.GetNodePositions()[_AttackPathIterator]);
            if (dist < _AttackPath.NodeAccuracyRadius) {

                // Update node iterator
                if (_AttackPathIterator + 1 < _AttackPath.GetNodePositions().Count) {

                    _AttackPathIterator++;
                    _AttackPathComplete = false;

                    // Go to point with random offset
                    Vector2 rand = Random.insideUnitCircle * 30f;
                    Vector3 pos = _AttackPath.GetNodePositions()[_AttackPathIterator] + new Vector3(rand.x, _AttackPath.GetNodePositions()[_AttackPathIterator].y, rand.y);

                    // Uncomment this if you want a waypoint to be created at the offsetted node locations in the attack path
                    ///Instantiate(GameManager.Instance.AgentSeekObject, _AttackPath.GetNodePositions()[_AttackPathIterator], Quaternion.identity);

                    StartCoroutine(AgentAttackPathSeekPosition(pos));
                }
                else { _AttackPathComplete = true; }
            }
            else {

                if (!_IsSeeking) { StartCoroutine(AgentAttackPathSeekPosition(_AttackPath.GetNodePositions()[_AttackPathIterator])); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void UpdateSquadSelection() {

        if (_Player != null) {

            if (_Player._KeyboardInputManager.CreateScreenSelection()) {

                Vector3 CamPos = _Player.CameraAttached.WorldToScreenPoint(transform.position);

                if (Player.SelectionScreen.Contains(CamPos)) {

                    if (this.GetObjectState() == WorldObject.WorldObjectStates.Active && !_IsCurrentlySelected) {

                        _Player.SelectedWorldObjects.Add(this);
                        _Player.SelectedUnits.Add(this);
                        this.SetPlayer(_Player);
                        this.SetIsSelected(true);
                        GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
                    }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Checks if unit is selected by click & drag box
    /// </summary>
    private void UpdateBoxSelection() {

        // Precautions
        if (_Player != null) {

            Vector3 camPos = _Player.CameraAttached.WorldToScreenPoint(transform.position);
            camPos.y = KeyboardInput.InvertMouseY(camPos.y);

            if (!Input.GetMouseButton(0)) {

                if (KeyboardInput.Selection.Contains(camPos)) {

                    if (this.GetObjectState() == WorldObject.WorldObjectStates.Active && !_IsCurrentlySelected) {

                        _Player.SelectedWorldObjects.Add(this);
                        _Player.SelectedUnits.Add(this);
                        this.SetPlayer(_Player);
                        this.SetIsSelected(true);
                        GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
                    }
                }
            }
            else {

                if (KeyboardInput.Selection.Contains(camPos)) {

                    if (this.GetObjectState() == WorldObject.WorldObjectStates.Active && !_IsCurrentlyHighlighted) {
                        
                        this.SetPlayer(_Player);
                        this.SetIsHighlighted(true);
                        GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
                    }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Starts the construction process of this AI object.
    /// </summary>
    /// <param name="buildingSlot"></param>
    public override void StartBuildingObject(BuildingSlot buildingSlot = null) {
        base.StartBuildingObject(buildingSlot);

        // Determine build time
        UpgradeManager upgradeManager = _Player.GetUpgradeManager();

        if (SceneManager.GetSceneByName("TutorialScene") == SceneManager.GetActiveScene()) {

            BuildingTimeTutorial *= (int)upgradeManager._UnitBuildingSpeedMultiplier;
        }
        else {

            BuildingTime *= (int)upgradeManager._UnitBuildingSpeedMultiplier;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object's state switches to active (Only once)
    /// </summary>
    protected override void OnBuilt() {
        base.OnBuilt();
        Team = GameManager.Team.Defending;
        // Play spawn sound
        _Dialogue.PlaySpawnSound();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected void UpdateChasingEnemy() {

        // Check if we should continue chasing or not
        float dist = Vector3.Distance(_ChaseOriginPosition, transform.position);
        if (dist >= ChasingDistance) {

            // Stop chasing & return to origin
            RemovePotentialTarget(_AttackTarget);
            _IsReturningToOrigin = true;
        }

        if (!_ChaseCoroutineIsRunning) { StartCoroutine(ChaseTarget(_AttackTarget)); }

        ///if (_AttackTarget != null) { AgentAttackObject(_AttackTarget); }
    }

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

        // Get reference to the newly cloned unit
        if (_ClonedWorldObject != null) {

            Unit unit = _ClonedWorldObject.GetComponent<Unit>();
            unit._BuildingSlotInstigator = buildingSlot;

            // Despawn build counter widget
            if (_BuildingProgressCounter != null) { ObjectPooling.Despawn(_BuildingProgressCounter.gameObject); }

            // Let the building attached know that it is "building" something
            if (buildingSlot.GetBuildingOnSlot() != null) {

                buildingSlot.GetBuildingOnSlot().SetIsBuildingSomething(true);
                buildingSlot.GetBuildingOnSlot().SetObjectBeingBuilt(_ClonedWorldObject);
            }

            // Set position to be at the bases spawn vector while it is building
            // (the gameobject should be hidden completely until its deployed)
            if (buildingSlot.AttachedBase != null) {

                _ClonedWorldObject.gameObject.transform.position = buildingSlot.AttachedBase.GroundUnitSpawnTransform.transform.position;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.AttachedBase.GroundUnitSpawnTransform.transform.rotation;
            }

            // No base attached
            else {

                // Set position to be at the buildings spawn vector while it is building
                // (the gameobject should be hidden completely until its deployed)
                _ClonedWorldObject.gameObject.transform.position = buildingSlot.transform.position + buildingSlot.transform.forward * 50.0f;
                _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.transform.rotation;
            }

            // Add to list of AI(army)
            _ClonedWorldObject._Player.AddToPopulation(_ClonedWorldObject as Unit);

            // Create a veterancy widget and allocate it to the unit
            if (unit._UnitVeterancyWidget == null) { _UnitVeterancyWidget = ObjectPooling.Spawn(GameManager.Instance.UnitVeterancyPanel.gameObject).GetComponent<UnitVeterancyCounter>(); }
            if (unit._UnitVeterancyWidget != null && _Player != null) {

                unit.StartingVeterancyLevel = 0;
                unit._UnitVeterancyWidget.SetCameraAttached(unit._Player.CameraAttached);
                unit._UnitVeterancyWidget.SetUnitAttached(unit);
                unit._UnitVeterancyWidget.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);
                unit._UnitVeterancyWidget.gameObject.SetActive(true);
            }
            unit.SetPathInterupted(false);

            _ClonedWorldObject.Team = GameManager.Team.Defending;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    public IEnumerator ChaseTarget(WorldObject target, int updateDelay = 3) {

        while (_AttackTarget != null && _IsChasing) {

            _ChaseCoroutineIsRunning = true;

            AgentAttackObject(_AttackTarget);
            yield return new WaitForSeconds(updateDelay);            
        }
        _ChaseCoroutineIsRunning = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Damages the object by a set amount.
    /// </summary>
    /// <param name="damage"></param>
    public override void Damage(float damage, WorldObject instigator) {
        base.Damage(damage, instigator);

        // Interupt the current path (if valid)
        _PathInterupted = true;
        _InteruptionInstigator = instigator;

        // Add intigator to the potential list
        if (instigator != null) { AddPotentialTarget(instigator); }

        // Change attack target if someone damages us whilst were attacking the core
        if (_AttackTarget == WaveManager.Instance.CentralCore || 
                             WaveManager.Instance.CentralCore.Spires[0] || 
                             WaveManager.Instance.CentralCore.Spires[1] || 
                             WaveManager.Instance.CentralCore.Spires[2] &&
                             Team == GameManager.Team.Attacking) { 

            DetermineWeightedTargetFromList(TargetWeights);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the unit 'dies'.
    /// </summary>
    public override void OnDeath(WorldObject instigator) {
        base.OnDeath(instigator);

        // If were in the wave manager's enemies array - remove it
        if (WaveManager.Instance.GetCurrentWaveEnemies().Contains(this)) { WaveManager.Instance.GetCurrentWaveEnemies().Remove(this); }
        if (Team == GameManager.Team.Attacking) {

            GameManager.Instance.WaveStatsHUD.DeductLifeFromCurrentPopulation();

            // Add stat to player
            if (_Player == null) { _Player = GameManager.Instance.Players[0]; }
            if (_Player != null) {

                _Player.AddToScore(ScoreGrantedEnemy, Player.ScoreType.EnemyKilled);
                _Player.AddUnitsKilled();
            }
        }

        // Remove from player's population counter
        else if (Team == GameManager.Team.Defending) {

            _Player.RemoveFromArmy(this);
            
            // Add stat to player
            if (_Player == null) { _Player = GameManager.Instance.Players[0]; }
            if (_Player != null) {

                _Player.SubstractFromScore(ScoreDeductedFriendly, Player.ScoreType.EnemyKilled);
                _Player.AddUnitsLost();
            }
        }

        // Destroy waypoint
        if (_SeekWaypoint != null) { ObjectPooling.Despawn(_SeekWaypoint.gameObject); }

        // Add xp to the instigator (if valid)
        if (instigator != null) {

            // Only units can gain XP from kills
            Unit unit = instigator.GetComponent<Unit>();
            if (unit != null) { unit.AddVeterancyXP(XPGrantedOnDeath); }
        }

        // Play ragdoll stuff here
        _Agent.enabled = false;

        // Play death sound
        if (_Dialogue != null) { _Dialogue.PlayDeathSound(); }

        // Reset team
        Team = GameManager.Team.Undefined;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called only once, when the unit transitions to an active state.
    /// </summary>
    public virtual void OnSpawn() {

        if (_PotentialTargets != null) { _PotentialTargets.Clear(); }
        _AttackTarget = null;
        _AttackPathIterator = 0;
        ResetHealth();

        // Enable components
        if (_Agent == null) { _Agent = GetComponent<NavMeshAgent>(); }
        _Agent.enabled = true;
        if (_CharacterController == null) { _CharacterController = GetComponent<CharacterController>(); ; }
        if (_CharacterController != null) { _CharacterController.enabled = true; }

        SetObjectState(WorldObjectStates.Active);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called every frame - updates the soldier/unit's movement and combat behaviours.
    /// </summary>
    protected virtual void UpdateAIControllerMovement() {

        // Is the unit currently AI controlled?
        if (!_IsBeingPlayerControlled && _Agent.enabled && _Agent.isOnNavMesh && IsAlive()) {

            // Update agent seeking status
            _IsSeeking = _Agent.remainingDistance > 2f;

            // Temporary until the origin return bug is squashed
            _IsReturningToOrigin = false;
            
            if (!_IsSeeking) {

                _IsChasing = false;
                _IsReturningToOrigin = false;

                // Have to check if the current target is dead otherwise the command variable
                // will be overriden as soon as they position themselves at their target
                if (_AttackTarget == null) { _IsFollowingPlayerCommand = false; }
            }

            // Update seeking waypoint visibility (only for player controlled units)
            if (_SeekWaypoint && Team == GameManager.Team.Defending) {

                if (_IsSeeking && _DisplaySeekWaypoint && (_IsCurrentlySelected || _IsCurrentlyHighlighted)) { _SeekWaypoint.SetActive(true); }
                else { _SeekWaypoint.SetActive(false); }
            }

            // Update distance to attacking target
            if (_AttackTarget != null) {

                // Attack target is still alive
                if (_AttackTarget.IsAlive()) {

                    // Double check that the attack target is an actual enemy
                    if (_AttackTarget.Team == Team) {

                        ///Debug.LogError("ERROR: " + gameObject.name + "'s attack target '" + _AttackTarget.gameObject.name + "' is on the same team. - " + _AttackTarget.Team + " / " + Team);

                        // Remove from target list
                        _AttackTarget = null;
                        RemovePotentialTarget(_AttackTarget);

                        // Get new attack target if possible
                        DetermineWeightedTargetFromList(TargetWeights);
                        return;
                    }

                    // Check if the target is within attacking range
                    _DistanceToTarget = Vector3.Distance(transform.position, _AttackTarget.transform.position);
                    if (_DistanceToTarget <= MaxAttackingRange && PrimaryWeapon != null) {

                        // Check for valid sight line and attack if possible
                        SightLineCheck();
                    }

                    // Target is too far away or we have no primary weapon
                    else { _IsAttacking = false; }

                    // If a target point has been set
                    if (_AttackTarget.TargetPoint != null) {

                        // Determine if we should snap to target or lerp rotation
                        if (_DistanceToTarget <= SnapLookAtRange) { LookAtSnap(_AttackTarget.TargetPoint.transform.position); }
                        else { LookAtLerp(_AttackTarget.TargetPoint.transform.position); }
                    }
                    else {

                        // Constantly face the attacking target
                        Vector3 FireAtPos = _AttackTarget.transform.position;
                        FireAtPos.y = FireAtPos.y + _AttackTarget.GetObjectHeight() * 0.75f;

                        // Determine if we should snap to target or lerp rotation
                        if (_DistanceToTarget <= SnapLookAtRange) { LookAtSnap(FireAtPos); }
                        else { LookAtLerp(FireAtPos); }
                    }
                }

                // Attack target is now dead
                else {

                    _IsFollowingPlayerCommand = false;
                    _AttackTarget = null;

                    // Remove from target list
                    RemovePotentialTarget(_AttackTarget);

                    // Get new attack target if possible
                    DetermineWeightedTargetFromList(TargetWeights);
                }
            }

            // There is no current attack target
            else { /// _AttackTarget == null

                _IsAttacking = false;
                _IsFollowingPlayerCommand = false;

                // Get new attack target if possible
                DetermineWeightedTargetFromList(TargetWeights);
                
                // Always attack the core if we dont have a target (ATTACKING TEAM ONLY)
                if (Team == GameManager.Team.Attacking) {

                    // Make sure we have completed the attack path allocated to us
                    if (_AttackTarget == null && _AttackPathComplete) {
                        
                        AddPotentialTarget(WaveManager.Instance.CentralCore.GetClosestObject(_Agent.transform.position));
                        DetermineWeightedTargetFromList(TargetWeights);
                        TryToChaseTarget(_AttackTarget);
                    }
                }
            }
        }

        // Update guard state (by forcing their movement speed to 0)
        if (_IsGuarding && _Agent != null) { _Agent.speed = 0f; }
        else if (_Agent != null) { _Agent.speed = _OriginalMovementSpeed; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void UpdatePlayerControlledMovement() { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame - raycasts for a building (any building that is an enemy) 
    //  and pauses the unit's attack path to combat it.
    /// </summary>
    private void UpdateBarrierDetection() {

        // Origin position starts from the "chest"
        Vector3 origin = transform.position;
        origin.y = origin.y + _ObjectHeight / 2;

        // Constantly fire a raycast forward
        RaycastHit hit;
        if (Physics.Raycast(origin, transform.forward * BarrierDetectionDistance, out hit, LayerMask.NameToLayer("Building"))) {

            // Only detect against barrier worldObjects
            Building building = hit.transform.GetComponent<Building>();
            if (building != null) {

                // Enemy team
                if (building.Team != Team) {

                    // Interrupt the unit's pathfinding (just stop moving it)
                    _PathInterupted = true;
                    AddPotentialTarget(building);
                    DetermineWeightedTargetFromList(TargetWeights);
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void SightLineCheck() {

        // Look at attack target
        _IsAttacking = true;
        
        // TEMPORARY JUST FIRE IMMEDIATELY WITHOUT A SIGHTLINE CHECK
        if (PrimaryWeapon.CanFire()) { OnFireWeapon(); }

        // Fire raycast to confirm valid line of sight to target
        int def = 1 << LayerMask.NameToLayer("Default");
        int units = 1 << LayerMask.NameToLayer("Units");
        int buildings = 1 << LayerMask.NameToLayer("Buildings");
        LayerMask mask = def | units | buildings;
        RaycastHit hit;
        if (Physics.Raycast(MuzzleLaunchPoints[0].transform.position, _AttackTarget.TargetPoint.transform.position, out hit, MaxAttackingRange, mask)) {

            // There is a line of sight to the target, fire the weapon (if possible)
            ///if (PrimaryWeapon.CanFire()) { OnFireWeapon(); }

            Debug.DrawLine(MuzzleLaunchPoints[0].transform.position, _AttackTarget.TargetPoint.transform.position, Color.green);
        }
        else { Debug.DrawLine(MuzzleLaunchPoints[0].transform.position, _AttackTarget.TargetPoint.transform.position, Color.red); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void LookAtLerp(Vector3 position) { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Snaps the agent's transform to the position specified.
    /// </summary>
    protected virtual void LookAtSnap(Vector3 position) {

        _Agent.transform.LookAt(position);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Coroutine used to make the enemy units follow their attack paths to the core.
    /// </summary>
    // <param name="pos"></param>
    /// <returns>
    //  IEnumerator
    /// </returns>
    protected IEnumerator AgentAttackPathSeekPosition(Vector3 pos) {

        if (!_PathInterupted) { AgentSeekPosition(pos, false, false); }
        yield return new WaitForEndOfFrame();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the agent's destination for seeking.
    /// </summary>
    // <param name="seekTarget"></param>
    public virtual void AgentSeekPosition(Vector3 seekTarget, bool overwrite = false, bool displayWaypoint = true) {

        _DisplaySeekWaypoint = displayWaypoint;

        if (overwrite) { PlayerCommandOverride(); }
        if (_Agent.isOnNavMesh) {

            // Set agent's new goto target
            _SeekTarget = seekTarget;
            _Agent.destination = _SeekTarget;
            _Agent.speed = InfantryMovementSpeed;

            // Show seeking waypoint
            if (_DisplaySeekWaypoint && Team == GameManager.Team.Defending) {

                // Create waypoint
                if (_SeekWaypoint == null) { _SeekWaypoint = ObjectPooling.Spawn(GameManager.Instance.AgentSeekObject, Vector3.zero, Quaternion.identity); }
                if (_SeekWaypoint != null) {

                    // Display waypoint if not already being displayed
                    if (_SeekWaypoint.activeInHierarchy != true) { _SeekWaypoint.SetActive(true); }

                    // Update waypoint position
                    _SeekWaypoint.transform.position = seekTarget;
                    _SeekWaypoint.transform.position += Vector3.up;
                }
            }

            // Hide the waypoint if its not meant to be displayed
            else {

                // Validity check
                if (_SeekWaypoint != null) { _SeekWaypoint.SetActive(false); }
            }
            _IsSeeking = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the attack target to the worldObject passed through, 
    //  as well as pathfinding to the position passed in.
    /// </summary>
    // <param name="attackTarget"></param>
    // <param name="seekPosition"></param>
    // <param name="overwrite"></param>
    // <param name="displayWaypoint"></param>
    public void AgentAttackObject(WorldObject attackTarget, Vector3 seekPosition, bool overwrite = false, bool displayWaypoint = true) {

        // Set target
        AddPotentialTarget(attackTarget);
        _AttackTarget = attackTarget;

        // Seek
        AgentSeekPosition(seekPosition, overwrite, displayWaypoint);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the attack target to the worldObject passed through &
    //  creates a seek position internally.
    /// </summary>
    // <param name="attackTarget"></param>
    public void AgentAttackObject(WorldObject attackTarget) {

        // Add target to list and make it the current target
        AddPotentialTarget(attackTarget);
        _AttackTarget = attackTarget;

        // Get attacking position for the target and seek to it (get closer if we have to, or move away if we should)
        Vector3 seekPos = transform.position;
        float dist = Vector3.Distance(transform.position, attackTarget.transform.position);

        // Move away from current target
        if (dist < IdealAttackRangeMin) { seekPos = GetAttackingPositionAtObject(_AttackTarget, IdealAttackRangeMin); }

        // Move towards current target
        else if (dist > IdealAttackRangeMax) { seekPos = GetAttackingPositionAtObject(_AttackTarget, IdealAttackRangeMax); }

        // Move to attacking position (or stay if were within the ideal range)
        AgentSeekPosition(seekPos, false, false);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    // <param name="hitPoint"></param>
    public virtual void AgentPerformAbility(Vector3 hitPoint) {

        // Does the unit have an ability?
        if (SecondaryWeapon != null) {

            // Fire secondary weapon (its used to be its ability)
            if (SecondaryWeapon.CanFire()) { SecondaryWeapon.FireWeapon(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Determines where the agent should seek to when it attacks the WorldObject passed in.
    /// </summary>
    /// <param name="worldObject"></param>
    /// <returns>
    //  Vector3
    /// </returns>
    public Vector3 GetAttackingPositionAtObject(WorldObject worldObject, float distanceFrom) {

        // Create transform and create a position in the direction of the target * distance specified
        Transform trans = new GameObject().transform;
        trans.position = worldObject.transform.position;
        trans.LookAt(transform.position);
        trans.position += trans.forward * distanceFrom;

        // Destroy obsolete transform and return the new attacking position
        Vector3 position = trans.position;
        Destroy(trans.gameObject);
        return position;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Determines where the agent should seek to when it attacks the WorldObject passed in.
    /// </summary>
    /// <param name="worldObject"></param>
    /// <returns>
    //  Vector3
    /// </returns>
    public Vector3 GetAttackingPositionAtObject(WorldObject worldObject) {

        // Create transform and create a position in the direction of the target * distance specified
        Transform trans = new GameObject().transform;
        trans.position = worldObject.transform.position;
        trans.LookAt(transform.position);
        trans.position += trans.forward * (IdealAttackRangeMax * 0.9f);

        // Destroy obsolete transform and return the new attacking position
        Vector3 position = trans.position;
        Destroy(trans.gameObject);
        return position;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns a list of positions in a arc shape facing the building target.
    /// </summary>
    // <param name="building"></param>
    // <param name="size"></param>
    // <param name="distance"></param>
    /// <returns>
    //  List<Vector3>
    /// </returns>
    public List<Vector3> GetAttackArcPositions(Building building, int size) {

        // Create transform and create a position in the direction of the target * distance specified
        Transform trans = new GameObject().transform;
        trans.position = building.transform.position;
        trans.LookAt(transform.position);
        trans.position += trans.forward * (MaxAttackingRange * 0.9f);

        // Destroy obsolete transform and return the new attacking position
        Vector3 direction = (-(trans.position - transform.position).normalized * (MaxAttackingRange / 2));
        Quaternion rotation_offset = Quaternion.Euler(0.0f, 10.0f, 0.0f);
        List<Vector3> positions = new List<Vector3>();

        // Create positions in an arc around the target
        for (int i = 0; i < size + 1; i++) {

            // Skip the first iterator
            if (i == 0) { continue; }
            else {
                
                direction = rotation_offset * direction;
                positions.Add(building.transform.position + direction);
            }
        }

        Destroy(trans.gameObject);
        return positions;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Makes the agent return back to its origin position.
    //  (Usually the position is where the unit was before it was pursuing/attacking a target).
    /// </summary>
    protected void ResetToOriginPosition() {
        
        // Seek to origin position (but dont override or show a waypoint)
        AgentSeekPosition(_ChaseOriginPosition, false, false);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    // <param name="delayTime"></param>
    protected virtual IEnumerator ResetToOrigin(int delayTime = 3) {
        
        while (_IsReturningToOrigin) {

            _ResetOriginCoroutineIsRunning = true;

            AgentSeekPosition(_ChaseOriginPosition, false, false);
            yield return new WaitForSeconds(delayTime);
        }
        _ResetOriginCoroutineIsRunning = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    // <param name="delayTime"></param>
    protected virtual IEnumerator ResetWeaponPosition(int delayTime) {

        yield return new WaitForSeconds(delayTime);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Set's the worldobject's hitpoints & shieldpoints to the current maximum value.
    /// </summary>
    public void ResetHealth() {

        _HitPoints = MaxHitPoints;
        _ShieldPoints = MaxShieldPoints;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds a WorldObject to the weighted target list
    /// </summary>
    // <param name="target"></param>
    public virtual void AddPotentialTarget(WorldObject target) {

        if (_PotentialTargets != null && _ObjectState == WorldObjectStates.Active) {

            // Not a friendly unit...
            if (target.Team != Team && target.Team != GameManager.Team.Undefined) {

                // Look for match
                bool match = false;
                for (int i = 0; i < _PotentialTargets.Count; i++) {

                    // Match found
                    if (_PotentialTargets[i] == target) {

                        match = true;
                        break;
                    }
                }

                // Add to list if no matching target was found
                if (!match) { _PotentialTargets.Add(target); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Checks if the WorldObject is contained in the weighted 
    //  target list & removes if it found.
    /// </summary>
    // <param name="target"></param>
    public void RemovePotentialTarget(WorldObject target) { _PotentialTargets.Remove(target); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Uses a weighted selection algorithm to determine a new attack target, 
    //  given by the argumented weightList.
    /// </summary>
    public virtual void DetermineWeightedTargetFromList(TargetWeight[] weightList) {

        // Multiple targets to select from
        if (_PotentialTargets.Count > 0) {
            
            // Use weighted selection to determine the next target
            List<int> targetWeights = new List<int>();
        /*
            if (weightList != null || weightList.Length > 0) {
                
                // For each known potential target
                for (int i = 0; i < _PotentialTargets.Count; i++) {

                    // Look for a match within the passed in weight list
                    for (int j = 0; j < weightList.Length - 1; j++) {

                        // Current potential target matches the current iterator in the weight list
                        WorldObject obj = _PotentialTargets[i];

                        if (obj is Unit) {

                            if ((obj as Unit).UnitType == weightList[j].UnitType) {

                                // Add to local targetweights array
                                targetWeights.Add(weightList[j].Weight);
                            }
                        }
                        else if (obj is Building && weightList[j].UnitType == EUnitType.Undefined) {

                            // Add building to the local targetweights array
                            targetWeights.Add(weightList[j].Weight);
                        }
                    }
                }
            }
            
            else { /// weightList == null

                // All potential targets have a weight of 1 to be the next target
                for (int i = 0; i < _PotentialTargets.Count; i++) { targetWeights.Add(1); }
            }
        */

            // All potential targets have a weight of 1 to be the next target
            for (int i = 0; i < _PotentialTargets.Count; i++) {
                
                // Update whether the target iterator should still be a potential target
                if (_PotentialTargets[i].Team == Team ||
                    _PotentialTargets[i].Team == GameManager.Team.Undefined ||
                    !_PotentialTargets[i].IsAlive()) {

                    _PotentialTargets.RemoveAt(i);
                    continue;
                }
                targetWeights.Add(1);
            }

            // Set new target
            _AttackTarget = _PotentialTargets[GetWeightedRandomIndex(targetWeights)];
        }

        // Only a single target in the array
        else if (_PotentialTargets.Count == 1) { _AttackTarget = _PotentialTargets[0]; }

        // No targets to choose from
        else { _AttackTarget = null; }
        if (_AttackTarget == null) { StartCoroutine(ResetWeaponPosition(3)); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Gets a random index based of a list of weighted ints.
    /// </summary>
    // <param name="weights"></param>
    /// <returns>
    //  int
    /// </returns>
    private int GetWeightedRandomIndex(List<int> weights) {

        // Get the total sum of all the weights.
        int weightSum = 0;
        for (int i = 0; i < weights.Count; ++i) { weightSum += weights[i]; }

        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = weights.Count - 1;
        while (index < lastIndex) {

            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index]) { return index; }

            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }

        // No other item was selected, so return very last index.
        return index;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Loops through the potential/known target list for the unit and checks to see if the target
    //  is contained in the list
    /// </summary>
    // <param name="target"></param>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsTargetInPotentialList(WorldObject target) { return _PotentialTargets.Contains(target); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Force set the _AttackTarget of the unit by the passed in WorldObject.
    /// </summary>
    // <param name="target"></param>
    public void SetAttackTarget(WorldObject target) { _AttackTarget = target; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
    //  Get reference of the current attack target.
    /// </summary>
    /// <returns>
    // WorldObject
    /// </returns>
    public WorldObject GetAttackTarget() { return _AttackTarget; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Attempts to chase the WorldObject passed in, if possible by an array of different parameters.
    /// </summary>
    // <param name="worldObject"></param>
    public virtual bool TryToChaseTarget(WorldObject objTarget) {

        // Validity check
        if (objTarget != null) {

            // Only chase the target if were not following a player command or returning to our origin position
            if (!_IsFollowingPlayerCommand && !_IsReturningToOrigin) {

                _AttackTarget = objTarget;
                _IsChasing = true;
                return true;
            }
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Overrides the current _AttackTarget of the unit and forces the unit to begin a chase sequence to it.
    /// </summary>
    // <param name="worldObject"></param>
    public virtual bool ForceChaseTarget(WorldObject objTarget, bool playerCommand = false) {

        // Validity check
        if (objTarget != null) {

            if (playerCommand) { PlayerCommandOverride(); }

            // Force chase the target
            _AttackTarget = objTarget;
            _IsChasing = true;
            AddPotentialTarget(_AttackTarget);
            return true;
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds XP to the veterancy level of the unit & levels up if neccessary.
    /// </summary>
    /// <param name="xp"></param>
    public void AddVeterancyXP(int xp) {

        // Level 3 veterancy is the highest level a unit can achieve
        if (_VeterancyLevel < 3) {

            // Reached enough XP to level up?
            _CurrentVetXP += xp;
            if (_CurrentVetXP >= _TargetVetXP) {

                // Create a veterancy widget and allocate it to the unit
                if (_UnitVeterancyWidget == null) { _UnitVeterancyWidget = ObjectPooling.Spawn(GameManager.Instance.UnitVeterancyPanel).GetComponent<UnitVeterancyCounter>(); }
                if (_UnitVeterancyWidget != null) {

                    _UnitVeterancyWidget.SetCameraAttached(_Player.CameraAttached);
                    _UnitVeterancyWidget.SetUnitAttached(this);
                    _UnitVeterancyWidget.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);
                    _UnitVeterancyWidget.gameObject.SetActive(true);
                }
                
                // Increase veterancy level 
                _VeterancyLevel++;

                // Reset XP stats
                _CurrentVetXP = 0;
                _TargetVetXP = VetTargetXPs[_VeterancyLevel];
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns the value of the unit's _VeterancyLevel.
    /// </summary>
    /// <returns>
    //  int
    /// </returns>
    public int GetVeterancyLevel() { return _VeterancyLevel; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected void PlayerCommandOverride() {

        _IsFollowingPlayerCommand = true;
        _IsReturningToOrigin = false;
        _IsChasing = false;
        _IsAttacking = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference of the NavMeshAgent component attached to this unit.
    /// </summary>
    /// <returns>
    //  NavMeshAgent
    /// </returns>
    public NavMeshAgent GetAgent() { return _Agent; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //   Returns a booling on whether the player is manually controlling the unit with WASD/Joystick input.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsBeingPlayerControlled() { return _IsBeingPlayerControlled; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Set's the attack path to the core for this individual unit.
    /// </summary>
    // <param name="path"></param>
    public void SetAttackPath(AttackPath path) { _AttackPath = path; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference to the _AttackPath being used by the unit.
    /// </summary>
    /// <returns>
    //  AttackPath
    /// </returns>
    public AttackPath GetAttackPath() { return _AttackPath; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called whenever the unit fires its weapon. 
    //  (Used for animation).
    /// </summary>
    public virtual void OnFireWeapon() {

        // Actually fire the weapon
        PrimaryWeapon.FireWeapon();
        _IsFiring = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets reference to the Lockdown base associated with this attacking unit. (Used for retreating)
    /// </summary>
    // <param name=""></param>
    public void SetLockdownBase(Base lockdownBase) {

        if (Team == GameManager.Team.Attacking) { _LockdownBase = lockdownBase; }
        else { _LockdownBase = null; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Checks if the enemy unit is closer to the core or their lockdown base & moves towards the closer target.
    /// </summary>
    public void RetreatToLockdownBaseCheck() {

        // Precaution check so only enemy units will do a retreat check
        if (_LockdownBase != null && Team == GameManager.Team.Attacking) {

            float distanceToCore = Vector3.Distance(_Agent.transform.position, WaveManager.Instance.CentralCore.transform.position);
            float distanceToBase = Vector3.Distance(_Agent.transform.position, _LockdownBase.transform.position);

            // Core is closer
            if (distanceToCore < distanceToBase) {

                _PathInterupted = false;

            }

            // Lockdown base is closer
            else {

                // Retreat
                _PathInterupted = true;
                AgentSeekPosition(_LockdownBase.GetPositionNearTheBase(), true, true);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets the path interupted value to the boolean specified.
    /// </summary>
    // <param name="value"></param>
    public void SetPathInterupted(bool value) { _PathInterupted = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Returns reference to the unit dialogue component.
    /// </summary>
    /// <returns>
    //  UnitDialogue
    /// </returns>
    public UnitDialogue GetDialogue() { return _Dialogue; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Sets whether the unit should be in their guard or agro state.
    /// </summary>
    // <param name="value"></param>
    public void SetGuard(bool value) {

        _IsGuarding = value;
        if (value) { _Agent.SetDestination(transform.position); }

        // Update worldspace healthbar guard logo visibility
        if (_HealthBar != null) { _HealthBar.GuardLogoImage.gameObject.SetActive(value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns whether the unit is in their guard or agro state.
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetGuardState() { return _IsGuarding; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}