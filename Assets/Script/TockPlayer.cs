using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    public Text text;


    // Use this for initialization
    void Start()
    {
        text = GameObject.Find("TextTockPlayer").GetComponent<Text>();
        goGMaster = GameObject.Find("NetworkGameMaster");
        gMaster = goGMaster.GetComponent<GameMaster>();
        PlayerColor = gMaster.CmdGiveNewPlayerColor();
        String blop = PlayerColor.ToString();
        this.tag = blop + "_Player";
        goBoard = GameObject.Find("toc");
        board = goBoard.GetComponent<TockBoard>();

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
    }

    [ClientRpc]
    public void RpcBuildPawnList()
    {
        Pawns=gMaster.getPawnOfAColor(PlayerColor);
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
