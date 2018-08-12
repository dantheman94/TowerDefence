using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******************************
//
//  Created by: Daniel Marton
//
//  Last edited by: Daniel Marton
//  Last edited on: 19/7/2018
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
    public float VerticalSpawnOffset = 4.1f;

    //******************************************************************************************************************************
    //
    //      VARIABLES
    //
    //******************************************************************************************************************************

    private Renderer _MinimapRenderer;
    private SphereCollider _SpawnSphere = null;

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
        Vector3 pos = new Vector3(randX, gameObject.transform.position.y + VerticalSpawnOffset, randZ);

        // Check if the position overlaps any other positions and rectify if true
        List<bool> nonOverlaps = new List<bool>();
        float radius = 5f;
        bool overlapping = true;
        while (overlapping) {

            // Restarting the overlap checks
            if (nonOverlaps.Count > 0) { nonOverlaps.Clear(); }

            // Create a bounds at the position * radius
            Bounds offsetBounds = new Bounds(pos, new Vector3(radius, radius, radius));

            // Test for intersections against other positions
            if (locations.Count > 0) {

                foreach (var testOffset in locations) {

                    // Do the bounds intersect?
                    Bounds testBounds = new Bounds(testOffset, new Vector3(radius, radius, radius));
                    if (offsetBounds.Intersects(testBounds)) {

                        // Move the offset in a random direction until its no longer overlapping
                        float distance = Vector3.Distance(pos, testOffset);
                        int additive = Random.Range(0, 3);
                        if      (additive == 0) { pos.Set(pos.x + (distance - radius), pos.y, pos.z + (distance - radius)); }
                        else if (additive == 1) { pos.Set(pos.x + (distance - radius), pos.y, pos.z - (distance - radius)); }
                        else if (additive == 2) { pos.Set(pos.x - (distance - radius), pos.y, pos.z + (distance - radius)); } 
                        else if (additive == 3) { pos.Set(pos.x - (distance - radius), pos.y, pos.z - (distance - radius)); }
                    }
                    else { nonOverlaps.Add(new bool()); }
                }

                // If this returns TRUE, then all offsets are correctly in place
                if (nonOverlaps.Count == locations.Count) { overlapping = false; }
            }

            // Bounds aren't overlapping if theres no other bounds to test against (IE: this is the first unit being created)
            else { overlapping = false; }
        }

        // Offset isn't intersecting against other units so we add it to the list to check against for other units
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

        // Loop for each location and add it to the list
        for (int i = 0; i < amount; i++) { locations.Add(GetSpawnLocationWithinPad(locations, _SpawnSphere)); }

        return locations;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

}
