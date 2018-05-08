using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/8/2018
//
//******************************

public class WorldObject : MonoBehaviour {

    //******************************************************************************************************************************
    // INSPECTOR

    [Space]
    [Header("-----------------------------------")]
    [Header(" WORLD OBJECT PROPERTIES")]
    public string ObjectName;
    public Texture2D BuildImage;
    public int CostSupplies = 0;
    public int CostPower = 0;
    public int CostPlayerLevel = 1;
    public int RecycleSupplies = 0;
    public int RecyclePower = 0;
    public int MaxHitPoints = 100;

    //******************************************************************************************************************************
    // VARIABES

    protected Player _Player = null;
    protected Bounds selectionBounds;
    protected bool _IsCurrentlySelected { get; set; }
    protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

    //******************************************************************************************************************************
    // FUNCTIONS

    protected virtual void Awake() {

        selectionBounds = ResourceManager.InvalidBounds;
        CalculateBounds();
    }

    protected virtual void Start() {

    }

    protected virtual void Update() {

    }

    protected virtual void OnGUI() {

        if (_IsCurrentlySelected)
            DrawSelection();
    }

    public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player player) {

        // Only handle input if currently selected
        if (_IsCurrentlySelected && hitObject && hitObject.name != "Ground") {
            WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();

            // Clicked on another selectable object
            if (worldObject)
                ChangeSelection(worldObject, player);
        }
    }
    private void ChangeSelection(WorldObject worldObject, Player player) {

        // This should be called by the following line, but there is an outside chance it will not
        SetSelection(false, playingArea);
        if (player.SelectedObject)
            player.SelectedObject.SetSelection(false, playingArea);

        player.SelectedObject = worldObject;
        worldObject.SetSelection(true, player._HUD.GetPlayingArea());
    }
    private void DrawSelection() {

        GUI.skin = ResourceManager.SelectBoxSkin;
        Rect selectBox = ResourceManager.CalculateSelectionBox(selectionBounds, playingArea);

        // Draw the selection box around the currently selected object, within the bounds of the playing area
        GUI.BeginGroup(playingArea);
        DrawSelectionBox(selectBox);
        GUI.EndGroup();
    }

    public void CalculateBounds() {
            
        selectionBounds = new Bounds(transform.position, Vector3.zero);
   
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) 
            selectionBounds.Encapsulate(r.bounds);        
    }

    public void SetSelection(bool selected, Rect playingArea) {

        _IsCurrentlySelected = selected;

        if (selected)
            this.playingArea = playingArea;
    }

    protected virtual void DrawSelectionBox(Rect selectBox) { GUI.Box(selectBox, ""); }

}
