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

    public class AbstractionCounter {

        public AbstractionCounter(Abstraction abs, int count = 1) {

            _Abstraction = abs;
            _AbsCount = count;
        }

        public Abstraction _Abstraction = null;
        public int _AbsCount = 0;
    }

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

            if (_BuildingAttached.GetBuildingQueue().Count > 0) {

                AddToQueue(_BuildingAttached.GetBuildingQueue()[0]);
            }

            // Add current items
            //for (int i = 0; i < _BuildingAttached.GetBuildingQueue().Count; i++) {
            //
            //    AddToQueue(_BuildingAttached.GetBuildingQueue()[i]);
            //}
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

    /// <summary>
    //
    /// </summary>
    /// <returns></returns>
    public int GetQueueSize() {

        if (_Items != null) { return _Items.Count; }
        else { return 0; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  List<AbstractionCounter>
    /// </returns>
    private List<AbstractionCounter> GetAmountOfEachUnits() {

        if (_BuildingAttached != null && _BuildingAttached.GetBuildingQueue() != null) {

            List<AbstractionCounter> absCounter = new List<AbstractionCounter>();
            for (int i = 0; i < _BuildingAttached.GetBuildingQueue().Count; i++) {

                if (absCounter.Count > 0) {

                    // Check for matches against both lists
                    for (int j = 0; j < absCounter.Count; j++) {

                        if (_BuildingAttached.GetBuildingQueue()[i].GetType() == absCounter[j]._Abstraction.GetType()) {

                            // Add to counter
                            absCounter[j]._AbsCount++;
                            break;
                        }

                        // Reached the end of the absCounter list
                        if (j == absCounter.Count - 1) {

                            // Add the item in the building queue that is being tested against
                            absCounter.Add(new AbstractionCounter(_BuildingAttached.GetBuildingQueue()[i]));
                        }
                    }
                }

                // First absCounter iterator
                else {

                    // Add the first item in the building queue
                    absCounter.Add(new AbstractionCounter(_BuildingAttached.GetBuildingQueue()[i]));
                }
            }
            return absCounter;
        }
        else { return null; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void UpdateSelectionWheelItemCounters() {
        
        // Get selection wheel reference
        SelectionWheel selectionWheel = null;
        if (GameManager.Instance._IsRadialMenu) { selectionWheel = GameManager.Instance.SelectionWheel.GetComponentInChildren<SelectionWheel>(); }
        else { selectionWheel = GameManager.Instance.selectionWindow.GetComponentInChildren<SelectionWheel>(); }

        // Update each item counter in the selection wheel based off the current building queue
        List<AbstractionCounter> absCounter = new List<AbstractionCounter>();
        absCounter = GetAmountOfEachUnits();
        for (int i = 0; i < selectionWheel._WheelButtons.Count; i++) {

            SelectionWheelUnitRef wheelUnitRef = selectionWheel._WheelButtons[i].GetComponent<SelectionWheelUnitRef>();

            if (absCounter.Count > 0) {

                // Button item has a valid abstraction reference
                Abstraction absRef = wheelUnitRef.AbstractRef;
                if (absRef != null) {

                    for (int k = 0; k < absCounter.Count; k++) {
                        
                        // Button item abstraction type matches the building queue abstraction type
                        if (absRef.GetType() == absCounter[k]._Abstraction.GetType()) {

                            // Update button item counter
                            wheelUnitRef.SetQueueCounter(absCounter[k]._AbsCount);
                            break;
                        }

                        // Reached the end of the absCounter array
                        if (k == absCounter.Count - 1) {

                            // Button item abstraction counter is 0
                            wheelUnitRef.SetQueueCounter(0);
                        }
                    }
                }
                else { continue; }
            }
            else { wheelUnitRef.SetQueueCounter(0); }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}