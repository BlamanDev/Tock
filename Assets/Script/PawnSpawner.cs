using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PawnSpawner : NetworkBehaviour {
    public GameObject PawnPrefab;
    private TockPlayer[] playerList;
    public Text text;
    // Use this for initialization
    void Start () {

        playerList = FindObjectsOfType<TockPlayer>();

        foreach (TockPlayer plop in playerList)
        {
                CmdPopulatePawns(plop.PlayerColor);

        }

    }

    // Update is called once per frame
    void Update () {
		
	}

    

    private Pawn CreatePawn(PlayerColorEnum player, int pawnIndex)
    {
        GameObject newPawn = Instantiate(PawnPrefab);
        Pawn retour = newPawn.GetComponent<Pawn>();
        retour.Initialise(player, pawnIndex);
        NetworkServer.Spawn(newPawn);
        return retour;
    }

    [Command]
    public void CmdPopulatePawns(PlayerColorEnum player)
    {
        text = FindObjectOfType<Text>();
        text.text += "Populating " + player.ToString() + " Pawn : ";
        for (int i = 0; i < 4; i++)
        {
            text.text += CreatePawn(player, i).Player.ToString() +" ";
        }
    }

}
