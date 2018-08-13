using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 6/08/2018
//
//******************************

public class Boids : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    List<Unit> _SquadMembers;
    int _SquadSize;
    float _MinVelocity = 0;
    float _MaxVelocity;

    Vector3 _SeparationForce;
    Vector3 _SeparationWeight = new Vector3(2, 0, 2);
    Vector3 _AlignmentForce;
    Vector3 _AllignmentSpeed = new Vector3(2, 0, 2);
    Vector3 _CohesionForce;
    Vector3 _CohesionWeight = new Vector3(2, 0, 2);

    float _FlockingRadius;
    Vector3 _ForceToApply;

    Unit _Unit;
    Squad squad;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called when the gameObject is created.
    /// </summary>
    void Start () {

        // Get reference to Unit
        squad = GetComponent<Squad>();
        _Unit = squad.SquadUnit;
        _MaxVelocity = _Unit.InfantryMovementSpeed;

        // Set the squad size
        _SquadSize = squad.GetSquadCount();
        // Set the squad list
        _SquadMembers = squad.GetSquadMembers();

        _FlockingRadius = squad.FlockingRadius;
    }

    /// <summary>
    ///  Called each frame. 
    /// </summary>
    void Update () {

        //// Flock center and Flock velocity
        //_FlockCenter   = Vector3.zero;
        //_FlockVelocity = Vector3.zero;

        //// If the unit is out of range from the squad
        //if (Vector3.Distance(unit.transform.position, squad.transform.position) > squad.FlockingRadius) {

        //    //transform.position 
        //}

        _AlignmentForce = Vector3.zero;
        _SeparationForce = Vector3.zero;
        _CohesionForce = Vector3.zero;

        foreach (Unit unit in _SquadMembers) {

            _AlignmentForce += unit.GetAgent().transform.position - squad.transform.position;
            _SeparationForce += unit.GetAgent().transform.position - squad.transform.position;
            _CohesionForce += squad.transform.position - unit.GetAgent().transform.position;


        }

        _AlignmentForce /= _SquadSize;
        _SeparationForce /= _SquadSize;
        _CohesionForce /= _SquadSize;

        foreach (Unit unit in _SquadMembers) {

            // Allignment stuffs
            _ForceToApply = Vector3.Scale((_AlignmentForce - squad.transform.forward * unit.GetAgent().speed), _AllignmentSpeed);

            // Separation stuffs
            _ForceToApply = Vector3.Scale((_SeparationForce - unit.GetAgent().transform.position), _SeparationWeight);

            // Cohesion stuffs
            _ForceToApply = Vector3.Scale((_CohesionForce - squad.transform.forward * unit.GetAgent().speed), _CohesionWeight);
            unit.GetAgent().velocity = _ForceToApply;
        }
    }
}
