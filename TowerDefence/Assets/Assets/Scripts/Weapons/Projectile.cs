using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//******************************

public class Projectile : MonoBehaviour {

    //***************************************************************
    // VARIABLES

    [Space]
    [Header("Movement")]
    public float                    _MovementSpeed          = 40f;
    public bool                     _HomingProjectile       = false;
    public float                    _MaxDistance            = 1000f;

    [Space]
    [Header("Damage")]
    public float                    _DamageBattleTank       = 0f;
    public float                    _DamageMortarTank       = 0f;
    public float                    _DamageJeep             = 0f;
    public float                    _DamageTropper          = 0f;

    protected float                 _DistanceTravelled      = 0f;
    protected Vector3               _Velocity;
    protected GroundVehicle         _Owner;
    protected GroundVehicle         _Target;

    //***************************************************************
    // FUNCTIONS    

    private void Start () {

        _Velocity = transform.forward;
	}
	
	private void Update () {

        if (_HomingProjectile) {

            // Get target

            // Rotate towards facing target
        }

        // Constantly move forward
        transform.position += _Velocity * _MovementSpeed * Time.deltaTime;
        
        // Destroy gameObject when it has reached max distance threshold
        if (_DistanceTravelled < _MaxDistance) _DistanceTravelled += Time.deltaTime;
        else { Destroy(gameObject); }
    }

    //***************************************************************
    // SET & GET    

    public void SetOwner(GroundVehicle newOwner) { _Owner = newOwner; }

    public GroundVehicle GetOwner() { return _Owner; }

}