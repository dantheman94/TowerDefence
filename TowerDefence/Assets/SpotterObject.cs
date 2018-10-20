using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotterObject : MonoBehaviour {

    public float SpotterLifetime = 10.0f;
    public Texture BoxImage;
    public Rect HitAreaGUI;
    

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        HitAreaGUI.position = new Vector2(Input.mousePosition.x - (HitAreaGUI.width / 2), Screen.height - Input.mousePosition.y - (HitAreaGUI.height/2));
		if(Input.GetMouseButtonDown(0))
        {
            GetComponent<FogUnit>().enabled = true;
            transform.position = GameManager.Instance.Players[0]._HUD.FindHitPoint();
            HitAreaGUI.Set(0, 0, 0, 0);
            Destroy(this.gameObject, SpotterLifetime);
        }
	}

    private void OnGUI()
    {
        GUI.color = new Color(1, 1, 1, 0.5f);
        GUI.DrawTexture(HitAreaGUI, BoxImage);
    }
}
