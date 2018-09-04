using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_MatchStart : MonoBehaviour {

    private bool _Init = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (_Init == false) {

            if (Input.GetKeyDown(KeyCode.Space)) {

                WaveManager.Instance.CinematicOpening.StartCinematic();
            }
        }
	}
}
