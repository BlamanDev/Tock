using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyColorOFAnother : MonoBehaviour {

    public MeshRenderer MrGo;
    private Color MemColor;
	// Use this for initialization
	void Start () {
        MemColor = MrGo.material.color;

    }
	
	// Update is called once per frame
	void Update () {

        if(MemColor!= GetComponent<MeshRenderer>().material.color)
        {
            GetComponent<MeshRenderer>().material.color = MrGo.material.color;
            MemColor = GetComponent<MeshRenderer>().material.color;
        }
        

    }
}
