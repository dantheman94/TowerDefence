using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Angus Secomb
//  Last edited on: 30/10/18
//
//******************************

public class InstanceManager : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" PLAYERS")]
    [Space]
    public UserSettings[] _PlayerSettings;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    public enum EFaction { Ethereal, Faction2, Faction3, Faction4 }

    public static InstanceManager Instance;
    
    public Info_Level _Level { get; set; }
    public Info_Difficulty _Difficulty { get; set; }

    public bool _LoadingGameplay { get; private set; }

    public string PlayerName;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  This is called before Startup().
    /// </summary>
    private void Awake() {

        // Initialize singleton
        if (Instance != null && Instance != this) {

            Destroy(this.gameObject);
            return;
        }

        

        Instance = this;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="gameplay"></param>
    public void SetLoadingType(bool gameplay) { _LoadingGameplay = gameplay; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}