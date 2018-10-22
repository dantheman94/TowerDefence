using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreZapper : MonoBehaviour {

    public static bool EnableZapper = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerEnter(Collider other)
    {
        if (EnableZapper == true)
        {
            if (other.GetComponentInParent<Unit>() != null && other.GetComponentInParent<Unit>().Team == GameManager.Team.Attacking)
            {
                other.GetComponentInParent<Unit>().Damage(100000000, null);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(EnableZapper == true)
        {
            if (other.GetComponentInParent<Unit>() != null && other.GetComponentInParent<Unit>().Team == GameManager.Team.Attacking)
            {
                other.GetComponentInParent<Unit>().Damage(100000000, null);
            }
        }
 
    }

    private void OnTriggerExit(Collider other)
    {
        if (EnableZapper == true)
        {
            if (other.GetComponentInParent<Unit>() != null && other.GetComponentInParent<Unit>().Team == GameManager.Team.Attacking)
            {
                other.GetComponentInParent<Unit>().Damage(100000000, null);
            }
        }
    }
}
