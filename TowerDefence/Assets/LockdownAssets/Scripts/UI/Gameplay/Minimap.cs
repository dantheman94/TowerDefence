using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=-=-=-=-==-=-=-=-=-=-=-
// Created by Angus Secomb
// Last Edited: 17/09/2018
// Editor: Angus Secomb
//=-=-=-=-=-=-=-=-=-=-=-=-
public class Minimap : MonoBehaviour
{

    //                            INSPECTOR
    /////////////////////////////////////////////////////////////////////////////////////
    public GameObject CameraObject;
    [Header(" MAP NODE OBJECTS ")]
    public GameObject TopLeft;
    public GameObject TopRight;
    public GameObject BottomLeft;
    public GameObject BottomRight;
    [Header(" MINI MAP BOUNDS ")]
    public Rectangle RectangleArea;
    public Rect MapArea;
    [Header(" DEBUG ")]
    [Tooltip("Determines whether map draws debug stuff.")]
    public bool debug = false;
    public Color BoundsColor = Color.yellow;
    public Texture BoundsTexture = new Texture();
    public float XOffset;
    public float YOffset;
    //                            VARIABLES
    /////////////////////////////////////////////////////////////////////////////////////

    private float _XSize;
    private float _YSize;
    private Vector2 _RectOrigin;
    private Ray ray;
    private RaycastHit hit;

    //                            FUNCTIONS
    /////////////////////////////////////////////////////////////////////////////////////

    // Use this for initialization
    void Start()
    {
  //      MapArea.x = Screen.width / 1.16f;
    //    MapArea.y = Screen.height / 1.31f;
        //On start get map information.
        GetBounds();
    }

    /////////////////////////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update()
    {

        if(TutorialScene.CurrentMessageData != null)
        {
        if(!TutorialScene.CurrentMessageData.LockCamera && !TutorialScene.CurrentMessageData.LockControls)
            {
                ClickedMinimap();
            }

        }
        else if(TutorialScene.CurrentMessageData == null)
        {
            ClickedMinimap();
        }
 
        //If debug is true run the debug functions.
        if (debug)
        {
            DebugMap();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Draw's map bounds on play and outputs the map size as a debug message.
    /// </summary>
    private void DebugMap()
    {
        Debug.Log("Map Size: (" + _XSize + ", " + _YSize + ")");
        Debug.DrawLine(TopLeft.transform.position, TopRight.transform.position, BoundsColor);
        Debug.DrawLine(TopLeft.transform.position, BottomLeft.transform.position, BoundsColor);
        Debug.DrawLine(TopRight.transform.position, BottomRight.transform.position, BoundsColor);
        Debug.DrawLine(BottomLeft.transform.position, BottomRight.transform.position, BoundsColor);
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the bounds based on the 4 map nodes.
    /// </summary>
    private void GetBounds()
    {
        _XSize = Vector3.Distance(TopLeft.transform.position, TopRight.transform.position);
        _YSize = Vector3.Distance(TopRight.transform.position, BottomRight.transform.position);
        _RectOrigin = new Vector2(MapArea.width / 2, MapArea.height / 2);
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Moves camera to relative location based on minimap click.
    /// </summary>
    private void ClickedMinimap()
    {
     
            if (Input.GetMouseButton(1))
            {
                Vector2 MousePOS = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
                //         Debug.Log("Mouse Pos: " + MousePOS);
                //         Debug.Log("Screen Dimensions: " + Screen.width + ", " + Screen.height);

                //if (MapArea.Contains(MousePOS, true))
                if (RectangleArea.PointInside(Input.mousePosition.x, Input.mousePosition.y))
                {
                    if (TopLeft.transform.position.x < 0)
                    {
                        Vector2 ActualBounds = new Vector2(MousePOS.x - RectangleArea.transform.position.x, (Screen.height - RectangleArea.GetTopLeft().y) - MousePOS.y);
                        CameraObject.transform.position = new Vector3((ActualBounds.x * (_XSize / RectangleArea.width)) + TopLeft.transform.position.x + XOffset,
                        CameraObject.transform.position.y, ActualBounds.y * (_YSize / RectangleArea.height) + TopRight.transform.position.z + YOffset);
                        //                 Debug.Log("Actual Bounds: " + ActualBounds);
                    }
                    else
                    {
                        Vector2 ActualBounds = new Vector2(MousePOS.x - MapArea.x, MapArea.y - MousePOS.y - 30);
                       CameraObject.transform.position = new Vector3((ActualBounds.x * (_XSize / MapArea.width)) - TopLeft.transform.position.x + XOffset,
                     CameraObject.transform.position.y, ActualBounds.y * (_YSize / MapArea.height) - TopRight.transform.position.z + YOffset);
                        //                Debug.Log("Actual Bounds: " + ActualBounds);
                    }
                }
            }
    //    }
    }

    /////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Draws Texture for minimap rectangle.
    /// </summary>
    private void OnGUI()
    {
        if (debug)
        {
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.DrawTexture(MapArea, BoundsTexture);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////
}
