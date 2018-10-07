using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//-=-=-=-=-=-=-=-=-=-
// Created by: Angus Secomb
// Last Edited: 8/10/18
// Editor: Angus Secomb
//-=-=-=-=-=-=-=-=-=-

public class HideObject : MonoBehaviour {

    public GameObject DesiredObject;

    private Toggle _Toggle;

	// Use this for initialization
	void Start () {
        _Toggle = GetComponent<Toggle>();
        _Toggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(_Toggle);
        });
    }
	
    public void Hide()
    {
        Destroy(DesiredObject);
    }

    void ToggleValueChanged(Toggle change)
    {
        Hide();
    }
}
