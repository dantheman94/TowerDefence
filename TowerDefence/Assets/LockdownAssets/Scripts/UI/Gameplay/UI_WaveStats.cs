using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 26/10/2018
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

    [Space]
    [Header("-----------------------------------")]
    [Header(" SCORE TEXT")]
    [Space]
    public Text ScoreAdditiveText = null;
    public Text ScoreTypeText = null;
    [Space]
    public float ScoreStartingPositionX = -600f;
    public float ScoreEndingPositionX = -240f;
    public float ScoreStartingPositionY = -600f;
    public float ScoreEndingPositionY = -200f;
    [Space]
    public float TypeStartingPositionX = -425f;
    public float TypeEndingPositionX = -240f;
    public float TypeStartingPositionY = -425f;
    public float TypeEndingPositionY = -200f;
    [Space]
    public float LerpDuration = 1f;
    [Space]
    public Color AdditiveColour = Color.green;
    public Color SubtractiveColour = Color.red;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private int _CurrentWavePopLeft = 0;
    private int _CurrentWavePopMax = 0;

    private GameObject _ScorePanelBeingDisplayed = null;
    private GameObject _ScoreTypeBeingDisplayed = null;
    private bool _Lerping = false;

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
    
    /// <summary>
    //  Displays a score + OR - for a few seconds, with a description.
    /// </summary>
    public void ScoreMessage(bool add, int amount, Player.ScoreType scoreType) {

        if (ScoreAdditiveText != null && ScoreTypeText != null) {

            // Remove current score widget
            if (_ScorePanelBeingDisplayed != null && _ScoreTypeBeingDisplayed != null) {

                _Lerping = false;
                StopCoroutine(ScoreMessage(_ScorePanelBeingDisplayed, _ScoreTypeBeingDisplayed, add, amount, scoreType));

                Destroy(_ScorePanelBeingDisplayed.gameObject);
                Destroy(_ScoreTypeBeingDisplayed.gameObject);
                _ScorePanelBeingDisplayed = null;
                _ScoreTypeBeingDisplayed = null;
            }

            // Create score widget
            if (_ScorePanelBeingDisplayed == null && _ScoreTypeBeingDisplayed == null) {

                _ScorePanelBeingDisplayed = Instantiate(ScoreAdditiveText.gameObject);
                _ScorePanelBeingDisplayed.gameObject.SetActive(true);
                _ScorePanelBeingDisplayed.transform.SetParent(transform);

                _ScoreTypeBeingDisplayed = Instantiate(ScoreTypeText.gameObject);
                _ScoreTypeBeingDisplayed.gameObject.SetActive(true);
                _ScoreTypeBeingDisplayed.transform.SetParent(transform);

                StartCoroutine(ScoreMessage(_ScorePanelBeingDisplayed, _ScoreTypeBeingDisplayed, add, amount, scoreType));
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  IEnumerator
    /// </returns>
    private IEnumerator ScoreMessage(GameObject panel, GameObject type, bool add, int amount, Player.ScoreType scoreType) {

        RectTransform rect = _ScorePanelBeingDisplayed.GetComponent<RectTransform>();
        Text text = panel.GetComponent<Text>();
        rect.anchoredPosition = new Vector3(ScoreStartingPositionX, ScoreStartingPositionY);

        RectTransform typerect = _ScoreTypeBeingDisplayed.GetComponent<RectTransform>();
        Text texttype = type.GetComponent<Text>();
        typerect.anchoredPosition = new Vector3(TypeStartingPositionX, TypeStartingPositionY);
        
        string sType = "";
        switch (scoreType) {
            case Player.ScoreType.EnemyKilled:

                sType = "Unit Killed";
                break;

            case Player.ScoreType.BaseDestroyed:

                sType = "Base Destroyed";
                break;

            case Player.ScoreType.BuildingDestroyed:

                sType = "Building Destroyed";
                break;

            case Player.ScoreType.SpireDestroyed:

                sType = "Spire Lost";
                break;

            case Player.ScoreType.UpgradedBase:
                
                sType = "Upgraded Base";
                break;

            case Player.ScoreType.BuildingBuilt:

                sType = "Building Constructed";
                break;

            case Player.ScoreType.WaveDefeated:

                sType = "Wave Defeated";
                break;
                
            default: break;
        }

        // Set score message properties
        if (add) {

            text.text = string.Concat("+" + amount.ToString());
            text.color = AdditiveColour;

            texttype.text = sType;
            texttype.color = AdditiveColour;
        }
        else {

            text.text = string.Concat("-" + amount.ToString());
            text.color = SubtractiveColour;

            texttype.text = sType;
            texttype.color = SubtractiveColour;
        }

        // Delay and lerp the positions
        _Lerping = true;
        float t = 0f;
        while (t <= 1f && _Lerping) {

            t += Time.deltaTime / LerpDuration;

            rect.anchoredPosition = Vector2.Lerp(new Vector2(ScoreStartingPositionX, ScoreStartingPositionY), new Vector2(ScoreEndingPositionX, ScoreEndingPositionY), t);
            typerect.anchoredPosition = Vector2.Lerp(new Vector2(TypeStartingPositionX, TypeStartingPositionY), new Vector2(TypeEndingPositionX, TypeEndingPositionY), t);
            yield return null;
        }

        // Remove widget
        _ScorePanelBeingDisplayed = null;
        _ScoreTypeBeingDisplayed = null;
        Destroy(panel);
        Destroy(type);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////f

}