using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/12/2018
//
//******************************

public class Core : Building {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" CORE PROPERTIES")]
    [Space]
    public float SeekSphereRadius = 80f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" SPIRES")]
    [Space]
    public List<Spire> Spires;

    public MessageFeed Messagefeed;
    
    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        // Initialize spires
        for (int i = 0; i < Spires.Count; i++) {

            Spires[i].SetPlayer(_Player);
            MaxShieldPoints += Spires[i].MaxHitPoints;
        }

        CreateQueueWidget();
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();
        //HideCoreNotification();
        // object is active in the world
        if (_ObjectState == WorldObjectStates.Active && IsAlive()) {

            // Show the healthbar
            ///if (_HealthBar != null) { _HealthBar.gameObject.SetActive(true); }

            // Create a healthbar if the unit doesn't have one linked to it
            ///else { CreateHealthBar(this, _Player.PlayerCamera); }

            // Update current shield hitpoints based on the spire hitpoints
            float points = 0;
            for (int i = 0; i < Spires.Count; i++) { points += Spires[i].GetHitPoints(); }
            _ShieldPoints = points;
        }

        // Always show the active state mesh
        if (ActiveState != null) { ActiveState.SetActive(true); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Vector3
    /// </returns>
    public Vector3 GetSeekPosition() { return Random.onUnitSphere * SeekSphereRadius; }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public override void OnDeath(WorldObject instigator) {
        base.OnDeath(instigator);

        // Game is over
        SoundManager.Instance.GetAnnouncer().PlayLoseCoreDestroyedSound();
        GameManager.Instance.OnGameover(false);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Damages the object by a set amount.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="instigator"></param>
    public override void Damage(float damage, WorldObject instigator = null) {

        // Notify the player
        //   WaveManager.Instance.CoreDamagedWidget.SetActive(true);

        SoundManager.Instance.GetAnnouncer().PlayCoreAttackedSound();

        // Flashing health bar
        WaveManager.Instance.CoreHealthBar.SetFlashingText(UI_CoreHealthBar.ObjectFlashing.Core, true);

        if (Messagefeed != null)
            Messagefeed.DisplayCoreDamaged();

        // Only damage the core if all spires are destroyed
        bool damageCore = true;
        for (int i = 0; i < Spires.Count; i++) {

            if (Spires[i].GetHitPoints() > 0) {

                damageCore = false;
                break;
            }
        }

        // Damage the core
        if (damageCore) { base.Damage(damage, instigator); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  WorldObject
    /// </returns>
    public WorldObject GetAttackObject() {

        // Return the center core if all the spires are destroyed
        bool core = true;
        for (int i = 0; i < Spires.Count; i++) {

            if (Spires[i].GetHitPoints() > 0) {

                core = false;
                break;
            }
        }
        if (core) { return this; }

        // Theres a spire still alive so return that instead
        Spire spire = null;
        bool match = false;
        while (!match) {

            int i = Random.Range(0, Spires.Count);
            if (Spires[i].GetHitPoints() > 0) {

                spire = Spires[i];
                match = true;
                break;
            }
        }
        if (spire != null) { return spire; }
        else { return this; } /// This should never hit but is a precaution so there isnt a null returned
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Gets the closet attack target tested against the position specified
    /// </summary>
    /// <param name="position"></param>
    /// <returns>
    //  WorldObject 
    /// </returns>
    public WorldObject GetClosestObject(Vector3 position) {
        
        float closestDist = 100000f;
        int iter = 0;
        bool core = true;
        for (int i = 0; i < Spires.Count; i++) {

            // Only if the spire is alive
            if (Spires[i].GetHitPoints() > 0) {

                // Check if the current spire iterator is closer than the previously known iterator
                float testDist = Vector3.Distance(position, Spires[i].transform.position);
                if (testDist < closestDist) {

                    closestDist = testDist;
                    iter = i;
                    core = false;
                }
            }
        }
        if (core) { return this; }
        else { return Spires[iter]; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}