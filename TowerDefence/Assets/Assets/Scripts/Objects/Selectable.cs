using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/10/2018
//
//******************************

public class Selectable : MonoBehaviour {

    //******************************************************************************************************************************
    // INSPECTOR

    [Space]
    [Header("-----------------------------------")]
    [Header(" SELECTABLE PROPERTIES")]
    public string ObjectName;

    //******************************************************************************************************************************
    // VARIABES

    protected Player _Player = null;
    public bool _IsCurrentlySelected { get; set; }
    protected Bounds selectionBounds;

    //******************************************************************************************************************************
    // FUNCTIONS

    protected virtual void Awake() {

        selectionBounds = ResourceManager.InvalidBounds;
        CalculateBounds();
    }

    protected virtual void Start() {

        GameManager.Instance.Selectables.Add(this);
    }

    protected virtual void Update() { }

    protected virtual void OnGUI() {

        // Update selection
        DrawSelection(_IsCurrentlySelected);
    }

    public virtual void CalculateBounds() {}

    protected virtual void ChangeSelection(Selectable selectObj) { }

    protected virtual void DrawSelection(bool draw) { }

    protected virtual void DrawSelectionBox(Rect selectBox) { GUI.Box(selectBox, ""); }

    public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint) { }
    
    public void SetPlayer(Player player) { _Player = player; }

    public void SetSelection(bool selected) { _IsCurrentlySelected = selected; }

}