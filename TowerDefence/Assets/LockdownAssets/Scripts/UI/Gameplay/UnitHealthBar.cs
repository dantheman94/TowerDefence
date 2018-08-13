using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 26/5/2018
//
//******************************

public class UnitHealthBar : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" UI COMPONENTS")]
    [Space]
    public Text _TextComponent;
    [Space]
    public Slider ShieldSlider = null;
    public Image ShieldFill = null;
    [Space]
    public Slider HealthSlider = null;
    public Image HealthFill = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" OFFSETS")]
    [Space]
    public Vector3 Offsetting = Vector3.zero;

    [Space]
    [Header("-----------------------------------")]
    [Header(" SHIELD-BAR COLOURS")]
    [Space]
    public Image ShieldbarTeamColourOutline = null;
    [Space]
    public Color ShieldOkayColour = Color.green;
    [Space]
    [Range(0f, 1f)]
    public float ShieldPercentageThresholdDamaged = 0.6f;
    public Color ShieldDamagedColour = Color.yellow;
    [Space]
    [Range(0f, 1f)]
    public float ShieldPercentageThresholdVeryDamaged = 0.2f;
    public Color ShieldVeryDamagedColour = Color.red;

    [Space]
    [Header("-----------------------------------")]
    [Header(" HEALTH-BAR COLOURS")]
    [Space]
    public Image HealthbarTeamColourOutline = null;
    [Space]
    public Color HealthOkayColour = Color.green;
    [Space]
    [Range(0f, 1f)]
    public float HealthPercentageThresholdDamaged = 0.6f;
    public Color HealthDamagedColour = Color.yellow;
    [Space]
    [Range(0f, 1f)]
    public float HealthPercentageThresholdVeryDamaged = 0.2f;
    public Color HealthVeryDamagedColour = Color.red;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Camera _CameraAttached = null;
    private WorldObject _WorldObject = null;
    private Building _BuildingAttached = null;

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
        
        if (_WorldObject != null && _CameraAttached != null) {

            // Display text if the object is a building
            if (_BuildingAttached != null) {

                _TextComponent.text = _WorldObject.ObjectName;

                // Only show the building name if it is built & active in the game world
                if (_WorldObject.GetObjectState() == WorldObject.WorldObjectStates.Active) {

                    _TextComponent.enabled = true;
                }
                else { _TextComponent.enabled = false; }
            }

            // Don't show the text component if the object is NOT a building
            else { _TextComponent.enabled = false; }

            // Update health bar
            if (HealthSlider != null) { HealthSlider.value = _WorldObject.GetHealth(); }

            // Update shield bar
            if (ShieldSlider != null) {

                // Show the shield bar if theres some shield left
                if (_WorldObject.GetShieldPoints() > 0) {

                    ShieldSlider.value = _WorldObject.GetShield();
                    ShieldSlider.gameObject.SetActive(true);
                }

                // Hide the shield
                else { ShieldSlider.gameObject.SetActive(false); }
            }

            // Object is alive - display the widget
            if (_WorldObject.IsInWorld()) {
                
                // Set world space position
                Vector3 pos = _WorldObject.transform.position + Offsetting;
                pos.y = pos.y + _WorldObject.GetObjectHeight();
                transform.position = pos;

                // Constantly face the widget towards the camera
                transform.LookAt(2 * transform.position - _CameraAttached.transform.position);
            }

            // Object is dead/destroyed
            else { ObjectPooling.Despawn(gameObject); }

            // Update team colour
            switch (_WorldObject.Team) {

                // White outline for undefined team
                case GameManager.Team.Undefined: {

                    if (ShieldbarTeamColourOutline != null) { ShieldbarTeamColourOutline.color = Color.white; }
                    if (HealthbarTeamColourOutline != null) { HealthbarTeamColourOutline.color = Color.white; }
                    break;
                }
                
                // Player owner's colour for defending team
                case GameManager.Team.Defending: { 

                    if (ShieldbarTeamColourOutline != null) { ShieldbarTeamColourOutline.color = _WorldObject._Player.TeamColor; }
                    if (HealthbarTeamColourOutline != null) { HealthbarTeamColourOutline.color = _WorldObject._Player.TeamColor; }
                    break;
                }

                // Red outline for attacking/AI team
                case GameManager.Team.Attacking: {

                    if (ShieldbarTeamColourOutline != null) { ShieldbarTeamColourOutline.color = WaveManager.Instance.AttackingTeamColour; }
                    if (HealthbarTeamColourOutline != null) { HealthbarTeamColourOutline.color = WaveManager.Instance.AttackingTeamColour; }
                    break;
                }

                default: break;
            }

            // Update shieldbar colour thresholds
            if (ShieldFill != null && ShieldSlider != null) {

                // Shield okay
                if (ShieldSlider.value >= ShieldPercentageThresholdDamaged) { ShieldFill.color = ShieldOkayColour; }

                // Shield damaged
                else if (ShieldSlider.value >= ShieldPercentageThresholdVeryDamaged) { ShieldFill.color = ShieldDamagedColour; }

                // Shield very damaged
                else { ShieldFill.color = ShieldVeryDamagedColour; }
            }

            // Update healthbar colour thresholds
            if (HealthFill != null && HealthSlider != null) {

                // Health okay
                if  (HealthSlider.value >= HealthPercentageThresholdDamaged) { HealthFill.color = HealthOkayColour; }

                // Health damaged
                else if (HealthSlider.value >= HealthPercentageThresholdVeryDamaged) { HealthFill.color = HealthDamagedColour; }

                // Health very damaged
                else { HealthFill.color = HealthVeryDamagedColour; }
            }
        } 
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="obj"></param>
    public void SetObjectAttached(WorldObject obj) {

        // Set localized reference of world object attached
        _WorldObject = obj;
        if (_WorldObject != null) {

            _BuildingAttached = _WorldObject.GetComponent<Building>();

            // Set object's health bar reference
            _WorldObject.SetHealthBar(this);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="cam"></param>
    public void SetCameraAttached(Camera cam) { _CameraAttached = cam; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
