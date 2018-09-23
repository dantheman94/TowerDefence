using System.Collections.Generic;
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

                    DifficultyManager dm = DifficultyManager.Instance;
                    DifficultyManager.EDifficultyModifiers mod = DifficultyManager.EDifficultyModifiers.Damage;
                    WaveManager wm = WaveManager.Instance;

                    // Damaging a unit?
                    Unit unitObj = worldObject.GetComponent<Unit>();
                    if (unitObj != null) {

                        // Friendly fire is OFF
                        if (unitObj.Team != _WeaponAttached.GetUnitAttached().Team) {
                            Unit unitA = _WeaponAttached.GetUnitAttached();

                            // Damage based on unit type
                            switch (unitObj.UnitType) {

                                case Unit.EUnitType.Undefined:          { unitObj.Damage(((_WeaponAttached.Damages.DamageDefault * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.CoreMarine:         { unitObj.Damage(((_WeaponAttached.Damages.DamageCoreInfantry * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.AntiInfantryMarine: { unitObj.Damage(((_WeaponAttached.Damages.DamageAntiInfantryMarine * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.Hero:               { unitObj.Damage(((_WeaponAttached.Damages.DamageHero * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.CoreVehicle:        { unitObj.Damage(((_WeaponAttached.Damages.DamageCoreVehicle * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.AntiAirVehicle:     { unitObj.Damage(((_WeaponAttached.Damages.DamageAntiAirVehicle * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.MobileArtillery:    { unitObj.Damage(((_WeaponAttached.Damages.DamageMobileArtillery * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.BattleTank:         { unitObj.Damage(((_WeaponAttached.Damages.DamageBattleTank * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.CoreAirship:        { unitObj.Damage(((_WeaponAttached.Damages.DamageCoreAirship * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.SupportShip:        { unitObj.Damage(((_WeaponAttached.Damages.DamageSupportShip * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                case Unit.EUnitType.HeavyAirship:       { unitObj.Damage(((_WeaponAttached.Damages.DamageHeavyAirship * unitA.VetDamages[unitA.GetVeterancyLevel()]) * wm.GetWaveDamageModifier(unitObj)) * dm.GetDifficultyModifier(unitObj, mod), unitA); break; }
                                default: break;
                            }
                        }
                    }

                    // Damage the object (its not a unit so use the default damage value)
                    else { worldObject.Damage((_WeaponAttached.Damages.DamageDefault * wm.GetWaveDamageModifier(worldObject)) * dm.GetDifficultyModifier(Unit.EUnitType.Undefined, worldObject.Team == GameManager.Team.Defending, mod)); }
                }
                i++;
            }
            _ParticleSystem.Stop();
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