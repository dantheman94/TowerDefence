using UnityEngine;
using System.Collections;

public static class AspectRatioC
{
    public static Vector2 GetAspectRatio(int x, int y)
    {
        float f = (float)x / (float)y;
        int i = 0;
        while (true)
        {
            i++;
            if (System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
                break;
        }
        return new Vector2((float)System.Math.Round(f * i, 2), i);
    }
    public static Vector2 GetAspectRatio(Vector2 xy)
    {
        float f = xy.x / xy.y;
        int i = 0;
        while (true)
        {
            i++;
            if (System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
                break;
        }
        return new Vector2((float)System.Math.Round(f * i, 2), i);
    }
    public static Vector2 GetAspectRatio(int x, int y, bool debug)
    {
        float f = (float)x / (float)y;
        int i = 0;
        while (true)
        {
            i++;
            if (System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
                break;
        }
        if (debug)
            Debug.Log("Aspect ratio is " + f * i + ":" + i + " (Resolution: " + x + "x" + y + ")");
        return new Vector2((float)System.Math.Round(f * i, 2), i);
    }
    public static Vector2 GetAspectRatio(Vector2 xy, bool debug)
    {
        float f = xy.x / xy.y;
        int i = 0;
        while (true)
        {
            i++;
            if (System.Math.Round(f * i, 2) == Mathf.RoundToInt(f * i))
                break;
        }
        if (debug)
            Debug.Log("Aspect ratio is " + f * i + ":" + i + " (Resolution: " + xy.x + "x" + xy.y + ")");
        return new Vector2((float)System.Math.Round(f * i, 2), i);
    }
}