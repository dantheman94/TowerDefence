using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 1/10/2018
//
//******************************

public class ParabolicArc : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private float _FiringAngle = 10f;
    private float _Gravity = -9.8f;
    
    private Transform _ProjectileTransform;

    //******************************************************************************************************************************
    //
    //      FUNCTIONS
    //
    //******************************************************************************************************************************

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    //  Initializes the projectile arc.
    /// </summary>
    /// <param name="target"></param>
    public void Init(Transform target) {

        _ProjectileTransform = gameObject.transform;
        StartCoroutine(SimulateProjectile(target));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  Ienumerator
    /// </returns>
    private IEnumerator SimulateProjectile(Transform target) {

        // Calculate distance to target
        float targetDistance = Vector3.Distance(_ProjectileTransform.position, target.position);

        // Calculate the velocity needed to throw the projectile at the specified angle
        float projVelocity = targetDistance / (Mathf.Sin(2 * _FiringAngle * Mathf.Deg2Rad) / _Gravity);

        // Extract the X & Y component of the velocity
        float Vx = Mathf.Sqrt(projVelocity) * Mathf.Cos(_FiringAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projVelocity) * Mathf.Sin(_FiringAngle * Mathf.Deg2Rad);

        // Calculate flight time
        float flightDuration = targetDistance / Vx;

        // Rotate projectile to face the target
        _ProjectileTransform.rotation = Quaternion.LookRotation(target.position - _ProjectileTransform.position);

        float elapseTime = 0;
        while (elapseTime < flightDuration) {

            _ProjectileTransform.Translate(0, (Vy - (_Gravity * elapseTime)) * Time.deltaTime, Vx * Time.deltaTime);
            elapseTime += Time.deltaTime;

            yield return null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
