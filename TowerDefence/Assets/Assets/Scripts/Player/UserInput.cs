using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/10/2018
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

    private void Start() {

        // Get component references
        _Player = GetComponent<Player>();
        if (_Player) { _PlayerIndex = _Player._PlayerIndex; }
        
        // Initialize center point for LookAt() function
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
        _LookPoint = hit.point;
    }

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
        }
    }

    private void FixedUpdate() {

        if (_ControllerIsRumbling) {

            // Start controller rumble
            GamePad.SetVibration(_PlayerIndex, _MotorLeft, _MotorRight);

            // Timer
            _TimerRumble += Time.deltaTime;
            if (_TimerRumble >= _RumbleTime) { _ControllerIsRumbling = false; }
        }

        else {

            // Stop controller rumble
            GamePad.SetVibration(_PlayerIndex, 0f, 0f);
            _TimerRumble = 0f;
        }
    }

    public void StartRumble(float motorLeft, float motorRight, float time) {

        // Set rumble properties
        _MotorLeft = motorLeft;
        _MotorRight = motorRight;
        _RumbleTime = time;

        // Start rumble
        _ControllerIsRumbling = true;
    }

    private void MoveCamera() {

        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);

        // Keyboard movement WASD
        if (Input.GetKey(KeyCode.W))
            movement.y += Settings.MovementSpeed;
        if (Input.GetKey(KeyCode.S))
            movement.y -= Settings.MovementSpeed;

        if (Input.GetKey(KeyCode.A) && (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))) {

            movement.x -= Settings.MovementSpeed;

            // Update center point for LookAt() function
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
            _LookPoint = hit.point;
        }

        if (Input.GetKey(KeyCode.D) && (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))) {

            movement.x += Settings.MovementSpeed;

            // Update center point for LookAt() function
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
            _LookPoint = hit.point;
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

            // Zoomming in
            if (fov > Settings.MinFov)
                Camera.main.fieldOfView -= Time.deltaTime * Settings.ZoomSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {

            // Zoomming out
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

    private void RotateCamera() {

        Vector3 rotOrigin = Camera.main.transform.eulerAngles;
        Vector3 rotDestination = rotOrigin;

        // Detect rotation amount if ALT is being held and the Right mouse button is down
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) {

            rotDestination.y += Input.GetAxis("Mouse X") * Settings.RotateSpeed;
            ///Camera.main.transform.LookAt(_LookPoint, Vector3.up);
        }

        // If a change in position is detected perform the necessary update
        if (rotDestination != rotOrigin)
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(rotOrigin, rotDestination, Time.deltaTime * Settings.RotateSpeed);
    }

    private void MouseActivity() {

        if (Input.GetMouseButtonDown(0))
            LeftMouseClick();

        else if (Input.GetMouseButtonDown(1))
            RightMouseClick();
    }

    private void LeftMouseClick() {

        if (_Player._HUD.MouseInBounds() && !_Player._HUD.WheelActive()) {

            // Precautions
            GameObject hitObject = _Player._HUD.FindHitObject();
            Vector3 hitPoint = _Player._HUD.FindHitPoint();
            if (hitObject && hitPoint != Settings.InvalidPosition) {
                
                if (hitObject.name != "Ground") {

                    // Not holding LEFT CONTROL
                    if (!Input.GetKey(KeyCode.LeftControl)) {

                        // Deselect any objects that are currently selected
                        foreach (var obj in _Player.SelectedWorldObjects) { obj.SetSelection(false); }
                        _Player.SelectedWorldObjects.Clear();

                        if (_Player.SelectedBuildingSlot != null) {

                            _Player.SelectedBuildingSlot.SetSelection(false);
                            _Player.SelectedBuildingSlot = null;
                        }
                    }

                    // Cast hit object to selectable objects
                    Building buildingObj = hitObject.transform.root.GetComponent<Building>();
                    BuildingSlot buildingSlot = hitObject.transform.root.GetComponent<BuildingSlot>();
                    WorldObject worldObj = hitObject.transform.root.GetComponent<WorldObject>();

                    // Building object
                    if (buildingObj) {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(worldObj);
                        buildingObj.SetPlayer(_Player);
                        buildingObj.SetSelection(true);
                        buildingObj.OnSelectionWheel();
                    }

                    // Building slot
                    else if (buildingSlot) {

                        // Empty building slot
                        if (buildingSlot.getBuildingOnSlot() == null) {

                            _Player.SelectedBuildingSlot = buildingSlot;
                            buildingSlot.SetPlayer(_Player);
                            buildingSlot.SetSelection(true);
                        }

                        // Builded slot
                        else {

                            // Add selection to list
                            _Player.SelectedWorldObjects.Add(buildingSlot.getBuildingOnSlot());
                            buildingSlot.getBuildingOnSlot().SetPlayer(_Player);
                            buildingSlot.getBuildingOnSlot().SetSelection(true);
                            buildingSlot.getBuildingOnSlot().OnSelectionWheel();
                        }
                    }

                    // World object
                    else if (worldObj) {

                        // Add selection to list
                        _Player.SelectedWorldObjects.Add(worldObj);
                        worldObj.SetPlayer(_Player);
                        worldObj.SetSelection(true);
                    }
                }
            }
        }
    }

    private void RightMouseClick() {
                
    }

    private void AbilitiesInput() {

        // On user input
        if (Input.GetKeyDown(KeyCode.F)) {

            // Deselect any objects that are currently selected
            foreach (var obj in _Player.SelectedWorldObjects) { obj.SetSelection(false); }
            _Player.SelectedWorldObjects.Clear();

            // Show/hide abilies wheel
            _Player._HUD.SetAbilitiesWheel(!_Player._HUD.AbilitiesWheel.activeInHierarchy);
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
