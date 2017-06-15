using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;

public class Pawn : NetworkBehaviour
{
    public int Progress = 0;
    public PlayerColorEnum Player;
    public GameObject PawnObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialise(PlayerColorEnum color)
    {
        Player = color;
        Instantiate(PawnObject);
        PawnObject.GetComponent<>()
    }
}
