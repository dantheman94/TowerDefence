//|================================|
//| Created by Angus Secomb.       |
//| Last modified: 6/06/2018.      |
//|================================|
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

//stores button press states.
public struct xButton
{
    public ButtonState previousState;
    public ButtonState state;
}

//Stores triggers state.
public struct TriggerState
{
    //Triggers previous value
    public float prevValue;
    //triggers current value.
    public float currValue;
}

//Rumble (vibration) event
public struct xRumble
{
    public float timer; // Rumble timer.
    public float duration; // Fade-out (in seconds).
    public Vector2 power; //Intensity of power.

    public void Update()
    {
        this.timer -= Time.deltaTime;
    }
}

public class xb_gamepad {

    private GamePadState prevState; //previous gamepad state.
    private GamePadState state; //current gamepad state.

    private int gamepadIndex; //controllers numeric index.
    private PlayerIndex playerIndex; //"Player" index.
    private List<xRumble> rumbleEvents; //stores the rumble events.

    //Button input map.
    private Dictionary<string, xButton> inputMap;

    //States for all controller buttons/inputs.
    private xButton A, X, B, Y;
    private xButton dPadUP, dPadDown, dPadLeft, dPadRight; //dpad directions.

    private xButton guide; //Xbox logo button
    private xButton back, start;
    private xButton L3, R3; //Thumbstick push buttons.
    private xButton LB, RB; //shoulder buttons.
    private TriggerState LT, RT; //triggers


    //constructor.
    public xb_gamepad(int controllerIndex)
    {
        //set gamepad index.
        gamepadIndex = controllerIndex - 1;
        playerIndex = (PlayerIndex)gamepadIndex;

        //Create xRumble container and input map.
        rumbleEvents = new List<xRumble>();
        inputMap = new Dictionary<string, xButton>();
    }
	
	// Update is called once per frame
	public void Update () {

        //get current state.
        state = GamePad.GetState(playerIndex);

        //check if gamepad is connected
        if(state.IsConnected)
        {
            A.state = state.Buttons.A;
            B.state = state.Buttons.B;
            X.state = state.Buttons.X;
            Y.state = state.Buttons.Y;

            dPadDown.state = state.DPad.Down;
            dPadLeft.state = state.DPad.Left;
            dPadUP.state = state.DPad.Up;
            dPadRight.state = state.DPad.Right;

            guide.state = state.Buttons.Guide;
            back.state = state.Buttons.Back;
            start.state = state.Buttons.Start;
            L3.state = state.Buttons.LeftStick;
            R3.state = state.Buttons.RightStick;
            LB.state = state.Buttons.LeftShoulder;
            RB.state = state.Buttons.RightShoulder;

            //read trigger values into the states.
            LT.currValue = state.Triggers.Left;
            RT.currValue = state.Triggers.Right;

            UpdateInputMap();
            HandleRumble();
        }
	}

    public void Refresh()
    {
        //stores current state for next update.
        prevState = state;

        //if controller is connected.
        if(state.IsConnected)
        {
            A.previousState = prevState.Buttons.A;
            B.previousState = prevState.Buttons.B;
            X.previousState = prevState.Buttons.X;
            Y.previousState = prevState.Buttons.Y;

            dPadUP.previousState = prevState.DPad.Up;
            dPadDown.previousState = prevState.DPad.Down;
            dPadLeft.previousState = prevState.DPad.Left;
            dPadRight.previousState = prevState.DPad.Right;

            guide.previousState = prevState.Buttons.Guide;
            back.previousState = prevState.Buttons.Back;
            start.previousState = prevState.Buttons.Start;
            L3.previousState = prevState.Buttons.LeftStick;
            R3.previousState = prevState.Buttons.RightStick;
            LB.previousState = prevState.Buttons.LeftShoulder;
            RB.previousState = prevState.Buttons.RightShoulder;

            //read trigger values into the states.
            LT.prevValue = prevState.Triggers.Left;
            RT.prevValue = prevState.Triggers.Right;

            UpdateInputMap();
        }
    }

    void UpdateInputMap()
    {
      
        inputMap["A"] = A;
        inputMap["B"] = B;
        inputMap["X"] = X;
        inputMap["Y"] = Y;

        inputMap["dPadUp"] = dPadUP;
        inputMap["dPadDown"] = dPadDown;
        inputMap["dPadLeft"] = dPadLeft;
        inputMap["dPadRight"] = dPadRight;

        inputMap["start"] = start;
        inputMap["back"] = back;
        inputMap["guide"] = guide;

        //thumbstick buttons
        inputMap["L3"] = L3;
        inputMap["R3"] = R3;

        //shoulder buttons
        inputMap["LB"] = LB;
        inputMap["RB"] = RB;
    }

