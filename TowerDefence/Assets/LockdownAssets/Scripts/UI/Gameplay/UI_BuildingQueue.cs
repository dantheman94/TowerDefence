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
    public float ItemSpacing = 5f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private float _ItemOffset = 45;

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
    public void AddToQueue(WorldObject worldObject) {

        if (StencilQueueItem != null) {

            // Create queue item
            UI_BuildingQueueItem queueItem = Instantiate(StencilQueueItem).GetComponent<UI_BuildingQueueItem>();

            // Show amount text if it is an AI object
            Ai checkAI = worldObject.GetComponent<Ai>();
            queueItem.SetAmountTextVisiblity(checkAI != null);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}