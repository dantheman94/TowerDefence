using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 10/11/2018
//
//******************************

public class ShowcaseDescription : MonoBehaviour {


    // INSPECTOR
    ///////////////////////////////////////////////////////////

    [System.Serializable]
    public struct UnitDecriptionDetails
    {
        public Sprite UnitIcon;
        public string Name;
        public string Description;
    }


    public Text DescriptionText;
    public Text NameText;
    public Image UnitIcon;

    public List<UnitDecriptionDetails> DescriptionDetails;

    // FUNCTIONS
    ///////////////////////////////////////////////////////////

    // Use this for initialization
    void Start () {
        DisplayUnitDescription("Dwarf Soldier");
	}

    ///////////////////////////////////////////////////////////

    public void DisplayUnitDescription(string a_buildable)
    {
        switch (a_buildable)
        {
            case "Dwarf Soldier":
                DescriptionText.text = DescriptionDetails[2].Description;
                NameText.text = DescriptionDetails[2].Name;
                UnitIcon.sprite = DescriptionDetails[2].UnitIcon;
                UnitIcon.color = Color.white;
                break;
            case "Infantry Soldier":
                DescriptionText.text = DescriptionDetails[5].Description;
                NameText.text = DescriptionDetails[5].Name;
                UnitIcon.sprite = DescriptionDetails[5].UnitIcon;
                UnitIcon.color = Color.white;
                break;
            case "Vehicle Specialist":
                DescriptionText.text = DescriptionDetails[6].Description;
                NameText.text = DescriptionDetails[6].Name;
                UnitIcon.sprite = DescriptionDetails[6].UnitIcon;
                UnitIcon.color = Color.white;
                break;
            case "Grumblebuster":
                DescriptionText.text = DescriptionDetails[0].Description;
                NameText.text = DescriptionDetails[0].Name;
                UnitIcon.sprite = DescriptionDetails[0].UnitIcon;

                break;
            case "Skylancer":
                DescriptionText.text = DescriptionDetails[3].Description;
                NameText.text = DescriptionDetails[3].Name;
                UnitIcon.sprite = DescriptionDetails[3].UnitIcon;
                break;
            case "Siege Engine":
                DescriptionText.text = DescriptionDetails[1].Description;
                NameText.text = DescriptionDetails[1].Name;
                UnitIcon.sprite = DescriptionDetails[1].UnitIcon;
                break;
            case "Light Airship":
                DescriptionText.text = DescriptionDetails[8].Description;
                NameText.text = DescriptionDetails[8].Name;
                UnitIcon.sprite = DescriptionDetails[8].UnitIcon;
                break;
            case "Support Airship":
                DescriptionText.text = DescriptionDetails[4].Description;
                NameText.text = DescriptionDetails[4].Name;
                UnitIcon.sprite = DescriptionDetails[4].UnitIcon;
                break;
            case "Heavy Airship":
                DescriptionText.text = DescriptionDetails[7].Description;
                NameText.text = DescriptionDetails[7].Name;
                UnitIcon.sprite = DescriptionDetails[7].UnitIcon;
                break;
            case "Siege Tower":
                DescriptionText.text = DescriptionDetails[11].Description;
                NameText.text = DescriptionDetails[11].Name;
                UnitIcon.sprite = DescriptionDetails[11].UnitIcon;
                break;
            case "Base Turret":
                DescriptionText.text = DescriptionDetails[10].Description;
                NameText.text = DescriptionDetails[10].Name;
                UnitIcon.sprite = DescriptionDetails[10].UnitIcon;
                break;
            case "Watchtower":
                DescriptionText.text = DescriptionDetails[9].Description;
                NameText.text = DescriptionDetails[9].Name;
                UnitIcon.sprite = DescriptionDetails[9].UnitIcon;
                break;
            case "Supply Generator":
                DescriptionText.text = DescriptionDetails[13].Description;
                NameText.text = DescriptionDetails[13].Name;
                UnitIcon.sprite = DescriptionDetails[13].UnitIcon;
                break;
            case "Power Generator":
                DescriptionText.text = DescriptionDetails[12].Description;
                NameText.text = DescriptionDetails[12].Name;
                UnitIcon.sprite = DescriptionDetails[12].UnitIcon;
                break;
            case "Barracks":
                DescriptionText.text = DescriptionDetails[14].Description;
                NameText.text = DescriptionDetails[14].Name;
                UnitIcon.sprite = DescriptionDetails[14].UnitIcon;
                break;
            case "Airpad":
                DescriptionText.text = DescriptionDetails[17].Description;
                NameText.text = DescriptionDetails[17].Name;
                UnitIcon.sprite = DescriptionDetails[17].UnitIcon;
                break;
            case "Garage":
                DescriptionText.text = DescriptionDetails[15].Description;
                NameText.text = DescriptionDetails[15].Name;
                UnitIcon.sprite = DescriptionDetails[15].UnitIcon;
                break;
            case "Townhall":
                DescriptionText.text = DescriptionDetails[18].Description;
                NameText.text = DescriptionDetails[18].Name;
                UnitIcon.sprite = DescriptionDetails[18].UnitIcon;
                break;
            case "War Room":
                DescriptionText.text = DescriptionDetails[16].Description;
                NameText.text = DescriptionDetails[16].Name;
                UnitIcon.sprite = DescriptionDetails[16].UnitIcon;
                break;
            default:
                break;
        }
        
    }

    ///////////////////////////////////////////////////////////
}
