using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-=-=-=-=-=-=-=-=-=-=-=-=
//Created by Angus Secomb
//Last Modified:1 0/9/2018
//Editor: Angus
//-=-=-=-=-=-=-=-=-=-=-=-=

public class SliceDrawer : MonoBehaviour {

    // INSPECTOR
    ///////////////////////////////////////////////////////////////////S

    public Material LineMaterial;
    public float LineWidth;
    public float Depth = 5;

    [HideInInspector]
    public Vector3 MouseEndPoint;
    [HideInInspector]
    public Vector3 LineStartPoint;
    [HideInInspector]
    public GameObject StartGO;
    [HideInInspector]
    public GameObject EndGO;

    public HUD hud;
    // VARIABLES
    ///////////////////////////////////////////////////////////////////
    private Vector3 _LineStartPoint;
    private Vector3 LineEndPoint;
    private Camera _Camera;
    private GameObject newobject;
    private LineRenderer lineRenderer;
    private Transform _PreviousTransform;

    //  FUNCTIONS
    ///////////////////////////////////////////////////////////////////
	// Use this for initialization
	void Start () {
        _Camera = GetComponent<Camera>();
	}

    ///////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {


       
            if (Input.GetMouseButtonDown(1))
            {
                _PreviousTransform = _Camera.transform;
                _LineStartPoint = GetMouseCameraPoint();
                StartGO = hud.FindHitObject();
                LineStartPoint = hud.FindHitPoint();
            }
            else if (Input.GetMouseButton(1))
            {

                LineEndPoint = GetMouseCameraPoint();
                if (newobject == null)
                {
                    newobject = new GameObject();
                    lineRenderer = newobject.AddComponent<LineRenderer>();
                }

                if (lineRenderer != null)
                {
                    lineRenderer.material = LineMaterial;

                    lineRenderer.SetPositions(new Vector3[] { _LineStartPoint, LineEndPoint });
                    lineRenderer.startWidth = LineWidth;
                    lineRenderer.endWidth = LineWidth;
                }


                // LineStartPoint = null;
            }
            if (Input.GetMouseButtonUp(1))
            {

                Destroy(newobject);
                MouseEndPoint = hud.FindHitPoint();
                EndGO = hud.FindHitObject();
            }
        
    
       
	}

    ///////////////////////////////////////////////////////////////////

    private Vector3 GetMouseCameraPoint()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.origin + ray.direction * Depth;
    }

    private void OnDisable()
    {
        if(lineRenderer != null)
        Destroy(lineRenderer.gameObject);
    }

    ///////////////////////////////////////////////////////////////////
}
