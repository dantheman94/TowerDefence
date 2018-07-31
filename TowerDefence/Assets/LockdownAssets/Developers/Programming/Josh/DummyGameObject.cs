using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyGameObject : WorldObject {

    public GameObject other;

    public Rigidbody rb;

    public WorldObject wo;

    public int speed = 500;

	protected override void Start () {

        rb = GetComponent<Rigidbody>();

        wo.SetHitPoints(100);

    }
	
	protected override void Update () {
        base.Update();

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
    }
}
