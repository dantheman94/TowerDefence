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
    public List<GroundVehicle> _StartingVehicles;

    //***************************************************************
    // VARIABLES

    public static VehicleManager _Instance;

    private List<GroundVehicle> _PlayerControlledVehicles;
    private List<GroundVehicle> _Vehicles;

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
        _Vehicles = new List<GroundVehicle>();

    }

    private void Start () {
        
        // Control starting vehicle gameobject
        if (_StartingVehicles.Count > 0) {

            foreach (var vehicle in _StartingVehicles) {

                vehicle.SetControllerType(GroundVehicle.EControllerType.PlayerControlled);
                _Vehicles.Add(vehicle);
                _PlayerControlledVehicles = _StartingVehicles;
            }
            _StartingVehicles[0].GetPlayerEntity().enabled = true;
        }
        else { /* _StartingVehicles.Count == 0 */

            Debug.LogWarning("ERROR: Starting vehicles has not been set in vehicle manager. Manager is allocating a vehicle from _Vehicles array for each player, this could have unintended results.");
            _StartingVehicles = new List<GroundVehicle>();
            _StartingVehicles[0] = _Vehicles[0];
            _StartingVehicles[1] = _Vehicles[1];
            _StartingVehicles[2] = _Vehicles[2];
            _StartingVehicles[3] = _Vehicles[3];
        }
        
    }
	
	private void Update () {

        // Enable player components in the vehicles that are currently being controlled by players
        ///foreach (var vehicle in _PlayerControlledVehicles) {
        ///
        ///    vehicle.GetPlayerEntity().enabled = true;
        ///}
    }

    //***************************************************************
    // SET & GET

    public List<GroundVehicle> GetVehicles() { return _Vehicles; }

    public List<GroundVehicle> GetPlayerControlledVehicles() { return _PlayerControlledVehicles; }
}