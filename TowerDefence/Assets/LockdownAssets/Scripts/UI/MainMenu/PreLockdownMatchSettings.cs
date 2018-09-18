using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 17/09/2018
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
    public Text LevelText = null;
    public Text DifficultyText = null;
    public Text FactionText = null;
    [Space]
    public Image LevelThumbnail = null;
    public Text TempLevelText = null;
    public Text TempDiffText = null;
    public Button StartMatchButton;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private InstanceManager.EFaction _Faction = InstanceManager.EFaction.Ethereal;
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
        DisableButton();

        // Update level text
        if (LevelText != null && _Level != null) { LevelText.text = _Level.LevelName.ToString(); }

        // Update level text
        if (DifficultyText != null && _Difficulty != null) { DifficultyText.text = _Difficulty.Difficulty.ToString(); }

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
    private void DisableButton()
    {
        if(_Level == null || _Difficulty == null)
        {
            StartMatchButton.interactable = false;
        }
        else if(_Level != null || _Difficulty != null)
        {
            StartMatchButton.interactable = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="faction"></param>
    public void SetFaction(int factionIndex) {

        _Faction = (InstanceManager.EFaction)factionIndex;
        InstanceManager.Instance._PlayerSettings[0].SetFaction(_Faction);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(Info_Level level) {

        _Level = level;
        InstanceManager.Instance._Level = _Level;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="difficulty"></param>
    public void SetDifficulty(Info_Difficulty difficulty) {

        _Difficulty = difficulty;
        InstanceManager.Instance._Difficulty = _Difficulty;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void StartMatch() {

        // Setup
        InstanceManager.Instance.SetLoadingType(true);

        // Load the "loading" scene
        ASyncLoading.Instance.LoadLevel(1);
        ASyncLoading.Instance.ActivateLevel();

        // Load the gameplay scene
        ASyncLoading.Instance.LoadLevel(_Level.LevelIndex);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetMapDefined() { return _Level != null; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetDifficultyDefined() { return _Difficulty != null; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetFactionDefined() { return _Faction == InstanceManager.EFaction.Ethereal; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}