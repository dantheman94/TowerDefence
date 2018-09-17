using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyGameObject : MonoBehaviour {

    private GameObject other;
    
    public int speed = 50;

	protected void Start () {
    }
	
	protected void Update () {

        // Move upwards
        if (Input.GetKey(KeyCode.W) == true)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) == true)
        {
            transform.position += transform.forward * -speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) == true)
        {
            transform.position += transform.right * -speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) == true)
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.P) == true)
        {
            SoundManager.Instance.PlaySound("Audio/pfb_Battle", 0.9f, 1.1f);
        }

    }
}
