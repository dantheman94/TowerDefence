using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Angus Secomb
//  Last edited on: 14/11/2018
//
//******************************

public class SoundManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SOUND MANAGER PROPERTIES")]
    [Space]
    public bool PlayMusicOnStart = true;

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
    private GameObject _MusicObject;
    private AudioSource _MusicSource;

    private bool _FadingIn;
    private bool _FadingOut;
    private float _CurrentFadeInLerp = 0f;
    private float _CurrentFadeOutLerp = 0f;

    private const int _TrackCount = 7;
    private bool[]  _TracksPlayed = new bool[_TrackCount] { false, false, false, false, false, false, false };
    private string[] _TrackPath = new string[_TrackCount] { "Music/_Music_Morning_Adventure",
                                                            "Music/_Music_Main1",
                                                            "Music/_Music_Main2",
                                                            "Music/_Music_Main2_Alt",
                                                            "Music/_Music_Enter_The_Dungeon",
                                                            "Music/_Music_The_Revolution",
                                                            "Music/_Music_With_Gun_And_Crucifix" };
    private int _PreviousTrackIndex;
    private AudioSource _CurrentDialogue = null;
    public bool _DialogueIsPlaying = false;

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
        _VoxelWaitingList = new List<AudioSource>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Start() {

        // Play starting track if specified
        if (PlayMusicOnStart) { PlayStartTrack(); }
        PlayAmbience();
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
        PlayRandomTrack();

        // Updating current dialogue
        if (_CurrentDialogue != null) { _DialogueIsPlaying = _CurrentDialogue.isPlaying; }
        else { _DialogueIsPlaying = false; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  A coroutine that waits a few seconds before despawning the sound gameobject reference
    /// </summary>
    /// <returns></returns>
    public IEnumerator DestroySound(AudioSource sound, float delayTime) {

        // Delay
        float normalizedTime = 0f;
        while (normalizedTime <= 1f) {

            normalizedTime += Time.deltaTime / delayTime;
            yield return null;
        }

        // Despawn
        ObjectPooling.Despawn(sound.gameObject);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public AudioSource GetSound(string soundLocation, float pitchMin, float pitchMax) {

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
        
        // Add the sound object to the List
        _Sounds.Add(soundSource);

        return soundSource;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public AudioSource PlaySound(string soundLocation, float pitchMin, float pitchMax, bool unitSound = true) {

        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("MainMenu") || !unitSound) {

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

            return soundSource;
        }
        else { return null; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public AudioSource PlaySoundAtLocation(string soundLocation, Vector3 position, float pitchMin, float pitchMax, bool unitSound = true) {

        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("MainMenu") || !unitSound) {

            // Create pooled game object for the sound
            GameObject soundObj = ObjectPooling.Spawn(Resources.Load<GameObject>(soundLocation));

            // Set 3d position of the sound
            soundObj.transform.position = position;

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

            return soundSource;
        }
        else { return null; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Play the first music track.
    /// </summary>
    public void PlayFirstTrack(string musicLocation) {

        // Create pooled game object for the music
        GameObject musicObj = ObjectPooling.Spawn(Resources.Load<GameObject>(musicLocation));
        // Grab the source for the music to play from
        AudioSource musicSource = musicObj.GetComponent<AudioSource>();

        // Play the music
        musicSource.Play();
        // Set track to true to indicate its been played
        _TracksPlayed[0] = true;

        // Start coroutine
        StartCoroutine(UpdateTracks(musicSource));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Plays the first track.
    /// </summary>
    public void PlayStartTrack()
    {
        _MusicObject = ObjectPooling.Spawn(Resources.Load<GameObject>("Music/_Music_Morning_Adventure"));
        _MusicSource = _MusicObject.GetComponent<AudioSource>();

        _TracksPlayed[0] = true;
        _PreviousTrackIndex = 0;
        if(!_MusicSource.isPlaying)
        _MusicSource.Play();
    }

    public void PlayRandomTrack()
    {
        if (_MusicSource.time >= _MusicSource.clip.length)
        {
            bool check = false;
           
            while (!check)
            {
                int randIndex = Random.Range(0, 6);
                if(_PreviousTrackIndex != randIndex)
                {
                    GameObject newMusicObject = Resources.Load<GameObject>(_TrackPath[randIndex]);
                    _MusicSource.clip = newMusicObject.GetComponent<AudioSource>().clip;
                    _MusicSource.Play();
                    _PreviousTrackIndex = randIndex;
                    check = true;
                }
            }
        }
    }

    public void PlayAmbience()
    {
        GameObject AmbientObject = ObjectPooling.Spawn(Resources.Load<GameObject>("Music/Ambient_Sound"));
    }

    /// <summary>
    // Play the second to fourth music tracks.
    /// </summary>
    public void PlayTrack() {

        // Reset song played counter
        int playedCount = 0;

        // Resets all the tracks to playable again if all the tracks have been played
        for (int i = 0; i < _TracksPlayed.Length - 1; i++) {

            if (_TracksPlayed[i] == true) { playedCount++; }
        }

        // Reset the tracks to false so they can be played again
        if (playedCount == _TrackCount) {

            for (int i = 0; i < _TracksPlayed.Length - 1; i++) {

                _TracksPlayed[i] = false;
            }

            // Set the first track to true so it only ever gets played once
            _TracksPlayed[0] = true;
        }

        // Set to false to find a new track to play
        bool foundTrack = false;

        // Searches for a new track to play
        while(foundTrack) {

            // Generate random number
            int i = Random.Range(1, _TrackCount);

            // If the track found hasn't been played
            if (_TracksPlayed[i] == false) {

                // Create pooled game object for the music
                GameObject musicObj = ObjectPooling.Spawn(Resources.Load<GameObject>(_TrackPath[i]));
                // Grab the source for the music to play from
                AudioSource musicSource = musicObj.GetComponent<AudioSource>();

                Debug.Assert(_TrackPath == null, "_TrackPath is null, check file paths.");

                // Play the music
                musicSource.Play();

                // Set to true
                foundTrack = true;

                // Set track to true to indicate its been played
                _TracksPlayed[i] = true;

                // Start coroutine
                StartCoroutine(UpdateTracks(musicSource));
                break;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Update the music playing.
    /// </summary>
    public IEnumerator UpdateTracks(AudioSource track)
    {
        // Check if the current music playing is finished
        yield return new WaitUntil(() => !track.isPlaying);

        // If so, play the next
        PlayTrack();
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

    public void PlayAnnouncerTrack(string soundLocation) {

        // Stop any dialogue thats currently playing
        if (_CurrentDialogue != null) { _CurrentDialogue.Stop(); }

        // Play announcer track
        AudioSource sound = PlaySound(soundLocation, 1f, 1f, false);
        _CurrentDialogue = sound;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="source"></param>
    public void SetCurrentDialogue(AudioSource sound) { _CurrentDialogue = sound; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}