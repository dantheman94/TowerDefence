using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 23/07/2018
//
//******************************

public class SoundManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public static AudioSource _AudioSource;

    // SFX
    private static AudioClip _SFX_ExampleA,
                             _SFX_ExampleB,
                             _SFX_ExampleC;

    // Music
    private static AudioClip _MUSIC_ExampleA,
                             _MUSIC_ExampleB;

    // Voxel
    private bool  _IsPlayingVoxel = false;
    private float _TimeSinceLastVoxel = 0f;
    private List<AudioSource> _VoxelWaitingList;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    // SFX
    [SerializeField]
    [Tooltip("The audio clip used for....")]
    public AudioClip _SFX_ExampleA_Clip;
    [SerializeField]
    [Tooltip("The audio clip used for....")]
    public AudioClip _SFX_ExampleB_Clip;
    [SerializeField]
    [Tooltip("The audio clip used for....")]
    public AudioClip _SFX_ExampleC_Clip;

    // Music

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called when the gameObject is created.
    /// </summary>
    private void Start () {

        // Initialize audio source
        _AudioSource = GetComponent<AudioSource>();

        // Initialize SFX
        _SFX_ExampleA = _SFX_ExampleA_Clip;
        _SFX_ExampleB = _SFX_ExampleB_Clip;
        _SFX_ExampleC = _SFX_ExampleC_Clip;

        // Initialize Music

        // Initialize voxel player
        _VoxelWaitingList = new List<AudioSource>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  Called each frame. 
    /// </summary>
    private void Update () {

        // Voxel player update
        if (_VoxelWaitingList != null)
            UpdateVoxelPlayer();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void UpdateVoxelPlayer() {

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

    public void PlaySFX(string clip) {

        switch (clip)
        {

            // Example A
            case "_SFX_ExampleA":
                _AudioSource.PlayOneShot(_SFX_ExampleA, 1f);
                break;

            // Example B
            case "_SFX_ExampleB":
                _AudioSource.PlayOneShot(_SFX_ExampleA, 1f);
                break;

            // Example C
            case "_SFX_ExampleC":
                _AudioSource.PlayOneShot(_SFX_ExampleA, 1f);
                break;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void PlayMusic() {

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public List<AudioSource> GetVoxelWaitingList() { return _VoxelWaitingList; }

    public void PlayVoxel()      { _IsPlayingVoxel = true; }

    public bool GetIsPlayingVoxel() { return _IsPlayingVoxel; }
}