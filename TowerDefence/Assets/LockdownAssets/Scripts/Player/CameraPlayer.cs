﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 1/10/2018
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

    [Space]
    [Header("-----------------------------------")]
    [Header(" CAMERA SHAKE")]
    [Space]
    public float ShakeTime;
    public float ShakeTrauma;
    private float ShakeOffsetX = 1f;
    private float ShakeOffsetY = 1f;
    private float ShakeOffsetZ = 1f;
    public bool IsShaking = false;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Renderer _MinimapRenderer;
    private Player _PlayerAttached = null;
    private KeyboardInput _KeyBoardInput;

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

    /// <summary>
    /// Checks if camera is between given map bounds.
    /// </summary>
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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        CheckCameraBounds();

        if (Input.GetKeyDown(KeyCode.B) == true)
        {
            StartCoroutine(CameraShake(ShakeTrauma, ShakeTime));
        }
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

    /// <summary>
    /// Creates camera shake.
    /// </summary>
    public IEnumerator CameraShake(float strength, float time) {

        // Store initial position
        Quaternion _InitialCameraRotation = transform.rotation;
        // Time elapsed since shake started
        float _ElapsedTime = 0.0f;

        float offsetX = 0f;
        float offsetY = 0f;
        float offsetZ = 0f;

        // Begin shaking
        while ((_ElapsedTime < time) /*&& IsShaking == false*/) {

            IsShaking = true;

            //offsetX = ShakeOffsetX * (Random.value * Mathf.PerlinNoise(Mathf.Ceil(Random.Range(10000, 99999)), time)) * strength;
            //offsetY = ShakeOffsetY * (Random.value * Mathf.PerlinNoise(Mathf.Ceil(Random.Range(10000, 99999)), time)) * strength;
            //offsetZ = ShakeOffsetZ * (Random.value * Mathf.PerlinNoise(Mathf.Ceil(Random.Range(10000, 99999)), time)) * strength;

            // Add camera shake offset to a rotation that the camera's rotation is set to match
            Quaternion newRot = Quaternion.Euler(transform.rotation.eulerAngles.x + offsetX,
                                                 transform.rotation.eulerAngles.y + offsetY, 
                                                 transform.rotation.eulerAngles.z + offsetZ);
            transform.rotation = newRot;
            Debug.Log(" Rotation X: " + newRot.eulerAngles.x + 
                      " / Rotation Y: " + newRot.eulerAngles.y + 
                      " / Rotation Z: " + newRot.eulerAngles.z);

            // Add to camera shake timer
            _ElapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = _InitialCameraRotation;
        IsShaking = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}