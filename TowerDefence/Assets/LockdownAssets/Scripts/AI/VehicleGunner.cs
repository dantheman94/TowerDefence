using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 12/9/2018
//
//******************************

public class VehicleGunner : Selectable {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" GUNNER PROPERTIES")]
    [Space]
    public GameObject Head = null;
    public float HeadAimingSpeed = 5f;
    public float BarrelRotationSpeed = 500f;
    [Space]
    public Weapon GunnerWeapon = null;
    public float MaxAttackRange = 200f;
    public List<GameObject> MuzzleLaunchPoints;
    [Space]
    public Vehicle _VehicleAttached = null;
    
    [Space]
    [Header("-----------------------------------")]
    [Header(" TARGETTING OBJECT WEIGHTS")]
    [Space]
    public Unit.TargetWeight[] TargetWeights = new Unit.TargetWeight[Unit._WeightLength];

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private enum EGunnerState { Idle, Attacking }
        
    private EGunnerState _CombatState = EGunnerState.Idle;
    private WorldObject _AttackTarget = null;
    private List<WorldObject> _PotentialTargets;

    private Vector3 _DirectionToTarget = Vector3.zero;
    private Quaternion _WeaponLookRotation = Quaternion.identity;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called before Start().
    /// </summary>
    protected override void Awake() {

        // Initialize lists
        _PotentialTargets = new List<WorldObject>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    // Called when the gameObject is created.
    /// </summary>
    protected override void Start() {

        // Get component references
        _VehicleAttached = GetComponentInParent<Vehicle>();

        // Create copy of its weaopn & re-assign it to replace the old reference
        if (GunnerWeapon != null) {

            GunnerWeapon = ObjectPooling.Spawn(GunnerWeapon.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Weapon>();
            GunnerWeapon.SetGunnerAttached(this);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {

        // Update facing target
        if (_AttackTarget != null) {

            // Target is alive so focus on it
            if (_AttackTarget.IsAlive()) {

                _CombatState = EGunnerState.Attacking;
                BaseLookAtLerp(_AttackTarget.transform.position);

                if (GunnerWeapon != null) {

                    // Fire weapon at the target
                    if (GunnerWeapon.CanFire()) { GunnerWeapon.FireWeapon(); }

                    // Check if the barrels should rotate
                    if (/*_IsFiring &&*/ Head != null) {

                        // Rotate the barrel by rotation speed
                        Head.transform.Rotate(Vector3.forward * (BarrelRotationSpeed * Time.deltaTime));
                    }
                }
            }

            // Target is dead so try to get a new attack target
            else {

                DetermineWeightedTargetFromList(TargetWeights);

                // There is currently no valid attack target >> return to idle
                if (_AttackTarget == null) { _CombatState = EGunnerState.Idle; }
            }
        }

        // Target is dead so try to get a new attack target
        else {

            DetermineWeightedTargetFromList(TargetWeights);

            // There is currently no valid attack target >> return to idle
            if (_AttackTarget == null) { _CombatState = EGunnerState.Idle; }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    private void BaseLookAtLerp(Vector3 position) {

        // Rotate the vehicle's weapon to face the target
        if (Head) {

            // Find the vector pointing from our position to the target
            _DirectionToTarget = (position - Head.transform.position).normalized;

            // Create the rotation we need to be in to look at the target
            _WeaponLookRotation = Quaternion.LookRotation(_DirectionToTarget);

            // Rotate us over time according to speed until we are in the required rotation
            Head.transform.rotation = Quaternion.LerpUnclamped(Head.transform.rotation, _WeaponLookRotation, Time.deltaTime * 2);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Adds a WorldObject to the weighted target list
    /// </summary>
    /// <param name="target"></param>
    public void AddPotentialTarget(WorldObject target) {

        // Can only fire at air units
        if (target is AirVehicle) {

            // Not a friendly unit...
            if (target.Team != Team) {

                // Look for match
                bool match = false;
                for (int i = 0; i < _PotentialTargets.Count; i++) {

                    // Match found
                    if (_PotentialTargets[i] == target) {

                        match = true;
                        break;
                    }
                }

                // Add to list if no matching target was found
                if (!match) { _PotentialTargets.Add(target); }
                if (_PotentialTargets.Count > 0 && _AttackTarget == null) { DetermineWeightedTargetFromList(TargetWeights); }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Checks if the WorldObject is contained in the weighted 
    //  target list & removes if it found.
    /// </summary>
    /// <param name="target"></param>
    public void RemovePotentialTarget(WorldObject target) {

        /*
            // Look for match
            for (int i = 0; i < _PotentialTargets.Count; i++) {

                // Match found
                if (_PotentialTargets[i] == target) {

                    _PotentialTargets.Remove(target);
                    break;
                }
            }
        */
        _PotentialTargets.Remove(target);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void DetermineWeightedTargetFromList(Unit.TargetWeight[] weightList) {

        // Multiple targets to select from
        if (_PotentialTargets.Count > 0) {

            // Use weighted selection to determine the next target
            List<int> targetWeights = new List<int>();
            if (weightList != null || weightList.Length > 0) {

                // For each known potential target
                for (int i = 0; i < _PotentialTargets.Count; i++) {

                    // Look for a match within the passed in weight list
                    for (int j = 0; j < weightList.Length - 1; j++) {

                        // Current potential target matches the current iterator in the weight list
                        Unit unit = _PotentialTargets[i].GetComponent<Unit>();
                        if (unit.UnitType == weightList[j].UnitType) {

                            // Add to local targetweights array
                            targetWeights.Add(weightList[j].Weight);
                        }
                    }
                }
            }

            else { /// weightList == null

                // All potential targets have a weight of 1 to be the next target
                for (int i = 0; i < _PotentialTargets.Count; i++) { targetWeights.Add(1); }
            }

            // Set new target
            _AttackTarget = _PotentialTargets[GetWeightedRandomIndex(targetWeights)];
        }

        // Only a single target in the array
        else if (_PotentialTargets.Count == 1) { _AttackTarget = _PotentialTargets[0]; }

        // No targets to choose from
        else { _AttackTarget = null; }

        // There is currently no valid attack target >> return to idle
        if (_AttackTarget == null) { _CombatState = EGunnerState.Idle; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Gets a random index based of a list of weighted ints.
    /// </summary>
    /// <param name="weights"></param>
    /// <returns>
    //  int
    /// </returns>
    private int GetWeightedRandomIndex(List<int> weights) {

        // Get the total sum of all the weights.
        int weightSum = 0;
        for (int i = 0; i < weights.Count; ++i) { weightSum += weights[i]; }

        // Step through all the possibilities, one by one, checking to see if each one is selected.
        int index = 0;
        int lastIndex = weights.Count - 1;
        while (index < lastIndex) {

            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index]) { return index; }

            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }

        // No other item was selected, so return very last index.
        return index;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Checks to see if the specified WorldObject is contained within the list.
    /// </summary>
    /// <param name="target"></param>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsTargetInPotentialList(WorldObject target) { return _PotentialTargets.Contains(target); }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Returns reference to the current attack target for this object.
    /// </summary>
    /// <returns>
    //  WorldObject
    /// </returns>
    public WorldObject GetAttackTarget() { return _AttackTarget; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
}
