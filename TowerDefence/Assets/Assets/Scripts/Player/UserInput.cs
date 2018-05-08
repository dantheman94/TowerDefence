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
//  Last edited on: 5/8/2018
//
//******************************

public class UserInput : MonoBehaviour {

    //******************************************************************************************************************************
    // VARIABLES

    private Player _Player;
    public PlayerIndex _PlayerIndex = PlayerIndex.One;

    // Gamepad
    private GamePadState _GamepadState;
    private GamePadState _PreviousGamepadState;
    private bool _ControllerIsRumbling = false;
    private float _TimerRumble, _RumbleTime = 0f;
    private float _MotorLeft, _MotorRight = 0f;

    //******************************************************************************************************************************
    // FUNCTIONS

    private void Start() {

        // Get component references
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update() {

        if (_Player) {

            // Update camera
            MoveCameraMouse();
            RotateCamera();

            // Update gamepad states
            _PreviousGamepadState = _GamepadState;
            _GamepadState = GamePad.GetState(_PlayerIndex);
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

    private void MoveCameraMouse() {

        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);

        // Keyboard movement WASD
        if (Input.GetKey(KeyCode.W))
            movement.y += ResourceManager.MovementSpeed;
        if (Input.GetKey(KeyCode.S))
            movement.y -= ResourceManager.MovementSpeed;
        if (Input.GetKey(KeyCode.A))
            movement.x -= ResourceManager.MovementSpeed;
        if (Input.GetKey(KeyCode.D))
            movement.x += ResourceManager.MovementSpeed;

        // Horizontal camera movement via mouse
        if (xPos >= 0 && xPos < ResourceManager.ScreenOffset)
            movement.x -= ResourceManager.MovementSpeed;
        else if (xPos <= Screen.width && xPos > Screen.width - ResourceManager.ScreenOffset)
            movement.x += ResourceManager.MovementSpeed;

        // Vertical camera movement via mouse
        if (yPos >= 0 && yPos < ResourceManager.ScreenOffset)
            movement.z -= ResourceManager.MovementSpeed;
        else if (yPos <= Screen.height && yPos > Screen.height - ResourceManager.ScreenOffset)
            movement.z += ResourceManager.MovementSpeed;

        // Make sure movement is in the direction the camera is pointing
        // but ignore the vertical tilt of the camera to get sensible scrolling
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        // Away from ground
        ///movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

        // Change camera facing angle
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {

            // Zoom in
            Vector3 rotOrigin = Camera.main.transform.rotation.eulerAngles;
            Vector3 rotDestination = rotOrigin;

            if (Camera.main.transform.rotation.x <= ResourceManager.MinRotationAngle)
                rotDestination.x -= ResourceManager.RotateZoomSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {

            // Zoom out
            Vector3 rotOrigin = Camera.main.transform.rotation.eulerAngles;
            Vector3 rotDestination = rotOrigin;

            if (Camera.main.transform.rotation.x >= ResourceManager.MaxRotationAngle)
                rotDestination.x += ResourceManager.RotateZoomSpeed;

            // Update zoom
            if (rotDestination != rotOrigin)
                Camera.main.transform.eulerAngles = Vector3.MoveTowards(rotOrigin, rotDestination, Time.deltaTime * ResourceManager.RotateZoomSpeed);
        }

        // Calculate desired camera position based on received input
        Vector3 posOrigin = Camera.main.transform.position;
        Vector3 posDestination = posOrigin;
        posDestination.x += movement.x;
        posDestination.y += movement.y;
        posDestination.z += movement.z;

        // Limit away from ground movement to be between a minimum and maximum distance
        if (posDestination.y > ResourceManager.MaxCameraHeight)
            posDestination.y = ResourceManager.MaxCameraHeight;

        else if (posDestination.y < ResourceManager.MinCameraHeight)
            posDestination.y = ResourceManager.MinCameraHeight;

        // If a change in position is detected perform the necessary update
        if (posDestination != posOrigin)
            Camera.main.transform.position = Vector3.MoveTowards(posOrigin, posDestination, Time.deltaTime * ResourceManager.MovementSpeed);
    }

    private void RotateCamera() {

        Vector3 rotOrigin = Camera.main.transform.eulerAngles;
        Vector3 rotDestination = rotOrigin;

        // Detect rotation amount if ALT is being held and the Right mouse button is down
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
            rotDestination.y += Input.GetAxis("Mouse X") * ResourceManager.RotateSpeed;

        // If a change in position is detected perform the necessary update
        if (rotDestination != rotOrigin)
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(rotOrigin, rotDestination, Time.deltaTime * ResourceManager.RotateSpeed);

    }

    private void MouseActivity() {

        if (Input.GetMouseButtonDown(0))
            LeftMouseClick();

        else if (Input.GetMouseButtonDown(1))
            RightMouseClick();
    }

    private void LeftMouseClick() {

        if (_Player._HUD.MouseInBounds()) {

            GameObject hitObject = _Player._HUD.FindHitObject();
            Vector3 hitPoint = _Player._HUD.FindHitPoint();

            if (hitObject && hitPoint != ResourceManager.InvalidPosition) {

                if (_Player.SelectedObject)
                    _Player.SelectedObject.MouseClick(hitObject, hitPoint, _Player);

                else if (hitObject.name != "Ground") {

                    WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();
                    if (worldObject) {

                        // We already know the player has no selected object
                        _Player.SelectedObject = worldObject;
                        worldObject.SetSelection(true, _Player._HUD.GetPlayingArea());
                    }
                }
            }
        }
    }

    private void RightMouseClick() {

        if (_Player._HUD.MouseInBounds() && !Input.GetKey(KeyCode.LeftAlt) && _Player.SelectedObject) {

            _Player.SelectedObject.SetSelection(false, _Player._HUD.GetPlayingArea());
            _Player.SelectedObject = null;
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
