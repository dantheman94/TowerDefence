using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//-=-=-=-=-=-=-=-=-=-=-=-=
//Created by: Angus Secomb
//Last modified: 28/11/18
//Editor: Angus
//-=-=-=-=-=-=-=-=-=-=-=-=


public class GamepadSchemeManager : MonoBehaviour {

    public struct GamepadScheme
    {
        public string AttackandMove;
        public string Ability;
        public string Selection;
        public string ExitMenuandUnselect;
        public string SelectAllUnits;
        public string SelectUnitsOnScreen;
        public string BaseSnapUp;
        public string BaseSnapDown;
        public string SquadToggleUp;
        public string SquadToggleDown;
        public string AssignSquad;
        public string ClearOrReplaceSquad;
    }

    public GamepadScheme SchemeOne;
    public GamepadScheme SchemeTwo;
    public GamepadScheme SchemeThree;
    public GamepadScheme CustomScheme;  

    private int _SchemeIndex = 1;
    private GamepadScheme _ActiveScheme;
    private static GamepadSchemeManager _Singleton;

	// Use this for initialization
	void Start () {

        if (_Singleton != null && _Singleton != this)
        {

            Destroy(this.gameObject);
            return;
        }

        _Singleton = this;


        _SchemeIndex = PlayerPrefs.GetInt("GamepadScheme");
        SchemeOne.AttackandMove = "X";
        SchemeOne.Selection = "A";
        SchemeOne.Ability = "Y";
        SchemeOne.ExitMenuandUnselect = "B";
        SchemeOne.SelectAllUnits = "LB";
        SchemeOne.SelectUnitsOnScreen = "RB";
        SchemeOne.BaseSnapUp = "dPadUp";
        SchemeOne.BaseSnapDown = "dPadDown";
        SchemeOne.SquadToggleUp = "dPadLeft";
        SchemeOne.SquadToggleDown = "dPadRight";

        SchemeTwo.AttackandMove = "A";
        SchemeTwo.Selection = "X";
        SchemeTwo.Ability = "B";
        SchemeTwo.ExitMenuandUnselect = "Y";
        SchemeTwo.SelectAllUnits = "RB";
        SchemeTwo.SelectUnitsOnScreen = "LB";
        SchemeTwo.BaseSnapUp = "dPadLeft";
        SchemeTwo.BaseSnapDown = "dPadRight";
        SchemeTwo.SquadToggleUp = "dPadUp";
        SchemeTwo.SquadToggleDown = "dPadDown";

        SchemeThree.AttackandMove = "A";
        SchemeThree.Selection = "X";
        SchemeThree.Ability = "B";
        SchemeThree.ExitMenuandUnselect = "Y";
        SchemeThree.SelectAllUnits = "LB";
        SchemeThree.SelectUnitsOnScreen = "RB";
        SchemeThree.BaseSnapUp = "dPadUp";
        SchemeThree.BaseSnapDown = "dPadDown";
        SchemeThree.SquadToggleUp = "dPadLeft";
        SchemeThree.SquadToggleDown = "dPadRight";

        _ActiveScheme = SchemeOne;
    }

    /// <summary>
    /// Switch between schemes.
    /// </summary>
    public void SwitchSchemes()
    {
        switch (_SchemeIndex)
        {
            case 1:
                _ActiveScheme = SchemeOne;
                break;
            case 2:
                _ActiveScheme = SchemeTwo;
                break;
            case 3:
                _ActiveScheme = SchemeThree;
                break;
            case 4:
                _ActiveScheme = CustomScheme;
                break;
            default:
                break;
        }
        
    }

    /// <summary>
    /// Cheeky little instance reference.
    /// </summary>
    public static GamepadSchemeManager Instance
    {
        get
        {
            if (_Singleton == null)
            {
                Debug.LogError("[Gamepad Scheme Manager]: Instance does not currently exist.");
                return null;
            }

            return _Singleton ;
        }
    }

    /// <summary>
    /// Assign shit.
    /// </summary>
    public void AssignCustomKey()
    {

    }

    /// <summary>
    /// Returns active gamepad scheme.
    /// </summary>
    /// <returns></returns>
    public GamepadScheme GetActiveScheme()
    {
        return _ActiveScheme;
    }
}
