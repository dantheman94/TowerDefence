using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 10/9/2018
//
//******************************

public class KeyboardInput : MonoBehaviour {
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _PlayerAttached;
    private CameraPlayer _PlayerCamera;
    private XboxGamepadInput _XboxGamepadInputManager = null;
    public bool IsPrimaryController { get; set; }
    public static Rect Selection = new Rect(0, 0, 0, 0);
    public Texture SelectionHighlight;
    public Minimap MiniMap;
    public SliceDrawer Slicedrawer;
    

    private Vector3 _LookPoint;
    private Vector3 _CurrentVelocity = Vector3.zero;
    private bool _RotatingCamera = false;
    private Vector3 _BoxStartPoint = -Vector3.one;

    private Selectable _HighlightFocus = null;
    private Unit _HighlightUnit = null;
    private Building _HighlightBuilding = null;
    private WorldObject _HighlightWorldObject = null;
    private bool SelectedTrue = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    private void Start() {

        // Get component references
        _PlayerAttached = GetComponent<Player>();
        _XboxGamepadInputManager = GetComponent<XboxGamepadInput>();
        _PlayerCamera = _PlayerAttached.CameraAttached.GetComponent<CameraPlayer>();

        // Initialize center point for LookAt() function
        CreateCenterPoint();

        IsPrimaryController = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetSet() { return SelectedTrue; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    // <param name="abool"></param>
    public void SetSelected(bool abool) { SelectedTrue = abool; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    /// 
    private void Update() {
        CreateSelectionBox();
        
        if(Input.GetMouseButtonUp(1))
        {
            LineMovement();
        }
        if (_PlayerAttached) {

            // Update primary controller
            if (Input.anyKeyDown) {

                // Disable gamepad / Enable keyboard
                IsPrimaryController = true;
                if (_XboxGamepadInputManager != null) { _XboxGamepadInputManager.IsPrimaryController = false; }
            }

            // Check for tutorial lock controls
            bool lockcontrols = false;
            if (TutorialScene.CurrentMessageData != null) { lockcontrols = TutorialScene.CurrentMessageData.LockControls; }

            if (IsPrimaryController && !GameManager.Instance._CinematicInProgress && !lockcontrols) {

                Cursor.visible = true;
               
               if(TutorialScene.CurrentMessageData != null)
               {
                    if(!TutorialScene.CurrentMessageData.LockCamera)
                    {
                        // Update camera
                        MoveCamera();
                        RotateCamera();
                    }
               }
               else
               {
                    // Update camera
                    MoveCamera();
                    RotateCamera();
               }
              

                // Update camera FOV
                ZoomCamera();

                // Update mouse input
                MouseActivity();

                // Update faction abilities input
                ///AbilitiesInput();

                // Update Ai abilities input
                AiAbilityCommandInput();

                // Update platoon input
                PlatoonInput();

                // Select all units
                if (Input.GetKeyDown(KeyCode.E)) {

                    // Loop through & select all army objects
                    foreach (var ai in _PlayerAttached.GetArmy()) {

                        // Add to selection list
                        _PlayerAttached.SelectedWorldObjects.Add(ai);
                        _PlayerAttached.SelectedUnits.Add(ai);
                        ai.SetPlayer(_PlayerAttached);
                        ai.SetIsSelected(true);
                    }

                    // Update units selected panels
                    GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Selection.Set(0, 0, 0, 0);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Updates the center screen world point used the camera rotating
    /// </summary>
    public void CreateCenterPoint() {

        // Update center point for RotateAround() function
        RaycastHit hit;
        Physics.Raycast(_PlayerAttached.CameraAttached.transform.parent.position, _PlayerAttached.CameraAttached.transform.parent.forward * 1000, out hit);
        _LookPoint = hit.point;
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="movement"></param>
    private void SnapMovement(Vector3 movement) {

        // Make sure movement is in the direction the camera is pointing
        // but ignore the vertical tilt of the camera to get sensible scrolling
        movement = _PlayerAttached.CameraAttached.transform.parent.TransformDirection(movement);
        movement.y = 0;

        // Calculate desired camera position based on received input
        Vector3 posOrigin = _PlayerAttached.CameraAttached.transform.parent.position;
        Vector3 posDestination = posOrigin;
        posDestination.x += movement.x;
        posDestination.y += movement.y;
        posDestination.z += movement.z;

        /*
         * Clamp ground movement to be between a minimum and maximum distance
         */

        // Too low
        if (posDestination.y > Settings.MaxCameraHeight) { posDestination.y = Settings.MaxCameraHeight; }
        // Too high
        else if (posDestination.y < Settings.MinCameraHeight) { posDestination.y = Settings.MinCameraHeight; }

        // If a change in position is detected perform the movement update
        if (posDestination != posOrigin) {

            // Update position
            _PlayerAttached.CameraAttached.transform.parent.position = Vector3.MoveTowards(posOrigin, posDestination, Settings.MovementSpeed * Time.deltaTime);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="movement"></param>
    private void SmoothMovement(Vector3 movement) {

        // Make sure movement is in the direction the camera is pointing
        // but ignore the vertical tilt of the camera to get sensible scrolling
        movement = _PlayerAttached.CameraAttached.transform.parent.TransformDirection(movement);
        movement.y = 0;

        // Calculate desired camera position based on received input
        Vector3 posOrigin = _PlayerAttached.CameraAttached.transform.parent.position;
        Vector3 posDestination = posOrigin;
        posDestination.x += movement.x;
        posDestination.y += movement.y;
        posDestination.z += movement.z;

        /*
         * Clamp ground movement to be between a minimum and maximum distance
         */

        // Too low
        if      (posDestination.y > Settings.MaxCameraHeight) { posDestination.y = Settings.MaxCameraHeight; }
        // Too high
        else if (posDestination.y < Settings.MinCameraHeight) { posDestination.y = Settings.MinCameraHeight; }

        // Smoothly move toward target position
        _PlayerAttached.CameraAttached.transform.parent.position = Vector3.SmoothDamp(posOrigin, posDestination, ref _CurrentVelocity, Settings.MovementSpeed * Time.deltaTime);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void MoveCameraHybrid() {

        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);

        // Move camera via input if the player ISNT currently controlling a unit
        if (GameManager.Instance.GetIsUnitControlling() == false) {

            if (Input.GetKey(KeyCode.LeftShift)) {

                // 'Sprint' movement speed
                ///Settings.MovementSpeed = Settings.CameraSprintSpeed;
            }
            else {

                // 'Walk' movement speed
                Settings.MovementSpeed = Settings.CameraWalkSpeed;
            }

            /* 
             * Keyboard movement WASD
             */
            // Move forwards
            if (Input.GetKey(KeyCode.W) && (!Input.GetKey(KeyCode.LeftAlt))) {

                movement.y += Settings.MovementSpeed;
                CreateCenterPoint();
                SnapMovement(movement);
            }

            // Smooth exit up
            else if (!Input.GetKeyUp(KeyCode.W) && (!Input.GetKey(KeyCode.LeftAlt))) {

                CreateCenterPoint();
                SmoothMovement(movement);
            }

            // Move backwards
            if (Input.GetKey(KeyCode.S) && (!Input.GetKey(KeyCode.LeftAlt))) {

                movement.y -= Settings.MovementSpeed;
                CreateCenterPoint();
                SnapMovement(movement);
            }

            // Smooth exit down
            else if (!Input.GetKey(KeyCode.S) && (!Input.GetKey(KeyCode.LeftAlt))) {

                CreateCenterPoint();
                SmoothMovement(movement);
            }

            // Move right
            if (Input.GetKey(KeyCode.D) && (!Input.GetKey(KeyCode.LeftAlt))) {

                movement.x += Settings.MovementSpeed;
                CreateCenterPoint();
                SnapMovement(movement);
            }

            // Smooth exit right
            else if (!Input.GetKey(KeyCode.D) && (!Input.GetKey(KeyCode.LeftAlt))) {

                CreateCenterPoint();
                SmoothMovement(movement);
            }

            // Move left
            if (Input.GetKey(KeyCode.A) && (!Input.GetKey(KeyCode.LeftAlt))) {

                movement.x -= Settings.MovementSpeed * 2;
                CreateCenterPoint();
                SnapMovement(movement);
            }

            // Smooth exit left
            else if (!Input.GetKey(KeyCode.A) && (!Input.GetKey(KeyCode.LeftAlt))) {

                CreateCenterPoint();
                SmoothMovement(movement);
            }
        }

        // Horizontal camera movement via mouse
        if (xPos >= 0 && xPos < Settings.ScreenOffset)
            movement.x -= Settings.MovementSpeed;
        else if (xPos <= Screen.width && xPos > Screen.width - Settings.ScreenOffset)
            movement.x += Settings.MovementSpeed;

        // Vertical camera movement via mouse
        if (yPos >= 0 && yPos < Settings.ScreenOffset)
            movement.z -= Settings.MovementSpeed;
        else if (yPos <= Screen.height && yPos > Screen.height - Settings.ScreenOffset)
            movement.z += Settings.MovementSpeed;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void MoveCamera() {

        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);

        // Move camera via input if the player ISNT currently controlling a unit
        if (GameManager.Instance.GetIsUnitControlling() == false) {


            //if (Input.GetMouseButton(2))
            //{
            //    if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0)
            //    {
            //        movement.x += Settings.CameraSprintSpeed * Input.GetAxis("Mouse X") * 500;
            //     //   _PlayerAttached.PlayerCamera.transform.Translate(_PlayerCamera.transform.right * Settings.MovementSpeed * Input.GetAxis("Mouse X")/2);
            //        CreateCenterPoint();
            //    }
            //    else if (Input.GetAxis("Mouse Y") < 0 || Input.GetAxis("Mouse Y") > 0)
            //    {
            //        movement.y += Settings.CameraSprintSpeed * Input.GetAxis("Mouse Y") * 500;
            //        CreateCenterPoint();
            //    }
            //}
            

            // Keyboard movement WASD
            if (Input.GetKey(KeyCode.W) && (!Input.GetKey(KeyCode.LeftAlt)) && (!_PlayerCamera.PastBoundsNorth)) {

                // Move forwards
                movement.y += Settings.MovementSpeed;
                CreateCenterPoint();
            }

            if (Input.GetKey(KeyCode.S) && (!Input.GetKey(KeyCode.LeftAlt)) && (!_PlayerCamera.PastBoundsSouth)) {

                // Move backwards
                movement.y -= Settings.MovementSpeed;
                CreateCenterPoint();
            }

            if (Input.GetKey(KeyCode.D) && (!Input.GetKey(KeyCode.LeftAlt)) && (!_PlayerCamera.PastBoundsEast)) {

                // Move right
                movement.x += Settings.MovementSpeed;
                CreateCenterPoint();
            }

            if (Input.GetKey(KeyCode.A) && (!Input.GetKey(KeyCode.LeftAlt)) && (!_PlayerCamera.PastBoundsWest))
            {

                // Move left
                movement.x -= Settings.MovementSpeed;
                CreateCenterPoint();
            }

            if (Input.GetKey(KeyCode.LeftShift)) {

                // 'Sprint' movement speed
                Settings.MovementSpeed = Settings.CameraSprintSpeed;
            }
            else {

                // 'Walk' movement speed
                Settings.MovementSpeed = Settings.CameraWalkSpeed;
            }
        }

        // Horizontal camera movement via mouse
        if (xPos >= 0 && xPos < Settings.ScreenOffset)
            movement.x -= Settings.MovementSpeed;
        else if (xPos <= Screen.width && xPos > Screen.width - Settings.ScreenOffset)
            movement.x += Settings.MovementSpeed;

        // Vertical camera movement via mouse
        if (yPos >= 0 && yPos < Settings.ScreenOffset)
            movement.z -= Settings.MovementSpeed;
        else if (yPos <= Screen.height && yPos > Screen.height - Settings.ScreenOffset)
            movement.z += Settings.MovementSpeed;

        // Make sure movement is in the direction the camera is pointing
        // but ignore the vertical tilt of the camera to get sensible scrolling
        movement = _PlayerAttached.CameraAttached.transform.parent.TransformDirection(movement);
        movement.y = 0;

        // Calculate desired camera position based on received input
        Vector3 posOrigin = _PlayerAttached.CameraAttached.transform.parent.position;
        Vector3 posDestination = posOrigin;
        posDestination.x += movement.x;
        posDestination.y += movement.y;
        posDestination.z += movement.z;

        // Limit away from ground movement to be between a minimum and maximum distance
        if      (posDestination.y > _PlayerCamera._MaxCameraHeight) { posDestination.y = _PlayerCamera._MaxCameraHeight; }
        else if (posDestination.y < _PlayerCamera._MinCameraHeight) { posDestination.y = _PlayerCamera._MinCameraHeight; }

        // If a change in position is detected perform the necessary update
        if (posDestination != posOrigin) {

            // Update position
            _PlayerAttached.CameraAttached.transform.parent.position = Vector3.MoveTowards(posOrigin, posDestination, Time.deltaTime * Settings.MovementSpeed);
        }

        // Smoothly move toward target position
        ///_PlayerAttached.PlayerCamera.transform.position = Vector3.SmoothDamp(posOrigin, posDestination, ref _CurrentVelocity, Settings.MovementSpeed * Time.deltaTime);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void RotateCamera() {

        Vector3 rotOrigin = _PlayerAttached.CameraAttached.transform.parent.eulerAngles;
        Vector3 rotDestination = rotOrigin;

        bool pressed = false;

        // Rotate camera state if ALT is being held down
        if (_RotatingCamera = (Input.GetKey(KeyCode.LeftAlt))) {

            // Hide mouse cursor
            Cursor.visible = false;

            // Calculate which direction to rotate in
            float dir = 0f;
            dir = Input.GetAxis("Mouse X");

            // Set point to "camera follows" target position if the player is manually controlling a unit
            if (GameManager.Instance.GetIsUnitControlling()) { _LookPoint = _PlayerAttached._CameraFollow.GetFollowTarget().transform.position; }

            // Rotate around point
            _PlayerAttached.CameraAttached.transform.parent.RotateAround(_LookPoint, Vector3.up, Settings.RotateSpeed * -dir * Time.deltaTime);

            // Used for resetting the mouse position
            pressed = true;
        }

        // Not rotating the camera
        else {

            // Always hide the mouse cursor whilst the player IS controlling a unit
            if (GameManager.Instance.GetIsUnitControlling()) {

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else { Cursor.visible = true; }
        }

        if (pressed) {

            // Reset mouse to center of the screen
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;
            pressed = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void ZoomCamera() {

        // Change camera fov
        float fov = _PlayerAttached.CameraAttached.fieldOfView;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {

            // Zooming in
            if (fov > Settings.MinFov)
                _PlayerAttached.CameraAttached.fieldOfView -= Time.deltaTime * Settings.ZoomSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {

            // Zooming out
            if (fov < Settings.MaxFov)
                _PlayerAttached.CameraAttached.fieldOfView += Time.deltaTime * Settings.ZoomSpeed;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void MouseActivity() {

        if      (Input.GetMouseButtonDown(0)) { LeftMouseClick(); }
        else if (Input.GetMouseButtonDown(1)) { RightMouseClick(); }

        // Highlighting selectables
        UpdateHighlightingObjects();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void UpdateHighlightingObjects() {

        // Not currently rotating the camera
        if (!_RotatingCamera) {

            // Highlighting world objects
            GameObject hitObject = _PlayerAttached._HUD.FindHitObject();
            Vector3 hitPoint = _PlayerAttached._HUD.FindHitPoint();
            if (hitObject && hitPoint != Settings.InvalidPosition) {

                if (hitObject.tag != "Ground") {

                    // Set highlight focus to raycast hitobject
                    if (_HighlightFocus == null) { _HighlightFocus = hitObject.gameObject.GetComponentInParent<Selectable>(); }

                    // There is currently a highlighted object? (or did the raycast hit a valid 'selectable'?
                    if (_HighlightFocus != null) {

                        // Is the raycast still hitting the highlighted object?
                        Selectable selectable = hitObject.gameObject.GetComponentInParent<Selectable>();
                        if (selectable != null) {

                            // Check if we should highlight the selectable
                            if (!_HighlightFocus.GetIsSelected()) {

                                // Check if its a building
                                if (_HighlightBuilding == null) { _HighlightBuilding = _HighlightFocus.GetComponent<Building>(); }
                                if (_HighlightBuilding != null) {

                                    if (_HighlightBuilding.GetObjectState() == Abstraction.WorldObjectStates.Active ||
                                        _HighlightBuilding.GetObjectState() == Abstraction.WorldObjectStates.Building) {

                                        // Highlight building
                                        _HighlightFocus.SetIsHighlighted(true);
                                    }
                                }

                                // Check if its an AI
                                if (_HighlightUnit == null) { _HighlightUnit = _HighlightFocus.GetComponent<Unit>(); }
                                if (_HighlightUnit != null) {

                                    if (_HighlightUnit.GetObjectState() == Abstraction.WorldObjectStates.Active) {

                                        Unit unit = _HighlightUnit.GetComponent<Unit>();
                                        if (unit != null) { _HighlightUnit.SetIsHighlighted(true); }                                        
                                    }
                                }

                                // Check if its a world object
                                if (_HighlightWorldObject == null) { _HighlightWorldObject = _HighlightFocus.GetComponent<WorldObject>(); }
                                if (_HighlightWorldObject != null && _HighlightUnit == null) {

                                    if (_HighlightWorldObject.GetObjectState() == WorldObject.WorldObjectStates.Active) {

                                        // Highlight building
                                        _HighlightFocus.SetIsHighlighted(true);
                                    }
                                }
                            }

                            // We arent supposed to be able to highlight it right now
                            else { _HighlightFocus.SetIsHighlighted(false); }
                        }

                        // selectable == null
                        else { ClearHighlight(); }
                    }
                }

                // hitObject.tag == "Ground"
                else { ClearHighlight(); }
            }

            // hitObject || hitPoint == Settings.InvalidPosition
            else { ClearHighlight(); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void ClearHighlight() {

        if (_HighlightFocus != null) _HighlightFocus.SetIsHighlighted(false);
        _HighlightFocus = null;
        _HighlightBuilding = null;
        _HighlightUnit = null;
        _HighlightWorldObject = null;

        for (int i = 0; i < _PlayerAttached.GetArmy().Count; i++) { _PlayerAttached.GetArmy()[i].SetIsHighlighted(false); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the left mouse button is clicked.
    /// </summary>
    private void LeftMouseClick() {

        if (_PlayerAttached._HUD.MouseInBounds() && !_PlayerAttached._HUD.WheelActive() && !GameManager.Instance.ConfirmRecycleScreen.activeInHierarchy) {

            // Hit tracing from camera to point mouse
            GameObject hitObject = _PlayerAttached._HUD.FindHitObject();
            Vector3 hitPoint = _PlayerAttached._HUD.FindHitPoint();
            if (hitObject && hitPoint != Settings.InvalidPosition) {

                if (hitObject.tag != "Ground") {

                    // Not holding LEFT CONTROL and LEFT SHIFT
                    if (!Input.GetKey(KeyCode.LeftControl)) {

                        if (!Input.GetKey(KeyCode.LeftShift)) {

                            // Deselect any objects that are currently selected
                            foreach (var obj in _PlayerAttached.SelectedWorldObjects) { obj.SetIsSelected(false); }
                            _PlayerAttached.SelectedWorldObjects.Clear();
                            foreach (var obj in _PlayerAttached.SelectedUnits) { obj.SetIsSelected(false); }
                            _PlayerAttached.SelectedUnits.Clear();

                            if (_PlayerAttached.SelectedBuildingSlot != null) {

                                _PlayerAttached.SelectedBuildingSlot.SetIsSelected(false);
                                _PlayerAttached.SelectedBuildingSlot = null;
                            }
                        }
                    }

                    // Cast hit object to selectable objects
                    Base baseObj = null;
                    Building buildingObj = null;
                    BuildingSlot buildingSlot = null;
                    Unit unit = null;
                    WorldObject worldObj = null;

                    baseObj = hitObject.GetComponentInParent<Base>();
                    buildingSlot = hitObject.GetComponent<BuildingSlot>();
                    worldObj = hitObject.GetComponentInParent<WorldObject>();

                    // Left clicking on something attached to a base
                    if (baseObj != null) {

                        buildingObj = hitObject.GetComponent<Building>();

                        // Left clicking on a base
                        if (buildingObj == null && buildingSlot == null) {

                            // Matching team
                            if (baseObj.Team == _PlayerAttached.Team) {

                                // Add selection to list
                                _PlayerAttached.SelectedWorldObjects.Add(baseObj);
                                baseObj.SetPlayer(_PlayerAttached);
                                baseObj.SetIsSelected(true);
                                baseObj.OnSelectionWheel();
                            }
                        }
                        
                        // Left clicking on a building
                        if (buildingObj != null) {

                            if (buildingSlot == null) {

                                // Matching team
                                if (buildingObj.Team == _PlayerAttached.Team) {

                                    // Add selection to list
                                    _PlayerAttached.SelectedWorldObjects.Add(buildingObj);
                                    buildingObj.SetPlayer(_PlayerAttached);
                                    buildingObj.SetIsSelected(true);
                                    buildingObj.OnSelectionWheel();
                                }
                            }
                        }
                        
                        // Left clicking on a building slot
                        if (buildingSlot != null) {

                            // Empty building slot
                            if (buildingSlot.GetBuildingOnSlot() == null) {

                                // Matching team
                                if (buildingSlot.AttachedBase.Team == _PlayerAttached.Team) {

                                    _PlayerAttached.SelectedBuildingSlot = buildingSlot;
                                    buildingSlot.SetPlayer(_PlayerAttached);
                                    buildingSlot.SetIsSelected(true);
                                }
                            }

                            // Builded slot
                            else {

                                // Matching team
                                if (buildingSlot.GetBuildingOnSlot().Team == _PlayerAttached.Team) {

                                    // Add selection to list
                                    _PlayerAttached.SelectedWorldObjects.Add(buildingSlot.GetBuildingOnSlot());
                                    buildingSlot.GetBuildingOnSlot().SetPlayer(_PlayerAttached);
                                    buildingSlot.GetBuildingOnSlot().SetIsSelected(true);
                                    buildingSlot.GetBuildingOnSlot().OnSelectionWheel();
                                }
                            }
                        }
                    }

                    // Left clicking on something NOT attached to a base
                    else {

                        buildingObj = hitObject.GetComponentInParent<Building>();

                        // Left clicking on a building
                        if (buildingObj != null) {

                            if (baseObj == null && buildingSlot == null) {

                                // Matching team
                                if (buildingObj.Team == _PlayerAttached.Team) {

                                    // Add selection to list
                                    _PlayerAttached.SelectedWorldObjects.Add(buildingObj);
                                    buildingObj.SetPlayer(_PlayerAttached);
                                    buildingObj.SetIsSelected(true);
                                    buildingObj.OnSelectionWheel();
                                }
                            }
                        }

                        // Hit an AI object?
                        unit = hitObject.GetComponentInParent<Unit>();
                        
                        // Left clicking on a unit
                        if (unit != null) {

                            // Unit is active in the world
                            if (unit.GetObjectState() == Abstraction.WorldObjectStates.Active) {

                                // Matching team
                                if (unit.Team == _PlayerAttached.Team) {

                                    // Add selection to list
                                    _PlayerAttached.SelectedWorldObjects.Add(unit);
                                    _PlayerAttached.SelectedUnits.Add(unit);
                                    unit.SetPlayer(_PlayerAttached);
                                    unit.SetIsSelected(true);
                                }
                            }                            
                        }

                        // Left clicking on a world object
                        if (worldObj != null) {

                            if (buildingSlot == null && buildingObj == null && baseObj == null && unit == null) {

                                // Add selection to list
                                _PlayerAttached.SelectedWorldObjects.Add(worldObj);
                                worldObj.SetPlayer(_PlayerAttached);
                                worldObj.SetIsSelected(true);
                            }
                        }

                        // Left clicking on a building slot
                        if (buildingSlot != null) {

                            // Empty building slot
                            if (buildingSlot.GetBuildingOnSlot() == null) {

                                _PlayerAttached.SelectedBuildingSlot = buildingSlot;
                                buildingSlot.SetPlayer(_PlayerAttached);
                                buildingSlot.SetIsSelected(true);
                            }

                            // Builded slot
                            else {

                                // Matching team
                                if (buildingSlot.GetBuildingOnSlot().Team == _PlayerAttached.Team) {

                                    // Add selection to list
                                    _PlayerAttached.SelectedWorldObjects.Add(buildingSlot.GetBuildingOnSlot());
                                    buildingSlot.GetBuildingOnSlot().SetPlayer(_PlayerAttached);
                                    buildingSlot.GetBuildingOnSlot().SetIsSelected(true);
                                    buildingSlot.GetBuildingOnSlot().OnSelectionWheel();
                                }
                            }
                        }
                    }

                }

                // Just clicked on the ground so deselect all objects
                else { _PlayerAttached.DeselectAllObjects(); }

                // Update units selected panels
                GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the right mouse button is clicked.
    /// </summary>
    private void RightMouseClick() {

        // Get lists of AIs that are selected
        List<Unit> UnitsSelected = new List<Unit>();

        GetAISelectedFromAllSelected(ref UnitsSelected);

        // Not currently controlling a unit manually
        if (GameManager.Instance.GetIsUnitControlling() == false) {

            // There are AI currently selected and therefore we can command them
            if (UnitsSelected.Count > 0) { AiMouseCommandsInput(UnitsSelected); 
             
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Checks for 'tab' input and creates a global rally point at the users mouse position in world space.
    /// </summary>
    private void CheckForGlobalRallyPoint() {

        if (Input.GetKeyDown(KeyCode.Tab)) {

        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //
    /// </summary>
    /// <param name="squadsSelected"></param>
    /// <param name="unitsSelected"></param>
    private void GetAISelectedFromAllSelected(ref List<Unit> unitsSelected) {
        
        // Cast selected objects to AI objects
        foreach (var obj in _PlayerAttached.SelectedWorldObjects) {
            
            // Checking for individual units
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null) {

                // Same team check
                if (unit.Team == _PlayerAttached.Team) { unitsSelected.Add(unit); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //
    /// </summary>
    private void AiMouseCommandsInput(List<Unit> units) {

        // Get point in world that is used to command the AI currently selected (go position, attack target, etc)
        GameObject hitObject = _PlayerAttached._HUD.FindHitObject();
        Vector3 hitPoint = _PlayerAttached._HUD.FindHitPoint();
        if (hitObject && hitPoint != Settings.InvalidPosition) {

            // AI seek to hitpoint vector
            if (hitObject.tag == "Ground") {

                // Create offsets for each of the individual units so that they dont fight over the same destination point
                for (int i = 0; i < units.Count; i++) {

                    // First unit goes for the center point
                    if (i == 0) { units[0].AgentSeekPosition(hitPoint, true, true); }
                    else {

                        // Every other unit goes around in a circle along the ground
                        Vector3 rand = (Random.insideUnitCircle * (i + 1)) * (units[i].GetAgent().radius * 3);
                        Vector3 destination = hitPoint += new Vector3(rand.x, 0, rand.y);
                        units[i].AgentSeekPosition(hitPoint, true, false);
                    }
                }
            }

            /// (hitObject.tag != "Ground")
            else {

                // Cast hit object to selectable objects
                Base baseObj = hitObject.transform.root.GetComponent<Base>();
                Building buildingObj = hitObject.GetComponentInParent<Building>();

                // Hit an AI object?
                Unit unitObj = hitObject.GetComponentInParent<Unit>();
                
                // Right clicking on a unit
                if (unitObj) {

                    // Enemy unit
                    if (unitObj.Team != GameManager.Team.Defending) {
                        
                        // If there are individually selected units
                        if (units.Count > 0) {

                            // Loop through all selected units & perform ATTACK command on the unit
                            ///foreach (var unit in units) { unit.AgentAttackObject(unitObj, unit.GetAttackingPositionAtObject(unitObj), true); }
                            foreach (var unit in units) { unit.ForceChaseTarget(unitObj, true); }
                        }
                    }                    
                }

                // Right clicking on a building
                else if (buildingObj) {

                    // Enemy building
                    if (buildingObj.Team != GameManager.Team.Defending) {

                        // Get attacking positions
                        List<Vector3> positions = new List<Vector3>();
                        if (units.Count > 0) {

                            // Get positions in a arc facing the building
                            positions = units[0].GetAttackArcPositions(buildingObj, units.Count);
                        }

                        // Attack the building
                        for (int i = 0; i < units.Count; i++) { units[i].AgentAttackObject(buildingObj, positions[i], true, i == 0 ? true : false); }                        
                    }

                    // Ally building
                    else {

                        // The building is garrisonable
                        if (buildingObj.Garrisonable) {

                            // And there is enough space for it
                            ///if (buildingObj.MaxGarrisonPopulation - buildingObj.GetCurrentGarrisonCount() >= )
                        }
                    }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void AiAbilityCommandInput() {

        if (Input.GetKeyDown(KeyCode.R)) {

            // Get point in world that is used to command the AI currently selected (go position, attack target, etc)
            GameObject hitObject = _PlayerAttached._HUD.FindHitObject();
            Vector3 hitPoint = _PlayerAttached._HUD.FindHitPoint();
            if (hitObject && hitPoint != Settings.InvalidPosition) {

                // Get lists of AIs that are selected
                List<Unit> UnitsSelected = new List<Unit>();

                GetAISelectedFromAllSelected(ref UnitsSelected);
                if (UnitsSelected.Count > 0) {

                    // Loop through all selected units & perform ABILITY command
                    foreach (var unit in UnitsSelected) { unit.AgentPerformAbility(hitPoint); }
                }
            }
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //
    /// </summary>
    private void PlatoonInput() {

        int iter = 0;

        /*
         *          
         *  PLATOON ONE 
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 1
        if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {
            
            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 1
        if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Select platoon 1
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }

        ++iter;

        /*
         *          
         *  PLATOON TWO 
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 2
        if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 2
        if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }
    
        // Select platoon 2
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }

        ++iter;

        /*
         *          
         *  PLATOON THREE 
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 3
        if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 3
        if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Select platoon 3
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }

        ++iter;

        /*
         *                  
         *  PLATOON FOUR 
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 4
        if ((Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 4
        if ((Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Select platoon 4
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }

        ++iter;

        /*
         *          
         *  PLATOON FIVE
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 5
        if ((Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 5
        if ((Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Select platoon 5
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }

        ++iter;

        /*
         *          
         *  PLATOON SIX
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 6
        if ((Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 6
        if ((Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Select platoon 6
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }

        ++iter;

        /*
         *          
         *  PLATOON SEVEN
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 7
        if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 7
        if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Select platoon 7
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }

        ++iter;

        /*
         *          
         *  PLATOON EIGHT
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 8
        if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 8
        if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Select platoon 8
        if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }

        ++iter;

        /*
         *          
         *  PLATOON NINE
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 9
        if ((Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 9
        if ((Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Select platoon 9
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }

        ++iter;

        /*
         *          
         *  PLATOON TEN
         *           
         */

        // Glow
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

            _PlayerAttached.GetPlatoon(iter).LightUpBlock();
        }
        if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

            // Binding glow UP if theres AI in the group
            _PlayerAttached.GetPlatoon(iter).LightUpText();
        }

        // Unglow
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {

            // Binding glow DOWN
            _PlayerAttached.GetPlatoon(iter).LightDownText();

            // Unglow the block
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                _PlayerAttached.GetPlatoon(iter).LightDownBlock();
            }
        }

        // Add to platoon 10
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0) && (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Replace platoon 10
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0) && (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))) {

            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Clear platoon
            _PlayerAttached.GetPlatoon(iter).Wipe();

            // Add any units selected to platoon
            for (int i = 0; i < UnitsSelected.Count; i++) {

                // Dont re-add the same unit
                if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[i])) {

                    // Safe to add
                    _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[i]);
                }
            }
        }

        // Select platoon 10
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _PlayerAttached.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _PlayerAttached.GetPlatoon(iter).GetAi()) {

                // Add to selection list
                _PlayerAttached.SelectedWorldObjects.Add(ai);
                _PlayerAttached.SelectedUnits.Add(ai);
                ai.SetPlayer(_PlayerAttached);
                ai.SetIsSelected(true);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool CreateScreenSelection() {

        if (Input.GetKeyDown(KeyCode.Q)) {
            return true;
        }
        else {
            return false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void LineMovement()
    {

        if (Slicedrawer != null) {

            // Get point in world that is used to command the AI currently selected (go position, attack target, etc)
            GameObject hitObjectstart = Slicedrawer.StartGO;
            Vector3 hitPointstart = Slicedrawer.LineStartPoint;
            GameObject hitObjectend = Slicedrawer.EndGO;
            Vector3 hitPointend = Slicedrawer.MouseEndPoint;
            float Distance = Vector3.Distance(hitPointstart, hitPointend);
            float relativeDistance = (Distance / _PlayerAttached.SelectedUnits.Count);
            float relativeCopy = relativeDistance;

            for (int i = 0; i < _PlayerAttached.SelectedUnits.Count; ++i) {

                if (_PlayerAttached.SelectedUnits.Count == 2) {

                    if (hitObjectstart.tag == "Ground") {
                        if (hitObjectstart && hitPointstart != Settings.InvalidPosition) {
                            if (i == 0) {
                                _PlayerAttached.SelectedUnits[i].AgentSeekPosition(hitPointstart, true, true);
                            }
                            else if (i == 1) {
                                _PlayerAttached.SelectedUnits[i].AgentSeekPosition(hitPointend, true, true);
                            }

                        }
                    }

                }
                else {

                    if (hitObjectstart && hitPointstart != Settings.InvalidPosition) {

                        // Use direction of end point to go from start point to next node using relativeDistance
                        // need to count count first and last

                        // AI seek to hitpoint vector
                        if (hitObjectstart.tag == "Ground") {
                            {
                                Vector3 newDirection = hitPointend - hitPointstart;
                                Vector3 directionOnly = newDirection.normalized;
                                Vector3 pointAlongDirection = hitPointstart + (directionOnly * relativeDistance);

                                _PlayerAttached.SelectedUnits[i].AgentSeekPosition(pointAlongDirection, true, true);
                                relativeDistance += relativeCopy;
                            }
                        }
                    }
                }
            }        
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Creates the selection boxes parameters.
    /// </summary>
    private void CreateSelectionBox() {

        if (Input.GetMouseButtonDown(0)) {
            if (!MiniMap.RectangleArea.PointInside(Input.mousePosition))
                _BoxStartPoint = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) {

            if (Selection.width < 0) {
                Selection.x += Selection.width;
                Selection.width = -Selection.width;
            }
            if (Selection.height < 0) {
                Selection.y += Selection.height;
                Selection.height = -Selection.height;
            }

            _BoxStartPoint = -Vector3.one;

            // Update units selected panels
            GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
        }

        if (Input.GetMouseButton(0)) {
      
            Selection = new Rect(_BoxStartPoint.x, InvertMouseY(_BoxStartPoint.y), Input.mousePosition.x - _BoxStartPoint.x,
                                 InvertMouseY(Input.mousePosition.y) - InvertMouseY(_BoxStartPoint.y));

            if (Selection.width < 0)
            {
                Selection.x += Selection.width;
                Selection.width = -Selection.width;
            }
            if (Selection.height < 0)
            {
                Selection.y += Selection.height;
                Selection.height = -Selection.height;
            }

            // Update units selected panels
            GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Draws the selection box for selecting.
    /// </summary>
    private void OnGUI() {

        if (_BoxStartPoint != -Vector3.one) {
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.DrawTexture(Selection, SelectionHighlight);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Inverts the mouses y position in screen space.
    /// </summary>
    /// <param name="y"></param>
    /// <returns>
    //  float
    /// </returns>
    public static float InvertMouseY(float y) { return Screen.height - y; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="a_vector"></param>
    public void SetStartPoint(Vector3 a_vector) { _BoxStartPoint = a_vector; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
