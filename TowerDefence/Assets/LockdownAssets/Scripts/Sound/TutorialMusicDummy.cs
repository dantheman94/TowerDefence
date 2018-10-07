using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//-=-=-=-=-=-=-=-=-=-
// Created by: Angus Secomb
// Last Edited: 8/10/18
// Editor: Angus Secomb
//-=-=-=-=-=-=-=-=-=-
public class TutorialMusicDummy : MonoBehaviour {

    // INSPECTOR
    /////////////////////////////////////////////////////////
    public AudioSource TutorialMusic;
    [Range(0,1)]
    public float MusicVolume;

    // FUNCTIONS
    /////////////////////////////////////////////////////////

    // Use this for initialization
    void Start () {
        TutorialMusic.Play();
        TutorialMusic.volume = MusicVolume;
	}

    /////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {
	    if(!TutorialMusic.isPlaying)
        {
            TutorialMusic.Play();
            
        }
	}

    /////////////////////////////////////////////////////////
}
