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
    static int _SquadSize;
    public float _MinVelocity;
    public float _MaxVelocity;
    public float _Randomness;
    Vector3 _FlockCenter;
    Vector3 _FlockVelocity;

    public Unit unit;
    public Squad squad;

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
        unit = GetComponent<Unit>();

        if (unit.IsInASquad()) {

            // Get reference to Squad
            squad = unit.GetSquadAttached();
            // Set the squad size
            _SquadSize = unit.GetSquadAttached().GetSquadCount();
            // Set the squad list
            _SquadMembers = unit.GetSquadAttached().GetSquadMembers();    
        }
	}

    /// <summary>
    ///  Called each frame. 
    /// </summary>
    void Update () {

        // Flock center and Flock velocity
        _FlockCenter   = Vector3.zero;
        _FlockVelocity = Vector3.zero;

        // If the unit is out of range from the squad
        if (Vector3.Distance(unit.transform.position, squad.transform.position) > squad.FlockingRadius) {

            //transform.position 
        }

	}
}
