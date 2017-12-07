using Assets.Script;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnPositions : NetworkBehaviour {
    //Contains the 
    public Dictionary<string, GameObject> Positions;
	// Use this for initialization
    /// <summary>
    /// Fill the Positions dictionnary with all the NetworkStartPosition
    /// </summary>
	void Start () {
        Positions = new Dictionary<string, GameObject>();
        NetworkStartPosition[] positions = GameObject.FindObjectsOfType<NetworkStartPosition>();
        foreach (NetworkStartPosition item in positions )
        {
            Positions[item.name.Split('_')[0]] = item.gameObject;
        } 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Return a GameObject with the start position corresponding to the given color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public GameObject getStartPosition(PlayerColorEnum color)
    {
        return Positions[color.ToString()];
    }

    public GameObject getStartPosition(int playerIndex)
    {
        return Positions[playerIndex.ToString()];
    }

    /// <summary>
    /// Return a GameObject with the out position corresponding to the given color and pawnIndex
    /// </summary>
    /// <param name="color"></param>
    /// <param name="pawnIndex"></param>
    /// <returns></returns>
    public GameObject getOutPosition(PlayerColorEnum color, int pawnIndex)
    {
        return Positions[color.ToString()+pawnIndex.ToString()];

    }

    public GameObject getOutPosition(string name)
    {
        return Positions[name];

    }

}
