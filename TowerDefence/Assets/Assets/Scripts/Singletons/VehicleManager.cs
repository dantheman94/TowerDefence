using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class VehicleManager : MonoBehaviour {

    //***************************************************************
    // INSPECTOR
    
    [Space]
    [Header("Player")]
    public PossessionWheel _PossessionWheel;
    [Space]
    public List<VehicleBase> _StartingVehicles;

    //***************************************************************
    // VARIABLES

    public static VehicleManager _Instance;

    private List<VehicleBase> _PlayerControlledVehicles;
    private List<VehicleBase> _Vehicles;

    //***************************************************************
    // FUNCTIONS    

    private void Awake () {

        // If the singleton has already been initialized
        if (_Instance != null && _Instance != this) {

            Destroy(this.gameObject);
            return;
        }

        // Set singleton
        _Instance = this;
        
        // Create vehicle arrays
        _Vehicles = new List<VehicleBase>();
    }

    private void Start () {
        
        // Control starting vehicle gameobject
        if (_StartingVehicles.Count > 0) {

            foreach (var vehicle in _StartingVehicles) {

                vehicle.SetControllerType(VehicleBase.EControllerType.PlayerControlled);
                _Vehicles.Add(vehicle);
                _PlayerControlledVehicles = _StartingVehicles;
            }
        }
        else { /* _StartingVehicles.Count == 0 */

            Debug.LogWarning("ERROR: Starting vehicles has not been set in vehicle manager. Manager is allocating a vehicle from _Vehicles array for each player, this could have unintended results.");
            _StartingVehicles = new List<VehicleBase>();
            _StartingVehicles[0] = _Vehicles[0];
            _StartingVehicles[1] = _Vehicles[1];
            _StartingVehicles[2] = _Vehicles[2];
            _StartingVehicles[3] = _Vehicles[3];
        }

        // Set player 1's vehicle
        ///PlayerManager._Instance._PlayerOne.SetVehicle(_StartingVehicles[0]);
        ///PlayerManager._Instance._PlayerOne.GetVehicle().SetPlayerEntity(PlayerManager._Instance._PlayerOne);
    }
	
	private void Update () {
        
    }

    //***************************************************************
    // SET & GET

    public List<VehicleBase> GetVehicles() { return _Vehicles; }

    public List<VehicleBase> GetPlayerControlledVehicles() { return _PlayerControlledVehicles; }
}