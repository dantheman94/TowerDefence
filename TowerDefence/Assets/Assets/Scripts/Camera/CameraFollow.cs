using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class CameraFollow : MonoBehaviour {

    //***************************************************************
    // VARIABLES

    public PlayerIndex _PlayerIndex;
    public float _xOffset = 0f;
    public float _yOffset = 140f;
    public float _zOffset = 110f;

    private GroundVehicle _TargetFollower;
    private Camera _Camera;
    private int _LocalTier = 1;

    //***************************************************************
    // FUNCTIONS    
    
    private void Start () {

        // Get component references
        _Camera = GetComponent<Camera>();

        // Get initial gameObject to follow (relative to the player)
        _TargetFollower = VehicleManager._Instance._StartingVehicles[(int)_PlayerIndex];
    }
	
	private void Update () {

        // Set camera's position relative to what object the player is currently controlling
        if (_TargetFollower = VehicleManager._Instance._StartingVehicles[(int)_PlayerIndex]) {

            // Set camera position
            _Camera.transform.position = new Vector3(_TargetFollower.transform.position.x - _xOffset, _TargetFollower.transform.position.y + _yOffset, _TargetFollower.transform.position.z - _zOffset);

            if (_LocalTier != _TargetFollower.GetTier().iTier) {

                _LocalTier = _TargetFollower.GetTier().iTier;
                CameraFov();
            }
        }
    }

    public void CameraFov() {

        // Set camera fov based on the object's tier
        int fovAdd = _TargetFollower.GetTier().iTier * 1;
        _Camera.fieldOfView = _Camera.fieldOfView + fovAdd;
    }
}
