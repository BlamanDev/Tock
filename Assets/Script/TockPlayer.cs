using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;

public class TockPlayer : NetworkBehaviour
{
    public String PlayerName="Player";
    public List<Pawn> Pawns;
    [SyncVar]
    public PlayerColorEnum PlayerColor;

    private GameObject goGMaster;
    private GameMaster gMaster;

    private Type typeOfPawns;

    // Use this for initialization
    void Start ()
	{
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

    }

    

    public override void OnStartServer()
    {
        base.OnStartServer();
        goGMaster = GameObject.Find("NetworkGameMaster");
        gMaster = goGMaster.GetComponent<GameMaster>();
        PlayerColor = gMaster.CmdGiveNewPlayerColor();
    }


    private void populatePawns(int nbPawns=4)
    {
        for (int i = 0;  i < nbPawns ;i++)
        {
            Pawns.Add(createPawn());
        }
    }

    private Pawn createPawn()
    {
        Pawn retour=null;
        switch (PlayerColor)
        {
               case PlayerColorEnum.Blue:
                retour = new BluePawn();
                break;
                case PlayerColorEnum.Green:
                retour = new GreenPawn();
                break;
                case PlayerColorEnum.Red:
                retour= new RedPawn();
                break;
                case PlayerColorEnum.Yellow:
                retour = new YellowPawn();
                break;
          
        }

        return retour;
    }

    private bool hasWin()
    {
        int inHouse = 0;
        foreach (Pawn pawnSelected in Pawns)
        {
            if (pawnSelected.Progress > 70) inHouse++;
        }
        return inHouse == 4;
    }
}
