using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/7/2018
//
//******************************

public class UI_WaveStats : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" WIDGET COMPONENTS")]
    [Space]
    public Text WaveName = null;
    public Text WavePopulation = null;
    public Text WaveCounterText = null;
    public Slider WaveSlider = null;
    [Space]
    public Text DamageModiferText = null;
    public Text HealthModiferText = null;
    [Space]
    public Text NextWaveTimerText = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" FLASHING TEXT")]
    [Space]
    public List<UnityEngine.UI.Outline> OutlineComponents = null;
    [Space]
    public float FlashingTextSpeed = 1f;
    public Color FlashStartingColour = Color.white;
    public Color FlashEndColour = Color.red;

    //******************************************************************************************************************************
    //
    //      VARIALBES
    //
    //******************************************************************************************************************************

    private int _CurrentWavePopLeft = 0;
    private int _CurrentWavePopMax = 0;

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

        // Update wave name text
        if (WaveName != null) { WaveName.text = string.Concat(WaveManager.Instance.GetCurrentWaveInfo().Name + " "); }

        // Update modifiers text
        if (DamageModiferText != null) { DamageModiferText.text = string.Concat(WaveManager.Instance.GetCurrentDamageModifer().ToString("0.00") + "%"); }
        if (HealthModiferText != null) { HealthModiferText.text = string.Concat(WaveManager.Instance.GetCurrentHealthModifer().ToString("0.00") + "%"); }

        // Update wave counter text
        if (WaveCounterText != null) { WaveCounterText.text = WaveManager.Instance.GetWaveCount().ToString(); }

        // Update wave population text
        if (WavePopulation != null) { WavePopulation.text = string.Concat(_CurrentWavePopLeft.ToString() + " / " + _CurrentWavePopMax.ToString()); }

        // Update wave slider population
        if (WaveSlider != null) {

            float percent;
            if (_CurrentWavePopLeft > 0f) { percent = (float)_CurrentWavePopLeft / (float)_CurrentWavePopMax; }
            else { percent = 0f; }

            WaveSlider.value = percent;
        }

        // Update next wave timer text
        if (!WaveManager.Instance.IsBossWave()) {

            // Not a boss wave - update next wave timer
            int minutes = Mathf.FloorToInt(WaveManager.Instance.GetTimeTillNextWave() / 60f);
            int seconds = Mathf.FloorToInt(WaveManager.Instance.GetTimeTillNextWave() - minutes * 60);
            string timeText = string.Format("{0:0}:{1:00} ", minutes, seconds);
            if (NextWaveTimerText != null) { NextWaveTimerText.text = timeText; }
        }

        // Is boss wave so dont show the timer
        else { if (NextWaveTimerText != null) { NextWaveTimerText.text = ""; } }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void UpdatePopulationCount(int current, int max) {

        _CurrentWavePopLeft = current;
        _CurrentWavePopMax = max;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void DeductLifeFromCurrentPopulation() { _CurrentWavePopLeft--; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}