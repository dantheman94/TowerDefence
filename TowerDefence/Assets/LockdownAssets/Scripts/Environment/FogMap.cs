using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// -=-=-=-=-=-=-=-=-
/// Created By: Angus Secomb
/// Last Edited: 19/09/18
/// Editor: Angus
/// =-=-=-=-=-=-=-=-=
public class FogMap {

    //                  VARIABLES
    ///////////////////////////////////////////////////////////

    public Vector2_1 Resolution;
    public float Size;
    public Vector2 Offset;
    public float PixelSize;
    public int PixelCount;
    public FogOfWarPlane Plane;
    public FilterMode FilterMode;
    public bool MultiThreaded;

    //             FUNCTIONS
    ///////////////////////////////////////////////////////////

    /// Constructor
    public FogMap(FogManager fogMan)
    {
        Set(fogMan);
    }

    ///////////////////////////////////////////////////////////

    /// <summary>
    /// Sets fog map paramets.
    /// </summary>
    /// <param name="fogMan"></param>
    public void Set(FogManager fogMan)
    {
        Resolution = fogMan.MapResolution;
        Size = fogMan.MapSize;
        Offset = fogMan.MapOffset;
        PixelSize = Resolution.x / Size;
        PixelCount = Resolution.x * Resolution.y;
        Plane = fogMan.Plane;
        FilterMode = fogMan.FilterMode;
        MultiThreaded = fogMan.MultiThreaded;
    }

    ///////////////////////////////////////////////////////////
}
