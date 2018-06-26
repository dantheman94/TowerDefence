using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class Tier : MonoBehaviour {

    //***************************************************************
    // VARIABLES

    public int iTier = 1;
    public UnityEvent[] _OnLevelUp;

    private int _Experience = 0;
    private int _ExperienceTarget = 10;

    //***************************************************************
    // FUNCTIONS
    
    public int GetExperience()         { return _Experience; }
    public void AddExperience(int add) {

        _Experience += add;
        if (_Experience >= _ExperienceTarget) {

            _OnLevelUp[iTier - 1].Invoke();
        }
    }
    
}