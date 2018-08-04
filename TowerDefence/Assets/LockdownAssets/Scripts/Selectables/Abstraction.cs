using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 27/5/2018
//
//******************************

public class Abstraction : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" OBJECT PROPERTIES")]
    [Space]
    [Tooltip("The name that shows in the selection wheel/menu for this object.")]
    public string ObjectName;
    [Tooltip("The short description that shows in the selection wheel/menu for this object.")]
    public string ObjectDescriptionShort;
    [Tooltip("The detailed description that shows in the selection wheel/menu for this object.")]
    public string ObjectDescriptionLong;
    [Tooltip("The thumbnail image that represents this object in.")]
    public Texture2D Logo;

}