using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/8/2018
//
//******************************

public class CameraPlayer : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MINIMAP PROPERTIES")]
    [Space]
    public GameObject MinimapQuad = null;
    public float TransparencyLevel = 120f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" DYNAMIC HEIGHT PROPERTIES")]
    [Space]
    public LayerMask _RaycastLayerMask;

    [Space]
    [Header("----------------------------------")]
    [Header(" MAP BOUNDS")]
    [Space]
    public float EastBounds;
    public float WestBounds;
    public float NorthBounds;
    public float SouthBounds;
    public bool LockCameraToBounds = false;

    [HideInInspector]
    public bool PastBoundsEast = false;
    [HideInInspector]
    public bool PastBoundsWest = false;
    [HideInInspector]
    public bool PastBoundsNorth = false;
    [HideInInspector]
    public bool PastBoundsSouth = false;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Renderer _MinimapRenderer;
    private Player _PlayerAttached = null;

    public float _MinCameraHeight { get; set; }
    public float _MaxCameraHeight { get; set; }
    private bool _RangesMinUpdated = false;
    private bool _RangesMaxUpdated = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Get component references
        if (MinimapQuad != null) { _MinimapRenderer = MinimapQuad.GetComponent<Renderer>(); }

        // Initialize camera heights
        _MinCameraHeight = Settings.MinCameraHeight;
        _MaxCameraHeight = Settings.MaxCameraHeight;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void CheckCameraBounds()
    {
        if (LockCameraToBounds)
        {
            if (transform.position.x <= WestBounds)
            {
                PastBoundsWest = true;
            }
            else
            {
                PastBoundsWest = false;
            }

            if (transform.position.x >= EastBounds)
            {
                PastBoundsEast = true;
            }
            else
            {
                PastBoundsEast = false;
            }

            if(transform.position.z <= SouthBounds)
            {
                PastBoundsSouth = true;
            }
            else
            {
                PastBoundsSouth = false;
            }

            if (transform.position.z >= NorthBounds)
            {
                PastBoundsNorth = true;
            }
            else
            {
                PastBoundsNorth = false;
            }
        }
    }

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        CheckCameraBounds();
        /*
        // If the min raycasts hits, then the camera is too low
        RaycastHit minHit;
        if (Physics.Raycast(transform.position, -Vector3.up, out minHit, Settings.MinCameraHeight, _RaycastLayerMask)) {
            
            float overlap = _MinCameraHeight - minHit.point.y;

            // UPDATE RANGES ONLY ONCE PER RAYCAST HIT SESSION
            if (!_RangesMinUpdated) {

                _RangesMinUpdated = true;

                // Set min camera height
                _MinCameraHeight += overlap;

                // Set max camera height
                float range = Settings.MaxCameraHeight - Settings.MinCameraHeight;
                _MaxCameraHeight = _MinCameraHeight + range;
            }

            Debug.DrawRay(transform.position, -Vector3.up * Settings.MinCameraHeight, Color.green);
        }
        else {
            
            _RangesMinUpdated = false;

            Debug.DrawRay(transform.position, -Vector3.up * Settings.MinCameraHeight, Color.red);

            // If the max range raycast doesnt hit, then the camera is too high
            if (!Physics.Raycast(transform.position, -Vector3.up, Settings.MaxCameraHeight, _RaycastLayerMask)) {

                // UPDATE RANGES ONLY ONCE PER RAYCAST HIT SESSION
                if (!_RangesMaxUpdated) {

                    _RangesMaxUpdated = true;

                    RaycastHit distHit;
                    Physics.Raycast(transform.position, -transform.up, out distHit, _RaycastLayerMask);

                    float diff = _MaxCameraHeight - (transform.position.y - distHit.point.y);

                    // Set min camera height
                    _MinCameraHeight -= diff;

                    // Set max camera height
                    float range = Settings.MaxCameraHeight - Settings.MinCameraHeight;
                    _MaxCameraHeight = _MinCameraHeight + range;
                }

                Debug.DrawRay(transform.position, -Vector3.up * Settings.MaxCameraHeight, Color.blue);
            }
            else {

                _RangesMaxUpdated = false;
            }
        }

        Debug.Log("Min: " + _MinCameraHeight + " / Max: " + _MaxCameraHeight);
        */
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayerAttached(Player player) {

        // Set player reference
        _PlayerAttached = player;

        // Set colour reference
        if (_PlayerAttached && _MinimapRenderer != null) {

            Color col = _PlayerAttached.TeamColor;
            col.a = TransparencyLevel;
            _MinimapRenderer.material.SetColor("Albedo", col);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
