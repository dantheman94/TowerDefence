using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//-=-=-=-=-=-=-=-=-=-=-=-=-=
// Created by: Angus Secomb
// Last modified: 23/10/18
// Editor: Angus
//-=-=-=-=-=-=-=-=-=-=-=-=-=

public class VeteranUI : MonoBehaviour {

    public Image PowerImage;
    public Image SupplyImage;
    public Text PowerCostText;
    public Text SupplyCostText;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if(VeteranUpgrade.IsActive)
        {
            PowerImage.enabled = true;
            SupplyImage.enabled = true;
            PowerCostText.enabled = true;
            SupplyCostText.enabled = true;
            PowerCostText.transform.position = new Vector2(Input.mousePosition.x + 110, Input.mousePosition.y - 240);
            SupplyCostText.transform.position = new Vector2(Input.mousePosition.x - 25, Input.mousePosition.y - 240);
            SupplyImage.transform.position = new Vector2(Input.mousePosition.x - 100, Input.mousePosition.y - 220);
            PowerImage.transform.position = new Vector2(Input.mousePosition.x + 50, Input.mousePosition.y - 220);
            PowerCostText.text = (VeteranUpgrade.PowerCost * VeteranUpgrade._Units.Count).ToString();
            SupplyCostText.text = (VeteranUpgrade.SupplyCost * VeteranUpgrade._Units.Count).ToString();
        }
        else
        {
            PowerImage.enabled = false;
            SupplyImage.enabled = false;
            PowerCostText.enabled = false;
            SupplyCostText.enabled = false;
        }


    }
}
