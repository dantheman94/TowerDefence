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
    [Header(" WORLD OBJECT STATES")]
    [Space]
    public UI_BuildingQueueItem StencilQueueItem = null;
    public Transform ListTransform = null;
    public float StartingPositionX = -30f;
    public float StartingPositionY = -40f;
    public float ItemSpacing = 35f;
    public int MaxItemsPerRow = 5;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private float _ItemOffset = 45;
    private List<UI_BuildingQueueItem> _Items;
    private List<int> _ItemRows;
    private int _CurrentRow = 0;

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
        _ItemRows = new List<int>();

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
    public void UpdateQueueItemList(Building buildingInstigator = null) {

        // Clear the list
        if (_Items.Count > 0)       { _Items.Clear(); }
        if (_ItemRows.Count > 0)    { _ItemRows.Clear(); }
        for (int i = 0; i < ListTransform.childCount; i++) { Destroy(ListTransform.GetChild(i).gameObject); }

        // Replace with current building queue items
        if (buildingInstigator != null) {

            // Add current items
            for (int i = 0; i < buildingInstigator.GetBuildingQueue().Count; i++) {

                AddToQueue(buildingInstigator.GetBuildingQueue()[i]);
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
                _CurrentRow = 0;
                _ItemRows.Add(1);
            }
            else {
                
                // Not the first item in the list
                _Items.Add(queueItem);
                RectTransform rect = queueItem.GetComponent<RectTransform>();
                float x, y;
                
                RectTransform previousRect = _Items[_Items.Count - 1].GetComponent<RectTransform>();

                // Still enough room in the row
                if (_ItemRows[_CurrentRow] < MaxItemsPerRow) {

                    x = StartingPositionX - _ItemOffset * _ItemRows[_CurrentRow] + 1;
                    y = StartingPositionY - _ItemOffset * _CurrentRow + 1;
                    _ItemRows[_CurrentRow] += 1;
                }
                
                // Make a new row
                else {

                   x = StartingPositionX;
                   y = previousRect.position.y - _ItemOffset;
                   _CurrentRow += 1;
                   _ItemRows.Add(1);
                }

                rect.anchoredPosition = new Vector2(x, y);

            }
            queueItem.gameObject.SetActive(true);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}