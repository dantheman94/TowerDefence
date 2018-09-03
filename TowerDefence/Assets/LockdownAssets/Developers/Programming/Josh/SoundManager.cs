using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 3/09/2018
//
//******************************

public class SoundManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    List<AudioSource> _Sounds;
    public static SoundManager Instance;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called when the gameObject is created.
    /// </summary>
    public void Awake () {

        // Initialize singleton
        if (Instance != null && Instance != this) {

            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // Initialize lists
        _Sounds = new List<AudioSource>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Update() {

        // Iterate through the sounds list
        for (int i = 0; i < _Sounds.Count; ++i) {

            // If there's a sound in the list that isn't playing
            if (!(_Sounds[i].isPlaying)) {

                // Remove the sound
                _Sounds.RemoveAt(i);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void PlaySound(string soundLocation, float pitchMin, float pitchMax) {

        // Create pooled game object for the sound
        GameObject soundObj = ObjectPooling.Spawn(Resources.Load(soundLocation) as GameObject);

        // Grab the source for the sound to play from
        AudioSource soundSource = soundObj.GetComponent<AudioSource>();

        // Randomize the sound's pitch based on the min and max specified
        soundSource.pitch = Random.Range(pitchMin, pitchMax);

        // Play the sound
        soundSource.Play();

        // Add the sound object to the List
        _Sounds.Add(soundSource);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}