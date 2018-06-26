using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 26/6/2018
//
//******************************

public class PowerStation : Generator {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" POWER STATION PROPERTIES")]
    [Space]
    public List<GameObject> Pylons;
    [Space]
    public float PylonVerticalOffset = 2f;
    public float RotateSpeed = 4f;
    public float OrbitSpeed = 100f;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    protected override void Update() {
        base.Update();

        // Rotate each of the pylons on a grouped axis at the center
        Vector3 center = new Vector3(gameObject.transform.position.x, PylonVerticalOffset, gameObject.transform.position.z);
        foreach (var pylon in Pylons) {

            pylon.transform.Rotate(Vector3.up, -RotateSpeed);
            pylon.transform.RotateAround(center, Vector3.up, OrbitSpeed * Time.deltaTime);
        }
    }

} 