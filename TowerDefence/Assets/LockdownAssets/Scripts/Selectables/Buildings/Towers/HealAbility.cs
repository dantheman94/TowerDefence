using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAbility : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" HEALING PROPERTIES")]
    [Space]
    public float healRadius = 20.0f;
    public float healRate = 15.0f;
    public float energyDrain = 5.0f;
    public float healRadiusHeight = 85.0f;
    public float yOffset = -10.0f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private CapsuleCollider capsuleCollider;
    private AirVehicle _AirVehicle;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called before Start().
    /// </summary>
    void Start () {

        capsuleCollider = this.gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.radius = healRadius;
        capsuleCollider.isTrigger = true;
        capsuleCollider.height = healRadiusHeight;
        capsuleCollider.center = new Vector3(0, yOffset, 0);
        _AirVehicle = GetComponent<AirVehicle>();        
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    void Update () {
		
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Unit")
        {
            Unit _unit;
            _unit = other.gameObject.GetComponent<Unit>();
            if(_unit.GetHitPoints() < _unit.MaxHitPoints)
            {
                _unit.AddHitPoints(healRate * Time.deltaTime);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Unit")
        {
            Unit _unit;
            _unit = other.gameObject.GetComponent<Unit>();
            if (_unit.GetHitPoints() < _unit.MaxHitPoints)
            {
                _unit.AddHitPoints(healRate * Time.deltaTime);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}