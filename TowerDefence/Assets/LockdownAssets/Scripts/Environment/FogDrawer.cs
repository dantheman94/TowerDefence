using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// -=-=-=-=-=-=-=-=-
/// Created By: Angus Secomb
/// Last Edited: 19/09/18
/// Editor: Angus
/// =-=-=-=-=-=-=-=-=

[System.Serializable]
public abstract class FogDrawer {

    protected FogMap _Map;

    public virtual void Initialise(FogMap map)
    {
        _Map = map;
        OnInitialise();
    }

    protected virtual void OnInitialise() { }
    public abstract void Clear(byte value);
    public abstract void Fade(int to, int amount);
    public abstract void GetValues(byte[] outvalues);
    public abstract void SetValues(byte[] values);

    protected abstract void DrawCircle(FogCircle circle);
    protected abstract void DrawTexture(FogShapeTexture texture);
    public abstract void Unfog(Rect rect);

    public void Draw(FogShape shape)
    { 
        if(shape is FogCircle)
        {
            DrawCircle(shape as FogCircle);
        }
        else if(shape is FogShapeTexture)
        {
            DrawTexture(shape as FogShapeTexture);
        }
     }
    
}
