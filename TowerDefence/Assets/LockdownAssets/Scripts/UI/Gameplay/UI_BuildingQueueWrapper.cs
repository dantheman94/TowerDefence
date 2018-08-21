using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 20/8/2018
//
//******************************

public class UI_BuildingQueueWrapper : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUILDING QUEUE PROPERTIES")]
    [Space]
    public UI_BuildingQueue BuildingQueueStencil = null;
    public Transform QueueListTransform = null;
    public int QueueSpacing = 5;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public static UI_BuildingQueueWrapper Instance;

    private List<UI_BuildingQueue> _Queues = null;
    private float _ItemOffset = 5;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  This is called before Startup().
    /// </summary>
    private void Awake() {

        // Initialize singleton
        if (Instance != null && Instance != this) {

            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // Initialize list
        _Queues = new List<UI_BuildingQueue>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this gameObject is created.
    /// </summary>
    private void Start() {
        
        if (BuildingQueueStencil != null) {

            // Update item offset
            _ItemOffset = BuildingQueueStencil.GetComponent<RectTransform>().rect.height + QueueSpacing;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="queue"></param>
    public void AddNewQueue(UI_BuildingQueue queue) {

        _Queues.Add(queue);
        UpdateQueuePositions();
    } 

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="queue"></param>
    public void RemoveFromQueue(UI_BuildingQueue queue) {

        // look for match
        for (int i = 0; i < _Queues.Count; i++) {

            // Look for match
            if (_Queues[i] == queue) {

                // Remove & destroy
                _Queues.RemoveAt(i);
                Destroy(queue);
                UpdateQueuePositions();
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void UpdateQueuePositions() {

        // Update the positions off all the currently active queues (older queues at the top)
        for (int i = 0; i < _Queues.Count; i++) {

            // First queue item is in the top left corner of the screen
            RectTransform rect = _Queues[i].GetComponent<RectTransform>();
            float x, y;
            if (i == 0) {

                x = rect.rect.width / 2 + QueueSpacing;
                y = (rect.rect.height / 2 + QueueSpacing) * -1;
            }

            // Remaining queues are allocated a new row for each
            else {

                x = rect.rect.width / 2 + QueueSpacing;
                y = ((rect.rect.height / 2 + QueueSpacing) * (i + 1)) * -1;
            }
            rect.anchoredPosition = new Vector2(x, y);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="queue"></param>
    /// <returns>
    //  bool
    /// </returns>
    public bool ContainsQueue(UI_BuildingQueue queue) {

        bool match = false;
        for (int i = 0; i < _Queues.Count; i++) {

            if (_Queues[i] == queue) {

                match = true;
                break;
            }
        }
        return match;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
