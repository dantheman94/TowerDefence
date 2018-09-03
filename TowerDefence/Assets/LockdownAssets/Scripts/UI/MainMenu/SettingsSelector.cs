using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsSelector : MonoBehaviour, ISelectHandler, IPointerExitHandler {


    public SettingsMenuNavigator _SettingsNavigator;
    public string CurrentSetting;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSelect(BaseEventData eventData)
    {
        _SettingsNavigator.CurrentSelection = CurrentSetting;
        Debug.Log("woot");
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
