using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawManager : FogDrawer
{
    byte[] _Values;
    FilterMode _FilterMode;

    public override void GetValues(byte[] outvalues)
    {
       for(int i =0; i < _Values.Length; ++i)
        {
            outvalues[i] = _Values[i];
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public override void SetValues(byte[] values)
    {
        for(int i = 0; i < _Values.Length; ++ i )
        {
            _Values[i] = values[i];
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    protected override void OnInitialise()
    {
        if(_Values == null || _Values.Length != _Map.PixelCount)
        {
            _Values = new byte[_Map.PixelCount];
        }
        _FilterMode = _Map.FilterMode;
    }

    ///////////////////////////////////////////////////////////////////////////

    public override void Clear(byte value)
    {
        for (int i = 0; i < _Values.Length; ++i)
        {
            _Values[i] = value;
        }  
    }

    ///////////////////////////////////////////////////////////////////////////

    public override void Fade(int to, int amount)
    {
        for(int i = 0; i < _Values.Length; ++i)
        {
            if(_Values[i] < to)
            {
                _Values[i] = (byte)Mathf.Min(_Values[i] + amount, to);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    bool LineOfSightCanSee(FogShape shape, Vector2 offset, float fogradius)
    {
        if(shape.LineOfSight == null)
        {
            return true;
        }

        float idx = FogTools.ClockwiseAngle(Vector2.up, offset) * shape.LineOfSight.Length / 360.0f;
        if(idx < 0)
        {
            idx += shape.LineOfSight.Length;
        }


        // sampling
        float value;
        if (_Map.FilterMode == FilterMode.Point)
            value = shape.LineOfSight[Mathf.RoundToInt(idx) % shape.LineOfSight.Length];
        else
        {
            int idxlow = Mathf.FloorToInt(idx);
            int idxhigh = (idxlow + 1) % shape.LineOfSight.Length;
            value = Mathf.LerpUnclamped(shape.LineOfSight[idxlow], shape.LineOfSight[idxhigh], idx % 1);
        }

        float dist = value * fogradius;
        return offset.sqrMagnitude < dist * dist;
    }

    ///////////////////////////////////////////////////////////////////////////

    bool LineOfSightCanSeeCell(FogShape shape, Vector2_1 offset)
    {
        if (shape.VisibleCells == null)
            return true;

        int radius = Mathf.RoundToInt(shape.Radius);
        int width = radius + radius + 1;

        offset.x += radius;
        if (offset.x < 0 || offset.x >= width)
            return true;

        offset.y += radius;
        if (offset.y < 0 || offset.y >= width)
            return true;

        return shape.VisibleCells[offset.y * width + offset.x];
    }

    ///////////////////////////////////////////////////////////////////////////

    struct DrawInfo
    {
        //                    VARIABLES
        ///////////////////////////////////////////////////////////////////////////

        public Vector2 fogCenterPos;
        public Vector2_1 fogEyePos;
        public Vector2 fogForward;
        public float forwardAngle;
        public int xMin;
        public int xMax;
        public int yMin;
        public int yMax;

        //                        FUNCTIONS
        ///////////////////////////////////////////////////////////////////////////

        public DrawInfo(FogMap map, FogShape shape, float xradius, float yradius)
        {
            // convert size to fog space
            fogForward = shape.Forward;
            forwardAngle = FogTools.ClockwiseAngle(Vector2.up, fogForward) * Mathf.Deg2Rad;
            float sin = Mathf.Sin(-forwardAngle);
            float cos = Mathf.Cos(-forwardAngle);
            Vector2 relativeoffset = new Vector2(shape.Offset.x * cos - shape.Offset.y * sin, shape.Offset.x * sin + shape.Offset.y * cos);

            fogCenterPos = FogConversion.WorldToFog(FogConversion.WorldToFogPlane(shape.EyePosition, map.Plane) + relativeoffset, map.Offset, map.Resolution, map.Size);
            fogEyePos = new Vector2_1(FogConversion.WorldToFog(shape.EyePosition, map.Plane, map.Offset, map.Resolution, map.Size));

            // find ranges
            if (shape.VisibleCells == null)
            {
                xMin = Mathf.Max(0, Mathf.RoundToInt(fogCenterPos.x - xradius));
                xMax = Mathf.Min(map.Resolution.x - 1, Mathf.RoundToInt(fogCenterPos.x + xradius));
                yMin = Mathf.Max(0, Mathf.RoundToInt(fogCenterPos.y - yradius));
                yMax = Mathf.Min(map.Resolution.y - 1, Mathf.RoundToInt(fogCenterPos.y + yradius));
            }
            else
            {
                fogCenterPos = FogConversion.SnapToNearestFogPixel(fogCenterPos);
                fogEyePos = new Vector2_1(FogConversion.SnapToNearestFogPixel(FogConversion.WorldToFog(shape.EyePosition, map.Offset, map.Resolution, map.Size)));

                Vector2_1 pos = new Vector2_1(Mathf.RoundToInt(fogCenterPos.x), Mathf.RoundToInt(fogCenterPos.y));
                Vector2_1 rad = new Vector2_1(Mathf.RoundToInt(xradius), Mathf.RoundToInt(yradius));
                xMin = Mathf.Max(0, Mathf.RoundToInt(pos.x - rad.x));
                xMax = Mathf.Min(map.Resolution.x - 1, Mathf.RoundToInt(pos.x + rad.x));
                yMin = Mathf.Max(0, Mathf.RoundToInt(pos.y - rad.y));
                yMax = Mathf.Min(map.Resolution.y - 1, Mathf.RoundToInt(pos.y + rad.y));
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    byte SampleTexture(Texture2D texture, float u, float v)
    {
        // GetPixel() and GetPixelBilinear() are not supported on other threads!
        if (_Map.MultiThreaded)
            return 0;

        float value = 0;
        if (_FilterMode == FilterMode.Point)
            value = 1 - texture.GetPixel(Mathf.FloorToInt(u * texture.width), Mathf.FloorToInt(v * texture.height)).r;
        else
            value = 1 - texture.GetPixelBilinear(u, v).r;
        return (byte)(value * 255);
    }

    ///////////////////////////////////////////////////////////////////////////

    void Unfog(int x, int y, byte v)
    {
        int index = y * _Map.Resolution.x + x;
        if (_Values[index] > v)
            _Values[index] = v;
    }

    ///////////////////////////////////////////////////////////////////////////

    protected override void DrawCircle(FogCircle shape)
    {
        int fogradius = Mathf.RoundToInt(shape.Radius * _Map.PixelSize);
        int fogradiussqr = fogradius * fogradius;
        DrawInfo info = new DrawInfo(_Map, shape, fogradius, fogradius);

        // view angle stuff
        float dotangle = 1 - shape.Angle / 90;

        for (int y = info.yMin; y <= info.yMax; ++y)
        {
            for (int x = info.xMin; x <= info.xMax; ++x)
            {
                // is pixel within circle radius
                Vector2 centeroffset = new Vector2(x, y) - info.fogCenterPos;
                if (shape.VisibleCells == null && centeroffset.sqrMagnitude >= fogradiussqr)
                    continue;

                // check if in view angle
                if (dotangle > -0.99f && Vector2.Dot(centeroffset.normalized, info.fogForward) <= dotangle)
                    continue;

                // can see pixel
                Vector2_1 offset = new Vector2_1(x, y) - info.fogEyePos;
                if (!LineOfSightCanSee(shape, offset.vector2, fogradius))
                    continue;

                if (!LineOfSightCanSeeCell(shape, offset))
                    continue;

                Unfog(x, y, shape.GetFallOff(centeroffset.magnitude / (_Map.PixelSize * shape.Radius)));
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    protected override void DrawTexture(FogShapeTexture shape)
    {
        if (shape.Texture == null)
            return;

        if (shape.RotateToForward)
        {
            DrawRotatedTexture(shape);
            return;
        }

        // convert size to fog space
        float fogradius = shape.Radius * _Map.PixelSize;
        DrawInfo info = new DrawInfo(_Map, shape, shape.Radius * _Map.PixelSize, shape.Radius * _Map.PixelSize);

        for (int y = info.yMin; y <= info.yMax; ++y)
        {
            for (int x = info.xMin; x <= info.xMax; ++x)
            {
                // can see pixel
                Vector2_1 offset = new Vector2_1(x, y) - info.fogEyePos;
                if (!LineOfSightCanSee(shape, offset.vector2, fogradius))
                    continue;

                if (!LineOfSightCanSeeCell(shape, offset))
                    continue;

                // read texture
                float u = Mathf.InverseLerp(info.xMin, info.xMax, x);
                float v = Mathf.InverseLerp(info.yMin, info.yMax, y);
                Unfog(x, y, SampleTexture(shape.Texture, u, v));
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    void DrawRotatedTexture(FogShapeTexture shape)
    {
        if (shape.Texture == null)
            return;

        // convert size to fog space
        float size = new Vector2(shape.Radius, shape.Radius).magnitude * _Map.PixelSize;
        Vector2 sizemul = new Vector2(size / (shape.Radius * _Map.PixelSize), size / (shape.Radius * _Map.PixelSize));
        float fogradius = size;
        DrawInfo info = new DrawInfo(_Map, shape, size, size);

        // rotation stuff
        float sin = Mathf.Sin(info.forwardAngle);
        float cos = Mathf.Cos(info.forwardAngle);

        for (int y = info.yMin; y < info.yMax; ++y)
        {
            for (int x = info.xMin; x < info.xMax; ++x)
            {
                // get rotated uvs
                float u = Mathf.InverseLerp(info.xMin, info.xMax, x) - 0.5f;
                float v = Mathf.InverseLerp(info.yMin, info.yMax, y) - 0.5f;

                float uu = (u * cos - v * sin) * sizemul.x + 0.5f;
                float vv = (v * cos + u * sin) * sizemul.y + 0.5f;
                if (uu < 0 || uu >= 1 || vv < 0 || vv >= 1)
                    continue;

                // can see pixel
                Vector2_1 offset = new Vector2_1(x, y) - info.fogEyePos;
                if (!LineOfSightCanSee(shape, offset.vector2, fogradius))
                    continue;

                if (!LineOfSightCanSeeCell(shape, offset))
                    continue;

                // read texture
                Unfog(x, y, SampleTexture(shape.Texture, uu, vv));
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public override void Unfog(Rect rect)
    {
        rect.xMin = Mathf.Max(rect.xMin, 0);
        rect.xMax = Mathf.Min(rect.xMax, _Map.Resolution.x);
        rect.yMin = Mathf.Max(rect.yMin, 0);
        rect.yMax = Mathf.Min(rect.yMax, _Map.Resolution.y);

        for (int y = (int)rect.yMin; y < (int)rect.yMax; ++y)
        {
            for (int x = (int)rect.xMin; x < (int)rect.xMax; ++x)
                _Values[y * _Map.Resolution.x + x] = 0;
        }
    }

///////////////////////////////////////////////////////////////////////////
}
