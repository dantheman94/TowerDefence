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

public class HUD : MonoBehaviour {

    //******************************************************************************************************************************
    // INSPECTOR

    public GUISkin resourceSkin, ordersSkin, selectBoxSkin;

    //******************************************************************************************************************************
    // VARIABLES

    private const int SELECTION_NAME_HEIGHT = 15;

    //******************************************************************************************************************************
    // FUNCTIONS

    public bool MouseInBounds() {

        // Screen coordinates start in the lower-left corner of the screen
        // not the top-left of the screen like the drawing coordinates do
        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width;
        bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height;

        return insideWidth && insideHeight;
    }

    public GameObject FindHitObject() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            return hit.collider.gameObject;

        return null;
    }

    public Vector3 FindHitPoint() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            return hit.point;

        return ResourceManager.InvalidPosition;
    }

    public Rect GetPlayingArea() {  return new Rect(0, 0, Screen.width, Screen.height); }
}
