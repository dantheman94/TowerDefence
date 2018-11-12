using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 12/11/2018
//
//******************************

public class CameraPlayer : MonoBehaviour
{

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
    [Header("RAYCAST MAP BOUND DIRECTIONS")]
    public Vector3 BackTransform;
    public Vector3 LeftTransform;
    public Vector3 RightTransform;
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
    public bool EnableScreenShake = true;
    [Space]
    public float ShakeMagnitude;
    public float ShakeMaxRange;



    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Renderer _MinimapRenderer;
    private Player _PlayerAttached = null;
    private KeyboardInput _KeyBoardInput;
    private Camera _Camera;

    private float ShakeOffsetX = 1f;
    private float ShakeOffsetY = 1f;
    private float ShakeOffsetZ = 1f;

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
    private void Start()
    {

        // Get component references
        if (MinimapQuad != null) { _MinimapRenderer = MinimapQuad.GetComponent<Renderer>(); }

        // Initialize camera heights
        _MinCameraHeight = Settings.MinCameraHeight;
        _MaxCameraHeight = Settings.MaxCameraHeight;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    /// <summary>
    /// Stops camera from passing map bounds using raycasts.
    /// </summary>
    private void RaycastBounds()
    {

        if (Physics.Raycast(transform.position, transform.forward, 250))
        {
            Debug.DrawRay(transform.position, transform.forward * 250, Color.green);
            PastBoundsNorth = true;
        }
        else
        {
            PastBoundsNorth = false;
            Debug.DrawRay(transform.position, transform.forward * 250, Color.red);
        }

        if (Physics.Raycast(transform.position, BackTransform, 250))
        {
            Debug.DrawRay(transform.position, BackTransform * 250, Color.green);
            PastBoundsSouth = true;
        }
        else
        {
            PastBoundsSouth = false;
            Debug.DrawRay(transform.position, BackTransform * 250, Color.red);
        }

        if (Physics.Raycast(transform.position, RightTransform, 250))
        {
            Debug.DrawRay(transform.position, RightTransform * 250, Color.green);
            PastBoundsEast = true;
        }
        else
        {
            PastBoundsEast = false;
            Debug.DrawRay(transform.position, RightTransform * 250, Color.red);
        }

        if (Physics.Raycast(transform.position, LeftTransform, 250))
        {
            Debug.DrawRay(transform.position, LeftTransform * 250, Color.green);
            PastBoundsWest = true;
        }
        else
        {
            PastBoundsWest = false;
            Debug.DrawRay(transform.position, LeftTransform * 250, Color.red);
        }

    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update()
    {

        RaycastBounds();
        if (_Camera != null)
        {
            //Debug.Log("FOV " + _Camera.fieldOfView);
        }

        //if (Input.GetKeyDown(KeyCode.P) == true)
        //{
        //    float rad = 20.0f;
        //    //SoundManager.Instance.PlaySound("Audio/pfb_Battle", 0.9f, 1.1f);
        //    ExplosionShake(transform.position, rad);

        //    Debug.Log("Button tapped.");
        //}

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayerAttached(Player player)
    {

        // Set player reference
        _PlayerAttached = player;

        // Set colour reference
        if (_PlayerAttached && _MinimapRenderer != null)
        {

            Color col = _PlayerAttached.TeamColor;
            col.a = TransparencyLevel;
            _MinimapRenderer.material.SetColor("Albedo", col);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates camera shake.
    /// Example call: StartCoroutine(CameraShake(ShakeTrauma, ShakeTime));
    /// </summary>
    public IEnumerator CameraShake(float strength, float time)
    {

        // Store initial position
        Quaternion _InitialCameraRotation = transform.rotation;

        // Time elapsed since shake started
        float _ElapsedTime = 0.0f;

        // Offsets
        float offsetX = 0f; float offsetY = 0f; float offsetZ = 0f;

        // Begin shaking
        while (_ElapsedTime < time)
        {

            // Set offset values
            offsetX = ShakeOffsetX * ((Random.value - 0.5f) * Mathf.PerlinNoise(Mathf.Ceil(Random.Range(10000, 99999)), time)) * strength;
            offsetY = ShakeOffsetY * ((Random.value - 0.5f) * Mathf.PerlinNoise(Mathf.Ceil(Random.Range(10000, 99999)), time)) * strength;
            offsetZ = ShakeOffsetZ * ((Random.value - 0.5f) * Mathf.PerlinNoise(Mathf.Ceil(Random.Range(10000, 99999)), time)) * strength;

            // Add camera shake offset to a rotation that the camera's rotation is set to match
            Quaternion newRot = Quaternion.Euler(transform.rotation.eulerAngles.x + offsetX,
                                                 transform.rotation.eulerAngles.y + offsetY,
                                                 transform.rotation.eulerAngles.z + offsetZ);

            // Set the rotation
            transform.rotation = newRot;

            // Add to camera shake timer
            _ElapsedTime += Time.deltaTime;
            yield return null;
        }

        // Insert Lerp loop here back to origin.
        transform.rotation = _InitialCameraRotation;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    public void ExplosionShake(Vector3 location, float radius)
    {

        if (EnableScreenShake)
        {

            if (_Camera == null) { _Camera = GetComponent<Camera>(); }

            // Get the distance between the explosion and the camera
            float distance = Vector3.Distance(location, transform.position);

            // If the distance is within the threshold, shake
            if (distance < ShakeMaxRange)
            {

                // Calculate strenth
                // FOV value becomes larger as you zoom out so we must use division here
                float shakeStrength = (1.0f * (Mathf.Abs(distance * 0.75f))) / (Mathf.Abs(_Camera.fieldOfView));

                //Debug.Log("Shake strength dist: " + (Mathf.Abs(distance * 0.75f)) + ", FOV: " + (Mathf.Abs(_Camera.fieldOfView)) + ", total strength: " + shakeStrength);

                // Calculate duration
                float shakeDuration = 1f;

                // Start shake
                StartCoroutine(CameraShake(shakeStrength * ShakeMagnitude, shakeDuration));
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}