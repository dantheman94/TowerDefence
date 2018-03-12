using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class PlayerEntity : MonoBehaviour {

    //***************************************************************
    // INSPECTOR

    public PlayerIndex _PlayerIndex = PlayerIndex.One;

    //***************************************************************
    // VARIABLES

    // Gamepad
    private GamePadState _GamepadState;
    private GamePadState _PreviousGamepadState;
    private bool _ControllerIsRumbling = false;
    private float _TimerRumble, _RumbleTime = 0f;
    private float _MotorLeft,   _MotorRight = 0f;

    // Economy
    private int _TokenCount = 0;
    private int _Score = 0;
    private int _WavesSurvived = 0;

    private PossessionWheel _PossessionWheel;
    
    //***************************************************************
    // FUNCTIONS

	private void Start () {

        // Initialize new player entity
        _TokenCount = 0;
        _Score = 0;
        _WavesSurvived = 0;
    }

    private void FixedUpdate() {

        if (_ControllerIsRumbling) {

            // Start controller rumble
            GamePad.SetVibration(_PlayerIndex, _MotorLeft, _MotorRight);
        }

        else {

            // Stop controller rumble
            GamePad.SetVibration(_PlayerIndex, 0f, 0f);
            _TimerRumble = 0f;
        }
    }

    private void Update () {

        // Update gamepad states
        _PreviousGamepadState = _GamepadState;
        _GamepadState = GamePad.GetState(_PlayerIndex);

        // Vehicle wheel checks
        if (GetButtonYClicked()) {

            // Flip widget visibility
            if (VehicleManager._Instance._PossessionWheel) { _PossessionWheel = VehicleManager._Instance._PossessionWheel; }
            _PossessionWheel.SetPlayerEntity(this);
            _PossessionWheel.gameObject.SetActive(!_PossessionWheel.gameObject.activeInHierarchy);
            _PossessionWheel.SetActive(_PossessionWheel.gameObject.activeInHierarchy);
        }

        // Pause game checks
        if (GetStartButtonClicked()) {

        }

        // Shop 
        if (GetBackButtonClicked()) {

        }

    }

    //***************************************************************
    // SET & GET

    //***************************
    // Special buttons

    /// Returns if the XBOX GUIDE button was pressed then released in the last 2 frames
    public bool GetGuideButtonClicked() { return (_PreviousGamepadState.Buttons.Guide == ButtonState.Released && _GamepadState.Buttons.Guide == ButtonState.Pressed); }

    /// Returns if the BACK button was pressed then released in the last 2 frames
    public bool GetBackButtonClicked() { return (_PreviousGamepadState.Buttons.Back == ButtonState.Released && _GamepadState.Buttons.Back == ButtonState.Pressed); }

    /// Returns if the START button was pressed then released in the last 2 frames
    public bool GetStartButtonClicked() { return (_PreviousGamepadState.Buttons.Start == ButtonState.Released && _GamepadState.Buttons.Start == ButtonState.Pressed); }

    //***************************
    // Shoulder / Bumper buttons

    /// Returns if the LEFT SHOULDER button was pressed then released in the last 2 frames
    public bool GetLeftShoulderClicked() { return (_PreviousGamepadState.Buttons.LeftShoulder == ButtonState.Released && _GamepadState.Buttons.LeftShoulder == ButtonState.Pressed); }

    /// Returns if the RIGHT SHOULDER button was pressed then released in the last 2 frames
    public bool GetRightShoulderClicked() { return (_PreviousGamepadState.Buttons.RightShoulder == ButtonState.Released && _GamepadState.Buttons.RightShoulder == ButtonState.Pressed); }
    
    //***************************
    // Face buttons
    
    /// Returns if the A button was pressed then released in the last 2 frames
    public bool GetButtonAClicked() { return (_PreviousGamepadState.Buttons.A == ButtonState.Released && _GamepadState.Buttons.A == ButtonState.Pressed); }

    /// Returns if the B button was pressed then released in the last 2 frames
    public bool GetButtonBClicked() { return (_PreviousGamepadState.Buttons.B == ButtonState.Released && _GamepadState.Buttons.B == ButtonState.Pressed); }

    /// Returns if the X button was pressed then released in the last 2 frames
    public bool GetButtonXClicked() { return (_PreviousGamepadState.Buttons.X == ButtonState.Released && _GamepadState.Buttons.X == ButtonState.Pressed); }

    /// Returns if the Y button was pressed then released in the last 2 frames
    public bool GetButtonYClicked() { return (_PreviousGamepadState.Buttons.Y == ButtonState.Released && _GamepadState.Buttons.Y == ButtonState.Pressed); }

    //**************************
    // D-Pad buttons

    /// Returns if the D-PAD UP button was pressed then released in the last 2 frames
    public bool GetDpadUpClicked() { return (_PreviousGamepadState.DPad.Up == ButtonState.Released && _GamepadState.DPad.Up == ButtonState.Pressed); }

    /// Returns if the D-PAD DOWN button was pressed then released in the last 2 frames
    public bool GetDpadDownClicked() { return (_PreviousGamepadState.DPad.Down == ButtonState.Released && _GamepadState.DPad.Down == ButtonState.Pressed); }

    /// Returns if the D-PAD RIGHT button was pressed then released in the last 2 frames
    public bool GetDpadRightClicked() { return (_PreviousGamepadState.DPad.Right == ButtonState.Released && _GamepadState.DPad.Right == ButtonState.Pressed); }

    /// Returns if the D-PAD LEFT button was pressed then released in the last 2 frames
    public bool GetDpadLeftClicked() { return (_PreviousGamepadState.DPad.Left == ButtonState.Released && _GamepadState.DPad.Left == ButtonState.Pressed); }

    //**************************
    // Thumbsticks

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
    public Vector3 GetRightThumbstickInput() {  return new Vector3(0, 90f - (Mathf.Atan2(_GamepadState.ThumbSticks.Right.Y, _GamepadState.ThumbSticks.Right.X)) * 180 / Mathf.PI, 0); }

    /// Returns boolean value if the LEFT THUMBSTICK is moving down
    public bool OnLeftThumbstickDown() { return _GamepadState.ThumbSticks.Left.Y < 0f; }

    /// Returns boolean value if the LEFT THUMBSTICK is moving up
    public bool OnLeftThumbstickUp() { return _GamepadState.ThumbSticks.Left.Y > 0f; }

    /// Returns boolean value if the LEFT THUMBSTICK is moving left
    public bool OnLeftThumbstickLeft() { return _GamepadState.ThumbSticks.Left.X < 0f; }

    /// Returns boolean value if the LEFT THUMBSTICK is moving right
    public bool OnLeftThumbstickRight() { return _GamepadState.ThumbSticks.Left.X > 0f; }

    //**************************
    // Triggers

    /// Returns the LEFT TRIGGER axis value
    public float GetLeftTriggeraxis() { return _GamepadState.Triggers.Left; }

    /// Returns the RIGHT TRIGGER axis value
    public float GetRightTriggeraxis() { return _GamepadState.Triggers.Right; }

    /// Returns boolean value if the LEFT TRIGGER is moving
    public bool OnLeftTrigger() { return _GamepadState.Triggers.Left > 0f; }

    /// Returns boolean value if the RIGHT TRIGGER is moving
    public bool OnRightTrigger() { return _GamepadState.Triggers.Right > 0f; }

}