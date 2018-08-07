//|================================|
//| Created by Angus Secomb.       |
//| Last modified: 6/06/2018.      |
//|================================|
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadManager : MonoBehaviour {

    public int gamepadCount = 2; //number of gamepads to support

    private List<xb_gamepad> gamepads; //holds gamepad instances
    private static GamepadManager singleton; //All the singleton ladies.

    // Use this for initialization
    void Awake () {
		

        //Found a duplicate of the controller singleton.
        if(singleton != null && singleton != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            //create that cheeky singleton instance.
            singleton = this;
            //Allows the manager to carry across scenes.
            DontDestroyOnLoad(this.gameObject);
            //Clamp the count to be between 1 and 4 players.
            gamepadCount = Mathf.Clamp(gamepadCount, 1, 4);

            gamepads = new List<xb_gamepad>();

            //create number of gamepads based on count
            for(int i = 0; i < gamepadCount; ++i)
            {
                gamepads.Add(new xb_gamepad(i + 1));
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
        for(int i = 0; i < gamepads.Count; ++i)
        {
            gamepads[i].Update();
        }

	}

    public void Refresh()
    {
        for(int i = 0; i < gamepadCount; ++i)
        {
            gamepads[i].Refresh();
        }
    }

    public static GamepadManager Instance
    {
        get
        {
            if(singleton == null)
            {
                Debug.LogError("[GamepadManager]: Instance does not currently exist.");
                return null;
            }

            return singleton;
        }
    }

    //gets gamepad basic on index.
    public xb_gamepad GetGamepad(int index)
    {
        //Search through gamepads if the indexed value exists return it
        for(int i = 0; i < gamepads.Count;)
        {
            if(gamepads[i].Index == (index - 1))
            {
                return gamepads[i];
            }
            else
            {
                ++i;
            }
        }
        //otherwise return null as it does not exist.
        Debug.LogError("[GamepadManager]: " + index + "is not a valid gamepad index");
        return null;
    }

    //returns number of connected gamepads.
    public int ConnectedTotal()
    {
        int total = 0;

        for (int i = 0; i < gamepads.Count; ++i)
        {
            if(gamepads[i].IsConnected)
            {
                total++;
            }
        }

        return total;
    }

    //Gets desired button press from any controller.
    //(useful for menus and what not).
    public bool GetButtonAny(string button)
    {
        for(int i = 0; i < gamepads.Count; ++i)
        {
            if(gamepads[i].IsConnected && gamepads[i].GetButton(button))
            {
                return true;
            }
        }

        return false;
    }

    // Return true if any controller goes from released to pressed.
    public bool GetButtonDownAny(string button)
    {
        for (int i = 0; i < gamepads.Count; ++i)
        {
            // Gamepad meets both conditions
            if (gamepads[i].IsConnected && gamepads[i].GetButtonDown(button))
                return true;
        }

        return false;
    }
    }
