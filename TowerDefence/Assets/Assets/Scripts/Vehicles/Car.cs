using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class Car : GroundVehicle {

    //***************************************************************
    // VARIABLES

    //***************************************************************
    // FUNCTIONS    
    
    protected override void Start () {

        // GroundVehicle.Start();
        base.Start();
        _VehicleType = EVehicleType.Car;
	}

    protected override void PlayerControlled_UpdateMovement_1() {

        if (_PlayerEntity) {

            // Rotate weapon
            if (_Weapon) {

                // Recieving rotation input
                if (_PlayerEntity.GetRightThumbstickYaxis() != 0 || _PlayerEntity.GetRightThumbstickXaxis() != 0) {

                    Vector3 direction = _PlayerEntity.GetRightThumbstickInput();
                    Quaternion targetRotation = Quaternion.Euler(direction);
                    _Weapon.transform.rotation = Quaternion.Lerp(_Weapon.transform.rotation, targetRotation, _Weapon.GetRotationSpeed() * Time.deltaTime);
                }
            }

            // Update base rotation (only if currently moving)
            if (_CurrentSpeed < -4 || _CurrentSpeed > 4) {

                _BaseRotation = _PlayerEntity.GetLeftThumbstickXaxis() * _BaseRotationSpeed;
                if (_BaseRotation != 0) { transform.eulerAngles += new Vector3(0f, _BaseRotation, 0f); }
            }

            // Update base movement
            if (_PlayerEntity.GetLeftThumbstickYaxis() > 0.01f) {

                // Apply forward acceleration
                _CurrentSpeed += _Acceleration;

                // Clamp to top speed
                if (_CurrentSpeed > _TopSpeed) { _CurrentSpeed = _TopSpeed; }
            }
            else if (_PlayerEntity.GetLeftThumbstickYaxis() < -0.01f) {

                // Apply backward acceleration
                _CurrentSpeed -= _Decceleration;

                // Clamp to reverse top speed
                if (_CurrentSpeed < _TopReverseSpeed) { _CurrentSpeed = _TopReverseSpeed; }
            }

            else {

                // Slowly decelerate to a stop
                if (_CurrentSpeed > 0.01) { _CurrentSpeed -= _Decceleration * 0.1f; }
                else if (_CurrentSpeed < -0.01) { _CurrentSpeed += _Decceleration * 0.1f; }
                else { _CurrentSpeed = 0f; }
            }
        }
    }
              
    protected override void PlayerControlled_UpdateMovement_2() {

        if (_PlayerEntity) {

            // Rotate weapon
            if (_Weapon) {

                // Recieving rotation input
                if (_PlayerEntity.GetRightThumbstickYaxis() != 0 || _PlayerEntity.GetRightThumbstickXaxis() != 0) {

                    Quaternion targetRotation = Quaternion.Euler(_PlayerEntity.GetRightThumbstickInput());
                    _Weapon.transform.rotation = Quaternion.Lerp(_Weapon.transform.rotation, targetRotation, _Weapon.GetRotationSpeed() * Time.deltaTime);
                }
            }

            // Update base rotation (only if currently moving)
            if (_CurrentSpeed < -4 || _CurrentSpeed > 4) {

                _BaseRotation = _PlayerEntity.GetLeftThumbstickXaxis() * _BaseRotationSpeed;
                if (_BaseRotation != 0) { transform.eulerAngles += new Vector3(0f, _BaseRotation, 0f); }
            }

            // Update base movement
            if (_PlayerEntity.OnRightTrigger()) {

                // Apply forward acceleration
                _CurrentSpeed += _Acceleration;

                // Clamp to top speed
                if (_CurrentSpeed > _TopSpeed) { _CurrentSpeed = _TopSpeed; }
            }
            else if (_PlayerEntity.OnLeftTrigger()) {

                // Apply backward acceleration
                _CurrentSpeed -= _Decceleration;

                // Clamp to reverse top speed
                if (_CurrentSpeed < _TopReverseSpeed) { _CurrentSpeed = _TopReverseSpeed; }
            }

            else {

                // Slowly decelerate to a stop
                if (_CurrentSpeed > 0.01) { _CurrentSpeed -= _Decceleration * 0.1f; }
                else if (_CurrentSpeed < -0.01) { _CurrentSpeed += _Decceleration * 0.1f; }
                else { _CurrentSpeed = 0f; }
            }
        }
    }
  
}