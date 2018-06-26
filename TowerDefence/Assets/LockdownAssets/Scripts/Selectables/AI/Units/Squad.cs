using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 23/6/2018
//
//******************************

public class Squad : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SQUAD PROPERTIES")]
    [Space]
    public int SquadMaxSize;
    public Unit SquadUnit;
    public float FlockingRadius;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private int _SquadCurrentSize;
    private float _SquadHealth;
    private List<Unit> _Squad;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected override void Start() {
        base.Start();

        _Squad = new List<Unit>();
    }

    protected override void Update() {
        base.Update();

        // Get total sum of health for all units in the squad
        int maxSquadHealth = 0, currentSquadHealth = 0;
        foreach (var unit in _Squad) {

            maxSquadHealth += unit.MaxHitPoints;
            currentSquadHealth += unit.getHitPoints();
        }

        // Normalize the health between a range of 0.0 - 1.0
        if (maxSquadHealth > 0) { _SquadHealth = currentSquadHealth / maxSquadHealth; }
    }

}
