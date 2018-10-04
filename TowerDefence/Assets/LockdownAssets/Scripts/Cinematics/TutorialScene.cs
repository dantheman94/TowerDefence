using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//-=-=-=-=-=-=-=-=-=-
// Created by: Angus Secomb
// Last Edited: 2/10/18
// Editor: Angus Secomb
//-=-=-=-=-=-=-=-=-=-

public class TutorialScene : MonoBehaviour {

    //                                       INSPECTOR
    //////////////////////////////////////////////////////////////////////////////////////////

    public enum RequiredAction
    {
        BUILD_RESOURCE_GENERATORS,
        KILL_ENEMIES,
        BUILD_BARRICADE,
        BUILD_TOWER,
        BUILD_BASIC_UNIT,
        BUILD_VEHICLE,
        UPGRADE_TOWNHALL
    }

    [System.Serializable]
    public struct MessageData
    {
        [Header(" NEW LOCATIONS ")]
        [Tooltip("Desired transform of the UI tutorial prompt.")]
        public RectTransform DesiredPosition;
        [Tooltip("Text for the tutorial message.")]
        public string MessageContent;
        [Tooltip("Camera target position. (For lerping)")]
        public GameObject TargetObject;
        [Tooltip("If you wish to highlight a particular building during the event.")]
        public Selectable HighlightObject;
        public float LerpSpeed;
        [Header(" EVENT BOOLEANS ")]
        [Tooltip("Enables flashing of resource text counters.")]
        public bool EnableTextFlash;
        [Tooltip("Highlights player starting base.")]
        public bool HighlightBase;
        [Tooltip("Enables the outline of the 'Hightlight Object'")]
        public bool HighlightSelectable;
        [Tooltip("Locks Camera from player movement.")]
        public bool LockControls;
        [Tooltip("Freezes time.")]
        public bool FreezeTime;
        [Header(" PLAYER REQUIRED ACTION TO CONTINUE ")]
        public bool ActionRequired;
        public RequiredAction Action;

        private bool LerpComplete;
        public bool IsLerpComplete() { return LerpComplete; }
        public void SetLerpComplete(bool a_bool) { LerpComplete = a_bool; }
    }

    public List<UnityEngine.UI.Outline> ResourceTextOutlines;
    public List<MessageData> MessageList;
    public GameObject MessagePanel;
    public  bool RunTutorial = false;
    [HideInInspector]
    public static MessageData CurrentMessageData;

    [Header(" SELECTABLE REFERENCES ")]
    public Selectable TownHall;
    public List<Selectable> BaseBuildingSlots;

    //                                    VARIABLES
    //////////////////////////////////////////////////////////////////////////////////////////

    private Camera _MainCamera;
    private Text _MessagePanelText;
    private Color OutlineColor;
    private int EventIndex = 0;
    private float _FlashTimer = 0.5f;
  
    //////////////////////////////////////////////////////////////////////////////////////////

    // Use this for initialization
    void Start () {
        _MainCamera = GameManager.Instance.Players[0].PlayerCamera;
        _MessagePanelText = MessagePanel.transform.GetComponentInChildren<Text>();
        OutlineColor = ResourceTextOutlines[0].effectColor;
	}

