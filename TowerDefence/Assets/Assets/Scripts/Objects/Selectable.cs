using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TowerDefence;
using XInputDotNetPure;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/10/2018
//
//******************************

public class Selectable : Abstraction {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************
    
    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public bool _IsCurrentlySelected { get; set; }
    protected Player _Player = null;
    protected Bounds selectionBounds;
    protected bool _PlayerOwned = false;
    protected PlayerIndex _PlayerIndex = PlayerIndex.One;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

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
        var v = this;
    }

    public virtual void CalculateBounds() {}

    protected virtual void ChangeSelection(Selectable selectObj) { }

    protected virtual void DrawSelection(bool draw) { }

    protected virtual void DrawSelectionBox(Rect selectBox) { GUI.Box(selectBox, ""); }

    public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint) { }
    
    public void SetPlayer(Player player) {

        _PlayerOwned = true;
        _Player = player;
        _PlayerIndex = player._PlayerIndex;
    }

    public void SetSelection(bool selected) { _IsCurrentlySelected = selected; }

}