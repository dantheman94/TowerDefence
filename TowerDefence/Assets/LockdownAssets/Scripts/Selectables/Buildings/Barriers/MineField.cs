using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 8/8/2018
//
//******************************

public class MineField : Barrier {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MINE-FIELD PROPERTIES")]
    [Space]
    public GameObject MineStencil;
    public List<GameObject> MineVectorObjects;
    public bool LargeMineField = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    List<GameObject> _UndetonatedMineList;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        _UndetonatedMineList = new List<GameObject>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected override void OnActiveState() {
        base.OnActiveState();

        // Create new mines
        for (int i = 0; i < MineVectorObjects.Count; i++) {

            // Initialize
            Vector3 spawnPosition = MineVectorObjects[i].transform.position;
            GameObject mine = ObjectPooling.Spawn(MineStencil.gameObject, spawnPosition);
            mine.SetActive(true);

            _UndetonatedMineList.Add(mine);
        }

        if (LargeMineField) {

            // Upgrade all the undetonated mines
            for (int i = 0; i < _UndetonatedMineList.Count; i++) {

                ///_UndetonatedMineList[i].Upgrade(explosionRadius, damage, damagefalloff);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}