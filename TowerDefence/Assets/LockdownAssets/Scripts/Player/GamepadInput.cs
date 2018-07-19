using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/7/2018
//
//******************************

public class GamepadInput : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _PlayerAttached;
    private KeyboardInput _KeyboardInputManager = null;
    public bool _IsPrimaryController { get; set; }

    private Vector3 _LookPoint;
    private Vector3 _CurrentVelocity = Vector3.zero;

    private GamePadState _GamepadState;
    private GamePadState _PreviousGamepadState;
    private bool _ControllerIsRumbling = false;
    private float _TimerRumble, _RumbleTime = 0f;
    private float _MotorLeft, _MotorRight = 0f;

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
        _PlayerAttached = GetComponent<Player>();
        _KeyboardInputManager = GetComponent<KeyboardInput>();

        // Initialize center point for LookAt() function
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
        _LookPoint = hit.point;

        _IsPrimaryController = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {

        if (_PlayerAttached) {

            // Update primary controller
            if (GetAnyButton()) {

                // Disable keyboard / Enable gamepad
                _IsPrimaryController = true;
                if (_KeyboardInputManager != null) { _KeyboardInputManager._IsPrimaryController = false; }
            }
            
            // Update gamepad states
            _PreviousGamepadState = _GamepadState;
            _GamepadState = GamePad.GetState(_PlayerAttached.Index);
            
            Debug.Log("GAMEPAD IS PRIMARY: " + _IsPrimaryController);
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
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward * 1000, out hit);
        _LookPoint = hit.point;
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

    // XBOX ONE Shoulder / Bumper buttons

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