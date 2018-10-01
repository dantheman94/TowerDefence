using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 1/10/2018
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
    public Mine MineStencil;
    public List<GameObject> MineVectorObjects;
    public bool LargeMineField = false;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    private List<Mine> _UndetonatedMineList;

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

        _UndetonatedMineList = new List<Mine>();
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected override void OnActiveState() {
        base.OnActiveState();

        // Create new mines (does it for both small & large minefields)
        for (int i = 0; i < MineVectorObjects.Count; i++) {

            // Initialize
            Vector3 spawnPosition = MineVectorObjects[i].transform.position;
            Mine mine = ObjectPooling.Spawn(MineStencil.gameObject, spawnPosition).GetComponent<Mine>();
            if (mine != null) {

                mine.SetTeam(Team);
                mine.SetAttachedMineField(this);
            }

            _UndetonatedMineList.Add(mine);
        }

        // Upgrade all the undetonated mines
        if (LargeMineField) {
            
            for (int i = 0; i < _UndetonatedMineList.Count; i++) {

                ///_UndetonatedMineList[i].Upgrade(explosionRadius, damage, damagefalloff);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference to the undetonated mines array.
    /// </summary>
    /// <returns>
    //  List<Mine>
    /// </returns>
    public List<Mine> GetUndetonatedMines() { return _UndetonatedMineList; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}