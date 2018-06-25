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

    public bool _IsCurrentlySelected;
    protected Player _Player = null;
    protected Bounds selectionBounds;
    protected bool _PlayerOwned = false;
    protected GameObject _SelectionObj = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Awake() {

        selectionBounds = Settings.InvalidBounds;
        CalculateBounds();
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Start() {

        GameManager.Instance.Selectables.Add(this);
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Update() { }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnGUI() {

        // Update selection
        DrawSelection(_IsCurrentlySelected);
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void CalculateBounds() {}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectObj"></param>
    protected virtual void ChangeSelection(Selectable selectObj) {

        // This should be called by the following line, but there is an outside chance it will not
        SetSelection(false);

        // Clear the world objects selection list
        foreach (var obj in _Player.SelectedWorldObjects) { obj.SetSelection(false); }
        _Player.SelectedWorldObjects.Clear();

        // Add new selection to the list
        _Player.SelectedWorldObjects.Add(selectObj);
        selectObj.SetSelection(true);
    }

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

                // Display prefab if not already being displayed
                if (_SelectionObj.activeInHierarchy != true) { _SelectionObj.SetActive(true); }

                // Update selection prefab position
                Vector3 pos = new Vector3();
                pos.x = transform.position.x;
                pos.y = 1.1f;
                pos.z = transform.position.z;
                _SelectionObj.transform.position = pos;
            }
        }

        // Hide selection
        else { if (_SelectionObj != null) { Destroy(_SelectionObj.gameObject); } }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectBox"></param>
    protected virtual void DrawSelectionBox(Rect selectBox) { GUI.Box(selectBox, ""); }

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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayer(Player player) {

        _PlayerOwned = true;
        _Player = player;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selected"></param>
    public void SetSelection(bool selected) { _IsCurrentlySelected = selected; }

}