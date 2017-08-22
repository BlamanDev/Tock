using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Card : NetworkBehaviour
{
    public CardsColorsEnum Color;
    public CardsValuesEnum Value;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(CardsColorsEnum color, CardsValuesEnum value)
    {
        Color = color;
        Value = value;
        this.name = value.ToString() + "_" + color.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //GetComponent<Rigidbody>().useGravity = false;
    }
}
