using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/10/2018
//
//******************************

public class UI_TurretVariant : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" COLOURS")]
    [Space]
    public Color BaseTurretColour = Color.clear;
    public Color AntiInfantryColour = Color.yellow;
    public Color AntiVehicleColour = Color.red;
    public Color AntiAirColour = Color.magenta;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Image _ImageComponent = null;
    private Tower _TowerAttached = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is created.
    /// </summary>
    private void Start() {

        _ImageComponent = GetComponent<Image>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {
        
        // Change image colour based on tower type
        if (_TowerAttached != null && _ImageComponent != null) {

            switch (_TowerAttached.TowerType) {

                case Tower.ETowerType.WatchTower: {

                    _ImageComponent.color = BaseTurretColour;
                    break;
                }

                case Tower.ETowerType.SiegeTurret: {

                    _ImageComponent.color = BaseTurretColour;
                    break;
                }

                case Tower.ETowerType.MiniTurret: {

                    _ImageComponent.color = BaseTurretColour;
                    break;
                }

                case Tower.ETowerType.AntiInfantryTurret: {

                    _ImageComponent.color = AntiInfantryColour;
                    break;
                }

                case Tower.ETowerType.AntiVehicleTurret: {

                    _ImageComponent.color = AntiVehicleColour;
                    break;
                }

                case Tower.ETowerType.AntiAirTurret: {

                    _ImageComponent.color = AntiAirColour;
                    break;
                }
                default: break;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Sets reference to the tower attached for this widget.
    /// </summary>
    /// <param name="tower"></param>
    public void SetTowerAttached(Tower tower) { _TowerAttached = tower; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
