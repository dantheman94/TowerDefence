using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 4/8/2018
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
    public float ItemSpacing = 5f;
    public float StartingPosition = -30f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

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
    /// <param name="worldObject"></param>
    public void AddToQueue(Abstraction abstraction) {

        if (StencilQueueItem != null) {

            // Create queue item
            UI_BuildingQueueItem queueItem = Instantiate(StencilQueueItem).GetComponent<UI_BuildingQueueItem>();
            queueItem.transform.SetParent(ListTransform);

            // Show amount text if it is an AI object
            Ai checkAI = abstraction.GetComponent<Ai>();
            queueItem.SetAmountTextVisiblity(checkAI != null);

            // Initialize gameobject precaution
            if (_Items == null) { Start(); }

            // Add to list & offset the position
            if (_Items.Count == 0) {

                // First item in the list
                _Items.Add(queueItem);
                RectTransform rect = queueItem.GetComponent<RectTransform>();
                rect.localPosition = new Vector2(StartingPosition, StartingPosition);
            }
            else {

                // Not the first item in the list
                _Items.Add(queueItem);
                RectTransform rect = queueItem.GetComponent<RectTransform>();
                float x, y;
                x = StartingPosition;
                y = _Items[_Items.Count - 1].GetComponent<RectTransform>().position.y - _ItemOffset;
                rect.localPosition = new Vector2(x, y);
            }
            queueItem.gameObject.SetActive(true);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}