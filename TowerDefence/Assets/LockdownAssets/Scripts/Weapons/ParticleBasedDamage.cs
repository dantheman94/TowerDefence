﻿using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 7/8/2018
//
//******************************

public class ParticleBasedDamage : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private ParticleSystem _ParticleSystem;
    private List<ParticleCollisionEvent> _CollisionEvents;

    private Weapon _WeaponAttached = null;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when this object is created.
    /// </summary>
    private void Start() {

        // Initialize
        _ParticleSystem = GetComponent<ParticleSystem>();
        _CollisionEvents = new List<ParticleCollisionEvent>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void OnParticleCollision(GameObject other) {

        // Does this particle belong to a weapon (used for damages)
        if (_WeaponAttached != null) {
            
            WorldObject worldObject = other.GetComponentInParent<WorldObject>();

            int i = 0;
            int numCollisionEvents = _ParticleSystem.GetCollisionEvents(other, _CollisionEvents);
            while (i < numCollisionEvents) {

                // Valid world object?
                if (worldObject != null) {

                    // Cant damage self
                    if (worldObject == _WeaponAttached.GetUnitAttached()) { return; }

                    // Damaging a unit?
                    Unit unitObj = worldObject.GetComponent<Unit>();
                    if (unitObj != null) {

                        // Damage based on unit type
                        switch (unitObj.UnitType) {

                            case Unit.EUnitType.Undefined:          { unitObj.Damage(_WeaponAttached.Damages.DamageDefault, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.CoreMarine:         { unitObj.Damage(_WeaponAttached.Damages.DamageCoreInfantry, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(_WeaponAttached.Damages.DamageAntiInfantryMarine, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.Hero:               { unitObj.Damage(_WeaponAttached.Damages.DamageHero, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(_WeaponAttached.Damages.DamageCoreVehicle, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(_WeaponAttached.Damages.DamageAntiAirVehicle, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(_WeaponAttached.Damages.DamageMobileArtillery, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.BattleTank:         { unitObj.Damage(_WeaponAttached.Damages.DamageBattleTank, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.CoreAirship:        { unitObj.Damage(_WeaponAttached.Damages.DamageCoreAirship, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.SupportShip:        { unitObj.Damage(_WeaponAttached.Damages.DamageSupportShip, _WeaponAttached.GetUnitAttached()); break; }
                            case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(_WeaponAttached.Damages.DamageHeavyAirship, _WeaponAttached.GetUnitAttached()); break; }
                            default: break;
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObject.Damage(_WeaponAttached.Damages.DamageDefault); }
                }
                i++;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="weapon"></param>
    public void SetWeaponAttached(Weapon weapon) { _WeaponAttached = weapon; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}