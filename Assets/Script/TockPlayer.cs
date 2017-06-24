using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;

public class TockPlayer : NetworkBehaviour
{
    [SyncVar]
    public String PlayerName = "Player";
    public List<Pawn> Pawns;
    [SyncVar]
    public PlayerColorEnum PlayerColor;

    public GameObject PawnPrefab;
    private GameObject goGMaster;
    private GameMaster gMaster;
    private GameObject goBoard;
    private TockBoard board;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        String blop = PlayerColor.ToString();
        this.tag = blop + "_Player";
        goBoard = GameObject.Find("toc");
        board = goBoard.GetComponent<TockBoard>();
        if (isServer)
        {
            //gMaster.CmdPopulatePawns(this.PlayerColor);

        }
    }

    [Command]
    private void CmdPopulatePawns()
    {
        for (int i = 0; i < 4; i++)
        {
            Pawns.Add(CreatePawn(i));
        }
    }


    private Pawn CreatePawn(int pawnIndex)
    {
        GameObject newPawn = Instantiate(PawnPrefab);
        Pawn retour = newPawn.GetComponent<Pawn>();
        retour.Initialise(PlayerColor,pawnIndex);
        NetworkServer.Spawn(newPawn);
        return retour;
    }

    private bool hasWin()
    {
        int inHouse = 0;
        foreach (Pawn pawnSelected in Pawns)
        {
            if (pawnSelected.Progress > board.NB_CASES) inHouse++;
        }
        return inHouse == 4;
    }
    [Command]
    public void CmdMovePawn(int pawnIndex)
    {
        this.Pawns[pawnIndex].Move();
    }

    public void CmdEnterPawn(int pawnIndex)
    {
        this.Pawns[pawnIndex].Enter();
    }
}
