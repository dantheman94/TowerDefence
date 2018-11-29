﻿using TowerDefence;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 27/08/2018
//
//******************************

public class XboxGamepadInput : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    
    [Header("-----------------------------------")]
    [Header(" GAMEPAD MULTISPHERE PROPERTIES")]
    [Space]
    public GameObject SphereSelectorObject;
    [Space]
    public float MaxSphereRadius = 250;
    public float SphereGrowRate = 10;
    public float SphereStartRadius = 5;

    [Space]
    [Header("-----------------------------------")]
    [Header(" RAYCAST LAYERMASK")]
    [Space]
    public LayerMask MaskBlock;

    [Space]
    [Header("-----------------------------------")]
    [Header(" XBOX ONE ICONS")]
    [Space]
    public Image CrosshairImage;
    [Space]
    public Image AButton;
    public Image XButton;
    public Image BButton;
    public Image YButton;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Vector3 _LookPoint;
    private Vector3 _CurrentVelocity = Vector3.zero;

    private GamePadState _GamepadState;
    private GamePadState _PreviousGamepadState;
    private bool _ControllerIsRumbling = false;
    private float _TimerRumble, _RumbleTime = 0f;
    private float _MotorLeft, _MotorRight = 0f;
    private xb_gamepad _Gamepad;
    private GameObject _SphereReference;
    private SelectionWheel _SelectionWheel;
    private float _CurrentAngle = 0f;
    float _AngleOffset = 360 / 10;
    private int _RadialIndex = 0;
    private Vector3 _MousePosReference;
    private GameObject _ReticleObject;

    private Player _PlayerAttached;
    private KeyboardInput _KeyboardInputManager = null;
    public bool IsPrimaryController { get; set; }

    [HideInInspector]
    public bool CanSelect = false;

    private int _PlatoonIteratorSelected = 0;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    private void Start() {
        _ReticleObject = CrosshairImage.gameObject;
        _Gamepad = GamepadManager.Instance.GetGamepad(1);
        _SelectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>();
        // Get component references
        _PlayerAttached = GetComponent<Player>();
        _KeyboardInputManager = GetComponent<KeyboardInput>();

        // Initialize center point for LookAt() function
        RaycastHit hit;
        Physics.Raycast(_PlayerAttached.CameraAttached.transform.position, _PlayerAttached.CameraAttached.transform.forward * 1000, out hit);
        _LookPoint = hit.point;

        IsPrimaryController = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {
    
   //     TogglePause();
        if (_PlayerAttached) {

            // Update primary controller
            if (GamepadManager.Instance.GetGamepad(1).GetAnyButton()) {

                // Disable keyboard / Enable gamepad
                IsPrimaryController = true;
                if (_KeyboardInputManager != null) { _KeyboardInputManager.IsPrimaryController = false; }

            }
            
            // Update gamepad states
            _PreviousGamepadState = _GamepadState;
            _GamepadState = GamePad.GetState(_PlayerAttached.Index);
            
            if (IsPrimaryController && !TutorialScene.CurrentMessageData.LockControls) {

                _ReticleObject.SetActive(true);
                //Gamepad function presses.
                DisplayButtonUI();
                MoveSelectedUnits(GamepadSchemeManager.Instance.GetActiveScheme().AttackandMove);
                ExitUI(GamepadSchemeManager.Instance.GetActiveScheme().ExitMenuandUnselect);
                ChangeSelectionWheel();
                StartCoroutine(Select());
                CreateSelection();
                ChangeReticle();
                DisableGamepadUI();
                if (!GameManager.Instance.SelectionWheel.activeInHierarchy)
                {
                    // Update camera
                    MoveCamera();
                    RotateCamera();

                    // Update camera FOV
                    ZoomCamera();
                }       

                // Update point/click input
                PointClickActivity();
                Cursor.visible = false;

                // Update abilities input
                ///AbilitiesInput();

                // Update platoon input
                PlatoonInput();
                
                // Select all units
                if (GetLeftShoulderClicked()) {

                    // Deselect all units
                    _PlayerAttached.DeselectAllObjects();

                    // Loop through & select all army objects
                    foreach (var ai in _PlayerAttached.GetArmy()) {

                        // Only select active units in the world
                        if (ai._ObjectState == Abstraction.WorldObjectStates.Active) {

                            // Add to selection list
                            _PlayerAttached.SelectedWorldObjects.Add(ai);
                            _PlayerAttached.SelectedUnits.Add(ai);
                            ai.SetPlayer(_PlayerAttached);
                            ai.SetIsSelected(true);
                        }
                    }

                    // Update units selected panels
                    GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
                }
            }
            else
            {
                _ReticleObject.SetActive(false);
            }
        }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame at a fixed-time framerate.
    /// </summary>
    private void FixedUpdate() {

        if (_ControllerIsRumbling) {

            // Start controller rumble
            GamePad.SetVibration(_PlayerAttached.Index, _MotorLeft, _MotorRight);

            // Timer
            _TimerRumble += Time.fixedDeltaTime;
            if (_TimerRumble >= _RumbleTime) { _ControllerIsRumbling = false; }
        }

        else {

            // Stop controller rumble
            GamePad.SetVibration(_PlayerAttached.Index, 0f, 0f);
            _TimerRumble = 0f;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void DisableGamepadUI()
    {
        if(!_PlayerAttached._KeyboardInputManager.IsPrimaryController)
        {
            if(_MousePosReference != Input.mousePosition && _MousePosReference != new Vector3(0,0,0))
            {
                _PlayerAttached._KeyboardInputManager.IsPrimaryController = true;
                IsPrimaryController = false;
            }
            _MousePosReference = Input.mousePosition;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void ChangeReticle()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        //Fire raycast from middle of the screen.
        if (Physics.Raycast(ray, out hit, 1000, _PlayerAttached._HUD.MaskBlock))
        {
            if (hit.transform.gameObject.tag != "Ground")
            {
                WorldObject wo = hit.transform.gameObject.GetComponent<WorldObject>();
                if(wo == null)
                {
                    wo = hit.transform.gameObject.GetComponentInChildren<WorldObject>();
                }
                if(wo != null)
                {
                   
                    if (wo.Team == GameManager.Team.Defending)
                    {
                        CrosshairImage.color = _PlayerAttached.TeamColor;
                    }
                    else if (wo.Team == GameManager.Team.Attacking)
                    {
                        CrosshairImage.color = WaveManager.Instance.AttackingTeamColour;
                    }
                }
        
            }
            else
            {
                CrosshairImage.color = Color.white;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void TogglePause()
    {
        if(GameManager.Instance.PauseWidget.transform.gameObject.activeInHierarchy)
        {
            if(_Gamepad.GetButtonDown("start"))
            {
                GameManager.Instance.PauseWidget.gameObject.SetActive(false);
            }
        }
        else
        {
            if (_Gamepad.GetButtonDown("start"))
            {
                GameManager.Instance.PauseWidget.gameObject.SetActive(true);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Exits user interface.
    /// </summary>
    private void ExitUI(string buttonPress)
    {
        if(GameManager.Instance.SelectionWheel.activeInHierarchy)
        {
            CrosshairImage.enabled = false;
            if (_Gamepad.GetButtonDown(buttonPress))
            {
                GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>().HideSelectionWheel();
            }

        }
        else
        {
            CrosshairImage.enabled = true;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Initiates the controller rumble process (vibration)
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
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Updates the center screen world point used the camera rotating
    /// </summary>
    public void CreateCenterPoint() {

        // Update center point for RotateAround() function
        RaycastHit hit;
        Physics.Raycast(_PlayerAttached.CameraAttached.transform.position, _PlayerAttached.CameraAttached.transform.forward * 1000, out hit);
        _LookPoint = hit.point;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Change selection.
    /// </summary>
    public void ChangeSelectionWheel()
    {
        if (GameManager.Instance.SelectionWheel.activeInHierarchy)
        {
            float _RawAngle;
            float globalOffset = 0;
            _RawAngle = Mathf.Atan2(_Gamepad.GetStick_L().Y, _Gamepad.GetStick_L().X) * Mathf.Rad2Deg;

            if (_Gamepad.GetStick_L().X != 0 || _Gamepad.GetStick_L().Y != 0)
            {
                _CurrentAngle = NormalizeAngle(-_RawAngle + 90 - globalOffset + (_AngleOffset / 2f));
                GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>().
                SelectionMarker.rotation = Quaternion.Euler(0, 0, _RawAngle + 270);
            }

            if (_AngleOffset != 0)
            {
                _RadialIndex = (int)(_CurrentAngle / _AngleOffset);
            }

            for(int i = 0; i < _SelectionWheel._WheelButtons.Count; ++i)
            {
                    if(_RadialIndex == i)
                   {
                    StartCoroutine(DelayedSelect(_SelectionWheel._WheelButtons[i]));
                }
            }
     
        }
    }

    IEnumerator DelayedSelect(Button a_button)
    {
        yield return new WaitForSeconds(0.05f);
        a_button.Select();
    }
 
    IEnumerator Select()
    {
        yield return new WaitForSeconds(0.1f);
    
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void DisplayButtonUI()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        //Fire raycast from middle of the screen.
        if (Physics.Raycast(ray, out hit, 1000, _PlayerAttached._HUD.MaskBlock))
        {
            if(hit.transform.gameObject.tag != "Ground" && !GameManager.Instance.SelectionWheel.activeInHierarchy)
            {
                AButton.enabled = true;
                if(_Gamepad.GetButton("A"))
                {
                    AButton.color = Color.grey;
                }
                else
                {
                    AButton.color = Color.white;    
                }
            }
            else
            {
                AButton.enabled = false;
            }

            if(hit.transform.gameObject.tag == "Ground" && _PlayerAttached.SelectedWorldObjects.Count > 0)
            {
                XButton.enabled = true;
                if(_Gamepad.GetButton("X"))
                {
                    XButton.color = Color.grey;
                }
                else
                {
                    XButton.color = Color.white;
                }
            }
            else
            {
                XButton.enabled = false;
            }


            YButton.enabled = false;
            BButton.enabled = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void MoveCamera() {
        
        Vector3 movement = new Vector3(0, 0, 0);

        // Move camera via input if the player ISNT currently controlling a unit
        if (GameManager.Instance.GetIsUnitControlling() == false) {

            // Keyboard movement WASD
            if (OnLeftThumbstickUp()) {

                // Move forwards
                movement.y += Settings.MovementSpeed;
                CreateCenterPoint();
            }

            if (OnLeftThumbstickDown()) {

                // Move backwards
                movement.y -= Settings.MovementSpeed;
                CreateCenterPoint();
            }

            if (OnLeftThumbstickRight()) {

                // Move right
                movement.x += Settings.MovementSpeed;
                CreateCenterPoint();
            }

            if (OnLeftThumbstickLeft()) {

                // Move left
                movement.x -= Settings.MovementSpeed;
                CreateCenterPoint();
            }

            if (_Gamepad.GetButton("L3")) {

                // 'Sprint' movement speed
                Settings.MovementSpeed = Settings.CameraSprintSpeed;
            }
            else {

                // 'Walk' movement speed
                Settings.MovementSpeed = Settings.CameraWalkSpeed;
            }
        }
        
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
        if (posDestination.y > Settings.MaxCameraHeight)
            posDestination.y = Settings.MaxCameraHeight;

        else if (posDestination.y < Settings.MinCameraHeight)
                 posDestination.y = Settings.MinCameraHeight;

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
    /// <param name="buttonPress"></param>
    private void MoveSelectedUnits(string buttonPress)
    {
        if (_Gamepad.GetButtonDown(buttonPress))
        {
            // Get lists of AIs that are selected
            List<Unit> UnitsSelected = new List<Unit>();

            GetAISelectedFromAllSelected(ref UnitsSelected);

            // Not currently controlling a unit manually
            if (GameManager.Instance.GetIsUnitControlling() == false)
            {

                // There are AI currently selected and therefore we can command them
                if (UnitsSelected.Count > 0) { AiControllerInput(UnitsSelected); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Takes centre screen point and gets all selected units to move towards it.
    /// </summary>
    private void AiControllerInput(List<Unit> units)
    {

        // Get point in world that is used to command the AI currently selected (go position, attack target, etc)
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        GameObject hitObject = null;
        // Return object from raycast
        if (Physics.Raycast(ray, out hit, 1000, MaskBlock))
        {
            hitObject = hit.collider.gameObject;
        }

        Vector3 hitPoint = Settings.InvalidPosition;
        // Return object from raycast
        if (Physics.Raycast(ray, out hit, 1000, MaskBlock))
        {
            hitPoint = hit.point;
        }

        //GameObject hitObject = _PlayerAttached._HUD.FindHitObject();
     //   Vector3 hitPoint = _PlayerAttached._HUD.FindHitPoint();
        if (hitObject && hitPoint != Settings.InvalidPosition)
        {

            // AI seek to hitpoint vector
            if (hitObject.tag == "Ground")
            {
                // If there are individually selected units
                if (units.Count > 0)
                {

                    // Loop through all selected units & perform SEEK command
                    foreach (var unit in units) { unit.AgentSeekPosition(hitPoint, true); }
                }
            }

            /// (hitObject.tag != "Ground")
            else
            {

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
                            foreach (var unit in units) { unit.AgentAttackObject(unitObj, unit.GetAttackingPositionAtObject(unitObj), true); }
                        }
                    }
                }

                // Right clicking on a building
                else if (buildingObj) {

                    // Enemy building
                    if (buildingObj.Team != GameManager.Team.Defending) {
                        // If there are individually selected units
                        if (units.Count > 0) {

                            // Loop through all selected units & perform ATTACK command on the building
                            foreach (var unit in units) { unit.AgentAttackObject(buildingObj, unit.GetAttackingPositionAtObject(buildingObj), true); }
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Get ai instances from all selected units.
    /// </summary>
    /// <param name="squadsSelected"></param>
    /// <param name="unitsSelected"></param>
    private void GetAISelectedFromAllSelected(ref List<Unit> unitsSelected)
    {
        // Cast selected objects to AI objects
        foreach (var obj in _PlayerAttached.SelectedWorldObjects)
        {            
            // Checking for individual units
            Unit unit = obj.GetComponent<Unit>();
            if (unit != null)
            {
                // Same team check
                if (unit.Team == _PlayerAttached.Team) { unitsSelected.Add(unit); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates sphere collider on raycast point.
    /// </summary>
    private void CreateSelection()
    {
        //If the selection window is not currently active.
        if (!_PlayerAttached._HUD.WheelActive())
        {
            //If A is pressed.
            if (_Gamepad.GetButtonDown(GamepadSchemeManager.Instance.GetActiveScheme().Selection))
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit hit;
                //Fire raycast from middle of the screen.
                if(Physics.Raycast(ray,out hit, 1000,_PlayerAttached._HUD.MaskBlock))
                {
                    _SphereReference = Instantiate(SphereSelectorObject, hit.point, new Quaternion());
                    _SphereReference.transform.localScale = new Vector3(SphereStartRadius, SphereStartRadius, SphereStartRadius);
                }
            }
            //Increase size of sphere while button is held down.
            if (_Gamepad.GetButton(GamepadSchemeManager.Instance.GetActiveScheme().Selection))
            {
                if(_SphereReference != null)
                {
                    if (_SphereReference.transform.localScale.x < MaxSphereRadius && _SphereReference.transform.localScale.y < MaxSphereRadius)
                        _SphereReference.transform.localScale += _SphereReference.transform.localScale * Time.deltaTime * SphereGrowRate;
                }
            }
        }
        //Destroy the sphere when the button is brought up.
        if (_Gamepad.GetButtonUp(GamepadSchemeManager.Instance.GetActiveScheme().Selection))
        {
            Destroy(_SphereReference);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Rotates camera.
    /// </summary>
    private void RotateCamera() {

        Vector3 rotOrigin = _PlayerAttached.CameraAttached.transform.parent.eulerAngles;

        bool pressed = false;

        // Rotate camera state if RIGHT THUMBSTICK is moving along X axis
        if (GetRightThumbstickXaxis() != 0) {

            // Hide mouse cursor
            Cursor.visible = false;

            // Calculate which direction to rotate in
            float dir = 0f;
            dir = GetRightThumbstickXaxis();

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
        if (GetRightThumbstickYaxis() > 0) {

            // Zooming in
            if (fov > Settings.MinFov)
                _PlayerAttached.CameraAttached.fieldOfView -= Time.deltaTime * Settings.ZoomSpeed;
        }
        if (GetRightThumbstickYaxis() < 0) {

            // Zooming out
            if (fov < Settings.MaxFov)
                _PlayerAttached.CameraAttached.fieldOfView += Time.deltaTime * Settings.ZoomSpeed;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void PointClickActivity() {

        if      (GetButtonAClicked())   { SelectInput(); }
        else if (GetButtonXClicked())   { CommandInput(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void SelectInput() { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void CommandInput() { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Checks for input and adds/replaces/selects units from the assigned 1-9 keybindings
    /// </summary>
    private void PlatoonInput() {

        int iter = 0;
        int keypad = 257;
        int alpha = 49;

        // There are 9 platoons
        for (int i = 0; i < 9; i++) {

            // Scroll through the platoon iterator
            if (GetDpadRightClicked()) {

                if (_PlatoonIteratorSelected < 9) { _PlatoonIteratorSelected++; }
                else { _PlatoonIteratorSelected = 1; }
            }
            else if (GetDpadLeftClicked()) {

                if (_PlatoonIteratorSelected > 0) { _PlatoonIteratorSelected--; }
                else { _PlatoonIteratorSelected = 9; }
            }

            // On DPAD PRESS
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) {

                // Make the entire platoon block glow
                _PlayerAttached.GetPlatoon(iter).LightUpBlock();
            }

            // There is at least 1 unit in the platoon
            if (_PlayerAttached.GetPlatoon(iter).GetAi().Count > 0) {

                // Make the platoon counter glow
                _PlayerAttached.GetPlatoon(iter).LightUpCounter();
            }

            // Not pressing ANY TRIGGER
            if (!OnLeftTrigger() && !OnRightTrigger() && _PlatoonIteratorSelected != iter) {

                // No units in the platoon
                if (_PlayerAttached.GetPlatoon(iter).GetAi().Count == 0) {

                    // Unglow the platoon counter
                    _PlayerAttached.GetPlatoon(iter).LightDownCounter();
                }
            }

            // Add to platoon (LEFT TRIGGER ONLY)
            if (_PlatoonIteratorSelected != 0 && (OnLeftTrigger())) {

                // Get lists of AIs that are selected
                List<Unit> UnitsSelected = new List<Unit>();

                GetAISelectedFromAllSelected(ref UnitsSelected);

                // Add any units selected to platoon
                for (int j = 0; j < UnitsSelected.Count; j++) {

                    // Remove it from any other existing platoons
                    _PlayerAttached.RemoveUnitFromAllPlatoons(UnitsSelected[j]);

                    // Dont re-add the same unit
                    if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[j])) {

                        // Safe to add
                        _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[j]);
                    }
                }
            }

            // Replace platoon (RIGHT TRIGGER ONLY)
            if (_PlatoonIteratorSelected != 0 && (OnRightTrigger())) {

                // Get lists of AIs that are selected
                List<Unit> UnitsSelected = new List<Unit>();

                GetAISelectedFromAllSelected(ref UnitsSelected);

                // Clear platoon
                _PlayerAttached.GetPlatoon(iter).Wipe();

                // Add any units selected to platoon
                for (int j = 0; j < UnitsSelected.Count; j++) {

                    // Remove it from any other existing platoons
                    _PlayerAttached.RemoveUnitFromAllPlatoons(UnitsSelected[j]);

                    // Dont re-add the same unit
                    if (!_PlayerAttached.GetPlatoon(iter).GetAi().Contains(UnitsSelected[j])) {

                        // Safe to add
                        _PlayerAttached.GetPlatoon(iter).GetAi().Add(UnitsSelected[j]);
                    }
                }
            }

            // Select platoon (DPAD ONLY (but using iterator to determine which one to select))
            if (_PlatoonIteratorSelected != 0) {

                _PlayerAttached.DeselectAllObjects();

                // Make the block & hotkey glow
                _PlayerAttached.UnselectAllPlatoons();
                _PlayerAttached.GetPlatoon(_PlatoonIteratorSelected).SetSelected(true);

                // Loop through & select all units in the platoon
                foreach (var ai in _PlayerAttached.GetPlatoon(_PlatoonIteratorSelected).GetAi()) {

                    _PlayerAttached.SelectedWorldObjects.Add(ai);
                    _PlayerAttached.SelectedUnits.Add(ai);
                    ai.SetPlayer(_PlayerAttached);
                    ai.SetIsSelected(true);
                }
            }

            ++iter;
            ++keypad;
            ++alpha;
        }

        // Update units selected panels
        GameManager.Instance.SelectedUnitsHUD.RefreshPanels();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /*
     *  XBOX Special buttons
     */

    /// <summary>
    //  Returns if the ANY XBOX buttons/axis's were pressed/changed
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetAnyButton() {

        bool result = false;

        // Special buttons
        if (GetGuideButtonClicked()) { result = true; }
        if (GetStartButtonClicked()) { result = true; }
        if (GetBackButtonClicked()) { result = true; }

        // Bumper & triggers
        if (GetLeftShoulderClicked()) { result = true; }
        if (GetRightShoulderClicked()) { result = true; }
        if (OnLeftTrigger()) { result = true; }
        if (OnRightTrigger()) { result = true; }

        // Face buttons
        if (GetButtonAClicked()) { result = true; }
        if (GetButtonBClicked()) { result = true; }
        if (GetButtonXClicked()) { result = true; }
        if (GetButtonYClicked()) { result = true; }

        // D-pad buttons
        if (GetDpadDownClicked()) { result = true; }
        if (GetDpadLeftClicked()) { result = true; }
        if (GetDpadRightClicked()) { result = true; }
        if (GetDpadUpClicked()) { result = true; }

        // Thumbsticks
        if (OnLeftThumbstickDown()) { result = true; }
        if (OnLeftThumbstickLeft()) { result = true; }
        if (OnLeftThumbstickRight()) { result = true; }
        if (OnLeftThumbstickUp()) { result = true; }
        if (OnRightThumbstickDown()) { result = true; }
        if (OnRightThumbstickLeft()) { result = true; }
        if (OnRightThumbstickRight()) { result = true; }
        if (OnRightThumbstickUp()) { result = true; }

        return result;
    }

    /// <summary>
    // Keeps the cheeky angle between 0 and 360.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns>
    //  float
    /// </returns>
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;

        if(angle < 0)
        {
            angle += 360;
        }

        return angle;
    }

    /// <summary>
    //  Returns if the XBOX GUIDE button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetGuideButtonClicked() { return (_PreviousGamepadState.Buttons.Guide == ButtonState.Released && _GamepadState.Buttons.Guide == ButtonState.Pressed); }
    
    /// <summary>
    //  Returns if the BACK button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetBackButtonClicked() { return (_PreviousGamepadState.Buttons.Back == ButtonState.Released && _GamepadState.Buttons.Back == ButtonState.Pressed); }

    /// <summary>
    //  Returns if the START button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetStartButtonClicked() { return (_PreviousGamepadState.Buttons.Start == ButtonState.Released && _GamepadState.Buttons.Start == ButtonState.Pressed); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /*
     *  XBOX Shoulder/bumper buttons
     */

    /// Returns if the LEFT SHOULDER button was pressed then released in the last 2 frames

    /// <summary>
    //  Returns if the XBOX GUIDE button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetLeftShoulderClicked() { return (_PreviousGamepadState.Buttons.LeftShoulder == ButtonState.Released && _GamepadState.Buttons.LeftShoulder == ButtonState.Pressed); }

    /// Returns if the RIGHT SHOULDER button was pressed then released in the last 2 frames

    /// <summary>
    //  Returns if the XBOX GUIDE button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetRightShoulderClicked() { return (_PreviousGamepadState.Buttons.RightShoulder == ButtonState.Released && _GamepadState.Buttons.RightShoulder == ButtonState.Pressed); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /*
     *  XBOX Face buttons
     */
    
    /// <summary>
    //  Returns if the A button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetButtonAClicked() { return (_PreviousGamepadState.Buttons.A == ButtonState.Released && _GamepadState.Buttons.A == ButtonState.Pressed); }
    
    /// <summary>
    //  Returns if the A button was released in during this frame
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetButtonAReleased() { return (_GamepadState.Buttons.A == ButtonState.Released); }
    
    /// <summary>
    //  Returns if the B button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetButtonBClicked() { return (_PreviousGamepadState.Buttons.B == ButtonState.Released && _GamepadState.Buttons.B == ButtonState.Pressed); }
    
    /// <summary>
    //  Returns if the B button was released in during this frame
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetButtonBReleased() { return (_GamepadState.Buttons.B == ButtonState.Released); }
    
    /// <summary>
    //  Returns if the X button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetButtonXClicked() { return (_PreviousGamepadState.Buttons.X == ButtonState.Released && _GamepadState.Buttons.X == ButtonState.Pressed); }
    
    /// <summary>
    //  Returns if the X button was released in during this frame
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetButtonXReleased() { return (_GamepadState.Buttons.X == ButtonState.Released); }
    
    /// <summary>
    //  Returns if the Y button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetButtonYClicked() { return (_PreviousGamepadState.Buttons.Y == ButtonState.Released && _GamepadState.Buttons.Y == ButtonState.Pressed); }
    
    /// <summary>
    //  Returns if the Y button was released in during this frame
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetButtonYReleased() { return (_GamepadState.Buttons.Y == ButtonState.Released); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /*
     *  XBOX D-PAD Buttons
     */

    /// <summary>
    //  Returns if the D-PAD UP button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetDpadUpClicked() { return (_PreviousGamepadState.DPad.Up == ButtonState.Released && _GamepadState.DPad.Up == ButtonState.Pressed); }
    
    /// <summary>
    //  Returns if the D-PAD DOWN button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetDpadDownClicked() { return (_PreviousGamepadState.DPad.Down == ButtonState.Released && _GamepadState.DPad.Down == ButtonState.Pressed); }
    
    /// <summary>
    //  Returns if the D-PAD RIGHT button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetDpadRightClicked() { return (_PreviousGamepadState.DPad.Right == ButtonState.Released && _GamepadState.DPad.Right == ButtonState.Pressed); }
    
    /// <summary>
    //  Returns if the D-PAD LEFT button was pressed then released in the last 2 frames
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetDpadLeftClicked() { return (_PreviousGamepadState.DPad.Left == ButtonState.Released && _GamepadState.DPad.Left == ButtonState.Pressed); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /*
     *  XBOX Thumbsticks
     */

    /// <summary>
    //  Returns the LEFT THUMBSTICK X axis value
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public float GetLeftThumbstickXaxis() { return _GamepadState.ThumbSticks.Left.X; }

    /// <summary>
    //  Returns the LEFT THUMBSTICK Y axis value
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public float GetLeftThumbstickYaxis() { return _GamepadState.ThumbSticks.Left.Y; }

    /// <summary>
    //  Returns the RIGHT THUMBSTICK X axis value
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public float GetRightThumbstickXaxis() { return _GamepadState.ThumbSticks.Right.X; }

    /// <summary>
    //  Returns the RIGHT THUMBSTICK Y axis value
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public float GetRightThumbstickYaxis() { return _GamepadState.ThumbSticks.Right.Y; }

    /// <summary>
    //  Returns the direction vector of the LEFT THUMBSTICK
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public Vector3 GetLeftThumbstickInput() { return new Vector3(0, 90f - (Mathf.Atan2(_GamepadState.ThumbSticks.Left.Y, _GamepadState.ThumbSticks.Left.X)) * 180 / Mathf.PI, 0); }
    
    /// <summary>
    //  Returns the direction vector of the RIGHT THUMBSTICK
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public Vector3 GetRightThumbstickInput() { return new Vector3(0, 90f - (Mathf.Atan2(_GamepadState.ThumbSticks.Right.Y, _GamepadState.ThumbSticks.Right.X)) * 180 / Mathf.PI, 0); }
    
    /// <summary>
    //  Returns boolean value if the LEFT THUMBSTICK is moving down
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnLeftThumbstickDown() { return _GamepadState.ThumbSticks.Left.Y < 0f; }

    /// <summary>
    //  Returns boolean value if the LEFT THUMBSTICK is moving up
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnLeftThumbstickUp() { return _GamepadState.ThumbSticks.Left.Y > 0f; }
    
    /// <summary>
    //  Returns boolean value if the LEFT THUMBSTICK is moving left
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnLeftThumbstickLeft() { return _GamepadState.ThumbSticks.Left.X < 0f; }
    
    /// <summary>
    //  Returns boolean value if the LEFT THUMBSTICK is moving right
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnLeftThumbstickRight() { return _GamepadState.ThumbSticks.Left.X > 0f; }

    /// <summary>
    //  Returns boolean value if the RIGHT THUMBSTICK is moving down
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnRightThumbstickDown() { return _GamepadState.ThumbSticks.Right.Y < 0f; }

    /// <summary>
    //  Returns boolean value if the RIGHT THUMBSTICK is moving up
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnRightThumbstickUp() { return _GamepadState.ThumbSticks.Right.Y > 0f; }

    /// <summary>
    //  Returns boolean value if the RIGHT THUMBSTICK is moving left
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnRightThumbstickLeft() { return _GamepadState.ThumbSticks.Right.X < 0f; }

    /// <summary>
    //  Returns boolean value if the RIGHT THUMBSTICK is moving right
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnRightThumbstickRight() { return _GamepadState.ThumbSticks.Right.X > 0f; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /*
     *  XBOX Triggers
     */

    /// <summary>
    //  Returns the LEFT TRIGGER axis value
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public float GetLeftTriggeraxis() { return _GamepadState.Triggers.Left; }
    
    /// <summary>
    //  Returns the RIGHT TRIGGER axis value
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public float GetRightTriggeraxis() { return _GamepadState.Triggers.Right; }
    
    /// <summary>
    //  Returns boolean value if the LEFT TRIGGER is moving
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnLeftTrigger() { return _GamepadState.Triggers.Left > 0f; }
    
    /// <summary>
    //  Returns boolean value if the RIGHT TRIGGER is moving
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool OnRightTrigger() { return _GamepadState.Triggers.Right > 0f; }
    
}