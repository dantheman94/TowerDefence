using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/9/2018
//
//******************************

public class AttackPath : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" NODE PROPERTIES")]
    [Space]
    public float NodeAccuracyRadius = 15f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" EDITOR PROPERTIES")]
    [Space]
    public Color DebugPathColor = Color.red;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private List<Vector3> _PathNodes = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called before Startup().
    /// </summary>
    private void Awake() {

        // Initialize lists and add nodes
        _PathNodes = new List<Vector3>();
        for (int i = 0; i < transform.childCount; i++) { _PathNodes.Add(transform.GetChild(i).position); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Draws a line between each child of this gameobject's transform
    //  NOTE: DEBUG ONLY
    /// </summary>
    private void OnDrawGizmos() {

        for (int i = 0, j = 1; j < transform.childCount; i++, j++) {

            Gizmos.color = DebugPathColor;
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(j).position);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference to the Vector3 positions in the _PathNodes list.
    /// </summary>
    /// <returns>
    //  List<Vector3>
    /// </returns>
    public List<Vector3> GetNodePositions() { return _PathNodes; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Vector3
    /// </returns>
    public Vector3 GetFirstNodeWithOffset() {

        if (_PathNodes.Count > 0) {
            
            Vector2 rand = Random.insideUnitCircle * 15f;
            Vector3 pos = _PathNodes[0] + new Vector3(rand.x, _PathNodes[0].y, rand.y);
            return pos;
        }
        else { return Vector3.zero; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
