using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/10/2018
//
//******************************

public class Cine_Opening : Cinematic {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" OPENING CINEMATIC PROPERTIES")]
    [Space]
    public float CoreOffset = 100f;
    public float InitialDelayTillFadeIn = 3f;
    public float TimeTillInitialCameraMove = 3f;
    public float StartingBaseOffset = 100f;
    public float MatchStartDelay = 3f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public Player _Player { get; set; }

    private bool _CinematicInProgress = false;
    private bool _CinematicComplete = false;

    private Transform _TargetTransform = null;
    private Vector3 _CurrentVelocity = Vector3.zero;

    private float _TargetFov = Settings.MaxFov;
    private float _CurrentLerpTime = 0f;
    private float _FovLerpTime = 3f;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is created.
    /// </summary>
    private void Start() {
        
        // Determine starting FOV
        _TargetFov = (Settings.MinFov + Settings.MaxFov) / 2;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        if (_CinematicInProgress && ViewCamera != null && _TargetTransform != null) {

            // Increment lerp timer once per frame
            _CurrentLerpTime += Time.deltaTime;
            if (_CurrentLerpTime > _FovLerpTime) { _CurrentLerpTime = _FovLerpTime; }

            // Smoothly move toward target position
            ViewCamera.transform.position = Vector3.SmoothDamp(ViewCamera.transform.position, _TargetTransform.position, ref _CurrentVelocity, CameraLerpSpeed * Time.deltaTime);

            // Smoothly lerp the view camera's FOV to the target FOV
            float percent = _CurrentLerpTime / _FovLerpTime;
            ///ViewCamera.fieldOfView = Mathf.Lerp(Settings.MaxFov, _TargetFov, percent);

            // View camera has reached target
            float dist = Vector3.Distance(ViewCamera.transform.position, _TargetTransform.position);
            if (dist < 10f) {

                if (GameManager.Instance.StartingBase != null) {

                    // Initialize starting base
                    Base startingBase = GameManager.Instance.StartingBase;
                    startingBase.CreateHealthBar(startingBase, startingBase._Player.PlayerCamera);
                    startingBase.CreateQueueWidget();
                    startingBase.CreateRallyPoint();

                    _CinematicComplete = true;
                    GameManager.Instance._CinematicInProgress = _CinematicInProgress = false;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Starts the cinematic
    /// </summary>
    public override void StartCinematic() {
        base.StartCinematic();

        // Set camera starting rotation
        float angle = ViewCamera.transform.rotation.eulerAngles.x;
        ViewCamera.transform.rotation = WaveManager.Instance.CentralCore.transform.rotation;
        ViewCamera.transform.rotation = Quaternion.Euler(angle, ViewCamera.transform.rotation.eulerAngles.y, ViewCamera.transform.rotation.eulerAngles.z);

        // Set camera starting position
        ViewCamera.transform.position = new Vector3(WaveManager.Instance.CentralCore.transform.position.x,
                                                    Settings.MaxCameraHeight,
                                                    WaveManager.Instance.CentralCore.transform.position.z);
        ViewCamera.transform.position -= ViewCamera.transform.up * CoreOffset;
        ViewCamera.transform.position = new Vector3(ViewCamera.transform.position.x, Settings.MaxCameraHeight, ViewCamera.transform.position.z);

        // Set FOV
        ViewCamera.fieldOfView = Settings.MaxFov;

        // Fade screen from black
        StartCoroutine(DelayedFadeIn());

        // Start coroutine
        GameManager.Instance._CinematicInProgress = _CinematicInProgress = true;
        StartCoroutine(DelayedCameraMove());
        StartCoroutine(CinematicFinish());
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Coroutine that waits a few seconds, then sets the target transform for the view camera to lerp to.
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    IEnumerator DelayedFadeIn() {

        // Initialize the screen to full black
        UI_ScreenFade.Instance.SetScreenColour(Color.black);

        yield return new WaitForSeconds(InitialDelayTillFadeIn);

        // Start camera fade in
        UI_ScreenFade.Instance.StartAnimation(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), 4f);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Coroutine that waits a few seconds, then sets the target transform for the view camera to lerp to.
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    IEnumerator DelayedCameraMove() {

        yield return new WaitForSeconds(TimeTillInitialCameraMove + InitialDelayTillFadeIn);

        // Set target tranform properties
        _TargetTransform = new GameObject().transform;
        _TargetTransform.position = new Vector3(GameManager.Instance.StartingBase.transform.position.x, 
                                                Settings.MaxCameraHeight,
                                                GameManager.Instance.StartingBase.transform.position.z - StartingBaseOffset);
        _TargetTransform.rotation = GameManager.Instance.StartingBase.transform.rotation;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Coroutine that waits until the cinematic is completed & starts the waves/match.
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    IEnumerator CinematicFinish() {

        yield return new WaitUntil(() => _CinematicComplete);

        // Hide cinematic black bars
        GameManager.Instance.CinematicBars.gameObject.SetActive(true);
        CinematicBars.Instance.StartAnimation(CinematicBars.AnimationDirection.Exit);

        // Show HUD
        GameManager.Instance.WorldSpaceCanvas.gameObject.SetActive(true);
        GameManager.Instance.HUDWrapper.gameObject.SetActive(true);

        // Show mouse cursor
        Cursor.visible = true;

        // Wait a few seconds before starting the waves
        yield return new WaitForSeconds(MatchStartDelay);

        // Start match check
        StartCoroutine(WaveManager.Instance.PermittedWaveStart());

        Destroy(_TargetTransform.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
