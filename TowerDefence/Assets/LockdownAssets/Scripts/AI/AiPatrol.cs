using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 11/8/2018
//
//******************************

public class AiPatrol : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PATROL PROPERTIES")]
    [Space]
    public float MinimumDistanceToEpicenter = 80f;
    public List<Transform> PatrolPositions;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private List<Vector3> _PatrolPositions;
    private int _PositionIterator = 0;
    private Vector3 _CurrentDestination = Vector3.zero;

    private Ai _Ai = null;
    private Squad _Squad = null;
    private Unit _Unit = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called before Start().
    /// </summary>
    private void Awake() {

        // Get internal patrol positions
        _PatrolPositions = new List<Vector3>();
        if (PatrolPositions != null && PatrolPositions.Count > 0) {

            // Set patrol positions if valid (these are ones specified through the inspector)
            for (int i = 0; i < PatrolPositions.Count; i++) { _PatrolPositions.Add(PatrolPositions[i].position); }
        }

        // Get component references
        _Ai = GetComponent<Ai>();
        _Squad = GetComponent<Squad>();
        _Unit = GetComponent<Unit>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected virtual void Update() {

        if (!_Ai.IsAttacking()) {

            Debug.Log("Patrolling");

            // For squad
            if (_Squad != null) {

                // Distance check to destination epicenter
                if (Vector3.Distance(_Squad.transform.position, _CurrentDestination) <= MinimumDistanceToEpicenter) {

                    // Move to next patrol position
                    _PositionIterator++;
                    if (_PositionIterator >= _PatrolPositions.Count) { _PositionIterator = 0; }

                    // Update current destination
                    _Squad.SquadSeek(_PatrolPositions[_PositionIterator]);
                    _CurrentDestination = _PatrolPositions[_PositionIterator];
                }
                else {

                    // Force seek to the current destination
                    if (_PatrolPositions.Count > 0) {

                        _Squad.SquadSeek(_PatrolPositions[_PositionIterator]);
                        _CurrentDestination = _PatrolPositions[_PositionIterator];
                    }
                }
            }

            // For unit
            if (_Unit != null) {

                // Distance check to destination epicenter
                if (Vector3.Distance(_Unit.transform.position, _CurrentDestination) <= MinimumDistanceToEpicenter) {

                    // Move to next patrol position
                    _PositionIterator++;
                    if (_PositionIterator >= _PatrolPositions.Count) { _PositionIterator = 0; }

                    // Update current destination
                    _Unit.AgentSeekPosition(_PatrolPositions[_PositionIterator]);
                    _CurrentDestination = _PatrolPositions[_PositionIterator];
                }
                else {

                    // Force seek to the current destination
                    if (_PatrolPositions.Count > 0) {

                        _Unit.AgentSeekPosition(_PatrolPositions[_PositionIterator]);
                        _CurrentDestination = _PatrolPositions[_PositionIterator];
                    }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public virtual void InitializePatrol() {

        // For squad
        if (_Squad != null) {

            // Set destination
            _PositionIterator = Random.Range(0, _PatrolPositions.Count - 1);
            _Squad.SquadSeek(_PatrolPositions[_PositionIterator]);
        }

        // For unit
        if (_Unit != null) {

            // Set destination
            _PositionIterator = Random.Range(0, _PatrolPositions.Count - 1);
            _Unit.AgentSeekPosition(_PatrolPositions[_PositionIterator]);
        }

        // Update current destination
        _CurrentDestination = _PatrolPositions[_PositionIterator];
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="positions"></param>
    public void UpdatePatrolPositions(List<Transform> positions) {

        // Initialise list
        if (_PatrolPositions == null) { _PatrolPositions = new List<Vector3>(); }

        // Replace the positions in the internal list
        _PatrolPositions.Clear();
        for (int i = 0; i < positions.Count; i++) { _PatrolPositions.Add(positions[i].position); }

        InitializePatrol();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}