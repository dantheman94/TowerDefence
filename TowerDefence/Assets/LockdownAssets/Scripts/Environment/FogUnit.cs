using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// -=-=-=-=-=-=-=-=-
/// Created By: Angus Secomb
/// Last Edited: 19/09/18
/// Editor: Angus
/// =-=-=-=-=-=-=-=-=

public enum FogShapeType
{
    Circle,
    Texture
}

public class FogUnit : MonoBehaviour {

    //                  INSPECTOR
    ///////////////////////////////////////////////////////////////////////////
    [Header("----SHAPE PROPERTIES----")]
    [Space]
    public FogShapeType ShapeType = FogShapeType.Circle;
    public Vector2 Offset = Vector2.zero;

    public float Radius = 5.0f;
    [Range(0.0F, 1.0F)]
    public float InnerRadius = 1;
    [Range(0.0f, 180.0f)]
    public float Angle = 180;

    [Header("----TEXTURE PROPERTIES----")]
    public Texture2D Texture;
    public bool RotateToForward = false;

    [Header("----LOS MASK SETTINGS----")]

    public LayerMask LineOfSightMask = 0;
    [Tooltip("How much the pixels go through objects.")]
    public float LineOfSightPenetration = 0;
    [Tooltip("Use if fog flickers.")]
    public bool AntiFlicker = false;

    //                      VARIABLES
    ///////////////////////////////////////////////////////////////////////////

    private float[] _Distances = null;
    private bool[] _VisibleCells = null;

    private Transform _Transform;

    static List<FogUnit> _RegisteredUnits = new List<FogUnit>();
    public static List<FogUnit> RegisteredUnits { get { return _RegisteredUnits; } }

    //                    FUNCTIONS
    ///////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        _Transform = transform;
    }

    ///////////////////////////////////////////////////////////////////////////

    private void OnEnable()
    {
        RegisteredUnits.Add(this);
    }

    ///////////////////////////////////////////////////////////////////////////

    private void OnDisable()
    {
        RegisteredUnits.Remove(this);
    }

    ///////////////////////////////////////////////////////////////////////////

    static bool CalculateLineOfSight(Vector3 eye, float radius, float penetration, LayerMask layermask, float[] distances, Vector3 up, Vector3 forward)
    {
        bool hashit = false;
        float angle = 360.0f / distances.Length;
        RaycastHit hit;

        for (int i = 0; i < distances.Length; ++i)
        {
            Vector3 dir = Quaternion.AngleAxis(angle * i, up) * forward;
            if (Physics.Raycast(eye, dir, out hit, radius, layermask))
            {
                distances[i] = (hit.distance + penetration) / radius;
                if (distances[i] < 1)
                    hashit = true;
                else
                    distances[i] = 1;
            }
            else
                distances[i] = 1;
        }

        return hashit;
    }

    ///////////////////////////////////////////////////////////////////////////

    public float[] CalculateLineOfSight(Vector3 eyepos, FogOfWarPlane plane)
    {
        if (LineOfSightMask == 0)
            return null;

        if (_Distances == null)
            _Distances = new float[256];

        if (plane == FogOfWarPlane.XZ) // 3D
        {
            if (CalculateLineOfSight(eyepos, Radius, LineOfSightPenetration, LineOfSightMask, _Distances, Vector3.up, Vector3.forward))
                return _Distances;
        }
        else if (plane == FogOfWarPlane.XY) // 3D
        {
            if (CalculateLineOfSight(eyepos, Radius, LineOfSightPenetration, LineOfSightMask, _Distances, Vector3.back, Vector3.up))
                return _Distances;
        }
        return null;
    }

    ///////////////////////////////////////////////////////////////////////////

    static float Sign(float v)
    {
        if (Mathf.Approximately(v, 0))
            return 0;
        return v > 0 ? 1 : -1;
    }

    ///////////////////////////////////////////////////////////////////////////

    void FillShape(FogManager fow, FogShape shape)
    {
        if (AntiFlicker)
        {
            // snap to nearest fog pixel
            shape.EyePosition = FogConversion.SnapWorldPositionToNearestFogPixel(fow, FogConversion.WorldToFogPlane(_Transform.position, fow.Plane), fow.MapOffset, fow.MapResolution, fow.MapSize);
            shape.EyePosition = FogConversion.FogPlaneToWorld(shape.EyePosition.x, shape.EyePosition.y, _Transform.position.y, fow.Plane);
        }
        else
            shape.EyePosition = _Transform.position;
        shape.Forward = FogConversion.TransformFogPlaneForward(_Transform, fow.Plane);
        shape.Offset = Offset;
        shape.Radius = Radius;
    }

    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Creates new vision shape.
    /// </summary>
    /// <param name="fow"></param>
    /// <returns></returns>
    FogShape CreateShape(FogManager fow)
    {
        if (ShapeType == FogShapeType.Circle)
        {
            FogCircle shape = new FogCircle();
            FillShape(fow, shape);
            shape.InnerRadius = InnerRadius;
            shape.Angle = Angle;
            return shape;
        }
        else if (ShapeType == FogShapeType.Texture)
        {
            if (Texture == null)
                return null;

            FogShapeTexture shape = new FogShapeTexture();
            FillShape(fow, shape);
            shape.Texture = Texture;
            shape.RotateToForward = RotateToForward;
            return shape;
        }
        return null;
    }

    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Returns fog of war line of sight shape.
    /// </summary>
    /// <param name="fow"></param>
    /// <param name="plane"></param>
    /// <returns></returns>
    public FogShape GetShape(FogManager fow,  FogOfWarPlane plane)
    {
        FogShape shape = CreateShape(fow);
        if (shape == null)
            return null;

            shape.LineOfSight = CalculateLineOfSight(shape.EyePosition, plane);
            shape.VisibleCells = null;
        return shape;
    }

    ///////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Draws vision sight in editor.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (ShapeType == FogShapeType.Circle)
            Gizmos.DrawWireSphere(transform.position, Radius);
        else if (ShapeType == FogShapeType.Texture)
            Gizmos.DrawWireCube(transform.position, new Vector3(Radius, Radius, Radius));
    }

    ///////////////////////////////////////////////////////////////////////////
}
