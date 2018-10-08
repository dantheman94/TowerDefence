using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceDrawer : MonoBehaviour {

    public Material LineMaterial;
    public float LineWidth;
    public float Depth = 5;

    private Vector3? LineStartPoint;
    private Camera _Camera;
    private GameObject newobject;
    private LineRenderer lineRenderer;
	// Use this for initialization
	void Start () {
        _Camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(1))
        {
            LineStartPoint = GetMouseCameraPoint();
        }
        else if(Input.GetMouseButton(1))
        {
            Debug.Log("release");
            if(!LineStartPoint.HasValue)
            {
                return;
            }
            var LineEndPoint = GetMouseCameraPoint();
            if(newobject == null)
            {
                newobject = new GameObject();
                lineRenderer = newobject.AddComponent<LineRenderer>();
            }

            if(lineRenderer != null)
            {
                lineRenderer.material = LineMaterial;
  
                lineRenderer.SetPositions(new Vector3[] { LineStartPoint.Value, LineEndPoint.Value });
                lineRenderer.startWidth = LineWidth;
                lineRenderer.endWidth = LineWidth;
            }
          

           // LineStartPoint = null;
        }
        if(Input.GetMouseButtonUp(1))
        {
            Destroy(newobject);
        }
       
	}

    private Vector3? GetMouseCameraPoint()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.origin + ray.direction * Depth;
    }
}
