using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 04/09/2018
//
//******************************


public class SettingsMenuNavigator : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      DEFINITION
    //
    //******************************************************************************************************************************

    [System.Serializable]
    public struct SettingsInfo
    {
        public Button SelectionMenuTypeNav;
        public Button WindowModeNav;
        public Button ResolutionNav;
        public Button AspectNav;
        public Button VSyncNav;
        public Button TextureQualityNav;
        public Button MasterVolumeNav;
        public Button MusicVolumeNav;
        public Button SFXVolumeNav;
        public Button SelectionMenuLeft;
        public Button SelectionMenuRight;
        public Button GamePadSchemes;
        public Button KeyboardBindings;
        public Button WindowModeLeft;
        public Button WindowModeRight;
        public Button ResolutionLeft;
        public Button ResolutionRight;
        public Button AspectRatioLeft;
        public Button AspectRatioRight;
        public Button V_SyncLeft;
        public Button V_SyncRight;
        public Button TextureQualityLeft;
        public Button TextureQualityRight;
    }

    public enum MenuType
    {
        RADIAL, WINDOW
    }

    public enum TextureQuality
    {
        LOW, MEDIUM, HIGH,
    }

    public enum WindowMode
    {
        FULLSCREEN, WINDOWED
    }

    public enum Resolution
    {
        NINETEEN_TWENTY_BY_TEN_EIGHTY, TWELVE_EIGHTY_BY_SEVEN_TWENTY, THIRTEEN_SIX_SIX_BY_SEVEN_SIX_EIGHT, SIXTEEN_HUNDRED_BY_NINE_HUNDRED,
        TWENTY_FIVE_SIXTY_BY_FOURTEEN_FORTY, THIRTY_EIGHT_FOURTY_BY_TWENTY_ONE_SIXTY
    }

    public enum AspectRatio
    {
        SIXTEEN_NINE, SIXTEEN_TEN, FOUR_THREE,THREE_TWO, FIVE_FOUR,
    }

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************


    [Tooltip("0 = false 1 = true for vsync")]
    public int VSync = 0;

    private AspectRatio _AspectRatio;
    private Resolution _Resolution;
    private WindowMode _WindowMode;
    private TextureQuality _TextureQuality;
    private MenuType _MenuType;
    private int _SchemeIndex = 0;

    [Header("----------------------")]
    [Space]
    [Header("CURRENT SELECTION")]
    [Tooltip("What menu is currently selected.")]
    public string CurrentSelection;

    [Header("----------------------")]
    [Space]
    [Header("VOLUME REFERENCES")]
    public int MasterVolume = 100;
    public int MusicVolume = 100;
    public int EffectsVolume = 100;


    [Header("----------------------")]
    [Space]
    [Header("TEXT REFERENCES")]
    public Text AspectRatioText;
    public Text MenuTypeText;
    public Text TextureQualityText;
    public Text VSyncText;
    public Text ResolutionText;
    public Text WindowModeText;

    [Header("----------------------")]
    [Space]
    [Header("SLIDER REFERENCES")]
    public Slider MasterVolSlider;
    public Slider MusicVolSlider;
    public Slider EffectVolSlider;

    [Header("----------------------")]
    [Space]
    [Header("SETTINGS INFO")]
    public SettingsInfo SettingInfo;

    [Header("----------------------")]
    [Space]
    [Header("GAMEPAD SCHEME OBJECTS")]
    public GameObject SchemeWrapper;
    public GameObject SettingsBase;
    public GameObject SchemeOne;
    public GameObject SchemeTwo;
    public GameObject SchemeThree;
    public GameObject SchemeFour;
    public Button StartButton;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private MenuNavigator _MenuNavigator;

    private xb_gamepad gamepad;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        LoadSettings();
        SwitchGamepadSchemes(_SchemeIndex);
        gamepad = GamepadManager.Instance.GetGamepad(1);
        _MenuNavigator = GetComponent<MenuNavigator>();
        UpdateTextAndApplySettings();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {
         NavigateSettingsMenu();
         SwitchSettings();
         ChangeControllerText();
     if(SchemeWrapper.activeInHierarchy)
     {
         if(gamepad.GetButton("B"))
         {
             SchemeWrapper.SetActive(false);
             SettingsBase.SetActive(true);
             StartCoroutine(DelayedSelect(SettingInfo.GamePadSchemes));
         }
     }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Switch gamepad schemes
    /// </summary>
    /// <param name="a_int"></param>
    public void SwitchGamepadSchemes(int a_int)
    {
        _SchemeIndex = a_int;
        switch (a_int)
        {
            case 1:
                SchemeOne.SetActive(true);
                SchemeTwo.SetActive(false);
                PlayerPrefs.SetInt("GamepadScheme", _SchemeIndex);
                PlayerPrefs.Save();
                break;
            case 2:
                SchemeOne.SetActive(false);
                SchemeTwo.SetActive(true);
                PlayerPrefs.SetInt("GamepadScheme", _SchemeIndex);
                PlayerPrefs.Save();
                break;
            case 3:
                PlayerPrefs.SetInt("GamepadScheme", _SchemeIndex);
                PlayerPrefs.Save();
                break;
            case 4:
                PlayerPrefs.SetInt("GamepadScheme", _SchemeIndex);
                PlayerPrefs.Save();
                break;
            default:
                break;
        }
        
    }

    /// <summary>
    ///  Do actions based on current active scene.
    /// </summary>
    private void SwitchSettings()
    {
         switch(CurrentSelection)
        {
            case "MenuType":
                if (InputChecker.CurrentController == "Controller")
                {
                    if (gamepad.GetButtonDown("RB"))
                    {
                        if (_MenuType < MenuType.WINDOW)
                            _MenuType++;
                        else
                        {
                            _MenuType = 0;
                        }
                        PlayerPrefs.SetInt("MenuType", (int)_MenuType);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                    else if (gamepad.GetButtonDown("LB"))
                    {
                        if (_MenuType > MenuType.RADIAL)
                            _MenuType--;
                        else
                        {
                            _MenuType = MenuType.WINDOW; ;
                        }
                        UpdateTextAndApplySettings();
                        PlayerPrefs.SetInt("MenuType", (int)_MenuType);
                        PlayerPrefs.Save();
                    }
                }
                break;
            case "WindowMode":
                if (InputChecker.CurrentController == "Controller")
                {
                    if (gamepad.GetButtonDown("RB"))
                    {
                        if (_WindowMode < WindowMode.WINDOWED)
                            _WindowMode++;
                        else
                        {
                            _WindowMode = 0;
                        }
                        PlayerPrefs.SetInt("WindowMode", (int)_WindowMode);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();

                    }
                    else if (gamepad.GetButtonDown("LB"))
                    {
                        if (_WindowMode > WindowMode.FULLSCREEN)
                            _WindowMode--;
                        else
                        {
                            _WindowMode = WindowMode.WINDOWED;
                        }
                        PlayerPrefs.SetInt("WindowMode", (int)_WindowMode);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                }
                break;
            case "Resolution":
                if (InputChecker.CurrentController == "Controller")
                {
                    if (gamepad.GetButtonDown("RB"))
                    {
                        if (_Resolution < Resolution.THIRTY_EIGHT_FOURTY_BY_TWENTY_ONE_SIXTY)
                        {
                            _Resolution++;
                        }
                        else
                        {
                            _Resolution = 0;
                        }
                        PlayerPrefs.SetInt("Resolution", (int)_Resolution);
                        PlayerPrefs.Save();
                    }
                    else if (gamepad.GetButtonDown("LB"))
                    {
                        if (_Resolution > Resolution.NINETEEN_TWENTY_BY_TEN_EIGHTY)
                        {
                            _Resolution--;
                        }
                        else
                        {
                            _Resolution = Resolution.THIRTY_EIGHT_FOURTY_BY_TWENTY_ONE_SIXTY;
                        }
                        PlayerPrefs.SetInt("Resolution", (int)_Resolution);
                        PlayerPrefs.Save();
                    }
                }
                break;
            case "AspectRatio":
                if (InputChecker.CurrentController == "Controller")
                {
                    if (gamepad.GetButtonDown("RB"))
                    {
                        if (_AspectRatio < AspectRatio.FIVE_FOUR)
                        {
                            _AspectRatio++;
                        }
                        else
                        {
                            _AspectRatio = AspectRatio.SIXTEEN_NINE;
                        }
                        PlayerPrefs.SetInt("AspectRatio", (int)_AspectRatio);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                    else if (gamepad.GetButtonDown("LB"))
                    {
                        if (_AspectRatio > AspectRatio.SIXTEEN_NINE)
                        {
                            _AspectRatio--;
                        }
                        else
                        {
                            _AspectRatio = AspectRatio.FIVE_FOUR;
                        }
                        PlayerPrefs.SetInt("AspectRatio", (int)_AspectRatio);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                }
                break;
            case "VSync":
                if (InputChecker.CurrentController == "Controller")
                {
                    if (gamepad.GetButtonDown("RB"))
                    {
                        if (VSync == 0)
                        {
                            VSync = 1;
                        }
                        else if (VSync == 1)
                        {
                            VSync = 0;
                        }
                        PlayerPrefs.SetInt("VSync", VSync);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                    else if (gamepad.GetButtonDown("LB"))
                    {
                        if (VSync == 0)
                        {
                            VSync = 1;
                        }
                        else if (VSync == 1)
                        {
                            VSync = 0;
                        }
                        PlayerPrefs.SetInt("VSync", VSync);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                }
                break;
            case "TextureQuality":
                if (InputChecker.CurrentController == "Controller")
                {
                    if (gamepad.GetButtonDown("RB"))
                    {
                        if (_TextureQuality < TextureQuality.HIGH)
                            _TextureQuality++;
                        else
                        {
                            _TextureQuality = 0;
                        }
                        PlayerPrefs.SetInt("TextureQuality", (int)_TextureQuality);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                    else if (gamepad.GetButtonDown("LB"))
                    {
                        if (_TextureQuality > TextureQuality.LOW)
                            _TextureQuality--;
                        else
                        {
                            _TextureQuality = TextureQuality.HIGH;
                        }
                        PlayerPrefs.SetInt("TextureQuality", (int)_TextureQuality);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                }
                break;
            case "MasterVolume":
                if (InputChecker.CurrentController == "Controller")
                {
                    if (gamepad.GetButton("LB"))
                    {
                        if (MasterVolume > 0)
                        {
                            MasterVolume--;
                        }
                        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                    else if (gamepad.GetButton("RB"))
                    {
                        if (MasterVolume < 100)
                        {
                            MasterVolume++;
                        }
                        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                }
                break;
            case "MusicVolume":
                if(InputChecker.CurrentController  == "Controller")
                {
                    if (gamepad.GetButton("LB"))
                    {
                        if (MusicVolume > 0)
                        {
                            MusicVolume--;
                        }
                        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                    else if (gamepad.GetButton("RB"))
                    {
                        if (MusicVolume < 100)
                        {
                            MusicVolume++;
                        }
                        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                }

                break;
            case "SFXVolume":
                if(InputChecker.CurrentController == "Controller")
                {
                    if (gamepad.GetButton("LB"))
                    {
                        if (EffectsVolume > 0)
                        {
                            EffectsVolume--;
                        }

                        PlayerPrefs.SetFloat("SFXVolume", EffectsVolume);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                    else if (gamepad.GetButton("RB"))
                    {
                        if (EffectsVolume < 100)
                        {
                            EffectsVolume++;
                        }

                        PlayerPrefs.SetFloat("SFXVolume", EffectsVolume);
                        UpdateTextAndApplySettings();
                        PlayerPrefs.Save();
                    }
                }
          
                break;
            default:
                break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Delayed Select.
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    IEnumerator DelayedSelect(Button button)
    {
        yield return new WaitForSeconds(0.05f);
        button.Select();
    }

    /// <summary>
    /// Toggle Scheme menus.
    /// </summary>
    public void ToggleSchemeMenu()
    {
        if(SettingsBase.activeInHierarchy)
        {
            SchemeWrapper.SetActive(true);
            SettingsBase.SetActive(false);
            StartCoroutine(DelayedSelect(StartButton));
            
        }
        else if(SchemeWrapper.activeInHierarchy)
        {
            SchemeWrapper.SetActive(false);
            SettingsBase.SetActive(true);
            StartCoroutine(DelayedSelect(SettingInfo.GamePadSchemes));
        }
    }

    public void IncreaseToggle(string DesiredMenu)
    {
        switch(DesiredMenu)
        {
            case "SelectionMenu":
                if (_MenuType < MenuType.WINDOW)
                    _MenuType++;
                else
                {
                    _MenuType = 0;
                }
                PlayerPrefs.SetInt("MenuType", (int)_MenuType);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
            case "WindowMode":
                if (_WindowMode < WindowMode.WINDOWED)
                    _WindowMode++;
                else
                {
                    _WindowMode = 0;
                }
                PlayerPrefs.SetInt("WindowMode", (int)_WindowMode);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
            case "Resolution":
                if (_Resolution < Resolution.THIRTY_EIGHT_FOURTY_BY_TWENTY_ONE_SIXTY)
                {
                    _Resolution++;
                }
                else
                {
                    _Resolution = 0;
                }
                PlayerPrefs.SetInt("Resolution", (int)_Resolution);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
            case "Aspect Ratio":
                if (_AspectRatio < AspectRatio.FIVE_FOUR)
                {
                    _AspectRatio++;
                }
                else
                {
                    _AspectRatio = AspectRatio.SIXTEEN_NINE;
                }
                PlayerPrefs.SetInt("AspectRatio", (int)_AspectRatio);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
            case "VSync":
                if (VSync == 0)
                {
                    VSync = 1;
                }
                else if (VSync == 1)
                {
                    VSync = 0;
                }
                PlayerPrefs.SetInt("VSync", VSync);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
            case "TextureQ":
                if (_TextureQuality < TextureQuality.HIGH)
                    _TextureQuality++;
                else
                {
                    _TextureQuality = 0;
                }
                PlayerPrefs.SetInt("TextureQuality", (int)_TextureQuality);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
        }
    }

    public void DecreaseToggle(string DesiredMenu)
    {
        switch (DesiredMenu)
        {
            case "SelectionMenu":
                if (_MenuType > MenuType.RADIAL)
                    _MenuType--;
                else
                {
                    _MenuType = MenuType.WINDOW; ;
                }
                UpdateTextAndApplySettings();
                PlayerPrefs.SetInt("MenuType", (int)_MenuType);
                PlayerPrefs.Save();
                break;
            case "WindowMode":
                if (_WindowMode > WindowMode.FULLSCREEN)
                    _WindowMode--;
                else
                {
                    _WindowMode = WindowMode.WINDOWED;
                }
                PlayerPrefs.SetInt("WindowMode", (int)_WindowMode);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
            case "Resolution":
                if (_Resolution > Resolution.NINETEEN_TWENTY_BY_TEN_EIGHTY)
                {
                    _Resolution--;
                }
                else
                {
                    _Resolution = Resolution.THIRTY_EIGHT_FOURTY_BY_TWENTY_ONE_SIXTY;
                }
                PlayerPrefs.SetInt("Resolution", (int)_Resolution);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
            case "Aspect Ratio":
                if (_AspectRatio > AspectRatio.SIXTEEN_NINE)
                {
                    _AspectRatio--;
                }
                else
                {
                    _AspectRatio = AspectRatio.FIVE_FOUR;
                }
                PlayerPrefs.SetInt("AspectRatio", (int)_AspectRatio);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
            case "VSync":
                if (VSync == 0)
                {
                    VSync = 1;
                }
                else if (VSync == 1)
                {
                    VSync = 0;
                }
                PlayerPrefs.SetInt("VSync", VSync);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
            case "TextureQ":
                if (_TextureQuality > TextureQuality.LOW)
                    _TextureQuality--;
                else
                {
                    _TextureQuality = TextureQuality.HIGH;
                }
                PlayerPrefs.SetInt("TextureQuality", (int)_TextureQuality);
                UpdateTextAndApplySettings();
                PlayerPrefs.Save();
                break;
        }
    }

    /// <summary>
    /// Loads all settins from player prefs.
    /// </summary>
    private void LoadSettings()
    {
        PlayerPrefs.GetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.GetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.GetFloat("SFXVolume", EffectsVolume);
        _Resolution = (Resolution)PlayerPrefs.GetInt("Resolution");
        _AspectRatio = (AspectRatio)PlayerPrefs.GetInt("AspectRatio");
        VSync = PlayerPrefs.GetInt("VSync");
        _WindowMode = (WindowMode)PlayerPrefs.GetInt("WindowMode");
        _MenuType = (MenuType)PlayerPrefs.GetInt("MenuType");
        _TextureQuality = (TextureQuality)PlayerPrefs.GetInt("TextureQuality");
        _SchemeIndex = PlayerPrefs.GetInt("GamepadScheme");
        Debug.Log("Player Settings Loaded!");
    }

    /// <summary>
    /// Updates setting text.
    /// </summary>
    private void UpdateTextAndApplySettings()
    {
        if(VSync == 0)
        {
            VSyncText.text = "OFF";
            QualitySettings.vSyncCount = 0;
        }
        else if(VSync == 1)
        {
            VSyncText.text = "ON";
            QualitySettings.vSyncCount = 1;
        }

        switch(_AspectRatio)
        {
            case AspectRatio.FIVE_FOUR:
                AspectRatioText.text = "5:4";
                break;
            case AspectRatio.FOUR_THREE:
                AspectRatioText.text = "4:3";
                break;
            case AspectRatio.SIXTEEN_NINE:
                AspectRatioText.text = "16:9";
                break;
            case AspectRatio.SIXTEEN_TEN:
                AspectRatioText.text = "16:10";
                break;
            case AspectRatio.THREE_TWO:
                AspectRatioText.text = "3:2";
                break;
            default:
                break;
        }

        switch(_MenuType)
        {
            case MenuType.RADIAL:
                MenuTypeText.text = "RADIAL";
                break;
            case MenuType.WINDOW:
                MenuTypeText.text = "WINDOW";
                break;
        }

        switch (_TextureQuality)
        {
            case TextureQuality.HIGH:
                TextureQualityText.text = "HIGH";
                QualitySettings.SetQualityLevel(2);
                break;
            case TextureQuality.MEDIUM:
                TextureQualityText.text = "MEDIUM";
                QualitySettings.SetQualityLevel(1);
                break;
            case TextureQuality.LOW:
                TextureQualityText.text = "LOW";
                QualitySettings.SetQualityLevel(0);
                break;
            //case TextureQuality.ULTRA:
            //    TextureQualityText.text = "ULTRA";
            //    QualitySettings.SetQualityLevel(5);
            //    break;
            //case TextureQuality.VERY_LOW:
            //    TextureQualityText.text = "VERY LOW";
            //    QualitySettings.SetQualityLevel(0);
            //    break;
            //case TextureQuality.VERY_HIGH:
            //    TextureQualityText.text = "VERY HIGH";
            //    QualitySettings.SetQualityLevel(4);
            default:
                break;
        }
        
        switch(_WindowMode)
        {
            case WindowMode.FULLSCREEN:
                WindowModeText.text = "FULLSCREEN";
                if (!Screen.fullScreen)
                {
                    Screen.fullScreen = !Screen.fullScreen;
                }
                break;
            case WindowMode.WINDOWED:
                WindowModeText.text = "WINDOWED";
                if (Screen.fullScreen)
                {
                    Screen.fullScreen = !Screen.fullScreen;
                }
                break;
            default:
                break;
        }
        bool fullscreen = false;
        switch (_Resolution)
        {
            case Resolution.NINETEEN_TWENTY_BY_TEN_EIGHTY:
                ResolutionText.text = "1920 x 1080";
             
                if(_WindowMode == WindowMode.FULLSCREEN)
                {
                    fullscreen = true;
                }
                Screen.SetResolution(1920, 1080,fullscreen);
                break;
            case Resolution.TWELVE_EIGHTY_BY_SEVEN_TWENTY:
                ResolutionText.text = "1280 X 720";
            
                if(_WindowMode == WindowMode.FULLSCREEN)
                {
                    fullscreen = true;
                }
                Screen.SetResolution(1280, 720, fullscreen);
                break;
            case Resolution.SIXTEEN_HUNDRED_BY_NINE_HUNDRED:
                ResolutionText.text = "1600 X 900";
                if(_WindowMode == WindowMode.FULLSCREEN)
                {
                    fullscreen = true;
                }
                Screen.SetResolution(1600, 900, fullscreen);
                break;
            case Resolution.TWENTY_FIVE_SIXTY_BY_FOURTEEN_FORTY:
                ResolutionText.text = "2560 x 1420";
                if(_WindowMode == WindowMode.FULLSCREEN)
                {
                    fullscreen = true;
                }
                Screen.SetResolution(2560, 1420, fullscreen);
                break;
            case Resolution.THIRTY_EIGHT_FOURTY_BY_TWENTY_ONE_SIXTY:
                ResolutionText.text = "3840 x 2160";
                if(_WindowMode == WindowMode.FULLSCREEN)
                {
                    fullscreen = true;
                }
                Screen.SetResolution(3840, 2160, fullscreen);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Changes text based on controller.
    /// </summary>
    private void ChangeControllerText()
    {
        if (_MenuNavigator.SettingsMenu.AreaState == MenuNavigator.SceneAreaState.ACTIVE)
        {
            if (InputChecker.CurrentController == "Controller" )
            {
                SettingInfo.SelectionMenuLeft.GetComponentInChildren<Text>().text = "LT";
                SettingInfo.SelectionMenuRight.GetComponentInChildren<Text>().text = "RT";
                SettingInfo.TextureQualityLeft.GetComponentInChildren<Text>().text = "LT";
                SettingInfo.TextureQualityRight.GetComponentInChildren<Text>().text = "RT";
                SettingInfo.V_SyncLeft.GetComponentInChildren<Text>().text = "LT";
                SettingInfo.V_SyncRight.GetComponentInChildren<Text>().text = "RT";
                SettingInfo.WindowModeLeft.GetComponentInChildren<Text>().text = "LT";
                SettingInfo.WindowModeRight.GetComponentInChildren<Text>().text = "RT";
                SettingInfo.ResolutionLeft.GetComponentInChildren<Text>().text = "LT";
                SettingInfo.ResolutionRight.GetComponentInChildren<Text>().text = "RT";
                SettingInfo.AspectRatioLeft.GetComponentInChildren<Text>().text = "LT";
                SettingInfo.AspectRatioRight.GetComponentInChildren<Text>().text = "RT";
            }
            else if(InputChecker.CurrentController == "Keyboard")
            {
                SettingInfo.SelectionMenuLeft.GetComponentInChildren<Text>().text = "<";
                SettingInfo.SelectionMenuRight.GetComponentInChildren<Text>().text = ">";
                SettingInfo.TextureQualityLeft.GetComponentInChildren<Text>().text = "<";
                SettingInfo.TextureQualityRight.GetComponentInChildren<Text>().text = ">";
                SettingInfo.V_SyncLeft.GetComponentInChildren<Text>().text = "<";
                SettingInfo.V_SyncRight.GetComponentInChildren<Text>().text = ">";
                SettingInfo.WindowModeLeft.GetComponentInChildren<Text>().text = "<";
                SettingInfo.WindowModeRight.GetComponentInChildren<Text>().text = ">";
                SettingInfo.ResolutionLeft.GetComponentInChildren<Text>().text = "<";
                SettingInfo.ResolutionRight.GetComponentInChildren<Text>().text = ">";
                SettingInfo.AspectRatioLeft.GetComponentInChildren<Text>().text = "<";
                SettingInfo.AspectRatioRight.GetComponentInChildren<Text>().text = ">";
            }
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Saves values from UI widgets to variables to be saved to player prefs.
        /// </summary>
    private void NavigateSettingsMenu()
    {
        if(InputChecker.CurrentController == "Controller")
        {
            MasterVolSlider.value = MasterVolume;
            MusicVolSlider.value = MusicVolume;
            EffectVolSlider.value = EffectsVolume;

            MasterVolSlider.GetComponentInChildren<Text>().text = MasterVolume.ToString();
            MusicVolSlider.GetComponentInChildren<Text>().text = MusicVolume.ToString();
            EffectVolSlider.GetComponentInChildren<Text>().text = EffectsVolume.ToString();
        }
        else if(InputChecker.CurrentController == "Keyboard")
        {
            MasterVolume = (int)MasterVolSlider.value;
            MusicVolume = (int)MusicVolSlider.value;
            EffectsVolume = (int)EffectVolSlider.value;
            MasterVolSlider.GetComponentInChildren<Text>().text = MasterVolume.ToString();
            MusicVolSlider.GetComponentInChildren<Text>().text = MusicVolume.ToString();
            EffectVolSlider.GetComponentInChildren<Text>().text = EffectsVolume.ToString();
        }

    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
