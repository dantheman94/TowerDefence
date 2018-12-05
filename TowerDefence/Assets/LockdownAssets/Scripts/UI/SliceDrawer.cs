using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-=-=-=-=-=-=-=-=-=-=-=-=
//Created by Angus Secomb
//Last Modified:1 07/11/2018
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
    private xb_gamepad gamepad;
    private float timer = 2;

    //  FUNCTIONS
    ///////////////////////////////////////////////////////////////////
	// Use this for initialization
	void Start () {
        _Camera = GetComponent<Camera>();
        gamepad = GamepadManager.Instance.GetGamepad(1);
	}

    ///////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {
        timer += Time.deltaTime * 100;

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
            }
            if (Input.GetMouseButtonUp(1))
            {

                Destroy(newobject);
                MouseEndPoint = hud.FindHitPoint();
                EndGO = hud.FindHitObject();
            }
    
                if(GameManager.Instance.Players[0]._XboxGamepadInputManager.GetButtonXClicked())
                {
                _LineStartPoint = GetCentrePoint();
                LineEndPoint = GetCentrePoint();
                }
                else if (gamepad.GetButton("X"))
                {
                
                    if (newobject == null)
                    {
                        newobject = new GameObject();
                        lineRenderer = newobject.AddComponent<LineRenderer>();
                     
                    }
                lineRenderer.material = LineMaterial;
            _LineStartPoint = GetCentrePoint((Screen.width / 2) - timer, Screen.height / 2);
            LineEndPoint = GetCentrePoint((Screen.width / 2) +timer, Screen.height / 2);
            lineRenderer.SetPositions(new Vector3[] { _LineStartPoint, LineEndPoint });
                lineRenderer.startWidth = LineWidth;
                lineRenderer.endWidth = LineWidth;
            }

                if(GameManager.Instance.Players[0]._XboxGamepadInputManager.GetButtonXReleased())
                {
            timer = 2;
                    Destroy(newobject);
                }
           
  
       
	}

    ///////////////////////////////////////////////////////////////////

    private Vector3 GetMouseCameraPoint()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return ray.origin + ray.direction * Depth;
    }

    private Vector3 GetCentrePoint()
    {
        var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
        return ray.origin + ray.direction * Depth;
    }

    private Vector3 GetCentrePoint(float x, float y)
    {
        var ray = Camera.main.ScreenPointToRay(new Vector3(x, y));
        return ray.origin + ray.direction * Depth;
    }

    private void OnDisable()
    {
        if(lineRenderer != null)
        Destroy(lineRenderer.gameObject);
    }

    ///////////////////////////////////////////////////////////////////
}
