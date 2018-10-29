using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// -=-=-=-=-=-=-=-=-
/// Created By: Angus Secomb
/// Last Edited: 29/10/18
/// Editor: Angus
/// =-=-=-=-=-=-=-=-=

public enum FogOfWarPhysics
{
    None,
    Physics2D,
    Physics3D
}

public enum FogOfWarPlane
{
    XY,
    YZ,
    XZ
}

class FogShaderID
{
    public int MainTex;
    public int SkyboxTex;
    public int FogColorTex;
    public int FogColorTexScale;
    public int CameraDir;
    public int CameraWS;
    public int FrustumCornersWS;
    public int FogColor;
    public int MapOffset;
    public int MapSize;
    public int FogTextureSize;
    public int FogTex;
    public int OutsideFogStrength;

    public void Initialise()
    {
        MainTex = Shader.PropertyToID("_MainTex");
        SkyboxTex = Shader.PropertyToID("_SkyboxTex");
        FogColorTex = Shader.PropertyToID("_FogColorTex");
        FogColorTexScale = Shader.PropertyToID("_FogColorTexScale");
        CameraDir = Shader.PropertyToID("_CameraDir");
        CameraWS = Shader.PropertyToID("_CameraWS");
        FrustumCornersWS = Shader.PropertyToID("_FrustumCornersWS");
        FogColor = Shader.PropertyToID("_FogColor");
        MapOffset = Shader.PropertyToID("_MapOffset");
        MapSize = Shader.PropertyToID("_MapSize");
        FogTextureSize = Shader.PropertyToID("_FogTextureSize");
        FogTex = Shader.PropertyToID("_FogTex");
        OutsideFogStrength = Shader.PropertyToID("_OutsideFogStrength");
    }
}

public class FogManager : MonoBehaviour {

    //                        INSPECTOR
    ////////////////////////////////////////////////////////////////////////////////
    [Header("------MAP SETTINGS------")]
    public Vector2_1 MapResolution = new Vector2_1(128, 128);
    public float MapSize = 128;
    public Vector2 MapOffset = Vector2.zero;

    public FogOfWarPlane Plane = FogOfWarPlane.XZ;

    [Header("Visuals")]
    public FilterMode FilterMode = FilterMode.Bilinear;
    public Color FogColor = Color.black;
    public Texture2D FogColorTexture = null;
    public float FogColorTextureScale = 1;
    public float FogColorTextureHeight = 0;
    public bool FogFarPlane = true;
    public int BlurAmount = 0;
    public int BlurIterations = 0;
    public FogBlurType BlurType = FogBlurType.Gaussian3;

    [Header("Behaviour")]
    public bool UpdateAutomatically = true;
    [Range(0.0f, 1.0f)]
    public float PartialFogAmount = 0.5f;
    private float _FadeAmount = 0;
    public float FadeSpeed = 10;

    [Header("Multithreading")]
    public bool MultiThreaded = false;
    [Range(1, 8)]
    public int Threads = 2;
    public double MaxMillisecondsPerFrame = 5;
    private FogPool _ThreadPool = null;
    private int _CurrentUnitProcessing = 0;
    private float _TimeSinceLastUpdate = 0;
    private System.Diagnostics.Stopwatch _StopWatch = new System.Diagnostics.Stopwatch();
    private bool _IsFirstProcessingFrame = true;
     
    //                   VARIABLES
    ////////////////////////////////////////////////////////////////////////////////

    private Material _Material;
    public Texture2D FogTexture { get; private set; }
    private Texture _FinalFogTexture = null;
    private byte[] _FogValuesCopy = null;
    public byte[] FogValues { get { return _FogValuesCopy; } set { } }
    private DrawManager _Drawer = null;
    private FogBlur _Blur = new FogBlur();

    private Transform _Transform;
    private Camera _Camera;

    static FogShaderID _Ids = null;
    static Shader _FogShader = null;
    public static Shader FogShader { get { if (_FogShader == null) _FogShader = Resources.Load<Shader>("FogOfWarShader");return _FogShader; } }
    static Shader _ClearFogShader = null;
    public static Shader ClearFogShader { get { if (_ClearFogShader == null) _ClearFogShader = Resources.Load<Shader>("ClearFogShader"); return _ClearFogShader; } }

    static List<FogManager> _Instances = new List<FogManager>();
    public static List<FogManager> Instances { get { return _Instances; } }

