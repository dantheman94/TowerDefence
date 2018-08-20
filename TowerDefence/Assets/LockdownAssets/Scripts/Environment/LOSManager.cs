using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//******************************
//
//  Created by: Angus Secomb
//
//  Last edited by: Angus Secomb
//  Last edited on: 15/8/2018
//
// Made using code and formulas from
// weesals blog.
//******************************

[ExecuteInEditMode]
public class LOSManager : MonoBehaviour
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                             INSPECTOR                                         ///
    public bool PreviewInEditor = false;

    // The parameters used to determine texture size and quality
    [Serializable]
    public class SizeParameters
    {
        public Terrain Terrain;
        public int Width = -1, Height = -1;
        public float Quality = 1;
        public bool HighDetailTexture = false;
    }
    public SizeParameters Size;
    public Terrain Terrain { get { return Size.Terrain; } }
    public int Width { get { return Size.Width; } }
    public int Height { get { return Size.Height; } }
    public float Scale { get { return Size.Quality; } }
    public bool HighDetailTexture { get { return Size.HighDetailTexture; } }

    public Color FogColor = Color.grey;

    // The parameters used for visual features
    [Serializable]
    public class VisualParameters
    {
        [Range(0, 255)]
        public int AOIntensity = 128;
        [Range(0, 1024)]
        public int InterpolationRate = 512;
        public float GrayscaleDecayDuration = 300;
        public bool RevealOnEntityDiscover = true;
    }
    public VisualParameters Visual;
    public int AOIntensity { get { return Visual.AOIntensity; } }
    public int InterpolationRate { get { return Visual.InterpolationRate; } }
    public float GrayscaleDecayDuration { get { return Visual.GrayscaleDecayDuration; } }
    public bool RevealOnEntityDiscover { get { return Visual.RevealOnEntityDiscover; } }

    // Parameters used to enable height blockers
    [Serializable]
    public class HeightBlockerParameters
    {
        public bool Enable = true;
        public bool AllowOwnTeamBlockers = false;
    }
    public HeightBlockerParameters HeightBlockers;
    public bool EnableHeightBlockers { get { return HeightBlockers.Enable; } }
    public bool AllowOwnTeamHeightBlockers { get { return HeightBlockers.AllowOwnTeamBlockers; } }
    //                                                                                                  //
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                    VARIABLES                                                   //
  
    // List of entities that interact with LOS
    [HideInInspector]
    public List<LOSEntity> Entities = new List<LOSEntity>();
    // List of entities currently animating their LOS
    [HideInInspector]
    public List<LOSEntity> AnimatingEntities = new List<LOSEntity>();


    // Some internal data
    int _FrameID = 0;
    float _Timer = 0;
    Color32[] _Pixels;
    Color32[] _LerpPixels;
    float[,] _BlockHeights;
    float[,] _TerrainHeightsCache;
    Texture2D _LosTexture;
    int[] jCache = new int[1024];

    // Used to determine when the user changes a field that requires
    // the texture to be recreated
    private int _PreviewParameterHash = 0;
    private int GenerateParameterHash() { return (Width + Height * 1024) + Scale.GetHashCode() + HighDetailTexture.GetHashCode(); }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //                                   FUNCTIONS                                                     //

    void Start()
    {
        if (Application.isPlaying) InitializeTexture();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    // Get a size from the provided properties
    private int SizeFromParams(int desired, float terrainSize, float scale)
    {
        int size = 128;
        if (desired > 0) size = Mathf.CeilToInt(desired * scale);
        else if (terrainSize > 0) size = Mathf.CeilToInt(terrainSize * scale);
        return Mathf.Clamp(size, 4, 512);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    // Create a texture matching the required properties
    void InitializeTexture()
    {
        int width = SizeFromParams(Width, Terrain != null ? Terrain.terrainData.size.x : 0, Scale);
        int height = SizeFromParams(Height, Terrain != null ? Terrain.terrainData.size.z : 0, Scale);

        if (_LosTexture != null) DestroyImmediate(_LosTexture);
        _BlockHeights = null;
        TextureFormat texFormat = HighDetailTexture ?
            (AOIntensity > 0 ? TextureFormat.ARGB32 : TextureFormat.RGB24) :
            (AOIntensity > 0 ? TextureFormat.ARGB4444 : TextureFormat.RGB565);
        _LosTexture = new Texture2D(width, height, texFormat, false);
        _Pixels = _LosTexture.GetPixels32();
        for (int p = 0; p < _Pixels.Length; ++p)
        {
            _Pixels[p] = FogColor;
        }
        _LosTexture.SetPixels32(_Pixels);
        _LerpPixels = null;
        if (Terrain != null)
        {
            Shader.SetGlobalTexture("_FOWTex", _LosTexture);
            Shader.SetGlobalVector("_FOWTex_ST",
                new Vector4(
                    Scale / width, Scale / height,
                    (0.5f - Scale * 0.5f) / width, (0.5f - Scale * 0.5f) / height
                )
            );
        }
        Debug.Log("FOW Texture created, " + width + " x" + height);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Runs every frame.
    /// </summary>
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (PreviewInEditor)
            {
                // Make sure we have a valid texture
                if (_LosTexture == null || _PreviewParameterHash != GenerateParameterHash())
                {
                    InitializeTexture();
                    _PreviewParameterHash = GenerateParameterHash();
                }
            }
            else
            {
                // Or just use a white texture as placeholder
                Shader.SetGlobalTexture("_FOWTex", UnityEditor.EditorGUIUtility.whiteTexture);
                if (_LosTexture != null) DestroyImmediate(_LosTexture);
                _LosTexture = null;
            }
        }
#endif
        if (_LosTexture != null)
        {
            // Update any animating entities (update their FOW color)
            for (int e = 0; e < AnimatingEntities.Count; ++e)
            {
                if (AnimatingEntities[e].UpdateFOWColor())
                    AnimatingEntities.RemoveAt(e--);
            }
            // If in editor mode
            if (!Application.isPlaying)
            {
                // Refresh the map each frame
                for (int p = 0; p < _Pixels.Length; ++p)
                {
                    _Pixels[p] = new Color32(0, 255, 0, 255);
                }
                // Add LOS and AO for all entities
                foreach (var entity in Entities)
                {
                    RevealLOS(entity, entity.IsRevealer ? 255 : 0, 255, 255);
                    if (entity.EnableAO && (AOIntensity > 0 || EnableHeightBlockers))
                    {
                        var bounds = entity.Bounds;
                        AddAO(bounds, entity.Height);
                    }
                }
            }
            else
            {
                bool forceFullUpdate = Time.frameCount == 1;
                // Reset all entities to be invisible
                if (forceFullUpdate)
                {
                    int revealerCount = 0;
                    foreach (var entity in Entities)
                    {
                        entity.RevealState = LOSEntity.RevealStates.Hidden;
                        if (entity.IsRevealer) revealerCount++;
                    }
                    if (revealerCount == 0)
                    {
                        Debug.LogError("No LOSEntity items were marked as revealers! Tick the 'Is Revealed' checkbox for at least 1 item.");
                    }
                }
                // Ensure we have space to store blocking heights (if enabled)
                if (_BlockHeights == null && EnableHeightBlockers)
                {
                    _BlockHeights = new float[_LosTexture.height, _LosTexture.width];
                    forceFullUpdate = true;
                }
                // Decay grayscale
                if (GrayscaleDecayDuration > 0)
                {
                    const int GrayscaleGranularity = 4;
                    int oldGrayDecay = (int)(256 / GrayscaleGranularity * _Timer / GrayscaleDecayDuration) * GrayscaleGranularity;
                    _Timer += Time.deltaTime;
                    int newGrayDecay = (int)(256 / GrayscaleGranularity * _Timer / GrayscaleDecayDuration) * GrayscaleGranularity;
                    int grayDecayCount = newGrayDecay - oldGrayDecay;
                    if (grayDecayCount != 0)
                    {
                        for (int p = 0; p < _Pixels.Length; ++p)
                        {
                            _Pixels[p].b = (byte)Mathf.Max(_Pixels[p].b - grayDecayCount, 0);
                        }
                    }
                }
                ++_FrameID;
                // Reset AO and LOS
                bool updateAo = (_FrameID % 2) == 0;
                if (updateAo || forceFullUpdate)
                {
                    for (int p = 0; p < _Pixels.Length; ++p)
                    {
                        _Pixels[p].r = 0;
                        //pixels[p].a = (byte)Mathf.Clamp(Costs.Costs[p / Costs.Width, p % Costs.Width] * 10 - 10, 0, 255);
                        _Pixels[p].a = 255;
                    }
                    if (AOIntensity > 0 || EnableHeightBlockers)
                    {
                        if (Terrain != null && EnableHeightBlockers && _BlockHeights != null)
                        {
                            if (_TerrainHeightsCache == null)
                            {
                                _TerrainHeightsCache = (float[,])_BlockHeights.Clone();
                                for (int y = 0; y < _BlockHeights.GetLength(0); ++y)
                                {
                                    for (int x = 0; x < _BlockHeights.GetLength(1); ++x)
                                    {
                                        var terrainData = Terrain.terrainData;
                                        int tx = Mathf.RoundToInt(x * terrainData.heightmapWidth / terrainData.size.x / Scale);
                                        int ty = Mathf.RoundToInt(y * terrainData.heightmapHeight / terrainData.size.z / Scale);
                                        _TerrainHeightsCache[y, x] = terrainData.GetHeight(tx, ty);
                                    }
                                }
                            }
                            for (int y = 0; y < _BlockHeights.GetLength(0); ++y)
                            {
                                for (int x = 0; x < _BlockHeights.GetLength(1); ++x)
                                {
                                    _BlockHeights[y, x] = _TerrainHeightsCache[y, x];
                                }
                            }
                        }
                        foreach (var entity in Entities)
                        {
                            var bounds = entity.Bounds;
                            if (entity.EnableAO && AOIntensity > 0) AddAO(bounds, entity.Height);
                            if (EnableHeightBlockers && (AllowOwnTeamHeightBlockers || !entity.IsRevealer))
                                AddHeightBlocker(bounds, entity.transform.position.y + entity.Height);
                        }
                    }
                }
                // Reveal LOS from all entities
                foreach (var entity in Entities)
                {
                    if (entity.IsRevealer) RevealLOS(entity, 255, 255, 330);
                }
                int count = 0;
                foreach (var entity in Entities)
                {
                    ++count;
                    var rect = entity.Bounds;
                    var fowColor = GetFOWColor(rect);
                    var visible = GetRevealFromFOW(fowColor);
                    if (entity.RevealState != visible && !(entity.RevealState == LOSEntity.RevealStates.Hidden && visible == LOSEntity.RevealStates.Fogged))
                    {
                        entity.RevealState = visible;
                        if (visible == LOSEntity.RevealStates.Unfogged && RevealOnEntityDiscover)
                        {
                            RevealLOS(rect, 0, entity.Height + entity.transform.position.y, 0, 255, 255);
                        }
                    }
                    if (visible != LOSEntity.RevealStates.Hidden || forceFullUpdate)
                    {
                        entity.SetFOWColor(GetQuantizedFOW(fowColor), !forceFullUpdate);
                        // Queue the item for FOW animation
                        if (entity.RequiresFOWUpdate && !AnimatingEntities.Contains(entity))
                            AnimatingEntities.Add(entity);
                    }
                }
            }
            bool isChanged = true;
            if (InterpolationRate > 0 && Application.isPlaying)
            {
                if (_LerpPixels == null) _LerpPixels = _Pixels.ToArray();
                else
                {
                    int rate = Mathf.Max(Mathf.RoundToInt(InterpolationRate * Time.deltaTime), 1);
                    for (int p = 0; p < _LerpPixels.Length; ++p)
                    {
                        byte r = EaseToward(_LerpPixels[p].r, _Pixels[p].r, rate),
                            g = EaseToward(_LerpPixels[p].g, _Pixels[p].g, rate),
                            b = EaseToward(_LerpPixels[p].b, _Pixels[p].b, rate),
                            a = EaseToward(_LerpPixels[p].a, _Pixels[p].a, rate);
                        if (isChanged || _LerpPixels[p].a != a || _LerpPixels[p].r != r || _LerpPixels[p].g != g || _LerpPixels[p].b != b)
                        {
                            isChanged = true;
                            _LerpPixels[p] = new Color32(r, g, b, a);
                        }
                    }
                }
            }
            else _LerpPixels = null;

            if (isChanged)
            {
                _LosTexture.SetPixels32(_LerpPixels ?? _Pixels);
                _LosTexture.Apply();
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// Eases LOS towards target area.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    private byte EaseToward(byte from, byte to, int amount)
    {
        if (Mathf.Abs(from - to) < amount) return to;
        return (byte)(from + (to > from ? amount : -amount));
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get extents of a box.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="inflateRange"></param>
    /// <param name="xMin"></param>
    /// <param name="yMin"></param>
    /// <param name="xMax"></param>
    /// <param name="yMax"></param>
    private void GetExtents(Vector2 pos, int inflateRange, out int xMin, out int yMin, out int xMax, out int yMax)
    {
        xMin = Mathf.RoundToInt(pos.x - inflateRange);
        xMax = Mathf.RoundToInt(pos.x + inflateRange);
        yMin = Mathf.RoundToInt(pos.y - inflateRange);
        yMax = Mathf.RoundToInt(pos.y + inflateRange);
        if (xMin < 0) xMin = 0; else if (xMax >= _LosTexture.width) xMax = _LosTexture.width - 1;
        if (yMin < 0) yMin = 0; else if (yMax >= _LosTexture.height) yMax = _LosTexture.height - 1;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Get extents of a box.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="inflateRange"></param>
    /// <param name="xMin"></param>
    /// <param name="yMin"></param>
    /// <param name="xMax"></param>
    /// <param name="yMax"></param>
    private void GetExtents(Rect rect, int inflateRange, out int xMin, out int yMin, out int xMax, out int yMax)
    {
        xMin = Mathf.RoundToInt(rect.xMin * Scale) - inflateRange;
        xMax = Mathf.RoundToInt(rect.xMax * Scale) + inflateRange;
        yMin = Mathf.RoundToInt(rect.yMin * Scale - 1) - inflateRange;
        yMax = Mathf.RoundToInt(rect.yMax * Scale - 1) + inflateRange;
        if (xMin < 0) xMin = 0; else if (xMax >= _LosTexture.width) xMax = _LosTexture.width - 1;
        if (yMin < 0) yMin = 0; else if (yMax >= _LosTexture.height) yMax = _LosTexture.height - 1;
        if (xMax < xMin) xMax = xMin;
        if (yMax < yMin) yMax = yMin;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Add a height blocker
    private void AddHeightBlocker(Rect rect, float height)
    {
        int xMin, yMin, xMax, yMax;
        GetExtents(rect, 0, out xMin, out yMin, out xMax, out yMax);
        for (int y = yMin; y <= yMax; ++y)
        {
            for (int x = xMin; x <= xMax; ++x)
            {
                _BlockHeights[y, x] = Mathf.Max(_BlockHeights[y, x], height);
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Add ambient occlusion around an eara
    private void AddAO(Rect rect, float height)
    {
        byte aoAmount = (byte)AOIntensity;
        byte nonAOAmount = (byte)(255 - aoAmount);
        float spreadRange = height / 2 + 0.5f;
        int spreadRangeI = Mathf.RoundToInt(spreadRange * Scale);
        int xMin, yMin, xMax, yMax;
        GetExtents(rect, spreadRangeI, out xMin, out yMin, out xMax, out yMax);
        for (int y = yMin; y <= yMax; ++y)
        {
            float yIntl = Mathf.Clamp(y / Scale, rect.yMin, rect.yMax - 1);
            for (int x = xMin; x <= xMax; ++x)
            {
                var nodePos = new Vector2(x, y) / Scale;
                var intlPos = new Vector2(Mathf.Clamp(nodePos.x, rect.xMin, rect.xMax - 1), yIntl);
                float dst2 = (intlPos - nodePos).sqrMagnitude;
                if (dst2 >= spreadRange * spreadRange) continue;
                int p = x + y * _LosTexture.width;
                //byte value = (byte)Mathf.Clamp(128 + (spreadAmnt) * dst2, 0, 255);
                byte value = (byte)(nonAOAmount + aoAmount *
                    2 * dst2 / (dst2 * 1 + spreadRange * spreadRange + 1)
                );
                if (_Pixels[p].a > value) _Pixels[p].a = value;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Reveal an area
    private void RevealLOS(LOSEntity sight, float los, float fow, float grayscale)
    {
        Rect rect = sight.Bounds;
        RevealLOS(rect, sight.Range, sight.Height + sight.transform.position.y, los, fow, grayscale);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void RevealLOS(Rect rect, float range, float height, float los, float fow, float grayscale)
    {
        int xMin, yMin, xMax, yMax;
        int rangeI = Mathf.RoundToInt(range * Scale);
        int xiMin, yiMin, xiMax, yiMax;
        GetExtents(rect, 0, out xiMin, out yiMin, out xiMax, out yiMax);
        GetExtents(rect, rangeI, out xMin, out yMin, out xMax, out yMax);
        if (EnableHeightBlockers && _BlockHeights != null)
        {
            for (int a = 0; a < 4; ++a)
            {
                int d = (a % 2);
                int jMin = d == 0 ? xMin : yMin, jMax = d == 0 ? xMax : yMax;
                int kMin = d == 0 ? yMin : xMin, kMax = d == 0 ? yMax : xMax;
                int jMid = d == 0 ? (xiMin + xiMax) / 2 : (yiMin + yiMax) / 2;
                int kMid = d == 0 ? (yiMin + yiMax) / 2 : (xiMin + xiMax) / 2;
                int prevMax = 0;
                for (int dj = jMin - jMid; dj <= jMax - jMid; ++dj)
                {
                    int kEnd = (a < 2 ? kMax - kMid : kMid - kMin);
                    int kStart = (a < 2 ? Mathf.Max(kMin - kMid, 0) : Mathf.Max(kMid - kMax, 0));
                    if (kEnd <= 0) continue;
                    for (int dk = kStart; dk <= kEnd; ++dk)
                    {
                        jCache[dk] = -1000;
                    }
                    int curMax = kEnd;
                    for (int dk = kStart; dk <= kEnd; ++dk)
                    {
                        int wj = jMid + dj * dk / kEnd;
                        if (jCache[dk] >= dj) continue;
                        jCache[dk] = dj;
                        int wk = kMid + dk * (a < 2 ? 1 : -1);
                        int wx = d == 0 ? wj : wk;
                        int wy = d == 0 ? wk : wj;
                        if (wx < 0 || wy < 0 || wx >= _LosTexture.width || wy >= _LosTexture.height) continue;
                        if (curMax == kEnd && dk >= 1 && (wx < xiMin || wy < yiMin || wx > xiMax || wy > yiMax))
                        {
                            if (_BlockHeights[wy, wx] > height) curMax = dk;
                        }
                        {
                            var nodePos = new Vector2(wx, wy) / Scale;
                            var intlPos = new Vector2(
                                Mathf.Clamp(nodePos.x, rect.xMin, rect.xMax - 1),
                                Mathf.Clamp(nodePos.y, rect.yMin, rect.yMax - 1)
                            );
                            float dist2 = (intlPos - nodePos).sqrMagnitude / (range * range);
                            if (dist2 > 1) continue;
                            float bright = 1;
                            const float FadeStart = 0.8f;
                            if (dist2 > FadeStart * FadeStart)
                            {
                                bright = Mathf.Clamp01((1 - Mathf.Sqrt(dist2)) / (1 - FadeStart));
                            }
                            int p = wx + wy * _LosTexture.width;
                            if (dk > curMax) bright = bright * (0.75f - 0.5f * (dk - curMax) / 3);
                            _Pixels[p].r = (byte)Mathf.Max(_Pixels[p].r, (byte)(bright * los));
                            _Pixels[p].g = (byte)Mathf.Max(_Pixels[p].g, (byte)(bright * fow));
                            _Pixels[p].b = (byte)Mathf.Max(_Pixels[p].b, (byte)(Mathf.Clamp(bright * grayscale, 0, 255)));
                        }
                        if (dk > curMax + 1)
                        {
                            if (dk >= prevMax) break;
                        }
                    }
                    prevMax = curMax;
                }
            }
        }
        else
        {
            for (int y = yMin; y <= yMax; ++y)
            {
                float yIntl = Mathf.Clamp(y, rect.yMin, rect.yMax - 1);
                for (int x = xMin; x <= xMax; ++x)
                {
                    var nodePos = new Vector2(x, y) / Scale;
                    var intlPos = new Vector2(Mathf.Clamp(nodePos.x, rect.xMin, rect.xMax - 1), yIntl);
                    float dist2 = (intlPos - nodePos).sqrMagnitude;
                    float range2 = (range * range);
                    if (dist2 > range2) continue;
                    const float FadeStart = 2;
                    float innerRange = Mathf.Max(range - FadeStart, 0);
                    float innerRange2 = innerRange * innerRange;
                    float bright = 1;
                    if (dist2 > innerRange2)
                    {
                        bright = Mathf.Clamp01((range - Mathf.Sqrt(dist2)) / (range - innerRange));
                    }
                    int p = x + y * _LosTexture.width;
                    _Pixels[p].r = (byte)Mathf.Max(_Pixels[p].r, bright * los);
                    _Pixels[p].g = (byte)Mathf.Max(_Pixels[p].g, bright * fow);
                    _Pixels[p].b = (byte)Mathf.Max(_Pixels[p].b, Mathf.Clamp(bright * grayscale, 0, 255));
                }
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Notify that the terrain heights are no longer valid
    public void InvalidateTerrainHeightsCache()
    {
        _TerrainHeightsCache = null;
        _BlockHeights = null;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Get states and colours for entities
    public Color32 GetFOWColor(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x * Scale),
            y = Mathf.RoundToInt(pos.x * Scale);
        int p = x + y * _LosTexture.width;
        return _Pixels[p];
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public Color32 GetFOWColor(Rect rect)
    {
        int xMin, yMin, xMax, yMax;
        GetExtents(rect, 0, out xMin, out yMin, out xMax, out yMax);
        Color32 color = new Color32(0, 0, 0, 0);
        for (int y = yMin; y <= yMax; ++y)
        {
            for (int x = xMin; x <= xMax; ++x)
            {
                int p = x + y * _LosTexture.width;
                color.r = (byte)Mathf.Max(color.r, _Pixels[p].r);
                color.g = (byte)Mathf.Max(color.g, _Pixels[p].g);
                color.b = (byte)Mathf.Max(color.b, _Pixels[p].b);
                color.a = (byte)Mathf.Max(color.a, _Pixels[p].a);
            }
        }
        return color;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public LOSEntity.RevealStates GetRevealFromFOW(Color32 px)
    {
        if (px.r >= 128) return LOSEntity.RevealStates.Unfogged;
        if (px.g >= 128) return LOSEntity.RevealStates.Fogged;
        return LOSEntity.RevealStates.Hidden;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public LOSEntity.RevealStates IsVisible(Vector2 pos)
    {
        return GetRevealFromFOW(GetFOWColor(pos));
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public LOSEntity.RevealStates IsVisible(Rect rect)
    {
        return GetRevealFromFOW(GetFOWColor(rect));
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public Color32 GetQuantizedFOW(Color32 px)
    {
        if (px.r >= 128)
        {
            px.r = px.g = px.b = 255;
        }
        else
        {
            px.r = 0;
            px.g = px.g < 128 ? (byte)0 : (byte)255;
        }
        return px;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Allow entities to tell us when theyre added
    public static void AddEntity(LOSEntity entity)
    {
        if (Instance != null && !Instance.Entities.Contains(entity)) Instance.Entities.Add(entity);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static void RemoveEntity(LOSEntity entity)
    {
        if (Instance != null) Instance.Entities.Remove(entity);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // A singleton instance of this class
    private static LOSManager _Instance;
    public static LOSManager Instance
    {
        get
        {
            if (_Instance == null) _Instance = GameObject.FindObjectOfType<LOSManager>();
            return _Instance;
        }
    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
