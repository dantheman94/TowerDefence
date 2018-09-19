using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Joshua Peake
//
//  Last edited by: Joshua Peake
//  Last edited on: 17/09/2018
//
//******************************

public class TutorialScript : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      CLASSES
    //
    //******************************************************************************************************************************

    public class TutorialEventsManager {

        TutorialEvents       _TutorialEvents;
        List<TutorialEvents> _TutorialEventsList;
        public Text          TutorialText;
        public Panel         TutorialTextBox;

        public void EventInitialise(string tutorialEvent) {

            // Iterate through the events list
            for (int i = 0; i < _TutorialEventsList.Count; ++i) {

                // If the element matches the event
                if (tutorialEvent == _TutorialEventsList[i]._TutorialEvent) {

                    // Start tutorial
                    _TutorialEvents.StartTutorial(tutorialEvent);
                }
            }
        }
    }

    public class TutorialEvents {

        TutorialEventsManager _TutorialEventsManager;

        bool _ObjectiveIsComplete;
        public string _TutorialEvent { get; set; }

        public void StartTutorial(string text) {

            // Set tutorial text
            _TutorialEventsManager.TutorialText.text = text;
        }

        void EventCompleted() {

            _TutorialEventsManager.EventInitialise(_TutorialEvent);
        }
    }

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start () {
		
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Update () {
		
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}