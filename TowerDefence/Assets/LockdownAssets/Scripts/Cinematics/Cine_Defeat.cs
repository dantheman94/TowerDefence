using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 2/9/2018
//
//******************************

public class Cine_Defeat : Cinematic {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" DEFEAT CINEMATIC PROPERTIES")]
    [Space]
    public float CoreOffset = 100f;
    public float TimeTillFinalExplosion = 5f;
    public float WidgetDisplayDelay = 2f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public Player _Player { get; set; }

    private bool _CinematicInProgress = false;

    private Transform _TargetTransform = null;
    private Vector3 _CurrentVelocity = Vector3.zero;

    private float _TimerCoreExplosion = 0f;
    private float _TimeTillWidgetIsDisplayed = 0f;

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

        _TimeTillWidgetIsDisplayed = TimeTillFinalExplosion + WidgetDisplayDelay;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        
        if (_CinematicInProgress && ViewCamera != null) {

            // Determine target point for the camera lerp
            if (_TargetTransform == null) {
                
                _TargetTransform = new GameObject().transform;
                _TargetTransform.position = new Vector3(WaveManager.Instance.CentralCore.transform.position.x, 
                                                        ViewCamera.transform.position.y, 
                                                        WaveManager.Instance.CentralCore.transform.position.z);
                _TargetTransform.position -= ViewCamera.transform.up * CoreOffset;
                
                // Clamp the chase position to mix / max heights
                if (_TargetTransform.position.y < Settings.MinCameraHeight) { _TargetTransform.position = new Vector3(_TargetTransform.position.x, Settings.MinCameraHeight, _TargetTransform.position.z); }
                if (_TargetTransform.position.y > Settings.MaxCameraHeight) { _TargetTransform.position = new Vector3(_TargetTransform.position.x, Settings.MaxCameraHeight, _TargetTransform.position.z); }
            }

            // Smoothly move toward target position
            ViewCamera.transform.position = Vector3.SmoothDamp(ViewCamera.transform.position, _TargetTransform.position, ref _CurrentVelocity, CameraLerpSpeed * Time.deltaTime);
            
            // View camera has reached target
            float dist = Vector3.Distance(ViewCamera.transform.position, _TargetTransform.position);
            if (dist < 10f) {

                // Display widget coroutine
                StartCoroutine(DelayedWidgetDisplay());

                // Play some explosions before the big finale
                _TimerCoreExplosion += Time.deltaTime;
                if (_TimerCoreExplosion >= TimeTillFinalExplosion) {

                    // Final explosion=
                    UI_ScreenFade.Instance.StartAnimation(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 1f);
                }
                else {

                    // Mini explosions with random scaling
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

        _CinematicInProgress = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Waits till the final core explosion is complete, then delays momentarily
    //  before showing the gameover widget with timescale being set to 0.
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayedWidgetDisplay() {

        yield return new WaitForSeconds(_TimeTillWidgetIsDisplayed);

        // Show widget
        GameManager.Instance.ShowGameOverWidget();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}