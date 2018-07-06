using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 22/6/2018
//
//******************************

public class Projectile : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header("MOVEMENT")]
    [Space]
    public float _MovementSpeed = 40f;
    public float _MaxDistance = 1000f;

    [Space]
    [Header("-----------------------------------")]
    [Header("DAMAGE")]
    [Space]
    public int DamageDefault = 1;
    public int DamageCoreInfantry = 1;
    public int DamageAntiInfantryMarine = 1;
    public int DamageAntiVehicleMarine = 1;
    public int DamageCoreVehicle = 1;
    public int DamageAntiAirVehicle = 1;
    public int DamageMobileArtillery = 1;
    public int DamageBattleTank = 1;
    public int DamageCoreAirship = 1;
    public int DamageSupportShip = 1;
    public int DamageBattleAirship = 1;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private float _DistanceTravelled  = 0f;
    private Vector3 _Velocity;
    private Weapon _WeaponAttached = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    private void Start () {

        // Set forward velocity
        _Velocity = transform.forward;
        _DistanceTravelled = 0f;
	}

    /// <summary>
    //  
    /// </summary>
    public void Init(Weapon wep) {

        // Reset stats
        _DistanceTravelled = 0f;
        _WeaponAttached = wep;
        _Velocity = transform.forward;

        // This should already be called when it is pulled from its object pool,
        // but this is just incase it somehow isn't
        gameObject.SetActive(true);
    }

    /// <summary>
    //  Called each frame. 
    /// </summary>
    private void Update () {
        
        // Constantly move forward
        transform.position += _Velocity * _MovementSpeed * Time.deltaTime;
        
        // Re-pool projectile when it has reached max distance threshold
        if (_DistanceTravelled < _MaxDistance) _DistanceTravelled += Time.deltaTime;
        else { RepoolProjectile(); }
    }

    /// <summary>
    //  Sends the projectile back to its object pool
    /// </summary>
    protected void RepoolProjectile() {

        gameObject.SetActive(false);

        ObjectPooling.Despawn(this.gameObject);
    }

    /// <summary>
    //  When this collider/rigid body enters another collider/rigidbody 
    //  (there must be at least 1 rigidbody in the collision for this to work)
    /// </summary>
    /// <param name="collision"></param>
    protected void OnCollisionEnter(Collision collision) {

        // Get object type
        GameObject gameObj = collision.gameObject;
        WorldObject worldObj = gameObj.GetComponent<WorldObject>();

        // Successful WorldObject cast
        if (worldObj != null) {

            // Check if object is of type unit
            Unit unitObj = worldObj.GetComponent<Unit>();
            if (unitObj != null) {

                // Friendly fire is OFF
                if (unitObj.Team != _WeaponAttached.GetUnitAttached().Team) {

                    // Damage based on unit type
                    switch (unitObj.Type) {

                        case Unit.UnitType.Undefined:           { unitObj.Damage(DamageDefault);            break; }
                        case Unit.UnitType.CoreMarine:          { unitObj.Damage(DamageCoreInfantry);       break; }
                        case Unit.UnitType.AntiInfantryMarine:  { unitObj.Damage(DamageAntiInfantryMarine); break; }
                        case Unit.UnitType.AntiVehicleMarine:   { unitObj.Damage(DamageAntiVehicleMarine);  break; }
                        case Unit.UnitType.CoreVehicle:         { unitObj.Damage(DamageCoreVehicle);        break; }
                        case Unit.UnitType.AntiAirVehicle:      { unitObj.Damage(DamageAntiAirVehicle);     break; }
                        case Unit.UnitType.MobileArtillery:     { unitObj.Damage(DamageMobileArtillery);    break; }
                        case Unit.UnitType.BattleTank:          { unitObj.Damage(DamageBattleTank);         break; }
                        case Unit.UnitType.CoreAirship:         { unitObj.Damage(DamageCoreAirship);        break; }
                        case Unit.UnitType.SupportShip:         { unitObj.Damage(DamageSupportShip);        break; }
                        case Unit.UnitType.BattleAirship:       { unitObj.Damage(DamageBattleAirship);      break; }
                        default: break;
                    }
                }

                // Destroy projectile
                RepoolProjectile();
            }

            // Damage the world object
            else {

                // Destroy projectile
                worldObj.Damage(DamageDefault);
                RepoolProjectile();
            }
        }
    }

}