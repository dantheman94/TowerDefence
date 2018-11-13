using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 10/9/2018
//
//******************************

public class LockdownPad : MonoBehaviour {

    //******************************************************************************************************************************
    //
    //      INSPECTOR
    //
    //******************************************************************************************************************************

    [Space]
    [Header("-----------------------------------")]
    [Header(" MINIMAP PROPERTIES")]
    [Space]
    public GameObject MinimapQuad = null;

    [Space]
    [Header("-----------------------------------")]
    [Header(" LOCKDOWNPAD PROPERTIES")]
    [Space]
    public BuildingSlot BuildingSlotAttached = null;
    public float SpawnRadius = 85f;
    [Space]
    public UI_LockdownPadHUD LockdownPadHud = null;
    [Space]
    public List<AttackPath> AttackPaths;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Renderer _MinimapRenderer;
    private SphereCollider _SpawnSphere = null;

    private float _VerticalSpawnOffset = 4.1f;

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

        // Get component references
        if (MinimapQuad != null) { _MinimapRenderer = MinimapQuad.GetComponent<Renderer>(); }

        // Set minimap icon colour to red
        if (_MinimapRenderer != null) { _MinimapRenderer.material.color = WaveManager.Instance.AttackingTeamColour; }

        // Initialize
        _VerticalSpawnOffset = transform.position.y;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  Called each frame.
    /// </summary>
    private void Update() {

        // No base on the slot
        if (BuildingSlotAttached != null) {

            if (BuildingSlotAttached.AttachedBase == null) {

                // Remove from waves manager
                WaveManager.Instance.LockdownPads.Remove(this);
            }

            // No attacking team base on the slot
            else if (BuildingSlotAttached.AttachedBase.Team != GameManager.Team.Attacking) {

                // Remove from waves manager
                WaveManager.Instance.LockdownPads.Remove(this);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="locations"></param>
    /// <param name="sphere"></param>
    /// <returns>
    //  Vector3
    /// </returns>
    private Vector3 GetSpawnLocationWithinPad(List<Vector3> locations, SphereCollider sphere) {
        
        // Random position somewhere within the sphere
        float randX = Random.Range(sphere.center.x - sphere.radius, sphere.center.x + sphere.radius);
        float randZ = Random.Range(sphere.center.z - sphere.radius, sphere.center.z + sphere.radius);
        Vector3 pos = new Vector3(randX, /*gameObject.transform.position.y +*/ _VerticalSpawnOffset, randZ);        
        return pos;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <param name="amount"></param>
    /// <returns>
    //  List<Vector3>
    /// </returns>
    public List<Vector3> GetSpawnLocations(int amount) {

        List<Vector3> locations = new List<Vector3>();
        if (_SpawnSphere == null) { _SpawnSphere = gameObject.AddComponent<SphereCollider>(); }
        _SpawnSphere.center = transform.position;
        _SpawnSphere.radius = SpawnRadius;
        _SpawnSphere.isTrigger = true;

        // Loop for each location and add it to the list
        for (int i = 0; i < amount; i++) { locations.Add(GetSpawnLocationWithinPad(locations, _SpawnSphere)); }
        return locations;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    //  
    /// </summary>
    /// <returns>
    //  AttackPath
    /// </returns>
    public AttackPath GetRandomAttackPath() {

        int i = Random.Range(0, AttackPaths.Count);
        return AttackPaths[i];
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
