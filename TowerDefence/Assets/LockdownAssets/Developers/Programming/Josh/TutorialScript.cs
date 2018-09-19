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
        
        void EventInitialise(string tutorialEvent) {

            // Iterate through the events list
            for (int i = 0; i < _TutorialEventsList.Count; ++i) {

                // If the element matches the event
                if (tutorialEvent == _TutorialEventsList[i]._TutorialEvent) {

                    // Start tutorial
                    _TutorialEvents.StartTutorial();
                }
            }
        }
    }

    public class TutorialEvents {

        bool _ObjectiveIsComplete;
        public string _TutorialEvent { get; set; }

        public void StartTutorial() {


        }

        void EventCompleted() {


        }
    }

    Text  TutorialText;
    Panel TutorialTextBox;

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