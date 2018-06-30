using System.Collections.Generic;
using TowerDefence;
using UnityEngine;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 22/6/2018
//
//******************************

public class UserInput : MonoBehaviour {
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player;
    private PlayerIndex _PlayerIndex;

    // Gamepad
    private GamePadState _GamepadState;
    private GamePadState _PreviousGamepadState;
    private bool _ControllerIsRumbling = false;
    private float _TimerRumble, _RumbleTime = 0f;
    private float _MotorLeft, _MotorRight = 0f;

    private Vector3 _LookPoint;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    private void Start() {

        // Get component references
        _Player = GetComponent<Player>();
        if (_Player) { _PlayerIndex = _Player._PlayerIndex; }
        
        // Initialize center point for LookAt() function
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
        _LookPoint = hit.point;
    }

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        if (_Player) {

            // Update camera
            MoveCamera();
            RotateCamera();

            // Update gamepad states
            _PreviousGamepadState = _GamepadState;
            _GamepadState = GamePad.GetState(_PlayerIndex);

            // Update mouse input
            MouseActivity();

            // Update abilities input
            AbilitiesInput();

            // Update platoon input
            PlatoonInput();

            // Select all units
            if (Input.GetKeyDown(KeyCode.E)) {

                // Loop through & select all army objects
                foreach (var ai in _Player.GetArmy()) {

                    // Add to selection list
                    _Player.SelectedWorldObjects.Add(ai);
                    ai.SetPlayer(_Player);
                    ai.SetIsSelected(true);
                }
            }
        }
    }

    /// <summary>
    //  Called each frame at a fixed-time framerate.
    /// </summary>
    private void FixedUpdate() {

        if (_ControllerIsRumbling) {

            // Start controller rumble
            GamePad.SetVibration(_PlayerIndex, _MotorLeft, _MotorRight);

            // Timer
            _TimerRumble += Time.fixedDeltaTime;
            if (_TimerRumble >= _RumbleTime) { _ControllerIsRumbling = false; }
        }

        else {

            // Stop controller rumble
            GamePad.SetVibration(_PlayerIndex, 0f, 0f);
            _TimerRumble = 0f;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="motorLeft"></param>
    /// <param name="motorRight"></param>
    /// <param name="time"></param>
    public void StartRumble(float motorLeft, float motorRight, float time) {

        // Set rumble properties
        _MotorLeft = motorLeft;
        _MotorRight = motorRight;
        _RumbleTime = time;

        // Start rumble
        _ControllerIsRumbling = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void MoveCamera() {

        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);

        // Keyboard movement WASD
        if (Input.GetKey(KeyCode.W) && (!Input.GetKey(KeyCode.LeftAlt))) {
            
            // Move forwards
            movement.y += Settings.MovementSpeed;

            // Update center point for RotateAround() function
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
            _LookPoint = hit.point;
        }
        if (Input.GetKey(KeyCode.S) && (!Input.GetKey(KeyCode.LeftAlt))) {

            // Move backwards
            movement.y -= Settings.MovementSpeed;

            // Update center point for RotateAround() function
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
            _LookPoint = hit.point;
        }

        if (Input.GetKey(KeyCode.D) && (!Input.GetKey(KeyCode.LeftAlt))) {

            // Move right
            movement.x += Settings.MovementSpeed;

            // Update center point for RotateAround() function
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
            _LookPoint = hit.point;
        }

        if (Input.GetKey(KeyCode.A) && (!Input.GetKey(KeyCode.LeftAlt))) {

            // Move left
            movement.x -= Settings.MovementSpeed;

            // Update center point for RotateAround() function
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
            _LookPoint = hit.point;
        }

        if (Input.GetKey(KeyCode.LeftShift)) {

            // 'Sprint' movement speed
            Settings.MovementSpeed = Settings.CameraSprintSpeed;
        }
        else {

            // 'Walk' movement speed
            Settings.MovementSpeed = Settings.CameraWalkSpeed;
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
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        // Change camera fov
        float fov = Camera.main.fieldOfView;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {

            // Zooming in
            if (fov > Settings.MinFov)
                Camera.main.fieldOfView -= Time.deltaTime * Settings.ZoomSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {

            // Zooming out
            if (fov < Settings.MaxFov)
                Camera.main.fieldOfView += Time.deltaTime * Settings.ZoomSpeed;
        }

        // Calculate desired camera position based on received input
        Vector3 posOrigin = Camera.main.transform.position;
        Vector3 posDestination = posOrigin;
        posDestination.x += movement.x;
        posDestination.y += movement.y;
        posDestination.z += movement.z;

        // Limit away from ground movement to be between a minimum and maximum distance
        if (posDestination.y > Settings.MaxCameraHeight)
            posDestination.y = Settings.MaxCameraHeight;

        else if (posDestination.y < Settings.MinCameraHeight)
            posDestination.y = Settings.MinCameraHeight;

        // If a change in position is detected perform the necessary update
        if (posDestination != posOrigin) {

            // Update position
            Camera.main.transform.position = Vector3.MoveTowards(posOrigin, posDestination, Time.deltaTime * Settings.MovementSpeed);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void RotateCamera() {

        Vector3 rotOrigin = Camera.main.transform.eulerAngles;
        Vector3 rotDestination = rotOrigin;

        bool pressed = false;

        // Rotate camera state if ALT is being held down
        if ((Input.GetKey(KeyCode.LeftAlt))) {

            // Hide mouse cursor
            Cursor.visible = false;
            
            // Calculate which direction to rotate in
            float dir = 0f;
            dir = Input.GetAxis("Mouse X");

            // Rotate
            Camera.main.transform.RotateAround(_LookPoint, Vector3.up, Settings.RotateSpeed * -dir * Time.deltaTime);

            // Used for resetting the mouse position
            pressed = true;
        }
        else { Cursor.visible = true; }

        if (pressed) {

            // Reset mouse to center of the screen
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.None;
            pressed = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void MouseActivity() {

        if      (Input.GetMouseButtonDown(0)) { LeftMouseClick(); }
        else if (Input.GetMouseButtonDown(1)) { RightMouseClick(); }
    }

    /// <summary>
    /// 
    /// </summary>
    private void LeftMouseClick() {

        if (_Player._HUD.MouseInBounds() && !_Player._HUD.WheelActive()) {

            // Hit tracing from camera to point mouse
            GameObject hitObject = _Player._HUD.FindHitObject();
            Vector3 hitPoint = _Player._HUD.FindHitPoint();
            if (hitObject && hitPoint != Settings.InvalidPosition) {
                
                if (hitObject.tag != "Ground") {

                    // Not holding LEFT CONTROL and LEFT SHIFT
                    if (!Input.GetKey(KeyCode.LeftControl)) {

                        if (!Input.GetKey(KeyCode.LeftShift)) {

                            // Deselect any objects that are currently selected
                            foreach (var obj in _Player.SelectedWorldObjects) { obj.SetIsSelected(false); }
                            _Player.SelectedWorldObjects.Clear();

                            if (_Player.SelectedBuildingSlot != null) {

                                _Player.SelectedBuildingSlot.SetIsSelected(false);
                                _Player.SelectedBuildingSlot = null;
                            }
                        }
                    }

                    // Cast hit object to selectable objects
                    Base baseObj = hitObject.transform.root.GetComponent<Base>();
                    Building buildingObj = null;
                    BuildingSlot buildingSlot = null;
                    WorldObject worldObj = null;
                    Squad squadObj = null;
                    Unit unitObj = null;

                    
                    // The root transform would be the base transform if base is valid (which overwrites the selection wheel buildables)
                    if (baseObj != null) {
                        
                        buildingObj = hitObject.GetComponent<Building>();
                        buildingSlot = hitObject.GetComponent<BuildingSlot>();
                        worldObj = hitObject.GetComponent<WorldObject>();

                        // Left clicking on a base
                        if (buildingObj == null && buildingSlot == null && worldObj == null) {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(baseObj);
                            baseObj.SetPlayer(_Player);
                            baseObj.SetIsSelected(true);
                            baseObj.OnSelectionWheel();
                        }
                    }

                    /// baseObj == null
                    else {

                        buildingObj = hitObject.transform.root.GetComponent<Building>();
                        buildingSlot = hitObject.transform.root.GetComponent<BuildingSlot>();
                        worldObj = hitObject.transform.root.GetComponent<WorldObject>();
                    }

                    // Hit an AI object?
                    squadObj = hitObject.GetComponent<Squad>();
                    unitObj = hitObject.GetComponentInParent<Unit>();

                    // Left clicking on a building object
                    if (buildingObj) {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(worldObj);
                        buildingObj.SetPlayer(_Player);
                        buildingObj.SetIsSelected(true);
                        buildingObj.OnSelectionWheel();
                    }

                    // Left clicking on a building slot
                    else if (buildingSlot) {

                        // Empty building slot
                        if (buildingSlot._BuildingOnSlot == null) {

                            _Player.SelectedBuildingSlot = buildingSlot;
                            buildingSlot.SetPlayer(_Player);
                            buildingSlot.SetIsSelected(true);
                        }

                        // Builded slot
                        else {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingSlot._BuildingOnSlot);
                            buildingSlot._BuildingOnSlot.SetPlayer(_Player);
                            buildingSlot._BuildingOnSlot.SetIsSelected(true);
                            buildingSlot._BuildingOnSlot.OnSelectionWheel();
                        }
                    }

                    // Left clicking on a squad
                    else if (squadObj) {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(squadObj);
                        squadObj.SetPlayer(_Player);
                        squadObj.SetIsSelected(true);
                    }

                    // Left clicking on a unit
                    else if (unitObj) {
                        
                        // Is the unit part of a squad?
                        if (unitObj.IsInASquad()) {

                            squadObj = unitObj.GetSquadAttached();

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(squadObj);
                            squadObj.SetPlayer(_Player);
                            squadObj.SetIsSelected(true);
                        }

                        // Unit is NOT in a squad
                        else {

                            // Add selection to list
                            ///_Player.SelectedWorldObjects.Add(unitObj);
                            ///unitObj.SetPlayer(_Player);
                            ///unitObj.SetSelection(true);
                        }
                    }

                    // Left clicking on a world object
                    else if (worldObj) {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(worldObj);
                        worldObj.SetPlayer(_Player);
                        worldObj.SetIsSelected(true);
                    }
                }

                // Just clicked on the ground so deselect all objects
                else {

                    // Deselect ALL worldObjects
                    _Player.DeselectAllObjects();
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void RightMouseClick() {

        // Get lists of AIs that are selected
        List<Squad> SquadsSelected = new List<Squad>();
        List<Unit> UnitsSelected = new List<Unit>();

        GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

        // There are AI currently selected and therefore we can command them
        if (SquadsSelected.Count > 0 || UnitsSelected.Count > 0) { AiCommandsInput(SquadsSelected, UnitsSelected); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="squadsSelected"></param>
    /// <param name="unitsSelected"></param>
    private void GetAISelectedFromAllSelected(ref List<Squad> squadsSelected, ref List<Unit> unitsSelected) {
        
        // Cast selected objects to AI objects
        foreach (var obj in _Player.SelectedWorldObjects) {

            // Checking for squads
            Squad squad = obj.GetComponent<Squad>();
            if (squad != null) { squadsSelected.Add(squad); }

            // Checking for individual units
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null) { unitsSelected.Add(unit); }
        }
    } 

    /// <summary>
    /// 
    /// </summary>
    private void PlatoonInput() {

        // Select platoon 1
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon1()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 1
        if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 1
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon1().Add(squad); }

            // Add any units selected to platoon 1
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon1().Add(unit); }
        }

        // Replace platoon 1
        if ((Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 1
            _Player.GetPlatoon1().Clear();

            // Add any squads selected to platoon 1
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon1().Add(squad); }

            // Add any units selected to platoon 1
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon1().Add(unit); }
        }

        // Select platoon 2
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon2()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 2
        if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 2
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon2().Add(squad); }

            // Add any units selected to platoon 2
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon2().Add(unit); }
        }

        // Replace platoon 2
        if ((Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 2
            _Player.GetPlatoon2().Clear();

            // Add any squads selected to platoon 2
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon2().Add(squad); }

            // Add any units selected to platoon 2
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon2().Add(unit); }
        }

        // Select platoon 3
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon3()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 3
        if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 3
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon3().Add(squad); }

            // Add any units selected to platoon 3
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon3().Add(unit); }
        }

        // Replace platoon 3
        if ((Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 3
            _Player.GetPlatoon3().Clear();

            // Add any squads selected to platoon 3
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon3().Add(squad); }

            // Add any units selected to platoon 3
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon3().Add(unit); }
        }

        // Select platoon 4
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon4()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 4
        if ((Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 4
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon4().Add(squad); }

            // Add any units selected to platoon 4
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon4().Add(unit); }
        }

        // Replace platoon 4
        if ((Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 4
            _Player.GetPlatoon4().Clear();

            // Add any squads selected to platoon 1
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon4().Add(squad); }

            // Add any units selected to platoon 1
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon4().Add(unit); }
        }

        // Select platoon 5
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon5()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 5
        if ((Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 5
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon5().Add(squad); }

            // Add any units selected to platoon 5
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon5().Add(unit); }
        }

        // Replace platoon 5
        if ((Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 5
            _Player.GetPlatoon5().Clear();

            // Add any squads selected to platoon 5
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon5().Add(squad); }

            // Add any units selected to platoon 5
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon5().Add(unit); }
        }

        // Select platoon 6
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon6()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 6
        if ((Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 6
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon6().Add(squad); }

            // Add any units selected to platoon 6
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon6().Add(unit); }
        }

        // Replace platoon 6
        if ((Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 6
            _Player.GetPlatoon6().Clear();

            // Add any squads selected to platoon 1
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon6().Add(squad); }

            // Add any units selected to platoon 1
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon6().Add(unit); }
        }

        // Select platoon 7
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon7()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 7
        if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 7
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon7().Add(squad); }

            // Add any units selected to platoon 7
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon7().Add(unit); }
        }

        // Replace platoon 7
        if ((Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 7
            _Player.GetPlatoon7().Clear();

            // Add any squads selected to platoon 7
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon7().Add(squad); }

            // Add any units selected to platoon 7
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon7().Add(unit); }
        }

        // Select platoon 8
        if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon8()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 8
        if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 8
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon8().Add(squad); }

            // Add any units selected to platoon 8
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon8().Add(unit); }
        }

        // Replace platoon 8
        if ((Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 8
            _Player.GetPlatoon8().Clear();

            // Add any squads selected to platoon 8
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon8().Add(squad); }
        
            // Add any units selected to platoon 8
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon8().Add(unit); }
        }

        // Select platoon 9
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon9()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 9
        if ((Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 9
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon9().Add(squad); }

            // Add any units selected to platoon 9
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon9().Add(unit); }
        }

        // Replace platoon 9
        if ((Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 9
            _Player.GetPlatoon9().Clear();

            // Add any squads selected to platoon 9
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon9().Add(squad); }

            // Add any units selected to platoon 9
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon9().Add(unit); }
        }

        // Select platoon 10
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0) && (!Input.GetKeyDown(KeyCode.LeftShift) && (!Input.GetKeyDown(KeyCode.LeftControl)))) {

            _Player.DeselectAllObjects();

            // Loop through & select all army objects
            foreach (var ai in _Player.GetPlatoon10()) {

                // Add to selection list
                _Player.SelectedWorldObjects.Add(ai);
                ai.SetPlayer(_Player);
                ai.SetIsSelected(true);
            }
        }

        // Add to platoon 10
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0) && (Input.GetKey(KeyCode.LeftShift)) && !Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Add any squads selected to platoon 10
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon10().Add(squad); }

            // Add any units selected to platoon 10
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon10().Add(unit); }
        }

        // Replace platoon 10
        if ((Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0) && (!Input.GetKey(KeyCode.LeftShift)) && Input.GetKey(KeyCode.LeftControl))) {

            // Get lists of AIs that are selected
            List<Squad> SquadsSelected = new List<Squad>();
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref SquadsSelected, ref UnitsSelected);

            // Clear platoon 10
            _Player.GetPlatoon10().Clear();

            // Add any squads selected to platoon 10
            foreach (var squad in SquadsSelected) { _Player.GetPlatoon10().Add(squad); }

            // Add any units selected to platoon 10
            foreach (var unit in UnitsSelected) { _Player.GetPlatoon10().Add(unit); }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void AiCommandsInput(List<Squad> squads, List<Unit> units) {

        // Get point in world that is used to command the AI currently selected (go position, attack target, etc)
        GameObject hitObject = _Player._HUD.FindHitObject();
        Vector3 hitPoint = _Player._HUD.FindHitPoint();
        if (hitObject && hitPoint != Settings.InvalidPosition) {

            // AI seek to hitpoint vector
            if (hitObject.tag == "Ground") {

                // If there are selected squads
                if (squads.Count > 0) {

                    // Loop through all selected squads & perform SEEK command
                    foreach (var squad in squads) { squad.SquadSeek(hitPoint); }
                }

                // If there are individually selected units
                if (units.Count > 0) {

                    // Loop through all selected units & perform SEEK command
                    foreach (var unit in units) { unit.AgentSeekPosition(hitPoint); }
                }
            }

            /// (hitObject.tag != "Ground")
            else {

                // Cast hit object to selectable objects
                Base baseObj = hitObject.transform.root.GetComponent<Base>();
                Building buildingObj = hitObject.GetComponentInParent<Building>(); ;
                ///BuildingSlot buildingSlot = null;
                ///WorldObject worldObj = null;

                // Hit an AI object?
                Squad squadObj = hitObject.GetComponent<Squad>();
                Unit unitObj = hitObject.GetComponentInParent<Unit>();

                // Right clicking on a squad
                if (squadObj) {
                    
                    // Enemy squad
                    if (squadObj.Team !=  GameManager.Team.Defending) {
                        
                        // If there are selected squads
                        if (squads.Count > 0) {

                            // Loop through all selected squads & perform ATTACK command on the squad
                            foreach (var squad in squads) { squad.SquadAttackObject(squadObj); }
                        }

                        // If there are individually selected units
                        if (units.Count > 0) {

                            // Loop through all selected units & perform ATTACK command on the squad
                            foreach (var unit in units) { unit.AgentAttackObject(squadObj); }
                        }
                    }
                }

                // Right clicking on a unit
                else if (unitObj) {

                    // Is the unit part of a squad?
                    if (unitObj.IsInASquad()) {

                        squadObj = unitObj.GetSquadAttached();

                        // Enemy squad
                        if (squadObj.Team != GameManager.Team.Defending) {

                            // If there are selected squads
                            if (squads.Count > 0) {

                                // Loop through all selected squads & perform ATTACK command on the squad
                                foreach (var squad in squads) { squad.SquadAttackObject(squadObj); }
                            }

                            // If there are individually selected units
                            if (units.Count > 0) {

                                // Loop through all selected units & perform ATTACK command on the squad
                                foreach (var unit in units) { unit.AgentAttackObject(squadObj); }
                            }
                        }
                    }

                    // Unit is NOT in a squad
                    else {

                        // Enemy unit
                        if (unitObj.Team != GameManager.Team.Defending) {

                            // If there are selected squads
                            if (squads.Count > 0) {

                                // Loop through all selected squads & perform ATTACK command on the unit
                                foreach (var squad in squads) { squad.SquadAttackObject(unitObj); }
                            }

                            // If there are individually selected units
                            if (units.Count > 0) {

                                // Loop through all selected units & perform ATTACK command on the unit
                                foreach (var unit in units) { unit.AgentAttackObject(unitObj); }
                            }
                        }
                    }
                }

                // Right clicking on a building
                else if (buildingObj) {

                    // Enemy building
                    if (buildingObj.Team != GameManager.Team.Defending) {

                        // If there are selected squads
                        if (squads.Count > 0) {

                            // Loop through all selected squads & perform ATTACK command on the building
                            foreach (var squad in squads) { squad.SquadAttackObject(buildingObj); }
                        }

                        // If there are individually selected units
                        if (units.Count > 0) {

                            // Loop through all selected units & perform ATTACK command on the building
                            foreach (var unit in units) { unit.AgentAttackObject(buildingObj); }
                        }
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

    /// <summary>
    /// 
    /// </summary>
    private void AbilitiesInput() {

        // Only check if theres a laboratory building in the world
        if (GameManager.Instance.GetIsLabratoryActive()) {

            // On user input
            if (Input.GetKeyDown(KeyCode.F)) {

                // Deselect any objects that are currently selected
                foreach (var obj in _Player.SelectedWorldObjects) { obj.SetIsSelected(false); }
                _Player.SelectedWorldObjects.Clear();

                // hide selection wheel if on screen
                if (_Player._HUD.SelectionWheel.gameObject.activeInHierarchy) { _Player._HUD.SetAbilitiesWheelVisibility(false); }

                // Show/hide abilities wheel
                _Player._HUD.SetAbilitiesWheelVisibility(!_Player._HUD.AbilitiesWheel.activeInHierarchy);
            }
        }

        // Force hide the abilities wheel if there is no active laboratory in the world
        else {

        }
    }
    
    //***************************
    // XBOX ONE Special buttons

    /// Returns if the XBOX GUIDE button was pressed then released in the last 2 frames
    public bool GetGuideButtonClicked() { return (_PreviousGamepadState.Buttons.Guide == ButtonState.Released && _GamepadState.Buttons.Guide == ButtonState.Pressed); }

    /// Returns if the BACK button was pressed then released in the last 2 frames
    public bool GetBackButtonClicked() { return (_PreviousGamepadState.Buttons.Back == ButtonState.Released && _GamepadState.Buttons.Back == ButtonState.Pressed); }

    /// Returns if the START button was pressed then released in the last 2 frames
    public bool GetStartButtonClicked() { return (_PreviousGamepadState.Buttons.Start == ButtonState.Released && _GamepadState.Buttons.Start == ButtonState.Pressed); }

    //***************************
    // XBOX ONE Shoulder / Bumper buttons

    /// Returns if the LEFT SHOULDER button was pressed then released in the last 2 frames
    public bool GetLeftShoulderClicked() { return (_PreviousGamepadState.Buttons.LeftShoulder == ButtonState.Released && _GamepadState.Buttons.LeftShoulder == ButtonState.Pressed); }

    /// Returns if the RIGHT SHOULDER button was pressed then released in the last 2 frames
    public bool GetRightShoulderClicked() { return (_PreviousGamepadState.Buttons.RightShoulder == ButtonState.Released && _GamepadState.Buttons.RightShoulder == ButtonState.Pressed); }

    //***************************
    // XBOX ONE Face buttons

    /// Returns if the A button was pressed then released in the last 2 frames
    public bool GetButtonAClicked() { return (_PreviousGamepadState.Buttons.A == ButtonState.Released && _GamepadState.Buttons.A == ButtonState.Pressed); }

    /// Returns if the A button was released in during this frame
    public bool GetButtonAReleased() { return (_GamepadState.Buttons.A == ButtonState.Released); }

    /// Returns if the B button was pressed then released in the last 2 frames
    public bool GetButtonBClicked() { return (_PreviousGamepadState.Buttons.B == ButtonState.Released && _GamepadState.Buttons.B == ButtonState.Pressed); }

    /// Returns if the B button was released in during this frame
    public bool GetButtonBReleased() { return (_GamepadState.Buttons.B == ButtonState.Released); }

    /// Returns if the X button was pressed then released in the last 2 frames
    public bool GetButtonXClicked() { return (_PreviousGamepadState.Buttons.X == ButtonState.Released && _GamepadState.Buttons.X == ButtonState.Pressed); }

    /// Returns if the X button was released in during this frame
    public bool GetButtonXReleased() { return (_GamepadState.Buttons.X == ButtonState.Released); }

    /// Returns if the Y button was pressed then released in the last 2 frames
    public bool GetButtonYClicked() { return (_PreviousGamepadState.Buttons.Y == ButtonState.Released && _GamepadState.Buttons.Y == ButtonState.Pressed); }

    /// Returns if the Y button was released in during this frame
    public bool GetButtonYReleased() { return (_GamepadState.Buttons.Y == ButtonState.Released); }

    //**************************
    // XBOX ONE D-Pad buttons

    /// Returns if the D-PAD UP button was pressed then released in the last 2 frames
    public bool GetDpadUpClicked() { return (_PreviousGamepadState.DPad.Up == ButtonState.Released && _GamepadState.DPad.Up == ButtonState.Pressed); }

    /// Returns if the D-PAD DOWN button was pressed then released in the last 2 frames
    public bool GetDpadDownClicked() { return (_PreviousGamepadState.DPad.Down == ButtonState.Released && _GamepadState.DPad.Down == ButtonState.Pressed); }

    /// Returns if the D-PAD RIGHT button was pressed then released in the last 2 frames
    public bool GetDpadRightClicked() { return (_PreviousGamepadState.DPad.Right == ButtonState.Released && _GamepadState.DPad.Right == ButtonState.Pressed); }

    /// Returns if the D-PAD LEFT button was pressed then released in the last 2 frames
    public bool GetDpadLeftClicked() { return (_PreviousGamepadState.DPad.Left == ButtonState.Released && _GamepadState.DPad.Left == ButtonState.Pressed); }

    //**************************
    // XBOX ONE Thumbsticks

    /// Returns the LEFT THUMBSTICK X axis value
    public float GetLeftThumbstickXaxis() { return _GamepadState.ThumbSticks.Left.X; }

    /// Returns the LEFT THUMBSTICK Y axis value
    public float GetLeftThumbstickYaxis() { return _GamepadState.ThumbSticks.Left.Y; }

    /// Returns the RIGHT THUMBSTICK X axis value
    public float GetRightThumbstickXaxis() { return _GamepadState.ThumbSticks.Right.X; }

    /// Returns the RIGHT THUMBSTICK Y axis value
    public float GetRightThumbstickYaxis() { return _GamepadState.ThumbSticks.Right.Y; }

    /// Returns the direction vector of the LEFT THUMBSTICK
    public Vector3 GetLeftThumbstickInput() { return new Vector3(0, 90f - (Mathf.Atan2(_GamepadState.ThumbSticks.Left.Y, _GamepadState.ThumbSticks.Left.X)) * 180 / Mathf.PI, 0); }

    /// Returns the direction vector of the RIGHT THUMBSTICK
    public Vector3 GetRightThumbstickInput() { return new Vector3(0, 90f - (Mathf.Atan2(_GamepadState.ThumbSticks.Right.Y, _GamepadState.ThumbSticks.Right.X)) * 180 / Mathf.PI, 0); }

    /// Returns boolean value if the LEFT THUMBSTICK is moving down
    public bool OnLeftThumbstickDown() { return _GamepadState.ThumbSticks.Left.Y < 0f; }

    /// Returns boolean value if the LEFT THUMBSTICK is moving up
    public bool OnLeftThumbstickUp() { return _GamepadState.ThumbSticks.Left.Y > 0f; }

    /// Returns boolean value if the LEFT THUMBSTICK is moving left
    public bool OnLeftThumbstickLeft() { return _GamepadState.ThumbSticks.Left.X < 0f; }

    /// Returns boolean value if the LEFT THUMBSTICK is moving right
    public bool OnLeftThumbstickRight() { return _GamepadState.ThumbSticks.Left.X > 0f; }

    //**************************
    // XBOX ONE Triggers

    /// Returns the LEFT TRIGGER axis value
    public float GetLeftTriggeraxis() { return _GamepadState.Triggers.Left; }

    /// Returns the RIGHT TRIGGER axis value
    public float GetRightTriggeraxis() { return _GamepadState.Triggers.Right; }

    /// Returns boolean value if the LEFT TRIGGER is moving
    public bool OnLeftTrigger() { return _GamepadState.Triggers.Left > 0f; }

    /// Returns boolean value if the RIGHT TRIGGER is moving
    public bool OnRightTrigger() { return _GamepadState.Triggers.Right > 0f; }
    
}
