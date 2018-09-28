using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Daniel Marton
//  Last edited on: 28/09/2018
//
//******************************

public class SoundManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    public static SoundManager Instance;

    List<AudioSource> _Sounds;

    private List<AudioSource> _VoxelWaitingList;
    private bool _IsPlayingVoxel = false;
    private float _TimeSinceLastVoxel = 0f;

    private bool _FadingIn;
    private bool _FadingOut;

    private float _CurrentFadeInLerp = 0f;
    private float _CurrentFadeOutLerp = 0f;

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    public AudioSource _MusicSource;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the gameObject is created.
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

        if (_FadingIn) { _CurrentFadeInLerp += Time.deltaTime; }

        if (_FadingOut) { _CurrentFadeOutLerp += Time.deltaTime; }

        UpdateVoxel();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void PlaySound(string soundLocation, float pitchMin, float pitchMax) {
        
        // Create pooled game object for the sound
        GameObject soundObj = ObjectPooling.Spawn(Resources.Load<GameObject>(soundLocation));

        // Grab the source for the sound to play from
        AudioSource soundSource = soundObj.GetComponent<AudioSource>();

        // Clammp min/max pitch (0, 3)
        if (pitchMin < 0) { pitchMin = 0f; }
        if (pitchMax > 3) { pitchMax = 3f; }
        if (pitchMin > pitchMax) { pitchMin = pitchMax; }
        if (pitchMax < pitchMin) { pitchMax = pitchMin; }

        // Randomize the sound's pitch based on the min and max specified
        soundSource.pitch = Random.Range(pitchMin, pitchMax);

        // Play the sound
        soundSource.Play();

        // Add the sound object to the List
        _Sounds.Add(soundSource);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void UpdateVoxel() {

        // If there are voxel sounds waiting to be played
        if (_VoxelWaitingList.Count > 0) {

            if (_IsPlayingVoxel == true) {

                // Find the voxel sound that is current playing
                AudioSource vox = null;
                foreach (var sound in _VoxelWaitingList) {
                    
                    // If a sound from the voxel list is playing
                    if (sound.isPlaying == true) {

                        // Then a voxel is playing
                        vox = sound;

                    }
                    break;
                }

                _IsPlayingVoxel = vox != null;
            }

            // A vox has finished playing
            else { /// _IsPlayingVoxel == false

                // Get the last voxel that was playing (should be at the front of the list) & remove it from the queue
                _VoxelWaitingList.RemoveAt(0);

                // If there are still voxels in the queue
                if (_VoxelWaitingList.Count > 0) {

                    // Play the next vox sound in the queue
                    _VoxelWaitingList[0].Play();

                    _IsPlayingVoxel = true;
                    _TimeSinceLastVoxel = 0f;
                }
            }
        }

        // No more voxels are left in the playing queue
        else if (_VoxelWaitingList.Count == 0) {

            // Add to timer
            _TimeSinceLastVoxel += Time.deltaTime;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void CallFadeIn(AudioSource musicSource, float timeToFade, float maxVolume) {

        StartCoroutine(FadeIn(musicSource, timeToFade, maxVolume));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void CallFadeOut(AudioSource musicSource, float timeToFade) {

        StartCoroutine(FadeOut(musicSource, timeToFade));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    IEnumerator FadeIn(AudioSource musicSource, float timeToFade, float maxVolume) {

        // Set fading in to be true
        _FadingIn = true;
        // Store the current volume
        float startingVolume = 0f;

        // While the music volume is less than the max volume
        while (musicSource.volume < maxVolume) {

            // Set the music volume to this variable
            float percent = _CurrentFadeOutLerp / timeToFade;
            // Clamp the volume
            if (_CurrentFadeInLerp > timeToFade) { _CurrentFadeInLerp = timeToFade; }
            // Lerp volume
            musicSource.volume = Mathf.Lerp(startingVolume, maxVolume, percent);

            // Set fading out to false when the volume reaches the max
            if (musicSource.volume >= maxVolume) { _FadingIn = false; }

            yield return new WaitForSeconds(0.1f);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    IEnumerator FadeOut (AudioSource musicSource, float timeToFade) {
        
        // Set fading out to true
        _FadingOut = true;
        // Store the current volume
        float startingVolume = musicSource.volume;

        // While the music volume is more than 0
        while (musicSource.volume > 0f) {
            
            // Set the music volume to this variable
            float percent = _CurrentFadeOutLerp / timeToFade;
            // Clamp the volume
            if (_CurrentFadeOutLerp > timeToFade) { _CurrentFadeOutLerp = timeToFade; }
            // Lerp volume
            musicSource.volume = Mathf.Lerp(startingVolume, 0f, percent);

            // Set fading out to false when the volume reaches 0
            if (musicSource.volume <= 0) { _FadingOut = false; }

            yield return new WaitForSeconds(0.1f);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}