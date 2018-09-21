using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsReel : MonoBehaviour {
    [Tooltip("The Image of the credits")]
    public GameObject creditsImage;

    [Tooltip("Multiply the speed of the reel by this much")]
    public float speed;
    
	void Update () {

        // Move Credits object up in world space by (Deltatime x speed) that is set in inspector
        creditsImage.transform.Translate(0, Time.deltaTime*10*speed, 0, Space.World);
    }
}
