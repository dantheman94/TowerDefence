using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/7/2018
//
//******************************

public class MatchFeed : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MATCHFEED SETTINGS")]
    [Space]
    public GameObject FeedTextStencil;
    public int PosX = 150;
    public int PosY = 15;
    public int VerticalOffset = 30;
    public int MessageOnScreenTime = 3;
    public float FadeOutRate = 2f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    public static MatchFeed Instance;

    [System.Serializable]
    private class MatchFeedObject {

        public Text _Text;
        public float _TimeOnScreen;
        public GameObject _GameObject;
    }

    private List<MatchFeedObject> _Messages;

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

        // Create lists
        _Messages = new List<MatchFeedObject>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update() {
        
        // Slowly time out all known messages
        for (int i = 0; i < _Messages.Count; i++) {

            _Messages[i]._TimeOnScreen -= Time.deltaTime;
            if (_Messages[i]._TimeOnScreen <= 0) {

                // Clamp
                _Messages[i]._TimeOnScreen = 0;

                // Fade out
                _Messages[i]._Text.color = Color.Lerp(_Messages[i]._Text.color, Color.clear, FadeOutRate * Time.deltaTime);
            }

            // Message has finished fade out
            if (_Messages[i]._Text.color.a < 0.01f) {

                // Destroy message object
                MatchFeedObject feedObject = _Messages[i];
                _Messages.RemoveAt(i);
                Destroy(feedObject._GameObject);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(string message) {
        
        if (FeedTextStencil != null) {

            // Push all current messages up
            for (int i = 0; i < _Messages.Count; i++) {

                RectTransform rect = _Messages[i]._Text.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + VerticalOffset);
            }

            // Create new message instance
            MatchFeedObject messageObj = new MatchFeedObject();
            GameObject obj = Instantiate(FeedTextStencil);
            Text txt = obj.GetComponent<Text>();
            RectTransform objrect = obj.GetComponent<RectTransform>();

            // Setup message properties
            txt.text = message.ToUpper();
            messageObj._Text = txt;
            messageObj._TimeOnScreen = MessageOnScreenTime;
            messageObj._GameObject = obj;
            obj.transform.SetParent(this.gameObject.transform);
            objrect.anchoredPosition = new Vector2(PosX, PosY);
            obj.SetActive(true);
            _Messages.Add(messageObj);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}