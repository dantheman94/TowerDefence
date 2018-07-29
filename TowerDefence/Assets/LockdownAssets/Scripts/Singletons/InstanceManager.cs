using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 29/7/2018
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

    public enum EFaction { EtherealNibbas, Faction2, Faction3, Faction4 }

    public static InstanceManager Instance;
    
    public Info_Level _Level { get; set; }
    public Info_Difficulty _Difficulty { get; set; }
    
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

}