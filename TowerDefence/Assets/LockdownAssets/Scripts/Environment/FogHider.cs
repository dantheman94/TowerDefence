using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogHider : MonoBehaviour {

    private FogManager _FogManager;
    public GameObject ActiveObject;

    // Use this for initialization
    void Start () {
        _FogManager = FogManager.GetFogManager();
    }
	
	// Update is called once per frame
	void Update ()
    {
            bool visible = !_FogManager.IsInFog(transform.position, 0.2f);

            if (visible)
            {
                ActiveObject.SetActive(true);
            }
            else
            {
                ActiveObject.SetActive(false);
            }
    }
}
