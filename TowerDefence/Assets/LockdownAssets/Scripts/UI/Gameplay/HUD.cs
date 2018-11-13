using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 18/7/2018
//
//******************************

public class HUD : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR  
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" HUD WRAPPERS")]
    [Space]
    public SelectionWheel SelectionWheel;
    public SelectionWheel SelectionWindow;
    public GameObject AbilitiesWheel;

    [Space]
    [Header("-----------------------------------")]
    [Header(" GUI TEXT COMPONENTS")]
    [Space]
    public Text PopulationText;
    public Text SuppliesCountText;
    public Text PowerCountText;
    public Text PlayerLevelText;
    [Space]
    public Text PlayerScoreText;

    [Space]
    [Header("-----------------------------------")]
    [Header(" RAYCAST LAYERMASK")]
    [Space]
    public LayerMask MaskBlock;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Player _Player;

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
        _Player = GetComponent<Player>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {
        
        // Update menu type
        if (!GameManager.Instance._IsRadialMenu) { SelectionWheel = SelectionWindow; }

        // Check for pause screen input
        UpdatePauseScreen();

        UpdateTextComponents();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void UpdatePauseScreen() {

        if (_Player != null) {

            // Keyboard input
            if (_Player._KeyboardInputManager.IsPrimaryController) {

                // On ESC or F10
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F10)) {

                    // Resume gameplay
                    if (GameManager.Instance.IsGamePause()) { GameManager.Instance.OnUnpause(); }

                    // Initiate pause
                    else { GameManager.Instance.OnPause(); }
                } 
            }

            // Xbox input
            if (_Player._XboxGamepadInputManager.IsPrimaryController) {

                // On START 
                if (GamepadManager.Instance.GetGamepad(1).GetButtonDown("start")) {
                    
                    // Resume gameplay
                    if (GameManager.Instance.IsGamePause()) { GameManager.Instance.OnUnpause(); }

                    // Initiate pause
                    else { GameManager.Instance.OnPause(); }                
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void UpdateTextComponents() {

        // Update player's army population count
        if (PopulationText != null && _Player != null)
            PopulationText.text = _Player.PopulationCount.ToString() + " / " + _Player.MaxPopulation.ToString();

        // Update player's supply count
        if (SuppliesCountText != null && _Player != null)
            SuppliesCountText.text = _Player.SuppliesCount.ToString()/* + " / " + _Player.MaxSupplyCount.ToString()*/;

        // Update player's power count
        if (PowerCountText != null && _Player != null)
            PowerCountText.text = _Player.PowerCount.ToString()/* + " / " + _Player.MaxPowerCount.ToString()*/;

        // Update player's tech level
        if (PlayerLevelText != null && _Player != null)
            PlayerLevelText.text = _Player.Level.ToString();    
        
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  GameObject
    /// </returns>
    public GameObject FindHitObject() {

        // Create raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Return object from raycast
        if (Physics.Raycast(ray, out hit, 1000, MaskBlock))
            return hit.collider.gameObject;

        return null;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Vector3
    /// </returns>
    public Vector3 FindHitPoint() {

        // Create raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Return hit point from raycast
        if (Physics.Raycast(ray, out hit, 1000, MaskBlock))
            return hit.point;

        return Settings.InvalidPosition;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Rect
    /// </returns>
    public Rect GetPlayingArea() {  return new Rect(0, 0, Screen.width, Screen.height); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool MouseInBounds() {

        // Screen coordinates start in the lower-left corner of the screen
        // not the top-left of the screen like the drawing coordinates do
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width;
        bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height;

        return insideWidth && insideHeight;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool WheelActive() { return GameManager.Instance.SelectionWheel.activeInHierarchy || GameManager.Instance.AbilityWheel.activeInHierarchy; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
