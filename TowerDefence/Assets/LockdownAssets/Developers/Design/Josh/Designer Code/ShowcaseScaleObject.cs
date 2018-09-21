using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowcaseScaleObject : MonoBehaviour {

    
    [Tooltip ("The X,Y,Z Scale to set for the Asset")]
    public float scaleSize;

    public void ScaleObject() {
        // Sets the local scale of the object by what the scaleSize is set to.
        transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);

    }

    public void ResetScale()
    {
        // Return the scale back to 1,1,1
        transform.localScale = new Vector3(1, 1, 1);

    }
}
