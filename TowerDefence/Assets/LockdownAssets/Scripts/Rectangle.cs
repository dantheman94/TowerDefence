using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle : MonoBehaviour {

    public float height;
    public float width;

    public Color LineColor = Color.yellow;

    private Vector2 MousePosition;

    private Vector3 TopLeft;
    private Vector3 TopRight;
    private Vector3 BottomLeft;
    private Vector3 BottomRight;
   
    //public float YMax { get; set; }

    //public float XMax { get; set; }

    //public float YMin { get; set; }
    //public float XMin { get; set; }

    //public float X { get; set; }

    //public float Height { get; set; }
    //public float Width { get; set; }
    //public Vector2 Max { get; set; }
    //public Vector2 Min { get; set; }
    //public Vector2 Center { get; set; }
    //public Vector2 Position { get; set; }
    //public float Y { get; set; }

    public Vector3 GetTopLeft() { return TopLeft; }
    public Vector3 GetTopRight() { return TopRight; }

    public Vector3 GetBottomLeft() { return BottomLeft; }

    public Vector3 GetBottomRight() { return BottomRight; }

    // Use this for initialization
    void Start () {
        TopLeft = transform.position;
        TopRight = new Vector3(GetMapWidthInPixelsScaled(), 0) + transform.position;
        BottomRight = new Vector3(GetMapWidthInPixelsScaled(), -GetMapHeightInPixelsScaled()) + transform.position;
        BottomLeft = new Vector3(0, -GetMapHeightInPixelsScaled()) + transform.position;
     
	}
	
	// Update is called once per frame
	void Update () {
        TopLeft = transform.position;
        TopRight = new Vector3(GetMapWidthInPixelsScaled(), 0) + transform.position;
        BottomRight = new Vector3(GetMapWidthInPixelsScaled(), -GetMapHeightInPixelsScaled()) + transform.position;
        BottomLeft = new Vector3(0, -GetMapHeightInPixelsScaled()) + transform.position;
        //if (Input.GetKeyDown(KeyCode.E))
        //{

        //    MousePosition.y = Screen.height - Input.mousePosition.y;
        //    MousePosition.x = Input.mousePosition.x;
        //    Debug.Log("Mouse: " + MousePosition);
        //    Debug.Log("Unfiltered Position: " + Input.mousePosition);
        //    Debug.Log("Rectangle wPosition: " + transform.position);
        //    Debug.Log("Rectangle lPosition: " + transform.localPosition);
        //    Vector3 topRight = new Vector3(GetMapWidthInPixelsScaled(), 0) + transform.position;
        //    Vector3 bottomRight = new Vector3(GetMapWidthInPixelsScaled(), -GetMapHeightInPixelsScaled()) + transform.position;
        //    Vector3 bottomLeft = new Vector3(0, -GetMapHeightInPixelsScaled()) + transform.position;

        //    Debug.Log("Top Right: " + topRight);
        //    Debug.Log("Top Left: " + transform.position);

        //    Debug.Log("Bottom Left: " + bottomLeft);
        //    Debug.Log("Bottom Right: " + bottomRight);

        //}
    }

    public void Set(float a_x, float a_y, float a_width, float a_height)
    {
        width = a_width;
        height = a_height;
    }

    public bool Contains(Vector3 point, bool allowInverse)
    {
        return true;
    }

    public bool Contains(Vector3 point)
    {
        return true;
    }

    public bool Contains(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, float x, float y)
    {
        //Break rectangle up into 4 corners (triangles).
        float A = Area(x1, y1, x2, y2, x3, y3) + Area(x1, y1, x4, y4, x3, y3);
        float A1 = Area(x, y, x1, y1, x2, y2);
        float A2 = Area(x, y, x2, y2, x3, y3);
        float A3 = Area(x, y, x3, y3, x4, y4);
        float A4 = Area(x, y, x1, y1, x4, y4);

        return (A == A1 + A2 + A3 + A4);
    }
    
    public bool PointInside(float x,float y)
    {
  
        if (x >= transform.position.x && x <= transform.position.x + width)
        {
            if (y >= BottomLeft.y && y <= BottomLeft.y + height)
            {
                return true;
            }
        }
        return false;

    }

    public float GetMapWidthInPixelsScaled()
    {
        return this.width * this.transform.lossyScale.x * 1;
    }

    public float GetMapHeightInPixelsScaled()
    {
        return this.height * this.transform.lossyScale.y * 1;
    }


    /// <summary>
    /// Calculates the area of a triangle formed by 3 points.
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="x3"></param>
    /// <param name="y3"></param>
    /// <returns></returns>
    static float Area(float x1, float y1, float x2, float y2, float x3, float y3)
    {
        return (float)Mathf.Abs((x1 * (y2 - y3) +
                                 x2 * (y3 - y1) +
                                 x3 * (y1 - y2)) / 2);
    }

    

    private void OnDrawGizmosSelected()
    {
        Vector3 pos_w = this.gameObject.transform.position;
        Vector3 topLeft = Vector3.zero + pos_w;
        Vector3 topRight = new Vector3(GetMapWidthInPixelsScaled(), 0) + pos_w;
        Vector3 bottomRight = new Vector3(GetMapWidthInPixelsScaled(), -GetMapHeightInPixelsScaled()) + pos_w;
        Vector3 bottomLeft = new Vector3(0, -GetMapHeightInPixelsScaled()) + pos_w;

        // To make gizmo visible, even when using depth-shader shaders, we decrease the z depth by the number of layers
        float depth_z = -1.0f;
        pos_w.z += depth_z;
        topLeft.z += depth_z;
        topRight.z += depth_z;
        bottomRight.z += depth_z;
        bottomLeft.z += depth_z;

        Gizmos.color = LineColor;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

    }
}
