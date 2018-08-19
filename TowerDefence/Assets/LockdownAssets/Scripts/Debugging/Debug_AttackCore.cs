using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_AttackCore : MonoBehaviour {

    private Unit _UnitAttached = null;
    WorldObject attack = null;

    // Use this for initialization
    void Start () {
        
        _UnitAttached = GetComponent<Unit>();
    }

    private void Update() {

        if (_UnitAttached != null)

            if (attack == null || _UnitAttached.GetAttackTarget() == null) {

                attack = WaveManager.Instance.CentralCore.GetAttackObject();
                _UnitAttached.AgentAttackObject(attack);
            }

            if (attack.GetHitPoints() <= 0) {

                attack = WaveManager.Instance.CentralCore.GetAttackObject();
                _UnitAttached.AgentAttackObject(attack);
            }
    }

}
