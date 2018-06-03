using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 2/6/2018
//
//******************************

public class HUD : MonoBehaviour {

    //******************************************************************************************************************************
    // INSPECTOR

    [Space]
    [Header("-----------------------------------")]
    [Header(" ")]
    public SelectionWheel SelectionWheel;
    public GameObject AbilitiesWheel;
    public GameObject SelectedPrefab;

    [Space]
    [Header("-----------------------------------")]
    [Header(" GUI TEXT COMPONENTS")]
    public Text PopulationText;
    public Text SuppliesCountText;
    public Text PowerCountText;
    public Text PlayerLevelText;

    //******************************************************************************************************************************
    // VARIABLES

    private Player _Player;

    //******************************************************************************************************************************
    // FUNCTIONS

    private void Start() {

        // Get component references
        _Player = GetComponent<Player>();

        ResourceManager.StoreSelectBoxItems(SelectedPrefab);
    }

    private void Update() {

        UpdateTextComponents();
    }

    private void UpdateTextComponents() {

        // Update player's army population count
        if (PopulationText != null && _Player != null)
            PopulationText.text = _Player.PopulationCount.ToString() + " / " + _Player.MaxPopulation.ToString();

        // Update player's supply count
        if (SuppliesCountText != null && _Player != null)
            SuppliesCountText.text = _Player.SuppliesCount.ToString();

        // Update player's power count
        if (PowerCountText != null && _Player != null)
            PowerCountText.text = _Player.PowerCount.ToString();

        // Update player's level
        if (PlayerLevelText != null && _Player != null)
            PlayerLevelText.text = _Player.Level.ToString();    
        
    }

    public GameObject FindHitObject() {

        // Create raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Return object from raycast
        if (Physics.Raycast(ray, out hit))
            return hit.collider.gameObject;

        return null;
    }

    public Vector3 FindHitPoint() {

        // Create raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Return hit point from raycast
        if (Physics.Raycast(ray, out hit))
            return hit.point;

        return ResourceManager.InvalidPosition;
    }

    public Rect GetPlayingArea() {  return new Rect(0, 0, Screen.width, Screen.height); }

    public bool MouseInBounds() {

        // Screen coordinates start in the lower-left corner of the screen
        // not the top-left of the screen like the drawing coordinates do
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width;
        bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height;

        return insideWidth && insideHeight;
    }

    public void SetAbilitiesWheel(bool show) {

        // Show/hide wheel
        if (AbilitiesWheel)            
            AbilitiesWheel.SetActive(show);
    }

    public bool WheelActive() { return SelectionWheel.transform.parent.gameObject.activeInHierarchy || AbilitiesWheel.transform.parent.gameObject.activeInHierarchy; }
    
}