    void HandleRumble()
    {
        //Ignore if there are no events.
        if (rumbleEvents.Count > 0)
        {
            Vector2 currentPower = new Vector2(0, 0);
            for (int i = 0; i < rumbleEvents.Count; ++i)
            {
                // Update current event
                rumbleEvents[i].Update();

                if (rumbleEvents[i].timer > 0)
                {
                    // Calculate current power
                    float timeLeft = Mathf.Clamp(rumbleEvents[i].timer / rumbleEvents[i].duration, 0f, 1f);
                    currentPower = new Vector2(Mathf.Max(rumbleEvents[i].power.x * timeLeft, currentPower.x),
                                               Mathf.Max(rumbleEvents[i].power.y * timeLeft, currentPower.y));

                    GamePad.SetVibration(playerIndex, currentPower.x, currentPower.y);
                }
                else
                {
                    // Cancel out any phantom vibration
                    GamePad.SetVibration(playerIndex, 0.0f, 0.0f);

                    // Remove expired event
                    rumbleEvents.Remove(rumbleEvents[i]);
                }
            }
        }
    }

    void AddRumble(float timer, Vector2 power, float duration)
    {
        xRumble rumble = new xRumble();

        rumble.timer = timer;
        rumble.power = power;
        rumble.duration = duration;

        rumbleEvents.Add(rumble);
    }
    //returns gamepad index.
    public int Index { get { return gamepadIndex; } }

    //returns gamepad connection state.
    public bool IsConnected { get { return state.IsConnected; } }

    //returns axis of left thumbstick.
    public GamePadThumbSticks.StickValue GetStick_L()
    {
        return state.ThumbSticks.Left;
    }

    //returns axis of right thumb stick.
    public GamePadThumbSticks.StickValue GetStick_R()
    {
        return state.ThumbSticks.Right;
    }

    //returns true if left stick has any sort of input based on deadzone (value the stick ignores)
    public bool IsStickDown_L(float deadZone)
    {
        if (state.ThumbSticks.Left.X > deadZone || state.ThumbSticks.Left.Y > deadZone
            || state.ThumbSticks.Left.X < -deadZone || state.ThumbSticks.Left.Y < -deadZone)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
   
    //Returns true if right stick has any sort of input based on deadzone (value the stick ignores)
    public bool IsStickDown_R(float deadZone)
    {
        if (state.ThumbSticks.Right.X > deadZone || state.ThumbSticks.Right.Y > deadZone
            || state.ThumbSticks.Right.X < -deadZone || state.ThumbSticks.Right.Y < -deadZone)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    //returns int based on if stick is in a negative or positive value
    public int IsStickForward_L(float deadZone)
    {
        if(state.ThumbSticks.Left.X > deadZone && state.ThumbSticks.Left.Y > deadZone)
        {
            return 1;
        }
        else if(state.ThumbSticks.Left.X < -deadZone && state.ThumbSticks.Left.Y < -deadZone)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    //returns value of left trigger
    public float GetTrigger_L() { return state.Triggers.Left; }

    //returns value of right trigger.
    public float GetTrigger_R() { return state.Triggers.Right; }

    public bool GetTriggerDown_R()
    {
        if (IsConnected)
            return (RT.currValue >= 0.1) ? true : false;
        else
            return false;
    }

    public bool GetTriggerDown_L()
    {
        if (IsConnected)
            return (LT.currValue >= 0.1) ? true : false;
        else
            return false;
    }


    //Returns true if trigger went from released to pressed.
    public bool GetTriggerTap_L()
    {
        if (IsConnected)
            return (LT.prevValue == 0f && LT.currValue >= 0.1f) ? true : false;
        else
            return false;
    }

    public bool GetTriggerTap_R()
    {
        if (IsConnected)
            return (RT.prevValue == 0f && RT.currValue >= 0.1f) ? true : false;
        else
            return false;
    }

    //Returns true if button is pressed.
    public bool GetButton(string button)
    {
        if (IsConnected)
        {

            return inputMap[button].state == ButtonState.Pressed ? true : false;
        }
        else
        return false;
    }

    //Returns true if button goes from released to pressed.
    public bool GetButtonDown(string button)
    {
        if(IsConnected)
        {
            return (inputMap[button].previousState == ButtonState.Released &&
            inputMap[button].state == ButtonState.Pressed) ? true : false;
        }
        else
        return false;
    }


    //Returns true if button goes from pressed to released.
    public bool GetButtonUp(string button)
    {
        if(IsConnected)
        {
            return (inputMap[button].previousState == ButtonState.Pressed &&
              inputMap[button].state == ButtonState.Released) ? true : false;
        }
        else
        return false;
      
    }
}
