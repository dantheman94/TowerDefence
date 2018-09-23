using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// -=-=-=-=-=-=-=-=-
/// Created By: Angus Secomb
/// Last Edited: 19/09/18
/// Editor: Angus
/// =-=-=-=-=-=-=-=-=
/// Fog Utility File.
/// Contains classes and structs used in fog.


/// <summary>
/// Fog shape base class
/// </summary>
public abstract class FogShape  {

    public Vector3 EyePosition;
    public Vector2 Forward;
    public Vector2 Offset;
    public float[] LineOfSight;
    public bool[] VisibleCells;
    public float Radius;
}

/// <summary>
/// Circle based fog shape.
/// </summary>
public class FogCircle : FogShape
{
    public float InnerRadius;
    public float Angle;

    public byte GetFallOff(float normdist)
    {
        if(normdist < InnerRadius)
        {
            return 0;
        }
        return (byte)(Mathf.InverseLerp(InnerRadius, 1, normdist) * 255);
    }
}

/// <summary>
///  Texture pixel pattern based fog shape,
/// </summary>
public class FogShapeTexture : FogShape
{
    public Texture2D Texture;
    public bool RotateToForward = false;
}

public static class FogTools
{
    /// <summary>
    /// Returns an angle between from and to
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float ClockwiseAngle(Vector2 from, Vector2 to)
    {
        float angle = Vector2.Angle(from, to);
        if (Vector2.Dot(from, new Vector2(to.y, to.x)) < 0.0f)
            angle = -angle;
        return angle;
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Used to swap texture references.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Allows keywords for shaders to be set on materials.
    /// </summary>
    /// <param name="material"></param>
    /// <param name="keyword"></param>
    /// <param name="enable"></param>
    public static void SetKeywordEnabled(this Material material, string keyword, bool enable)
    {
        if (enable)
            material.EnableKeyword(keyword);
        else
            material.DisableKeyword(keyword);
    }
    ////////////////////////////////////////////////////////////////////////////////
}

/// <summary>
/// Vector2 class in ints.
/// For some reason vector2Int can't
/// operate with vector2s and i was getting weird
/// results with floats.
/// </summary>
[System.Serializable]
public struct Vector2_1
{
    public int x;
    public int y;

    public int this[int idx]
    {
        get { return idx == 0 ? x : y; }
        set
        {
            switch (idx)
            {
                case 0:
                    x = value;
                    break;
                default:
                    y = value;
                    break;
                        }
        }
    }

    public Vector2 vector2
    {
        get
        {
            return new Vector2(x, y);
        }
    }


    public Vector2_1 perp
    {
        get
        {
            return new Vector2_1(y, x);
        }
    }

    public Vector2 normalized
    {
        get
        {
            float invmag = 1.0f / magnitude;
            return new Vector2(invmag * x, invmag * y);
        }
    }

    public float magnitude
    {
        get { return Mathf.Sqrt(x * x + y * y); }
    }

    public int sqrMagnitude
    {
        get { return x * x + y * y; }
    }

    public int manhattanMagnitude
    {
        get { return Mathf.Abs(x) + Mathf.Abs(y); }
    }

