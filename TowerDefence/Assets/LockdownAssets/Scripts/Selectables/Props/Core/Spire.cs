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
    public List<StreamLights> StreamLighters;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    [System.Serializable]
    public class StreamLights {

        public Light Light;
        public bool Flicker = true;
        public float FlickerIntensity = 1f;

        [HideInInspector]
        public float _BaseLuminosity;
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

        // Get reference to the base intensity for all the lights in the list
        for (int i = 0; i < StreamLighters.Count; i++) {

            StreamLighters[i]._BaseLuminosity = StreamLighters[i].Light.intensity;
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

            // Flicker lights
            if (StreamLighters.Count > 0) {

                for (int i = 0; i < StreamLighters.Count; i++) {

                    // Flicker the lights if needed
                    if (StreamLighters[i].Flicker) {

                        float noise = Mathf.PerlinNoise(Random.Range(0f, 1000f), Time.time);
                        float baseLuminosity = StreamLighters[i]._BaseLuminosity;
                        float flickerintensity = StreamLighters[i].FlickerIntensity;
                        StreamLighters[i].Light.intensity = Mathf.Lerp(baseLuminosity - flickerintensity, baseLuminosity, noise);
                    }
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
        WaveManager.Instance.CoreDamagedWidget.ShowNotification();

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
