using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TowerDefence;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/10/2018
//
//******************************

public class Building : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" BUILDABLES ")]
    public List<Unit> Units;

    [Space]
    [Header("-----------------------------------")]
    [Header(" UPGRADES ")]
    public List<Upgrades> Upgrades;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************



    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    public void OnSelectionWheel() {

        // Show building slot wheel
        if (_Player) {

            // Cast object
            List<Abstraction> selectables = new List<Abstraction>();
            foreach (var item in Units)     { selectables.Add(item); }
            foreach (var item in Upgrades)  { selectables.Add(item); }

            // Update list then display on screen
            ///_Player._HUD.SelectionWheel.UpdateList(selectables);

            // Show selection wheel
            GameManager.Instance.SelectionWheel.SetActive(true);

            _IsCurrentlySelected = true;
        }
    }

    public override void OnWheelSelect(BuildingSlot buildingSlot) { base.OnWheelSelect(buildingSlot);
        
        // Update building slot ref with building
        buildingSlot.setBuildingOnSlot(this.GetComponent<Building>());
    }

}