    //////////////////////////////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {
        CurrentMessageData = MessageList[EventIndex];
        if(RunTutorial)
        {
            TutorialEvent();
        }
	}

    //////////////////////////////////////////////////////////////////////////////////////////

    void TutorialEvent()
    {
        //Set desired text to panel.
        _MessagePanelText.text = MessageList[EventIndex].MessageContent;
        MessagePanel.transform.position = MessageList[EventIndex].DesiredPosition.position;

        if (MessageList[EventIndex].IsLerpComplete() || !GameManager.Instance._CinematicInProgress)
        {
            //Display the panel.
            MessagePanel.SetActive(true);

        }

        //Highlight allied base.
        if (MessageList[EventIndex].HighlightBase)
        {
            TownHall.SetIsHighlighted(true);
            for (int i = 0; i < BaseBuildingSlots.Count; ++i)
            {
                BaseBuildingSlots[i].SetIsHighlighted(true);
            }
        }
        else if (!MessageList[EventIndex].HighlightBase)
        {
            TownHall.SetIsHighlighted(false);
            for (int i = 0; i < BaseBuildingSlots.Count; ++i)
            {
                BaseBuildingSlots[i].SetIsHighlighted(false);
            }
        }

        //Highlight selectable event object.
        if (MessageList[EventIndex].HighlightObject != null)
            if (MessageList[EventIndex].HighlightSelectable)
            {
                MessageList[EventIndex].HighlightObject.SetIsHighlighted(true);
            }
            else if (!MessageList[EventIndex].HighlightSelectable)
            {
                MessageList[EventIndex].HighlightObject.SetIsHighlighted(false);
            }

        //Flash Resources text outlines.
        if (MessageList[EventIndex].EnableTextFlash)
        {
            OutlineColorFlash();
        }
        else
        {
            for (int i = 0; i < ResourceTextOutlines.Count; ++i)
            {
                if (ResourceTextOutlines[i].effectColor != OutlineColor)
                {
                    ResourceTextOutlines[i].effectColor = OutlineColor;
                }
            }
        }

        if(MessageList[EventIndex].TargetObject != null)
        {
            if (_MainCamera.transform.position != MessageList[EventIndex].TargetObject.transform.position)
            {
                //Lerp Camera to desired position.
                LerpCamera(MessageList[EventIndex]);
                LerpCameraRotation(MessageList[EventIndex]);
            }
            if (Vector3.Distance(_MainCamera.transform.position, MessageList[EventIndex].TargetObject.transform.position) < 0.1f)
            {
                MessageList[EventIndex].SetLerpComplete(true);
                _MainCamera.transform.position = MessageList[EventIndex].TargetObject.transform.position;
            }
            if (_MainCamera.transform.position == MessageList[EventIndex].TargetObject.transform.position)
            {
                if (Input.GetKeyDown(KeyCode.Return) || GamepadManager.Instance.GetGamepad(1).GetButtonDown("A"))
                {
                    if (!MessageList[EventIndex].ActionRequired)
                    {
                        MessagePanel.SetActive(false);
                        if (EventIndex + 1 < MessageList.Count)
                            EventIndex++;
                    }
                }
            }
        }
  
        if (EventIndex == 0 || MessageList[EventIndex].TargetObject == null)
        {
            if(!GameManager.Instance._CinematicInProgress)
            {
                if (Input.GetKeyDown(KeyCode.Return) || GamepadManager.Instance.GetGamepad(1).GetButtonDown("A"))
                {
                    if (!MessageList[EventIndex].ActionRequired)
                    {
                        MessagePanel.SetActive(false);
                        if (EventIndex + 1 < MessageList.Count)
                            EventIndex++;
                    }

                }
            }
        }
       
    }

    //////////////////////////////////////////////////////////////////////////////////////////

    void LockCamera()
    {

    }

    void LerpCamera(MessageData md)
    {
        _MainCamera.transform.position = Vector3.Lerp(_MainCamera.transform.position, md.TargetObject.transform.position, md.LerpSpeed);
    }

    void LerpCameraRotation(MessageData md)
    {
        _MainCamera.transform.rotation = Quaternion.Lerp(_MainCamera.transform.rotation, md.TargetObject.transform.rotation, md.LerpSpeed);
    }

    IEnumerator PanCamera(MessageData md)
    {
        yield return new WaitForSeconds(md.LerpSpeed);
    }

    IEnumerator OutlineFlash(float timer)
    {
        OutlineColorFlash();
        yield return new WaitForSeconds(timer);
    }

    void OutlineColorFlash()
    {
        if (_FlashTimer < 0)
        {
            for (int i = 0; i < ResourceTextOutlines.Count; ++i)
            {
                if (ResourceTextOutlines[i].effectColor == Color.red)
                {
                    ResourceTextOutlines[i].effectColor = OutlineColor;
                }
                else if (ResourceTextOutlines[i].effectColor == OutlineColor)
                {
                    ResourceTextOutlines[i].effectColor = Color.red;
                }
            }
            _FlashTimer = 0.5f;

        }
        else
        {
            _FlashTimer -= Time.deltaTime;
        }
    }

    private void ActionRequired(MessageData md)
    {
        switch(md.Action)
        {
            case RequiredAction.BUILD_BARRICADE:

                break;
            case RequiredAction.BUILD_BASIC_UNIT:

                break;
            case RequiredAction.BUILD_RESOURCE_GENERATORS:

                break;
            case RequiredAction.BUILD_TOWER:
                break;
            case RequiredAction.BUILD_VEHICLE:
                break;
            case RequiredAction.KILL_ENEMIES:
                break;
            case RequiredAction.UPGRADE_TOWNHALL:
                break;
            default:
                break;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////
}
