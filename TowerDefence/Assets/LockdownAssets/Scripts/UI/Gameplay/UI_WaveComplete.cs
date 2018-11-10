using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 17/10/2018
//
//******************************

public class UI_WaveComplete : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" RESOURCE BOOST")]
    [Space]
    public float ResourceBoostTime = 10;
    public int ResourceBoostAdditiveSupply = 2;
    public int ResourceBoostAdditivePower = 2;

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
    [Header(" FADING WIDGET")]
    [Space]
    public Text WaveCompleteTitle = null;
    public Text ResourceAddedText = null;
    public Image LineHorizontal = null;
    [Space]
    public float FadeDuration = 0.5f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _Flashing = false;
    private Color _CurrentFlashColour = Color.white;

    private bool _FadingOut = false;
    private bool _FadingIn = false;
    private float _FadeProgress = 0f;
    private Color _FadeColour;

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
        
        if (_Flashing) {

            // Lerp between Colour A & B
            _CurrentFlashColour = Color.Lerp(FlashStartingColour, FlashEndColour, (Mathf.Sin(Time.time * FlashingTextSpeed) + 1) / 2f);

            // Set the outline component(s) colour
            for (int i = 0; i < OutlineComponents.Count; i++) { OutlineComponents[i].effectColor = _CurrentFlashColour; }
        }

        // Widget fading in sequence
        if (_FadingIn) {

            // Lerp the widget components into full alpha
            _FadeProgress -= Time.deltaTime / FadeDuration;
            _FadeColour = Color.Lerp(Color.white, Color.clear, _FadeProgress);

            if (WaveCompleteTitle != null) { WaveCompleteTitle.color = _FadeColour; }
            if (ResourceAddedText != null) { ResourceAddedText.color = _FadeColour; }
            if (LineHorizontal != null) { LineHorizontal.color = _FadeColour; }

            // Fade in complete
            if (_FadeColour == Color.white) {

                _FadingIn = false;
                _FadeProgress = 0f;
            }
        }

        // Widget fading out sequence
        if (_FadingOut) {

            // Lerp the widget components into full transparency
            _FadeProgress += Time.deltaTime / FadeDuration;
            _FadeColour = Color.Lerp(Color.white, Color.clear, _FadeProgress);

            if (WaveCompleteTitle != null) { WaveCompleteTitle.color = _FadeColour; }
            if (ResourceAddedText != null) { ResourceAddedText.color = _FadeColour; }
            if (LineHorizontal != null) { LineHorizontal.color = _FadeColour; }

            // Fade out complete
            if (_FadeColour == Color.clear) {

                _FadingOut = false;
                _FadeProgress = 0f;
                gameObject.SetActive(false);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnWaveComplete() {

        // Reset center message colours
        _FadeColour = Color.white;
        if (WaveCompleteTitle != null) { WaveCompleteTitle.color = _FadeColour; }
        if (ResourceAddedText != null) { ResourceAddedText.color = _FadeColour; }
        if (LineHorizontal != null) { LineHorizontal.color = _FadeColour; }

        // Start text flashing
        _Flashing = true;
        _FadingIn = true;
        _FadeProgress = 1f;

        // Boost resources
        Player player = GameManager.Instance.Players[0];
        ResourceManager rm = player.gameObject.GetComponent<ResourceManager>();
        StartCoroutine(rm.ResourceBoost(ResourceBoostTime, ResourceBoostAdditiveSupply, ResourceBoostAdditivePower));

        // Update stat
        player.AddToScore(WaveManager.Instance.ScoreGrantedOnWaveDefeated, Player.ScoreType.WaveDefeated);

        // Hide widget after boost is complete
        StartCoroutine(DelayedWidgetHide(ResourceBoostTime));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns>
    //  IEnumerator
    /// </returns>
    public IEnumerator DelayedWidgetHide(float seconds) {

        yield return new WaitForSeconds(seconds);

        _Flashing = false;
        _CurrentFlashColour = FlashStartingColour;
        for (int i = 0; i < OutlineComponents.Count; i++) { OutlineComponents[i].effectColor = _CurrentFlashColour; }
        _FadingOut = true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}
