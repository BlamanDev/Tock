using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour {
    public Canvas NextPopup;
    public Canvas PreviousPopup;
    public Canvas GameCanvas;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Close()
    {
        this.GetComponent<Canvas>().enabled = false;
        GameCanvas.enabled = true;
    }

    public void Next()
    {
        if (NextPopup != null)
        {
            this.GetComponent<Canvas>().enabled = false;

            NextPopup.enabled = true;

        }
        else
        {
            Close();
        }
    }

    public void Previous()
    {
        if (PreviousPopup != null)
        {
            this.GetComponent<Canvas>().enabled = false;

            PreviousPopup.enabled = true;

        }
        else
        {
            Close();
        }

    }
}
