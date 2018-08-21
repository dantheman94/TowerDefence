using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 20/8/2018
//
//******************************

public class UI_BuildingQueue : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUILDING QUEUE PROPERTIES")]
    [Space]
    public UI_BuildingQueueItem StencilQueueItem = null;
    public Transform ListTransform = null;
    public float StartingPositionX = -30f;
    public float StartingPositionY = -40f;
    public float ItemSpacing = 35f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Building _BuildingAttached = null;
    private float _ItemOffset = 45;
    private List<UI_BuildingQueueItem> _Items;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this gameObject is created.
    /// </summary>
    private void Start() {

        // Initialize lists
        _Items = new List<UI_BuildingQueueItem>();

        if (StencilQueueItem != null) {

            // Update item offset
            _ItemOffset = StencilQueueItem.GetComponent<RectTransform>().rect.width + ItemSpacing;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="buildingInstigator"></param>
    public void UpdateQueueItemList() {

        // Clear the list
        if (_Items != null) {

            if (_Items.Count > 0) { _Items.Clear(); }
            for (int i = 0; i < ListTransform.childCount; i++) { Destroy(ListTransform.GetChild(i).gameObject); }
        }
        else {

            // Reinitialize precaution
            Start();
            UpdateQueueItemList();
        }

        // Replace with current building queue items
        if (_BuildingAttached != null) {

            // Add current items
            for (int i = 0; i < _BuildingAttached.GetBuildingQueue().Count; i++) {

                AddToQueue(_BuildingAttached.GetBuildingQueue()[i]);
            }
        }        
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds a building queue item to the widget
    /// </summary>
    /// <param name="worldObject"></param>
    public void AddToQueue(Abstraction abstraction) {

        if (StencilQueueItem != null) {
            
            // Initialize gameobject precaution
            if (_Items == null) { Start(); }

            // Create queue item
            UI_BuildingQueueItem queueItem = Instantiate(StencilQueueItem).GetComponent<UI_BuildingQueueItem>();
            queueItem.transform.SetParent(ListTransform);
            queueItem.SetAbstractionAttached(abstraction);
            
            // Add to list & offset the position
            if (_Items.Count == 0) {

                // First item in the list
                _Items.Add(queueItem);
                RectTransform rect = queueItem.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(StartingPositionX, StartingPositionY);
            }
            else {
                
                // Not the first item in the list
                _Items.Add(queueItem);
                RectTransform rect = queueItem.GetComponent<RectTransform>();
                RectTransform previousRect = _Items[_Items.Count - 1].GetComponent<RectTransform>();
                float x, y;
                x = StartingPositionX + (_ItemOffset * _Items.Count) + (ItemSpacing * _Items.Count);
                y = StartingPositionY;
                
                rect.anchoredPosition = new Vector2(x, y);
            }
            queueItem.gameObject.SetActive(true);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="building"></param>
    public void SetBuildingAttached(Building building) { _BuildingAttached = building; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}