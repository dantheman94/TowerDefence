using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 29/7/2018
//
//******************************

public class PreLockdownMatchSettings : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" COMPONENTS")]
    [Space]
    public Text FactionText = null;
    [Space]
    public Image LevelThumbnail = null;
    public Text TempLevelText = null;
    public Image DifficultyThumbnail = null;
    public Text TempDiffText = null;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public enum EFaction { EtherealNibbas, Faction2, Faction3, Faction4 }

    private EFaction _Faction = EFaction.EtherealNibbas;
    private Info_Level _Level = null;
    private Info_Difficulty _Difficulty = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        
        // Update faction text
        if (FactionText != null) { FactionText.text = _Faction.ToString(); }

        // Update level thumbnail
        if (LevelThumbnail != null && _Level != null) { LevelThumbnail.sprite = _Level.LevelThumbnailSprite; }

        // Update temp level text
        if (TempLevelText != null && _Level != null) { TempLevelText.text = _Level.LevelName; }

        // Update temp diff text
        if (TempDiffText != null && _Difficulty != null) { TempDiffText.text = _Difficulty.Difficulty.ToString(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="faction"></param>
    public void SetFaction(int factionIndex) { _Faction = (EFaction)factionIndex; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(Info_Level level) { _Level = level; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="difficulty"></param>
    public void SetDifficulty(Info_Difficulty difficulty) { _Difficulty = difficulty; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}