    public Vector2_1(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2_1(Vector2 v)
    {
        x = Mathf.RoundToInt(v.x);
        y = Mathf.RoundToInt(v.y);
    }

    public static Vector2_1 operator +(Vector2_1 c1, Vector2_1 c2)
    {
        return new Vector2_1(c1.x + c2.x, c1.y + c2.y);
    }

    public static Vector2_1 operator +(Vector2_1 c1, int c2)
    {
        return new Vector2_1(c1.x + c2, c1.y + c2);
    }

    public static Vector2 operator +(Vector2_1 c1, Vector2 c2)
    {
        return new Vector3(c1.x + c2.x, c1.y + c2.y);
    }

    public static Vector2 operator +(Vector2 c1, Vector2_1 c2)
    {
        return new Vector3(c1.x + c2.x, c1.y + c2.y);
    }

    public static Vector2_1 operator -(Vector2_1 c1, Vector2_1 c2)
    {
        return new Vector2_1(c1.x - c2.x, c1.y - c2.y);
    }

    public static Vector2_1 operator *(Vector2_1 c1, int c2)
    {
        return new Vector2_1(c1.x * c2, c1.y * c2);
    }

    public static Vector2 operator *(Vector2_1 c1, float c2)
    {
        return new Vector2(c1.x * c2, c1.y * c2);
    }

    public static Vector2_1 operator *(int c1, Vector2_1 c2)
    {
        return new Vector2_1(c1 * c2.x, c1 * c2.y);
    }

    public static Vector2 operator *(float c1, Vector2_1 c2)
    {
        return new Vector2(c1 * c2.x, c1 * c2.y);
    }

    public override bool Equals(System.Object obj)
    {
        if (obj == null)
            return false;

        Vector2_1 p = (Vector2_1)obj;
        if ((System.Object)p == null)
            return false;

        return (x == p.x) && (y == p.y);
    }

    public bool Equals(Vector2_1 p)
    {
        if ((object)p == null)
            return false;

        return (x == p.x) && (y == p.y);
    }

    public static bool operator ==(Vector2_1 a, Vector2_1 b)
    {
        if (System.Object.ReferenceEquals(a, b))
            return true;

        if (((object)a == null) || ((object)b == null))
            return false;

        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2_1 a, Vector2_1 b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return x ^ y;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }

    public static Vector2_1 zero { get { return new Vector2_1(0, 0); } }
    public static Vector2_1 one { get { return new Vector2_1(1, 1); } }
}

public static class FogConversion
{
    /// <summary>
    ///  converts world position to fog plane pos where x and y are on the plane
    /// </summary>
    /// <param name="position"></param>
    /// <param name="plane"></param>
    /// <returns></returns>
    public static Vector2 WorldToFogPlane(Vector3 position, FogOfWarPlane plane)
    {
        if (plane == FogOfWarPlane.XY)
            return new Vector2(position.x, position.y);
        else if (plane == FogOfWarPlane.YZ)
            return new Vector2(position.y, position.z);
        else if (plane == FogOfWarPlane.XZ)
            return new Vector2(position.x, position.z);

        Debug.LogError("FogOfWarPlane is an invalid value!");
        return Vector2.zero;
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  converts world position to fog plane pos where x and y are on the plane and z is perpendicular
    /// </summary>
    /// <param name="position"></param>
    /// <param name="plane"></param>
    /// <returns></returns>
    public static Vector3 WorldToFogPlane3(Vector3 position, FogOfWarPlane plane)
    {
        if (plane == FogOfWarPlane.XY)
            return new Vector3(position.x, position.y, position.z);
        else if (plane == FogOfWarPlane.YZ)
            return new Vector3(position.y, position.z, position.x);
        else if (plane == FogOfWarPlane.XZ)
            return new Vector3(position.x, position.z, position.y);
        else
        {
            Debug.LogError("FogOfWarPlane is an invalid value!");
            return Vector2.zero;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///  converts world position to fog plane pos where x and y are on the plane and z is perpendicular
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="plane"></param>
    /// <returns></returns>
    public static Vector2 TransformFogPlaneForward(Transform transform, FogOfWarPlane plane)
    {
        if (plane == FogOfWarPlane.XY)
            return new Vector2(transform.up.x, transform.up.y).normalized;
        else if (plane == FogOfWarPlane.YZ)
            return new Vector2(transform.up.z, transform.up.y).normalized;
        else if (plane == FogOfWarPlane.XZ)
            return new Vector2(transform.forward.x, transform.forward.z).normalized;

        Debug.LogError("FogOfWarPlane is an invalid value!");
        return Vector2.zero;
    }

    ////////////////////////////////////////////////////////////////////////////////

    public static Vector2 FogToWorldSize(Vector2 fpos, Vector2_1 resolution, float size)
    {
        Vector2 res = resolution.vector2;
        fpos.x *= size / res.x;
        fpos.y *= size / res.y;
        return fpos;
    }

    ////////////////////////////////////////////////////////////////////////////////

    public static Vector2 FogToWorld(Vector2 fpos, Vector2 offset, Vector2_1 resolution, float size)
    {
        Vector2 res = resolution.vector2;
        fpos -= res * 0.5f;
        fpos.x *= size / res.x;
        fpos.y *= size / res.y;
        return fpos + offset;
    }

    ////////////////////////////////////////////////////////////////////////////////

    public static Vector2 WorldToFog(Vector2 wpos, Vector2 offset, Vector2_1 resolution, float size)
    {
        wpos -= offset;
        Vector2 res = resolution.vector2;
        wpos.x *= res.x / size;
        wpos.y *= res.y / size;
        return wpos + res * 0.4999f; // this should be 0.5f, but it can cause some visible floating point issues when using lower resolution maps
    }

    ////////////////////////////////////////////////////////////////////////////////

    public static Vector2 WorldToFog(Vector3 wpos, FogOfWarPlane plane, Vector2 offset, Vector2_1 resolution, float size)
    {
        return WorldToFog(WorldToFogPlane(wpos, plane), offset, resolution, size);
    }

    ////////////////////////////////////////////////////////////////////////////////

    public static Vector3 FogPlaneToWorld(float x, float y, float z, FogOfWarPlane plane)
    {
        if (plane == FogOfWarPlane.XY)
            return new Vector3(x, y, z);
        else if (plane == FogOfWarPlane.YZ)
            return new Vector3(z, x, y);
        else if (plane == FogOfWarPlane.XZ)
            return new Vector3(x, z, y);

        Debug.LogError("FogOfWarPlane is an invalid value!");
        return Vector3.zero;
    }

    ////////////////////////////////////////////////////////////////////////////////

    public static Vector2 SnapToNearestFogPixel(Vector2 fogpos)
    {
        fogpos.x = Mathf.Floor(fogpos.x) + 0.4999f;
        fogpos.y = Mathf.Floor(fogpos.y) + 0.4999f;
        return fogpos;
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// // Snaps a world point to a fog pixel. It returns the position 
    /// </summary>
    /// <param name="fow"></param>
    /// <param name="worldpos"></param>
    /// <param name="offset"></param>
    /// <param name="resolution"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Vector2 SnapWorldPositionToNearestFogPixel(FogManager fow, Vector2 worldpos, Vector2 offset, Vector2_1 resolution, float size)
    {
        Vector2 fogpos = WorldToFog(worldpos, fow.MapOffset, fow.MapResolution, fow.MapSize);
        fogpos = SnapToNearestFogPixel(fogpos);
        return FogToWorld(fogpos, fow.MapOffset, fow.MapResolution, fow.MapSize);
    }

    ////////////////////////////////////////////////////////////////////////////////
}

public enum FogBlurType
{
    Gaussian3,
    Gaussian5,
    Antialias
}

public class FogBlur
{
    //                     VARIABLES
    ////////////////////////////////////////////////////////////////////////////////

    RenderTexture _target;
    RenderTexture _source;
    static Material _blurMaterial = null;

    //                      FUNCTIONS
    ////////////////////////////////////////////////////////////////////////////////

    void SetupRenderTarget(Vector2_1 resolution, ref RenderTexture tex)
    {
        if (tex == null)
            tex = new RenderTexture(resolution.x, resolution.y, 0);
        else if (tex.width != resolution.x || tex.height != resolution.y)
        {
            tex.width = resolution.x;
            tex.height = resolution.y;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    public Texture Apply(Texture2D fogtexture, Vector2_1 resolution, int amount, int iterations, FogBlurType type)
    {
        if (amount <= 0 || iterations <= 0)
            return fogtexture;

        if (_blurMaterial == null)
            _blurMaterial = new Material(Shader.Find("Hidden/FogOfWarBlurShader"));

        _blurMaterial.SetFloat("_BlurAmount", amount);
        _blurMaterial.SetKeywordEnabled("GAUSSIAN3", type == FogBlurType.Gaussian3);
        _blurMaterial.SetKeywordEnabled("GAUSSIAN5", type == FogBlurType.Gaussian5);
        _blurMaterial.SetKeywordEnabled("ANTIALIAS", type == FogBlurType.Antialias);

        SetupRenderTarget(resolution, ref _target);
        if (iterations > 1)
            SetupRenderTarget(resolution, ref _source);

        RenderTexture lastrt = RenderTexture.active;

        RenderTexture.active = _target;
        Graphics.Blit(fogtexture, _blurMaterial);

        for (int i = 1; i < iterations; ++i)
        {
            FogTools.Swap(ref _target, ref _source);
            RenderTexture.active = _target;
            Graphics.Blit(_source, _blurMaterial);
        }

        RenderTexture.active = lastrt;

        return _target;
    }

    ////////////////////////////////////////////////////////////////////////////////
}