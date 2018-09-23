using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///=-=-=-=-=-=-=-=-=-=-=
/// Created By: Angus Secomb
/// Last Edited: 20/09/18
/// Editor: Angus Secomb
//=-=-=-=-=-=-=-=-=-=-=-=
public class FogMinimap : MonoBehaviour {

    //                       INSPECTOR
    /////////////////////////////////////////////////////////////////////////////////////

    public RawImage RawImage;

    //                       VARIABLES
    /////////////////////////////////////////////////////////////////////////////////////

    private FogManager _FogManager;
    private Texture2D _Texture;

    //                       FUNCTIONS
    /////////////////////////////////////////////////////////////////////////////////////

    // Use this for initialization
    void Start()
    {
        _FogManager = GetComponent<FogManager>();
        if (_FogManager.FogTexture != null)
        {
            _Texture = new Texture2D(_FogManager.FogTexture.width, _FogManager.FogTexture.height);
            _Texture.wrapMode = TextureWrapMode.Clamp;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {

        //If can't get reference then dont continue.
        if (_FogManager.FogTexture == null)
        {
            return;
        }

        if (_Texture == null)
        {
            _Texture = new Texture2D(_FogManager.FogTexture.width, _FogManager.FogTexture.height);
            _Texture.wrapMode = TextureWrapMode.Clamp;
        }

        //Get raw texture data from fog texture.
        byte[] original = _FogManager.FogTexture.GetRawTextureData();

        //Create an array of 32bit colors.
        Color32[] pixels = new Color32[original.Length];

        //Set pixels to match fogtexture data.
        for (int i = 0; i < pixels.Length; ++i)
            pixels[i] = original[i] < 255 ? new Color32(0, 0, 0, 10) : new Color32(0, 0, 0, 255);

        //Apply Pixels to texture then to the rawImage UI.
        _Texture.SetPixels32(pixels);
        _Texture.Apply();
        RawImage.texture = _Texture;
    }

    /////////////////////////////////////////////////////////////////////////////////////
}
