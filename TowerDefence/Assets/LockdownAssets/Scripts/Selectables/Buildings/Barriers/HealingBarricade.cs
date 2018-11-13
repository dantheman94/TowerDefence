using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingBarricade : Barrier {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" HEALING PROPERTIES")]
    [Space]
    public float HealingRate = 1f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private List<WorldObject> _HealingTargets;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Called when the object is created.
    /// </summary>
    protected override void Start() {
        base.Start();

        _HealingTargets = new List<WorldObject>();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    protected override void Update() {
        base.Update();

        // Heal all of the known targets
        for (int i = 0; i < _HealingTargets.Count; i++) {

            Unit unit = _HealingTargets[0] as Unit;
            if (unit.GetHitPoints() < unit.MaxHitPoints) {

                // Heal the unit
                unit.Damage(-HealingRate * Time.deltaTime, null);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Set's the current healing target for this unit to follow.
    /// </summary>
    /// <param name="target"></param>
    public void AddHealingTarget(WorldObject target) {

        // Only objects of the same team can be healed by this object
        if (target.Team == Team) { _HealingTargets.Add(target); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  List<WorldObject>
    /// </returns>
    public List<WorldObject> GetHealingTargets() { return _HealingTargets; }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}