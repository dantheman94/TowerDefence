using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 13/10/2018
//
//******************************

public class Spire : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SPIRE PROPERTIES")]
    [Space]
    public List<LightData> StreamLights;
    [Space]
    public MessageFeed Messagefeed;
    public string SpireLetter;
    [Space]
    public int SpireID;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    public enum LightInterpolationType { SmoothCurve, Linear }

    [System.Serializable]
    public class LightData {

        public Light Light;
        public LightInterpolationType InterpolationType = LightInterpolationType.Linear;
        public float LuminosityMax = 3f;
        public float GlowSpeed = 1f;

        [HideInInspector]
        public float _BaseLuminosity;
        [HideInInspector]
        public float LuminosityRange;
        [HideInInspector]
        public float LuminosityOffset;
    }

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        // Get reference to the base intensities & ranges for all the lights in the list
        for (int i = 0; i < StreamLights.Count; i++) {

            StreamLights[i]._BaseLuminosity = StreamLights[i].Light.intensity;
            StreamLights[i].LuminosityRange = (StreamLights[i].LuminosityMax - StreamLights[i]._BaseLuminosity) / 2;
            StreamLights[i].LuminosityOffset = StreamLights[i].LuminosityRange + StreamLights[i]._BaseLuminosity;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // object is active in the world
        if (_ObjectState == WorldObjectStates.Active && IsAlive()) {

            // Show the healthbar
            if (_HealthBar != null) { _HealthBar.gameObject.SetActive(true); }

            // Create a healthbar if the unit doesn't have one linked to it
            else {

                GameObject healthBarObj = ObjectPooling.Spawn(GameManager.Instance.UnitHealthBar.gameObject);
                _HealthBar = healthBarObj.GetComponent<UnitHealthBar>();
                _HealthBar.SetObjectAttached(this);
                healthBarObj.gameObject.SetActive(true);
                healthBarObj.transform.SetParent(GameManager.Instance.WorldSpaceCanvas.gameObject.transform, false);

                if (_Player == null) {

                    Player plyr = GameManager.Instance.Players[0];
                    _HealthBar.SetCameraAttached(plyr.CameraAttached);
                }
                else { _HealthBar.SetCameraAttached(_Player.CameraAttached); }
            }

            // Glowing stream lights
            if (StreamLights.Count > 0) {

                for (int i = 0; i < StreamLights.Count; i++) {

                    float luminosity = 1f;

                    // Interpolate between min & max
                    switch (StreamLights[i].InterpolationType) {

                        // Smooth curve (sin)
                        case LightInterpolationType.SmoothCurve: {

                            luminosity = StreamLights[i].LuminosityOffset + Mathf.Sin(Time.time * StreamLights[i].GlowSpeed) * StreamLights[i].LuminosityRange;
                            break;
                        }

                        // Linear (ping pong)
                        case LightInterpolationType.Linear: {

                            luminosity = Mathf.PingPong(Time.time * StreamLights[i].GlowSpeed, StreamLights[i].LuminosityMax) + StreamLights[i]._BaseLuminosity;
                            break;
                        }

                        default: break;
                    }
                    StreamLights[i].Light.intensity = luminosity;
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Damages the object by a set amount.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
    public override void Damage(float damage, WorldObject instigator = null) {
        base.Damage(damage, instigator);

        // Notify the player
        //  WaveManager.Instance.CoreDamagedWidget.ShowNotification();

        SoundManager.Instance.GetAnnouncer().PlaySpireAttackedSound();

        // Flashing health bar
        WaveManager.Instance.CoreHealthBar.SetFlashingText((UI_CoreHealthBar.ObjectFlashing)SpireID, true);

        if (Messagefeed != null)
            Messagefeed.DisplaySpireDamaged(SpireLetter);
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
