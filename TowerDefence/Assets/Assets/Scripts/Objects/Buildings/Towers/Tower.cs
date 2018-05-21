using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 5/8/2018
//
//******************************

public class Tower : Building {

    //******************************************************************************************************************************
    // INSPECTOR

    public enum ETowerType { WatchTower, SiegeTurret, Turret, AntiInfantry, AntiVehicle, AntiAir }

    //******************************************************************************************************************************
    // FUNCTIONS

    protected override void Awake() { base.Awake();

    }

    protected override void Start () { base.Start();

	}

    protected override void Update () { base.Update();

	}

    protected override void OnGUI() { base.OnGUI();

    }

}