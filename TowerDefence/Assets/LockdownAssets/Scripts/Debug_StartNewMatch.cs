﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_StartNewMatch : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if (Input.GetKeyDown(KeyCode.Space)) {

            WaveManager.Instance.StartNewMatch();
        }
	}
}
