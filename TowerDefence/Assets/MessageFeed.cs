using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageFeed : MonoBehaviour {

    public Text MessageText;
    public GameObject MessageObject;
    private float _DisplayTime = 4.0f;
    private bool MessageIsActive = false;

	// Use this for initialization
	void Start () {
        MessageObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(MessageObject.activeInHierarchy)
        {
            _DisplayTime -= Time.deltaTime;
        }
        if(_DisplayTime < 0)
        {
            _DisplayTime = 4.0f;
            MessageObject.SetActive(false);
        }
	}

    public void DisplayCoreDamaged()
    {
        MessageText.text = "The core is under attack!";
        MessageObject.SetActive(true);

    }

    public void DisplaySpireDamaged(string SpireNumber)
    {
        MessageText.text = "Spire " + SpireNumber + " is under attack.";
        MessageObject.SetActive(true);
    }

    public void DisplayTownHallUpgraded()
    {
        MessageText.text = "Your town hall has upgraded to level " + GameManager.Instance.Players[0].Level;
        MessageObject.SetActive(true);
    }

    public void DisplayBaseCaptured()
    {
        MessageText.text = "You have secured an enemies base.";
        MessageObject.SetActive(true);
    }
}
