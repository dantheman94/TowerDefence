using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//-=-=-=-=-=-=-=-=-=
// Created by: Angus Secomb
// Last modified: 23/10/18
// Editor: Angus

public class InputChecker : MonoBehaviour {


    public Button ReferenceButton;
    

    // VARIABLES
    //////////////////////////

    public static string CurrentController;
    private xb_gamepad _Gamepad;
    private Vector3 _MousePosition;

	// Use this for initialization
	void Start () {
        
        _Gamepad = GamepadManager.Instance.GetGamepad(1);
      if(_Gamepad.IsConnected)
        {
            CurrentController = "Controller";
            _MousePosition = Input.mousePosition;
        }
      else
        {
            CurrentController = "Keyboard";
        }
	}
	
    void GetActiveInputDevice()
    {
        if(CurrentController == "Keyboard")
        {
            if (_Gamepad.IsConnected)
            {
                if (_Gamepad.GetAnyButton() || _Gamepad.GetStick_L().X != 0)
                {
                    CurrentController = "Controller";
                    _MousePosition = Input.mousePosition;
                    if(ReferenceButton != null)
                    ReferenceButton.Select();
                    
                }
            }
        }
        else if(CurrentController == "Controller")
        {
            
            if(Input.GetMouseButton(0) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Return) || _MousePosition != Input.mousePosition)
            {
                CurrentController = "Keyboard";
            }
        }
    }

	// Update is called once per frame
	void Update () {
        GetActiveInputDevice();
	}
}
