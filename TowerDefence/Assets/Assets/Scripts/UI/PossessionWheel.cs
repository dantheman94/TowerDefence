using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class PossessionWheel : MonoBehaviour {

    //***************************************************************
    // INSPECTOR

    //***************************************************************
    // VARIABLES

    private bool _Active = false;
    private PlayerEntity _Player;
    private GroundVehicle _CurrentVehicle;
    private GroundVehicle _NextVehicle;

    //***************************************************************
    // FUNCTIONS

    private void Start () {
		
	}
    
    private void Update () {
		
        if (_Active && _Player) {

            // Get reference to current vehicle
            foreach (var vehicle in VehicleManager._Instance.GetPlayerControlledVehicles()) {

                // Only the current vehicle should have its player entity enabled
                if (vehicle.GetPlayerEntity().enabled == true) {

                    // Update current vehicle once found
                    _CurrentVehicle = vehicle;
                    break;
                }
            }

            // Rightstick UP
            if (_Player.GetLeftThumbstickYaxis() > 0f) {

                _NextVehicle = VehicleManager._Instance.GetPlayerControlledVehicles()[0];
            }

            // Rightstick DOWN
            if (_Player.GetLeftThumbstickYaxis() < 0f) {

                _NextVehicle = VehicleManager._Instance.GetPlayerControlledVehicles()[1];
            }

            // Rightstick RIGHT
            if (_Player.GetLeftThumbstickXaxis() > 0f) {

                _NextVehicle = VehicleManager._Instance.GetPlayerControlledVehicles()[2];
            }

            // Rightstick LEFT
            if (_Player.GetLeftThumbstickXaxis() < 0f) {

                _NextVehicle = VehicleManager._Instance.GetPlayerControlledVehicles()[3];
            }

            // Check for confirmation input
            if (_Player.GetButtonAClicked() && _NextVehicle && _CurrentVehicle) {

                // Disable old vehicle
                _CurrentVehicle.GetPlayerEntity().enabled = false;

                // Enable new vehicle
                _NextVehicle.GetPlayerEntity().enabled = true;

                // Remove widget from screen
                gameObject.SetActive(false);
            }
        }
    }

    //***************************************************************
    // SET & GET

    public void SetActive(bool value) { _Active = value; }
    
    public bool GetActive() { return _Active; }

    public void SetPlayerEntity(PlayerEntity player) { _Player = player; }

}