    //                      VARIABLES
    ////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////

    void Awake()
    {
        if(_Ids == null)
        {
            _Ids = new FogShaderID();
            _Ids.Initialise();
        }

        Reinitialize();
    }

    ////////////////////////////////////////////////////////////////////////////////

    private void OnEnable()
    {
        _Instances.Add(this);
    }

    ////////////////////////////////////////////////////////////////////////////////

    private void OnDisable()
    {
        _Instances.Remove(this);
    }

    public void Reinitialize()
    {
        if (_FogValuesCopy == null || _FogValuesCopy.Length != MapResolution.x * MapResolution.y)
            _FogValuesCopy = new byte[MapResolution.x * MapResolution.y];

        if (_Drawer == null)
            _Drawer = new DrawManager();
        _Drawer.Initialise(new FogMap(this));
        _Drawer.Clear(255);

        if (_Material == null)
        {
            _Material = new Material(FogShader);
            _Material.name = "FogMaterial";
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    // Increase skip to improve performance but sacrifice accuracy
    public float ExploredArea(int skip = 1)
    {
        skip = Mathf.Max(skip, 1);
        int total = 0;
        for (int i = 0; i < _FogValuesCopy.Length; i += skip)
            total += _FogValuesCopy[i];
        return (1.0f - total / (_FogValuesCopy.Length * 255.0f / skip)) * 2;
    }

    ////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        _Transform = transform;
        _Camera = GetComponent<Camera>();
        _Camera.depthTextureMode |= DepthTextureMode.Depth;
    }

    ////////////////////////////////////////////////////////////////////////////////

    public Vector2_1 WorldPositionToFogPosition(Vector3 position)
    {
        Vector2 mappos = FogConversion.WorldToFogPlane(position, Plane) - MapOffset;
        mappos.Scale(MapResolution.vector2 / MapSize);

        Vector2_1 mapposi = new Vector2_1(mappos);
        mapposi += new Vector2_1(MapResolution.x >> 1, MapResolution.y >> 1);
        return mapposi;
    }

    ////////////////////////////////////////////////////////////////////////////////

    // Returns a value between 0 (not in fog) and 255 (fully fogged)
    public byte GetFogValue(Vector3 position)
    {
        Vector2_1 mappos = WorldPositionToFogPosition(position);
        mappos.x = Mathf.Clamp(mappos.x, 0, MapResolution.x - 1);
        mappos.y = Mathf.Clamp(mappos.y, 0, MapResolution.y - 1);
        return _FogValuesCopy[mappos.y * MapResolution.x + mappos.x];
    }

    ////////////////////////////////////////////////////////////////////////////////

    public bool IsInFog(Vector3 position, byte minfog)
    {
        return GetFogValue(position) > minfog;
    }

    ////////////////////////////////////////////////////////////////////////////////

    public bool IsInFog(Vector3 position, float minfog)
    {
        return IsInFog(position, (byte)(minfog * 255));
    }

    ////////////////////////////////////////////////////////////////////////////////

    public bool IsInCompleteFog(Vector3 position)
    {
        return IsInFog(position, 240);
    }

    ////////////////////////////////////////////////////////////////////////////////

    public bool IsInPartialFog(Vector3 position)
    {
        return IsInFog(position, 20);
    }

    ////////////////////////////////////////////////////////////////////////////////

    public void Unfog(Rect rect)
    {
        _Drawer.Unfog(rect);
    }

    ////////////////////////////////////////////////////////////////////////////////

    public void Unfog(Bounds bounds)
    {
        Rect rect = new Rect();
        rect.min = FogConversion.WorldToFog(bounds.min, Plane, MapOffset, MapResolution, MapSize);
        rect.max = FogConversion.WorldToFog(bounds.max, Plane, MapOffset, MapResolution, MapSize);
        Unfog(rect);
    }

    ////////////////////////////////////////////////////////////////////////////////

    // Checks the visibility of an area, where a value of 0 is fully unfogged and 1 if fully fogged
    public float VisibilityOfArea(Bounds worldbounds)
    {
        Vector2 min = FogConversion.WorldToFog(worldbounds.min, Plane, MapOffset, MapResolution, MapSize);
        Vector2 max = FogConversion.WorldToFog(worldbounds.max, Plane, MapOffset, MapResolution, MapSize);

        int xmin = Mathf.Clamp(Mathf.RoundToInt(min.x), 0, MapResolution.x);
        int xmax = Mathf.Clamp(Mathf.RoundToInt(max.x), 0, MapResolution.x);
        int ymin = Mathf.Clamp(Mathf.RoundToInt(min.y), 0, MapResolution.y);
        int ymax = Mathf.Clamp(Mathf.RoundToInt(max.y), 0, MapResolution.y);

        float total = 0;
        int count = 0;
        for (int y = ymin; y < ymax; ++y)
        {
            for (int x = xmin; x < xmax; ++x)
            {
                ++count;
                total += _FogValuesCopy[y * MapResolution.x + x] / 255.0f;
            }
        }

        return total / count;
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets all the values based on the byte value.
    /// </summary>
    /// <param name="value"></param>
    public void SetAll(byte value = 255)
    {
        _Drawer.Clear(value);
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Process all units on the fog layer.
    /// </summary>
    /// <param name="checkstopwatch"></param>
    void ProcessUnits(bool checkstopwatch)
    {
        // remove any invalid units
        FogUnit.RegisteredUnits.RemoveAll(u => u == null);

        double millisecondfrequency = 1000.0 / System.Diagnostics.Stopwatch.Frequency;
        for (; _CurrentUnitProcessing < FogUnit.RegisteredUnits.Count; ++_CurrentUnitProcessing)
        {
            if (!FogUnit.RegisteredUnits[_CurrentUnitProcessing].isActiveAndEnabled )
                continue;

            FogShape shape = FogUnit.RegisteredUnits[_CurrentUnitProcessing].GetShape(this, Plane);
            if (MultiThreaded && UpdateAutomatically)
                _ThreadPool.Run(() => _Drawer.Draw(shape));
            else
                _Drawer.Draw(shape);

            // do the timer check here so that at least one unit will be processed!
            if (checkstopwatch && _StopWatch.ElapsedTicks * millisecondfrequency >= MaxMillisecondsPerFrame)
            {
                ++_CurrentUnitProcessing;
                break;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Forces the fog to update.
    /// </summary>
    public void ManualUpdate()
    {
        ManualUpdate(1);
    }

    ////////////////////////////////////////////////////////////////////////////////

    public void ManualUpdate(float timesincelastupdate)
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Cannot do Manual Update when not playing!", this);
            return;
        }

        if (UpdateAutomatically)
        {
            Debug.LogWarning("Cannot do Manual Update when updateAutomatically is true!", this);
            return;
        }

        ProcessUnits(false);
        CompileFinalTexture(ref timesincelastupdate, false);
    }

    ////////////////////////////////////////////////////////////////////////////////

    void Update()
    {
        if (!UpdateAutomatically)
            return;

        // prepare threads
        if (MultiThreaded)
        {
            if (_ThreadPool == null)
                _ThreadPool = new FogPool();
            Threads = Mathf.Clamp(Threads, 2, 8);
            _ThreadPool.MaxThreads = Threads;
            _ThreadPool.Clean();
        }
        else if (_ThreadPool != null)
        {
            _ThreadPool.StopAllThreads();
            _ThreadPool = null;
        }

        _StopWatch.Reset();
        _StopWatch.Start();

        // draw shapes
        ProcessUnits(true);

        // compile final texture
        _TimeSinceLastUpdate += Time.deltaTime;
        CompileFinalTexture(ref _TimeSinceLastUpdate, true);

        _StopWatch.Stop();
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Compiles final texture after all shapes and geometry are checked.
    /// </summary>
    /// <param name="timesincelastupdate"></param>
    /// <param name="checkstopwatch"></param>
    void CompileFinalTexture(ref float timesincelastupdate, bool checkstopwatch)
    {
        if (+_CurrentUnitProcessing >= FogUnit.RegisteredUnits.Count && (!checkstopwatch || !MultiThreaded || _ThreadPool.HasAllFinished))
        {
            _Drawer.GetValues(_FogValuesCopy);
            _CurrentUnitProcessing = 0;

            // prepare texture
            if (FogTexture == null)
            {
                FogTexture = new Texture2D(MapResolution.x, MapResolution.y, TextureFormat.Alpha8, false);
                FogTexture.wrapMode = TextureWrapMode.Clamp;
                FogTexture.filterMode = FilterMode;
            }
            else if (FogTexture.width != MapResolution.x || FogTexture.height != MapResolution.y)
                FogTexture.Resize(MapResolution.x, MapResolution.y, TextureFormat.Alpha8, false);
            else
                FogTexture.filterMode = FilterMode;
            FogTexture.LoadRawTextureData(_FogValuesCopy);
            FogTexture.Apply();

            // apply blur
            _FinalFogTexture = _Blur.Apply(FogTexture, MapResolution, BlurAmount, BlurIterations, BlurType);

            // fade in fog
            _FadeAmount += FadeSpeed * timesincelastupdate;
            byte fadebytes = (byte)(_FadeAmount * 255);
            if (fadebytes > 0)
            {
                _Drawer.Fade((byte)(PartialFogAmount * 255), fadebytes);
                _FadeAmount -= fadebytes / 255.0f;
            }

            timesincelastupdate = 0;

            if (!_IsFirstProcessingFrame)
                ProcessUnits(checkstopwatch);
            _IsFirstProcessingFrame = true;
        }
        else
            _IsFirstProcessingFrame = false;
    }

    ////////////////////////////////////////////////////////////////////////////////

    // Returns the corner points relative to the camera's position (not rotation)
    static Matrix4x4 CalculateCameraFrustumCorners(Camera cam, Transform camtransform)
    {
        // Most of this was copied from the GlobalFog image effect standard asset due to the weird way to reconstruct the world position
        Matrix4x4 frustumCorners = Matrix4x4.identity;
        float camAspect = cam.aspect;
        float camNear = cam.nearClipPlane;
        float camFar = cam.farClipPlane;

        if (cam.orthographic)
        {
            float orthoSize = cam.orthographicSize;

            Vector3 far = camtransform.forward * camFar;
            Vector3 rightOffset = camtransform.right * (orthoSize * camAspect);
            Vector3 topOffset = camtransform.up * orthoSize;

            frustumCorners.SetRow(0, far + topOffset - rightOffset);
            frustumCorners.SetRow(1, far + topOffset + rightOffset);
            frustumCorners.SetRow(2, far - topOffset + rightOffset);
            frustumCorners.SetRow(3, far - topOffset - rightOffset);
        }
        else // perspective
        {
            float fovWHalf = cam.fieldOfView * 0.5f;
            float fovWHalfTan = Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            Vector3 toRight = camtransform.right * (camNear * fovWHalfTan * camAspect);
            Vector3 toTop = camtransform.up * (camNear * fovWHalfTan);

            Vector3 topLeft = (camtransform.forward * camNear - toRight + toTop);
            float camScale = topLeft.magnitude * camFar / camNear;

            topLeft.Normalize();
            topLeft *= camScale;

            Vector3 topRight = (camtransform.forward * camNear + toRight + toTop);
            topRight.Normalize();
            topRight *= camScale;

            Vector3 bottomRight = (camtransform.forward * camNear + toRight - toTop);
            bottomRight.Normalize();
            bottomRight *= camScale;

            Vector3 bottomLeft = (camtransform.forward * camNear - toRight - toTop);
            bottomLeft.Normalize();
            bottomLeft *= camScale;

            frustumCorners.SetRow(0, topLeft);
            frustumCorners.SetRow(1, topRight);
            frustumCorners.SetRow(2, bottomRight);
            frustumCorners.SetRow(3, bottomLeft);
        }
        return frustumCorners;
    }

    ////////////////////////////////////////////////////////////////////////////////

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderFog(source, destination, _Camera, _Transform);
    }

    ////////////////////////////////////////////////////////////////////////////////

    public void RenderFog(RenderTexture source, RenderTexture destination, Camera cam, Transform camtransform)
    {

            RenderFogFull(source, destination, cam, camtransform);
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Sets shader of fog material.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="cam"></param>
    /// <param name="camtransform"></param>
    void RenderFogFull(RenderTexture source, RenderTexture destination, Camera cam, Transform camtransform)
    {
        _Material.SetTexture(_Ids.FogTex, _FinalFogTexture);
        _Material.SetVector(_Ids.FogTextureSize, MapResolution.vector2);
        _Material.SetFloat(_Ids.MapSize, MapSize);
        _Material.SetVector(_Ids.MapOffset, MapOffset);
        _Material.SetColor(_Ids.FogColor, FogColor);
        _Material.SetMatrix(_Ids.FrustumCornersWS, CalculateCameraFrustumCorners(cam, camtransform));
        _Material.SetVector(_Ids.CameraWS, camtransform.position);
        _Material.SetFloat(_Ids.OutsideFogStrength, 1.0f);

        Vector4 camdir = camtransform.forward;
        camdir.w = cam.nearClipPlane;
        _Material.SetVector(_Ids.CameraDir, camdir);

        // orthographic is treated very differently in the shader, so we have to make sure it executes the right code
        _Material.SetKeywordEnabled("CAMERA_PERSPECTIVE", !cam.orthographic);
        _Material.SetKeywordEnabled("CAMERA_ORTHOGRAPHIC", cam.orthographic);

        // which plane will the fog be rendered to?
        _Material.SetKeywordEnabled("PLANE_XY", Plane == FogOfWarPlane.XY);
        _Material.SetKeywordEnabled("PLANE_YZ", Plane == FogOfWarPlane.YZ);
        _Material.SetKeywordEnabled("PLANE_XZ", Plane == FogOfWarPlane.XZ);

        _Material.SetKeywordEnabled("TEXTUREFOG", FogColorTexture != null);
        if (FogColorTexture != null)
        {
            _Material.SetTexture(_Ids.FogColorTex, FogColorTexture);
            _Material.SetVector(_Ids.FogColorTexScale, new Vector2(FogColorTextureScale, FogColorTextureHeight));
        }

        _Material.SetKeywordEnabled("FOGFARPLANE", FogFarPlane);
        _Material.SetKeywordEnabled("CLEARFOG", false);

        CustomGraphicsBlit(source, destination, _Material);
    }

    ////////////////////////////////////////////////////////////////////////////////

    void RenderClearFog(RenderTexture source, RenderTexture destination)
    {
        // create skybox camera
        Camera skyboxcamera = new GameObject("TempSkyboxFogCamera").AddComponent<Camera>();
        skyboxcamera.transform.parent = transform;
        skyboxcamera.transform.position = transform.position;
        skyboxcamera.transform.rotation = transform.rotation;
        skyboxcamera.fieldOfView = _Camera.fieldOfView;
        skyboxcamera.clearFlags = CameraClearFlags.Skybox;
        skyboxcamera.targetTexture = new RenderTexture(source.width, source.height, source.depth);
       // skyboxcamera.cullingMask = ClearFogMask;
        skyboxcamera.orthographic = _Camera.orthographic;
        skyboxcamera.orthographicSize = _Camera.orthographicSize;
        skyboxcamera.rect = _Camera.rect;

        // render skyboxcamera to texture
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = skyboxcamera.targetTexture;
        skyboxcamera.Render();
        Texture2D skyboximage = new Texture2D(skyboxcamera.targetTexture.width, skyboxcamera.targetTexture.height);
        skyboximage.ReadPixels(new Rect(0, 0, skyboxcamera.targetTexture.width, skyboxcamera.targetTexture.height), 0, 0);
        skyboximage.Apply();
        RenderTexture.active = currentRT;

        // overlay renders on eachother
        RenderTexture.active = destination;
        Material clearfogmat = new Material(ClearFogShader);
        clearfogmat.SetTexture(_Ids.SkyboxTex, skyboximage);
        CustomGraphicsBlit(source, destination, clearfogmat);

        // ensure temp objects are destroyed
        Destroy(skyboxcamera.targetTexture);
        Destroy(skyboxcamera.gameObject);
        Destroy(clearfogmat);
        Destroy(skyboximage);
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Send texture information to openGL.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="material"></param>
    static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material material)
    {
        RenderTexture.active = dest;
        material.SetTexture(_Ids.MainTex, source);
        material.SetPass(0);

        GL.PushMatrix();
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

        GL.End();
        GL.PopMatrix();
    }

    public static FogManager GetFogManager()
    {
        return Instances[0];
    }


    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Draw map area bounds.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 offset = FogConversion.FogPlaneToWorld(MapOffset.x, MapOffset.y, 0, Plane);
        Vector3 size = FogConversion.FogPlaneToWorld(MapSize, MapSize, 0, Plane);
        Gizmos.DrawWireCube(offset, size);

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawCube(offset, size);
    }

    ////////////////////////////////////////////////////////////////////////////////
}

