﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChangeState : MonoBehaviour {

    public Info_Level LevelInfo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BackToMenu()
    {
        InstanceManager.Instance._Difficulty = DifficultyManager.Instance._Difficulty;
        InstanceManager.Instance._Level = GameManager.Instance._Level;
        InstanceManager.Instance.SetLoadingType(true);
  
        //Load the "loading" scene
        ASyncLoading.Instance.LoadLevel(1);
        ASyncLoading.Instance.ActivateLevel();

        //Load the gameplay scene.
        ASyncLoading.Instance.LoadLevel(0);
    }

    public void RestartGame()
    {
        InstanceManager.Instance._Difficulty = DifficultyManager.Instance._Difficulty;
        InstanceManager.Instance._Level = GameManager.Instance._Level;
        InstanceManager.Instance.SetLoadingType(true);
       
        //Load the "loading" scene
        ASyncLoading.Instance.LoadLevel(1);
        ASyncLoading.Instance.ActivateLevel();
        ObjectPooling.DestroyAll();
        //Load the gameplay scene.
        ASyncLoading.Instance.LoadLevel(InstanceManager.Instance._Level.LevelIndex);
        GameManager.Instance.OnUnpause();

    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
