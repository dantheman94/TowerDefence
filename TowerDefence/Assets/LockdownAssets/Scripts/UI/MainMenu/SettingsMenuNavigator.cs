using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 27/08/2018
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
        VERY_LOW, LOW, MEDIUM, HIGH, VERY_HIGH, ULTRA
    }

    public enum WindowMode
    {
        FULLSCREEN, WINDOWED
    }

    public enum Resolution
    {
        NINETEEN_TWENTY_BY_TEN_EIGHTY,
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

    [Header("----------------------")]
    [Space]
    [Header("CURRENT SELECTION")]
    [Tooltip("What menu is currently selected.")]
    public string CurrentSelection;

    [Header("----------------------")]
    [Space]
    [Header("VOLUME REFERENCES")]
    public float MasterVolume = 100;
    public float MusicVolume = 100;
    public float EffectsVolume = 100;


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

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************


    private MenuNavigator _MenuNavigator;

    private xb_gamepad gamepad;

    private MenuNavigator.MenuArea SettingsMenu;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        LoadSettings();
        gamepad = GamepadManager.Instance.GetGamepad(1);
        _MenuNavigator = GetComponent<MenuNavigator>();
        UpdateTextAndApplySettings();
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {
        NavigateSettingsMenu();
        SwitchSettings();
        UpdateTextAndApplySettings();
        ApplySettings();
        Debug.Log(CurrentSelection);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Saves player settings to player prefs.
    /// </summary>
    public void SavePlayerSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", EffectsVolume);
        PlayerPrefs.SetInt("Resolution", (int)_Resolution);
        PlayerPrefs.SetInt("AspectRatio", (int)_AspectRatio);
        PlayerPrefs.SetInt("VSync", VSync);
        PlayerPrefs.SetInt("WindowMode", (int)_WindowMode);
        PlayerPrefs.SetInt("TextureQuality", (int)_TextureQuality);
        PlayerPrefs.Save();
        Debug.Log("Player Settings Saved!");
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  Do actions based on current active scene.
    /// </summary>
    private void SwitchSettings()
    {
         switch(CurrentSelection)
        {
            case "Gamepad":
                
                break;
            case "KeyboardBindings":
                break;
            case "MenuType":
                if(gamepad.GetButtonDown("RB"))
                {
                    if(_MenuType < MenuType.WINDOW)
                    _MenuType++;
                    else
                    {
                        _MenuType = 0;
                    }
                }
                else if(gamepad.GetButtonDown("LB"))
                {
                    if (_MenuType > MenuType.RADIAL)
                        _MenuType--;
                    else
                    {
                        _MenuType = MenuType.WINDOW; ;
                    }
                }
                break;
            case "WindowMode":
                if (gamepad.GetButtonDown("RB"))
                {
                    if (_WindowMode < WindowMode.WINDOWED)
                        _WindowMode++;
                    else
                    {
                        _WindowMode = 0;
                    }
                }
                else if (gamepad.GetButtonDown("LB"))
                {
                    if (_WindowMode > WindowMode.FULLSCREEN)
                        _WindowMode--;
                    else
                    {
                        _WindowMode = WindowMode.WINDOWED;
                    }
                }
                break;
            case "Resolution":
                break;
            case "AspectRatio":
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
                }
                else if(gamepad.GetButtonDown("LB"))
                {
                    if (_AspectRatio > AspectRatio.SIXTEEN_NINE)
                    {
                        _AspectRatio--;
                    }
                    else
                    {
                        _AspectRatio = AspectRatio.FIVE_FOUR;
                    }
                }
                break;
            case "VSync":
                if (gamepad.GetButtonDown("RB"))
                {
                   if(VSync == 0)
                    {
                        VSync = 1;
                    }
                   else if(VSync == 1)
                    {
                        VSync = 0;
                    }
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
                }
                break;
            case "TextureQuality":
                if (gamepad.GetButtonDown("RB"))
                {
                    if (_TextureQuality < TextureQuality.ULTRA)
                        _TextureQuality++;
                    else
                    {
                        _TextureQuality = 0;
                    }
                }
                else if (gamepad.GetButtonDown("LB"))
                {
                    if (_TextureQuality > TextureQuality.VERY_LOW)
                        _TextureQuality--;
                    else
                    {
                        _TextureQuality = TextureQuality.ULTRA ;
                    }
                }
                break;
            case "MasterVolume":
                if(gamepad.GetButton("LB"))
                {
                    if(MasterVolume > 0)
                    {
                        MasterVolume--;
                    }
                }
                else if(gamepad.GetButton("RB"))
                {
                    if (MasterVolume < 100)
                    {
                        MasterVolume++;
                    }
                }
                break;
            case "MusicVolume":
                if (gamepad.GetButton("LB"))
                {
                    if (MusicVolume > 0)
                    {
                        MusicVolume--;
                    }
                }
                else if (gamepad.GetButton("RB"))
                {
                    if (MusicVolume < 100)
                    {
                        MusicVolume++;
                    }
                }
                break;
            case "SFXVolume":
                if (gamepad.GetButton("LB"))
                {
                    if (EffectsVolume > 0)
                    {
                        EffectsVolume--;
                    }
                }
                else if (gamepad.GetButton("RB"))
                {
                    if (EffectsVolume < 100)
                    {
                        EffectsVolume++;
                    }
                }
                break;
            default:
                break;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void ApplySettings()
    {
        switch (_AspectRatio)
        {
            case AspectRatio.SIXTEEN_NINE:
                break;
            default:
                break;
        }

        switch (_WindowMode)
        {
            case WindowMode.FULLSCREEN:
           
                break;
            case WindowMode.WINDOWED:
            
                break;
            default:
                break;
        }
        
    }

    private void LoadSettings()
    {
        PlayerPrefs.GetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.GetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.GetFloat("SFXVolume", EffectsVolume);
        _Resolution = (Resolution)PlayerPrefs.GetInt("Resolution");
        _AspectRatio = (AspectRatio)PlayerPrefs.GetInt("AspectRatio");
        VSync = PlayerPrefs.GetInt("VSync");
        _WindowMode = (WindowMode)PlayerPrefs.GetInt("WindowMode");
        _TextureQuality = (TextureQuality)PlayerPrefs.GetInt("TextureQuality");
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
                QualitySettings.SetQualityLevel(3);
                break;
            case TextureQuality.MEDIUM:
                TextureQualityText.text = "MEDIUM";
                QualitySettings.SetQualityLevel(2);
                break;
            case TextureQuality.LOW:
                TextureQualityText.text = "LOW";
                QualitySettings.SetQualityLevel(1);
                break;
            case TextureQuality.ULTRA:
                TextureQualityText.text = "ULTRA";
                QualitySettings.SetQualityLevel(5);
                break;
            case TextureQuality.VERY_LOW:
                TextureQualityText.text = "VERY LOW";
                QualitySettings.SetQualityLevel(0);
                break;
            case TextureQuality.VERY_HIGH:
                TextureQualityText.text = "VERY HIGH";
                QualitySettings.SetQualityLevel(4);
                break;
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

        switch(_Resolution)
        {
            case Resolution.NINETEEN_TWENTY_BY_TEN_EIGHTY:
                ResolutionText.text = "1920 x 1080";
                bool fullscreen = false;
                if(_WindowMode == WindowMode.FULLSCREEN)
                {
                    fullscreen = true;
                }
                Screen.SetResolution(1920, 1080,fullscreen);
                break;
            default:
                break;
        }

        if (_MenuNavigator.SettingsMenu.AreaState == MenuNavigator.SceneAreaState.ACTIVE)
        {
            if (gamepad.IsConnected)
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
            else
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
        MasterVolSlider.value = MasterVolume;
        MusicVolSlider.value = MusicVolume;
        EffectVolSlider.value = EffectsVolume;
        MasterVolSlider.GetComponentInChildren<Text>().text = MasterVolume.ToString();
        MusicVolSlider.GetComponentInChildren<Text>().text = MusicVolume.ToString();
        EffectVolSlider.GetComponentInChildren<Text>().text = EffectsVolume.ToString();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
