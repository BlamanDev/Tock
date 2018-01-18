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
    /// Return a GameObject with the start position corresponding to the given player Index
    /// </summary>
    /// <param name="playerIndex">int - index of player</param>
    /// <returns>GameObject corresponding to a player position</returns>
    public GameObject getStartPosition(int playerIndex)
    {
        return Positions[playerIndex.ToString()];
    }

    /// <summary>
    /// Return a GameObject with the out position corresponding to the given name
    /// </summary>
    /// <param name="name">name of the pawn</param>
    /// <returns></returns>
    public GameObject GetOutPosition(string name)
    {
        return Positions[name];

    }

}
