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

public class Cine_TutorialOpen : Cinematic {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" OPENING CINEMATIC PROPERTIES")]
    [Space]
    public float BaseOffset = 100f;
    public float InitialDelayTillFadeIn = 3f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public Player _Player { get; set; }

    private bool _CinematicInProgress = false;
    private bool _CinematicComplete = false;

    private Transform _TargetTransform = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        // Check for cinematic completion
        if (_CinematicInProgress && ViewCamera != null) {

            if (GameManager.Instance.StartingBase != null) {

                // Initialize starting base
                Base startingBase = GameManager.Instance.StartingBase;
                startingBase.CreateHealthBar(startingBase, startingBase._Player.PlayerCamera);
                startingBase.CreateQueueWidget();
                startingBase.CreateRallyPoint();

                // Cinematic is finished
                _CinematicComplete = true;
                GameManager.Instance._CinematicInProgress = _CinematicInProgress = false;
            }            
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Starts the cinematic.
    /// </summary>
    public override void StartCinematic() {
        base.StartCinematic();

        // Set camera starting rotation
        float angle = ViewCamera.transform.rotation.eulerAngles.x;
        ViewCamera.transform.rotation = GameManager.Instance.StartingBase.transform.rotation;
        ViewCamera.transform.rotation = Quaternion.Euler(angle, ViewCamera.transform.rotation.eulerAngles.y, ViewCamera.transform.rotation.eulerAngles.z);

        // Set camera starting position
        ViewCamera.transform.position = new Vector3(GameManager.Instance.StartingBase.transform.position.x,
                                                    Settings.MaxCameraHeight,
                                                    GameManager.Instance.StartingBase.transform.position.z);
        ViewCamera.transform.position -= ViewCamera.transform.up * BaseOffset;
        ViewCamera.transform.position = new Vector3(ViewCamera.transform.position.x, Settings.MaxCameraHeight, ViewCamera.transform.position.z);

        // Fade screen from black
        StartCoroutine(DelayedFadeIn());

        // Start coroutine
        GameManager.Instance._CinematicInProgress = _CinematicInProgress = true;
        StartCoroutine(CinematicFinish());
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Coroutine that waits a few seconds, then fades in the screen from black.
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

        // Start match check
        StartCoroutine(WaveManager.Instance.PermittedMatchStart());
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
