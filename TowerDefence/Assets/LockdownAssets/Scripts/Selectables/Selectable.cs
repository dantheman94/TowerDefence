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
//  Last edited on: 19/7/2018
//
//******************************

public class Selectable : Abstraction {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SELECTABLE PROPERTIES")]
    [Space]
    public bool ShowSelectionGUI = true;
    [Tooltip("The team associated with this object.")]
    public GameManager.Team Team;
    [Tooltip("The controller/player reference attached to this object.")]
    public Player _Player = null;
    [Space]
    [Tooltip("The radius of the Fog Of War sphere attached to this object.")]
    public float FogOfWarRadius = 400f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected bool _IsCurrentlyHighlighted;
    protected bool _IsCurrentlySelected;
    protected Bounds selectionBounds;
    protected bool _PlayerOwned = false;
    protected GameObject _SelectionObj = null;
    protected GameObject _HighlightObj = null;
    private Renderer _SelectionObjRenderer = null;
    private Renderer _HighlightObjRenderer = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  This is called before Startup().
    /// </summary>
    protected virtual void Awake() {

        selectionBounds = Settings.InvalidBounds;
        CalculateBounds();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    protected virtual void Start() {

        GameManager.Instance.Selectables.Add(this);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected virtual void Update() { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void OnGUI() {

        // Update selection
        DrawSelection(_IsCurrentlySelected);
        DrawHighlight(_IsCurrentlyHighlighted);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public virtual void CalculateBounds() {}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="selectObj"></param>
    protected virtual void ChangeSelection(Selectable selectObj) {

        // This should be called by the following line, but there is an outside chance it will not
        SetIsSelected(false);

        // Clear the world objects selection list
        foreach (var obj in _Player.SelectedWorldObjects) { obj.SetIsSelected(false); }
        _Player.SelectedWorldObjects.Clear();

        // Add new selection to the list
        _Player.SelectedWorldObjects.Add(selectObj);
        selectObj.SetIsSelected(true);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="draw"></param>
    protected virtual void DrawSelection(bool draw) {

        // Show selection
        if (draw) {

            // Show selection prefab at the bottom of the object
            if (_SelectionObj == null) { _SelectionObj = Instantiate(Settings.SelectBoxObjects); }
            if (_SelectionObj != null) {

                // Display prefab
                _SelectionObj.SetActive(true);

                // Update selection prefab position
                Vector3 pos = new Vector3();
                pos.x = transform.position.x;
                pos.y = 1.1f;
                pos.z = transform.position.z;
                _SelectionObj.transform.position = pos;

                // Update selection prefab colour
                if (_SelectionObjRenderer == null) { _SelectionObjRenderer = _SelectionObj.GetComponent<Renderer>(); }
                if (_SelectionObjRenderer != null) {

                    switch (Team) {
                        case GameManager.Team.Undefined: { _SelectionObjRenderer.material.color = Color.black; break; }
                        case GameManager.Team.Defending: { _SelectionObjRenderer.material.color = _Player.TeamColor; break; }
                        case GameManager.Team.Attacking: { _SelectionObjRenderer.material.color = Color.red; break; }
                        default: break;
                    }
                }
            }
        }

        // Hide selection
        else { if (_SelectionObj != null) { Destroy(_SelectionObj.gameObject); } }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="highlight"></param>
    protected virtual void DrawHighlight(bool highlight) {

        // Show highlight
        if (highlight) {

            // Show highlight prefab at the bottom of the object
            if (_HighlightObj == null) { _HighlightObj = Instantiate(Settings.HighlightBoxObjects); }
            if (_HighlightObj != null) {

                // Display prefab
                _HighlightObj.SetActive(true);

                // Update highlight prefab position
                Vector3 pos = new Vector3();
                pos.x = transform.position.x;
                pos.y = 1.1f;
                pos.z = transform.position.z;
                _HighlightObj.transform.position = pos;

                // Update highlight prefab colour
                if (_HighlightObjRenderer == null) { _HighlightObjRenderer = _HighlightObj.GetComponent<Renderer>(); }
                if (_HighlightObjRenderer != null) {

                    switch (Team) {
                        case GameManager.Team.Undefined: { _HighlightObjRenderer.material.color = Color.grey; break; } /// Temporary colour - just so it isnt white (will need to be white later but it clashes with the environment atm)
                        case GameManager.Team.Defending: { _HighlightObjRenderer.material.color = Color.cyan; break; } /// Temporary colour - just to show a different colour to selected (ideally it should be a shade lighter than the player's colour!)
                        case GameManager.Team.Attacking: { _HighlightObjRenderer.material.color = Color.red; break; }
                        default: break;
                    }
                }
            }
        }

        // Hide highlight
        else { if (_HighlightObj != null) { Destroy(_HighlightObj.gameObject); } }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectBox"></param>
    protected virtual void DrawSelectionBox(Rect selectBox) { GUI.Box(selectBox, ""); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hitObject"></param>
    /// <param name="hitPoint"></param>
    public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint) {

        // Only handle input if currently selected
        if (_IsCurrentlySelected && hitObject && hitObject.name != "Ground") {

            // Cast hit object to selectable
            Selectable selectable = hitObject.transform.root.GetComponent<Selectable>();

            if (selectable)
                ChangeSelection(selectable);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayer(Player player) {

        _PlayerOwned = true;
        _Player = player;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selected"></param>
    public void SetIsSelected(bool selected) { _IsCurrentlySelected = selected; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetIsSelected() { return _IsCurrentlySelected; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <param name="highlighted"></param>
    public void SetIsHighlighted(bool highlighted) { _IsCurrentlyHighlighted = highlighted; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetIsHighlighted() { return _IsCurrentlyHighlighted; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}