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
        BUILD_POWER_GENERATOR,
        BUILD_SUPPLY_GENERATOR,
        BUILD_TOWER,
        BUILD_BASIC_UNIT,
        BUILD_VEHICLE,
        BUILD_BARRACKS,
        UPGRADE_TOWNHALL,
        NONE
    }

    [System.Serializable]
    public class MessageData
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
        public bool HighlightSlots;
        [Tooltip("Enables the outline of the 'Hightlight Object'")]
        public bool HighlightSelectable;
        [Tooltip("Locks players controls.")]
        public bool LockControls;
        [Tooltip("Disables continue prompt on message.")]
        public bool DisablePrompt;
        [Tooltip("Prevents camera from moving.")]
        public bool LockCamera;
        [Tooltip("Locks all buttons in lockable buttons")]
        public bool LockButtons;
        [Tooltip("The buttons that will be locked if LockButtons = true")]
        public List<Button> LockableButtons;
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
    public Text ObjectiveText;
    public Text ObjectiveTextTwo;
    public Text PromptText;
    public BuildingSlot TurretSlot;
    public BuildingSlot BarricadeSlot;
    public GameObject StartBase;
    public GameObject ControlsObject;
    public Color UIGreyoutColor;
    public  bool RunTutorial = false;
    [HideInInspector]
    public static MessageData CurrentMessageData;

    [Header(" SELECTABLE REFERENCES ")]
    public Selectable TownHall;
    public List<BuildingSlot> BaseBuildingSlots;

    //                                    VARIABLES
    //////////////////////////////////////////////////////////////////////////////////////////

    private Camera _MainCamera;
    private Text _MessagePanelText;
    private Color OutlineColor;
    private int EventIndex = 0;
    private float _FlashTimer = 0.5f;
    private Player _Player;
    private ResourceManager _ResourceManager;
    private bool ActionBool = false;
    private int _PopulationCount = 0;
    private bool StartTheWaves = false;
  
    //////////////////////////////////////////////////////////////////////////////////////////

    // Use this for initialization
    void Start () {
        _Player = GameManager.Instance.Players[0];
        _MainCamera = GameManager.Instance.Players[0].PlayerCamera;
        _MessagePanelText = MessagePanel.transform.GetComponentInChildren<Text>();
        OutlineColor = ResourceTextOutlines[0].effectColor;
        _ResourceManager = _Player.GetResourceManager();
	}

    //////////////////////////////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {
        //Sets static message data based on index.
        CurrentMessageData = MessageList[EventIndex];
        if(RunTutorial)
        {
            TutorialEvent();
        }
	}

    //////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Runs tutorial event based on the message data index.
    /// </summary>
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

        if (MessageList[EventIndex].LockControls || MessageList[EventIndex].LockCamera)
        {
            ControlsObject.SetActive(false);
        }
        else if (!MessageList[EventIndex].LockControls && !MessageList[EventIndex].LockCamera)
        {
            ControlsObject.SetActive(true);
        }

        if(MessageList[EventIndex].LockButtons)
        {
            for(int i = 0; i < MessageList[EventIndex].LockableButtons.Count; ++i)
            {
                MessageList[EventIndex].LockableButtons[i].interactable = false;
                MessageList[EventIndex].LockableButtons[i].gameObject.GetComponent<Image>().color = UIGreyoutColor;
            }
        }
        else
        {
            for (int i = 0; i < MessageList[EventIndex].LockableButtons.Count; ++i)
            {
                MessageList[EventIndex].LockableButtons[i].interactable = true;
            }
        }

        if(PromptText != null)
        {
            if (MessageList[EventIndex].DisablePrompt)
            {
                PromptText.enabled = false;
            }
            else
            {
                PromptText.enabled = true;
            }
        }
    

        if(MessageList[EventIndex].ActionRequired)
        {
            ActionRequired(MessageList[EventIndex]);
            if(ActionBool)
            {
                MessageList[EventIndex].ActionRequired = false;
                MessageList[EventIndex].DisablePrompt = false;
            }
        }
        else
        {
            ObjectiveText.text = "";
            ObjectiveTextTwo.text = "";
        }

        if(MessageList[EventIndex].HighlightSlots)
        {
            for (int i = 0; i < BaseBuildingSlots.Count; ++i)
            {
                BaseBuildingSlots[i].SetIsHighlighted(true);
            }
        }
        else if (!MessageList[EventIndex].HighlightSlots)
        {
            for (int i = 0; i < BaseBuildingSlots.Count; ++i)
            {
                BaseBuildingSlots[i].SetIsHighlighted(false);
            }
        }

        //Highlight allied base.
        if (MessageList[EventIndex].HighlightBase)
        {
            TownHall.SetIsHighlighted(true);  
        }
        else if (!MessageList[EventIndex].HighlightBase)
        {
            TownHall.SetIsHighlighted(false);
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
            if (Vector3.Distance(_MainCamera.transform.position, MessageList[EventIndex].TargetObject.transform.position) < 3.5f)
            {
                MessageList[EventIndex].SetLerpComplete(true);
                _MainCamera.transform.position = MessageList[EventIndex].TargetObject.transform.position;
            }
            if (MessageList[EventIndex].IsLerpComplete())
            {
                if (Input.GetKeyDown(KeyCode.Return) || GamepadManager.Instance.GetGamepad(1).GetButtonDown("A"))
                {
                    if (!MessageList[EventIndex].ActionRequired)
                    {
                        ActionBool = false;
                        _PopulationCount = 0;
                        MessagePanel.SetActive(false);
                        if (EventIndex + 1 < MessageList.Count)
                            EventIndex++;
                    }
                }
            }
        }
  
        else if (EventIndex == 0 || MessageList[EventIndex].TargetObject == null)
        {
            if(!GameManager.Instance._CinematicInProgress)
            {
                if (Input.GetKeyDown(KeyCode.Return) || GamepadManager.Instance.GetGamepad(1).GetButtonDown("A") )
                {
                    if (!MessageList[EventIndex].ActionRequired)
                    {
                        ActionBool = false;
                        _PopulationCount = 0;
                        MessagePanel.SetActive(false);
                        if (EventIndex + 1 < MessageList.Count)
                            EventIndex++;
                    }
                }
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Lerps camera to new location.
    /// </summary>
    /// <param name="md"></param>
    void LerpCamera(MessageData md)
    {
        _MainCamera.transform.position = Vector3.Lerp(_MainCamera.transform.position, md.TargetObject.transform.position, md.LerpSpeed);
    }

    /// <summary>
    /// Rotates camera to new location.
    /// </summary>
    /// <param name="md"></param>
    void LerpCameraRotation(MessageData md)
    {
        _MainCamera.transform.rotation = Quaternion.Lerp(_MainCamera.transform.rotation, md.TargetObject.transform.rotation, md.LerpSpeed);
    }

    /// <summary>
    /// Flashes Resource text.
    /// </summary>
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

    /// <summary>
    /// Prevents tutorial event from continuing until
    /// the required action is met.
    /// </summary>
    /// <param name="md"></param>
    private void ActionRequired(MessageData md)
    {
        switch(md.Action)
        {
            case RequiredAction.BUILD_BARRICADE:
            if(BarricadeSlot.GetBuildingOnSlot() != null)
            {
                if ((BarricadeSlot.GetBuildingOnSlot().ObjectName == BarricadeSlot.Buildings[1].ObjectName && BarricadeSlot.GetBuildingOnSlot()._ObjectState == WorldObject.WorldObjectStates.Active)
                                            || (BarricadeSlot.GetBuildingOnSlot().ObjectName == BarricadeSlot.Buildings[2].ObjectName && BarricadeSlot.GetBuildingOnSlot()._ObjectState == WorldObject.WorldObjectStates.Active))
                {
                    ObjectiveText.text = "Build Barricade: 1/1";
                    ActionBool = true;
                }
                else
                {
                    ObjectiveText.text = "Build Barricade: 0/1";
                }
            }
            else
            {
                ObjectiveText.text = "Build Barricade: 0/1";
            }
                break;
            case RequiredAction.BUILD_BASIC_UNIT:
                if(_PopulationCount == 1)
                {
                    ActionBool = true;
                    ObjectiveText.text = "Build Dwarf Soldier 1/1";
                }
                else
                {
                    ObjectiveText.text = "Build Dwarf Soldier 0/1";
                }
                break;
            case RequiredAction.BUILD_POWER_GENERATOR:
                ObjectiveText.text = "Power Station: " + _ResourceManager.GetPowerGeneratorCount() + "/1";
                if(_ResourceManager.GetPowerGeneratorCount() ==1)
                {
                    ActionBool = true;
                }
                break;
            case RequiredAction.BUILD_SUPPLY_GENERATOR:
                ObjectiveText.text = "Supply Generator: " + _ResourceManager.GetSupplyGeneratorCount() + "/1";
                if(_ResourceManager.GetSupplyGeneratorCount() == 1)
                {
                    ActionBool = true;
                }
                break;
            case RequiredAction.BUILD_TOWER:

                if (TurretSlot.GetBuildingOnSlot() != null)
                {
                    if ((TurretSlot.GetBuildingOnSlot().ObjectName == TurretSlot.Buildings[1].ObjectName && TurretSlot.GetBuildingOnSlot()._ObjectState == Abstraction.WorldObjectStates.Active)
                 || (TurretSlot.GetBuildingOnSlot().ObjectName == TurretSlot.Buildings[2].ObjectName && TurretSlot.GetBuildingOnSlot()._ObjectState == Abstraction.WorldObjectStates.Active)
                 || (TurretSlot.GetBuildingOnSlot().ObjectName == TurretSlot.Buildings[3].ObjectName && TurretSlot.GetBuildingOnSlot()._ObjectState == Abstraction.WorldObjectStates.Active))
                    {
                        ObjectiveText.text = "Build Tower: 1/1";
                        ActionBool = true;
                    }
                    else
                    {
                        ObjectiveText.text = "Build Tower: 0/1";
                    }
                }
                else
                {
                    ObjectiveText.text = "Build Tower: 0/1";
                }
               
                break;
            case RequiredAction.BUILD_VEHICLE:
                break;
            case RequiredAction.KILL_ENEMIES:
                break;
            case RequiredAction.UPGRADE_TOWNHALL:
                if(!StartBase.activeInHierarchy)
                {
                    ActionBool = true;
                    ObjectiveText.text = "Town Hall Tier II 1/1";
                }
                else
                {
                    ObjectiveText.text = "Town Hall Tier II 0/1";
                }
                break;
            case RequiredAction.BUILD_BARRACKS:
                for(int i = 0; i < BaseBuildingSlots.Count; ++i)
                {
                    if(BaseBuildingSlots[i].GetBuildingOnSlot() != null)
                    {
                        if (BaseBuildingSlots[i].GetBuildingOnSlot().ObjectName == BaseBuildingSlots[i].Buildings[9].ObjectName
                    && BaseBuildingSlots[i].GetBuildingOnSlot()._ObjectState == Abstraction.WorldObjectStates.Active)
                        {
                            ObjectiveText.text = "Build Barracks 1/1";
                            ActionBool = true;
                        }
                        else
                        {
                            ObjectiveText.text = "Build Barracks 0/1";
                        }
                    }
                    else
                    {
                        ObjectiveText.text = "Build Barracks 0/1";
                    }
                }
                break;
            default:
                break;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Added to selection button to know when a unit is being built.
    /// </summary>
    public void IncreasePop() { _PopulationCount++; }
}
