using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//-=-=-=-=-=-=-=-=-=-=-=-=-=
// Created by: Angus Secomb
// Last modified: 23/10/18
// Editor: Angus
//-=-=-=-=-=-=-=-=-=-=-=-=-=

public class VeteranUpgrade : MonoBehaviour {

    // INSPECTOR
    //////////////////////////////////////////

    public Texture BoxImage;

    public static Rect Area;
        
    public static bool IsActive = false;


    // VARIABLES
    //////////////////////////////////////////
    private int _SupplyCost;
    private int _PowerCost;
    public static List<Unit> _Units;
    public static int PowerCost;
    public static int SupplyCost;

    // FUNCTIONS
    ///////////////////////////////////////////

	// Use this for initialization
	void Start () {
        _Units = new List<Unit>();
        BoxImage = GameManager.Instance.Players[0]._KeyboardInputManager.SelectionHighlight;
        PowerCost = _PowerCost;
        SupplyCost = _SupplyCost;
        IsActive = true;
    }

    ////////////////////////////////////////////////////////////////////////

    // Update is called once per frame
    void Update () {
        Area.position = new Vector2(Input.mousePosition.x - (Area.width / 2), Screen.height - Input.mousePosition.y - (Area.height / 2));
        Debug.Log("Power: " + PowerCost);
        Debug.Log("Cost: " + SupplyCost);
        Debug.Log("Units: " + _Units.Count);
   
        if(Area != null)
        {
            for(int i = 0; i < _Units.Count; ++i)
            {
                Vector3 camPos = GameManager.Instance.Players[0].CameraAttached.WorldToScreenPoint(_Units[i].transform.position);
                if (!Area.Contains(camPos))
                {
                    _Units.RemoveAt(i);
                }
            }
           
        }
            
        if (Input.GetMouseButtonDown(0))
        {
                if(_Units.Count > 0)
                {
                    GameManager.Instance.Players[0].SuppliesCount -= _SupplyCost * _Units.Count;
                    GameManager.Instance.Players[0].PowerCount -= _PowerCost * _Units.Count;
                    _Units.Clear();
                    Area.Set(0, 0, 0, 0);
                    IsActive = false;
                    Destroy(this.gameObject);
                }          
        }

        if(Input.GetMouseButtonDown(1))
        {
            _Units.Clear();
            Area.Set(0, 0, 0, 0);
            IsActive = false;
            Destroy(this.gameObject);
        }

    }

    ////////////////////////////////////////////////////////////////////////

    private void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, 0.5f);
        GUI.DrawTexture(Area, BoxImage);
    }

    ////////////////////////////////////////////////////////////////////////

    public void SetSupplyCost(int supplycost) { _SupplyCost = supplycost; }

    ////////////////////////////////////////////////////////////////////////

    public void SetPowerCost(int powercost) { _PowerCost = powercost; }

    ////////////////////////////////////////////////////////////////////////

    public void SetArea(int height, int width) { Area.height = height;
        Area.width = width;
    }
   
    ////////////////////////////////////////////////////////////////////////
}
