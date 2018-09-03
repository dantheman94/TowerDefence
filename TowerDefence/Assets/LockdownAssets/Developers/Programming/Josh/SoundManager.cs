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
    private List<AudioSource> _VoxelWaitingList;
    public static SoundManager Instance;
    private bool _IsPlayingVoxel = false;
    private float _TimeSinceLastVoxel = 0f;

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

        UpdateVoxel();
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

    private void UpdateVoxel()
    {

        // If there are voxel sounds waiting to be played
        if (_VoxelWaitingList.Count > 0)
        {

            if (_IsPlayingVoxel == true)
            {

                // Find the voxel sound that is current playing
                AudioSource vox = null;
                foreach (var sound in _VoxelWaitingList)
                {
                    
                    // If a sound from the voxel list is playing
                    if (sound.isPlaying == true)
                    {

                        // Then a voxel is playing
                        vox = sound;

                    }
                    break;
                }

                _IsPlayingVoxel = vox != null;
            }

            // A vox has finished playing
            else
            { /// _IsPlayingVoxel == false

                // Get the last voxel that was playing (should be at the front of the list) & remove it from the queue
                _VoxelWaitingList.RemoveAt(0);

                // If there are still voxels in the queue
                if (_VoxelWaitingList.Count > 0)
                {

                    // Play the next vox sound in the queue
                    _VoxelWaitingList[0].Play();

                    _IsPlayingVoxel = true;
                    _TimeSinceLastVoxel = 0f;
                }
            }
        }

        // No more voxels are left in the playing queue
        else if (_VoxelWaitingList.Count == 0)
        {

            // Add to timer
            _TimeSinceLastVoxel += Time.deltaTime;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}