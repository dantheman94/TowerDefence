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

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private bool _Flashing = false;
    private Color _CurrentFlashColour = Color.white;

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
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void OnWaveComplete() {

        // Start text flashing
        _Flashing = true;

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

        ///Player player = GameManager.Instance.Players[0];
        ///ResourceManager rm = player.gameObject.GetComponent<ResourceManager>();
        ///rm.ResourceBoostComplete(ResourceBoostAdditiveSupply, ResourceBoostAdditivePower);

        _Flashing = false;
        _CurrentFlashColour = FlashStartingColour;
        gameObject.SetActive(false);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
