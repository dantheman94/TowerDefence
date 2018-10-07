using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMusicDummy : MonoBehaviour {

    public AudioSource TutorialMusic;
    [Range(0,1)]
    public float MusicVolume;

	// Use this for initialization
	void Start () {
        TutorialMusic.Play();
        TutorialMusic.volume = MusicVolume;
	}
	
	// Update is called once per frame
	void Update () {
	    if(!TutorialMusic.isPlaying)
        {
            TutorialMusic.Play();
            
        }
	}
}
