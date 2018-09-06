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

public class Ai : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" AI BEHAVIOURS")]
    [Space]
    public float _ChasingDistance = 30f;

    [Space]
    [Header("-----------------------------------")]
    [Header(" TARGETTING OBJECT WEIGHTS")]
    [Space]
    public TargetWeight[] TargetWeights = new TargetWeight[_WeightLength];

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    [System.Serializable]
    public struct TargetWeight {

        public Unit.EUnitType UnitType;
        public int Weight;
    }

    public const int _WeightLength = (int)Unit.EUnitType.ENUM_COUNT;

    protected WorldObject _AttackTarget = null;
    protected List<WorldObject> _PotentialTargets;

    protected bool _IsFollowingPlayerCommand = false;
    protected bool _IsAttacking;
    protected bool _IsChasing;
    protected bool _IsSeeking;
    protected bool _IsReturningToOrigin;

    protected Vector3 _ChaseOriginPosition = Vector3.zero;
    protected Vector3 _SeekTarget = Vector3.zero;

    protected Building _AttachedBuilding;

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
        base.Awake();

        // Initialize lists
        _PotentialTargets = new List<WorldObject>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Damages the object by a set amount.
    /// </summary>
    /// <param name="damage"></param>
    public override void Damage(float damage, WorldObject instigator) {
        base.Damage(damage);

        // Add intigator to the potential list
        if (instigator != null) { AddPotentialTarget(instigator); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected override void Update() {
        base.Update();

        // Update chasing enemy
        if (_IsChasing) { UpdateChasingEnemy(); }
        else { _ChaseOriginPosition = transform.position; }

        if (_IsFollowingPlayerCommand) { UpdatePlayerOverrideCheck(); }
        if (_IsReturningToOrigin) { ResetToOriginPosition(); }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called when the object is killed/destroyed
    /// </summary>
    public override void OnDeath() {
        base.OnDeath();

        // If were in the wave manager's enemies array - remove it
        if (WaveManager.Instance.GetCurrentWaveEnemies().Contains(this)) { WaveManager.Instance.GetCurrentWaveEnemies().Remove(this); }
        if (Team == GameManager.Team.Attacking) { GameManager.Instance.WaveStatsHUD.DeductLifeFromCurrentPopulation(); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame. 
    /// </summary>
    protected virtual void UpdateChasingEnemy() {

        // Check if we should continue chasing or not
        float dist = Vector3.Distance(_ChaseOriginPosition, transform.position);
        if (_IsReturningToOrigin = dist >= _ChasingDistance) {

            // Stop chasing & get a new target
            RemovePotentialTarget(_AttackTarget);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected void UpdatePlayerOverrideCheck() {
        
        ///if (!_IsSeeking) { _IsFollowingPlayerCommand = false; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Adds a WorldObject to the weighted target list
    /// </summary>
    /// <param name="target"></param>
    public virtual void AddPotentialTarget(WorldObject target) {

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
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Checks if the WorldObject is contained in the weighted 
    //  target list & removes if it found.
    /// </summary>
    /// <param name="target"></param>
    public void RemovePotentialTarget(WorldObject target) {

        // Look for match
        for (int i = 0; i < _PotentialTargets.Count; i++) {

            // Match found
            if (_PotentialTargets[i] == target) {

                _PotentialTargets.Remove(target);
                break;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public virtual void DetermineWeightedTargetFromList(TargetWeight[] weightList) {

        // Multiple targets to select from
        if (_PotentialTargets.Count > 0) {

            // WHICH IS THE TANKIEST TARGET?

            // WHICH TARGET HAS DAMAGED ME THE MOST?

            // WHICH TARGET IS THE CLOSEST?

            // WHICH TARGET AM I THE MOST EFFECTIVE AGAINST?
            
            List<int> targetWeights = new List<int>();
            /*
            if (weightList != null || weightList.Length > 0) {
                
                // For each knwon potential target
                for (int i = 0; i < _PotentialTargets.Count; i++) {

                    // Look for a match within the passed in weight list
                    for (int j = 0; j < weightList.Length; j++) {

                        // Current potential target matches the current iterator in the weight list
                        Unit unit = _PotentialTargets[i].GetComponent<Unit>();
                        if (unit.UnitType == weightList[j].UnitType) {

                            // Add to local targetweights array
                            targetWeights.Add(weightList[j].Weight);
                        }
                    }
                }

            }
            */
            ///else { /// weightList == null

                // All potential targets have a weight of 1 to be the next target
                for (int i = 0; i < _PotentialTargets.Count; i++) { targetWeights.Add(1); }
            ///}

            // Set new target
            _AttackTarget = _PotentialTargets[GetWeightedRandomIndex(targetWeights)];
        }

        // Only a single target in the array
        else if (_PotentialTargets.Count == 1) { _AttackTarget = _PotentialTargets[0]; }

        // No targets to choose from
        else { _AttackTarget = null; }
        if (_AttackTarget == null) { StartCoroutine(ResetWeaponPosition(3)); }
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
    //  
    /// </summary>
    /// <param name="target"></param>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsTargetInPotentialList(WorldObject target) {

        // Look for match
        bool match = false;
        for (int i = 0; i < _PotentialTargets.Count; i++) {

            // Match found
            if (_PotentialTargets[i] == target) {

                match = true;
                break;
            }
        }
        return match;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    /// <param name="target"></param>
    public void SetAttackTarget(WorldObject target) { _AttackTarget = target; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
    //  Get refernence of the current attack target.
    /// </summary>
    /// <returns>
    // WorldObject
    /// </returns>
    public WorldObject GetAttackTarget() { return _AttackTarget; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="delayTime"></param>
    protected virtual IEnumerator ResetWeaponPosition(int delayTime) {

        yield return new WaitForSeconds(delayTime);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="worldObject"></param>
    public bool TryToChaseTarget(WorldObject objTarget) {

        if (objTarget != null) {

            if (!_IsFollowingPlayerCommand && !_IsReturningToOrigin) {

                _AttackTarget = objTarget;
                _IsChasing = true;
                return true;
            }
        }
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected virtual void ResetToOriginPosition() {
        
        _IsChasing = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    protected void CommandOverride() {

        _IsFollowingPlayerCommand = true;
        _IsReturningToOrigin = false;
        _IsChasing = false;
        _IsAttacking = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    public void ResetToOrigin() { _IsReturningToOrigin = true; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsChasing() { return _IsChasing; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsReturningToOrigin() { return _IsReturningToOrigin; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  bool
    /// </returns>
    public bool IsAttacking() { return _IsAttacking; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
