using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 3/10/2018
//
//******************************

public class SupplyGenerator : Generator {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header(" SUPPLY GENERATOR PROPERTIES")]
    [Header("-----------------------------------")]
    [Space]
    public GameObject MetalSawObject = null;
    public float MetalSawRotationSpeed = 1f;
    [Space]
    public GameObject WaterWheelObject = null;
    public float WaterWheelRotationSpeed = 1f;

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

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();
        
        // Constantly rotate the water wheel
        if (WaterWheelObject != null) {

            // Additive rotation
            Quaternion rot = new Quaternion(WaterWheelObject.transform.rotation.x,
                                            WaterWheelObject.transform.rotation.y,
                                            WaterWheelObject.transform.rotation.z,
                                            WaterWheelObject.transform.rotation.w);
            rot.eulerAngles += new Vector3(0, 0, WaterWheelRotationSpeed * Time.deltaTime);
            WaterWheelObject.transform.rotation = rot;
        }

        // Constantly rotate the metal saw object in the opposite direction to the saw
        if (MetalSawObject != null) {

            // Additive rotation
            Quaternion rot = new Quaternion(MetalSawObject.transform.rotation.x,
                                            MetalSawObject.transform.rotation.y,
                                            MetalSawObject.transform.rotation.z,
                                            MetalSawObject.transform.rotation.w);
            rot.eulerAngles += new Vector3(0, 0, -MetalSawRotationSpeed * Time.deltaTime);
            MetalSawObject.transform.rotation = rot;
        }

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
