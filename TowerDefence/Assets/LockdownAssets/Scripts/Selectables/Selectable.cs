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
//  Last edited on: 18/9/2018
//
//******************************

public class Selectable : Abstraction {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header(" SELECTABLE PROPERTIES")]
    [Header("-----------------------------------")]
    [Space]
    [Tooltip("The team associated with this object.")]
    public GameManager.Team Team;
    [Tooltip("The local player reference attached to this object.")]
    public Player _Player = null;
    [Space]
    [Tooltip("When the player clicks on this object, does the selection wheel (Radial or box) display?")]
    public bool ShowSelectionGUI = true;
    public bool ShowOutline = true;
    public float OutlineHighlightedWidth = 2f;
    public float OutlineSelectedWidth = 3f;
    [Space]
    public GameObject TargetPoint = null;

    [Space]
    [Header(" SHOWCASE PROPERTIES")]
    [Header("-----------------------------------")]
    [Space]
    public float ShowcaseOffsetY = 0f;
    public float ShowcaseFOV = 40f;
    public float ShoecaseScale = 1f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************
    
    protected Bounds selectionBounds;
    protected bool _IsCurrentlyHighlighted;
    protected bool _IsCurrentlySelected;
    protected bool _ForceHighlightOutlineDraw = false;
    protected bool _ForceSelectOutlineDraw = false;
    private Outline _OutlineComponent = null;
    private Color _HighlightingOutlineColour = Color.black;
    private Color _SelectedOutlineColour = Color.black;

    private FogUnit _FogVision = null;

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

        // Get components
        _FogVision = GetComponent<FogUnit>();
        _OutlineComponent = GetComponent<Outline>();
        if (_OutlineComponent == null) { _OutlineComponent = GetComponentInChildren<Outline>(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    protected virtual void Start() {

        if (GameManager.Instance != null) { GameManager.Instance.Selectables.Add(this); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected virtual void Update() {

        // Only enable fog vision if this unit is a defending unit (player team friendly)
        if (_FogVision != null) { _FogVision.enabled = Team == GameManager.Team.Defending; }
    }

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
        foreach (var obj in _Player.SelectedUnits) { obj.SetIsSelected(false); }
        _Player.SelectedUnits.Clear();

        // Add new selection to the list
        _Player.SelectedWorldObjects.Add(selectObj);
        selectObj.SetIsSelected(true);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="draw"></param>
    public virtual void DrawSelection(bool draw) {

        // Show selection
        if ((draw || _ForceSelectOutlineDraw) && ShowOutline && _OutlineComponent != null) {

            // Black is used as an undefined colour
            if (_SelectedOutlineColour == Color.black) {

                // Get the selected colour based on the object's team
                Color col = new Color();
                switch (Team) {

                    case GameManager.Team.Undefined: { col = Color.white; break; }
                    case GameManager.Team.Defending: { col = _Player.TeamColor; break; }
                    case GameManager.Team.Attacking: { col = WaveManager.Instance.AttackingTeamColour; break; }
                    default: break;
                }
                _SelectedOutlineColour = col;
            }

            // Set outline properties
            _OutlineComponent.OutlineColor = _SelectedOutlineColour;
            _OutlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
            _OutlineComponent.OutlineWidth = OutlineSelectedWidth;
            _OutlineComponent.enabled = true;
        }

        // Hide selection
        else { if (_OutlineComponent != null && !_IsCurrentlyHighlighted) { _OutlineComponent.enabled = false; } }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="highlight"></param>
    public virtual void DrawHighlight(bool highlight) {

        // Show selection
        if ((highlight || _ForceHighlightOutlineDraw) && ShowOutline && _OutlineComponent != null) {

            // Black is used as an undefined colour
            if (_HighlightingOutlineColour == Color.black) {
                
                Color col = new Color();
                switch (Team) {

                    case GameManager.Team.Undefined: { col = Color.white; break; }
                    case GameManager.Team.Defending: { col = _Player.TeamColor; break; }
                    case GameManager.Team.Attacking: { col = WaveManager.Instance.AttackingTeamColour; break; }
                    default: break;
                }

                // Slightly darker shade for the highlighting colour
                col.r -= 0.1f;
                col.g -= 0.1f;
                col.b -= 0.1f;
                col.a /= 2f;

                _HighlightingOutlineColour = col;
            }

            // Set outline properties
            _OutlineComponent.OutlineColor = _HighlightingOutlineColour;
            _OutlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
            _OutlineComponent.OutlineWidth = OutlineHighlightedWidth;
            _OutlineComponent.enabled = true;
        }

        // Hide selection
        else { if (_OutlineComponent != null && !_IsCurrentlySelected) { _OutlineComponent.enabled = false; } }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="selectBox"></param>
    protected virtual void DrawSelectionBox(Rect selectBox) { GUI.Box(selectBox, ""); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
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
    //  
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayer(Player player) {
        
        _Player = player;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="selected"></param>
    public void SetIsSelected(bool selected) {

        _IsCurrentlySelected = selected;
        if (_IsCurrentlySelected) { SetIsHighlighted(false); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetIsSelected() { return _IsCurrentlySelected; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="highlighted"></param>
    public void SetIsHighlighted(bool highlighted) { _IsCurrentlyHighlighted = highlighted; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool GetIsHighlighted() { return _IsCurrentlyHighlighted; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //
    /// </summary>
    /// <param name="visible"></param>
    public void SetOutlineVisibility(bool visible) { _OutlineComponent.enabled = visible; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="value"></param>
    public void SetForceHighlightOutline(bool value) { _ForceHighlightOutlineDraw = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="value"></param>
    public void SetForceSelectOutline(bool value) { _ForceSelectOutlineDraw = value; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}