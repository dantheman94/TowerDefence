using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 27/6/2018
//
//******************************

public class Squad : WorldObject {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" SQUAD PROPERTIES")]
    [Space]
    public int SquadMaxSize;
    public Unit SquadUnit;
    public float FlockingRadius;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private int _SquadCurrentSize;
    private float _SquadHealth;
    private List<Unit> _Squad;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    protected override void Start() {
        base.Start();

        _Squad = new List<Unit>();
    }

    protected override void Update() {
        base.Update();

        // Force the unit to skip the deployable state and go straight to being active in the world
        if (_ObjectState == WorldObjectStates.Deployable) {

            _ObjectState = WorldObjectStates.Active;
            foreach (var unit in _Squad) { unit.SetObjectState(WorldObjectStates.Active); }
        }

        if (_ObjectState == WorldObjectStates.Active) {

            // Get total sum of health for all units in the squad
            int maxSquadHealth = 0, currentSquadHealth = 0;
            foreach (var unit in _Squad) {

                maxSquadHealth += unit.MaxHitPoints;
                currentSquadHealth += unit.getHitPoints();
            }

            // Normalize the health between a range of 0.0 - 1.0
            if (maxSquadHealth > 0) { _SquadHealth = currentSquadHealth / maxSquadHealth; }
        }
    }

    /// <summary>
    //  Called when the player presses a button on the selection wheel with this world object linked to the button.
    /// </summary>
    /// <param name="buildingSlot">
    //  The building slot that instigated the selection wheel.
    //  (EG: If you're making a building, this is the building slot thats being used.)
    /// </param>
    public override void OnWheelSelect(BuildingSlot buildingSlot) {
        base.OnWheelSelect(buildingSlot);

        // Get reference to the newely cloned unit
        if (_ClonedWorldObject != null) {

            // Set position to be at the spawn vector while it is building (it should be hidden until its deployed)
            _ClonedWorldObject.gameObject.transform.position = buildingSlot.AttachedBase.UnitSpawnTransform.transform.position;
            _ClonedWorldObject.gameObject.transform.rotation = buildingSlot.AttachedBase.UnitSpawnTransform.transform.rotation;

            // Create units
            for (int i = 0; i < SquadMaxSize; i++) {

                // Create unit
                Unit unit = Instantiate(SquadUnit);
                _Squad.Add(unit);
                _SquadCurrentSize = i;

                // Offset each unit somewhere within the flocking radius
                SphereCollider sphere = _ClonedWorldObject.gameObject.AddComponent<SphereCollider>();
                sphere.transform.position = _ClonedWorldObject.gameObject.transform.position;
                sphere.radius = FlockingRadius;
                float randX = Random.Range(sphere.transform.position.x - (FlockingRadius / 2) + (FlockingRadius / 4), sphere.transform.position.x + (FlockingRadius / 2) - (FlockingRadius / 4));
                float randZ = Random.Range(sphere.transform.position.z - (FlockingRadius / 2) + (FlockingRadius / 4), sphere.transform.position.z + (FlockingRadius / 2) - (FlockingRadius / 4));
                Vector3 vecPos = new Vector3(randX, _ClonedWorldObject.gameObject.transform.position.y, randZ);
                unit.transform.position = vecPos;

                unit.gameObject.SetActive(true);
            }
        }
    }
    
}