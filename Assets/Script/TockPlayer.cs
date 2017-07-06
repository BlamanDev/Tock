using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Player Script
/// </summary>
public class TockPlayer : NetworkBehaviour
{
    //Player name
    [SyncVar]
    public String PlayerName = "Player";
    //List of the pawns owned by the player
    public List<Pawn> Pawns;

    //Color of the player
    [SyncVar]
    public PlayerColorEnum PlayerColor;

    //Prefab used for the Pawn
    public GameObject PawnPrefab;
    //references to the the component used by the script
    private GameObject goGMaster;
    private GameMaster gMaster;
    private GameObject goBoard;
    private TockBoard board;

    //for debugging
    public Text text;


    /// <summary>
    /// Find the references, add tag, colorize player
    /// </summary>
    void Start()
    {
        //Find references
        text = GameObject.Find("TextTockPlayer").GetComponent<Text>();
        goGMaster = GameObject.Find("NetworkGameMaster");
        gMaster = goGMaster.GetComponent<GameMaster>();
        goBoard = GameObject.Find("toc");
        board = goBoard.GetComponent<TockBoard>();

        //colorize player
        PlayerColor = gMaster.CmdGiveNewPlayerColor();

        //add tag to the player
        String blop = PlayerColor.ToString();
        this.tag = blop + "_Player";


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


    private static int ComparePawnsByPawnIndex(Pawn x, Pawn y)
    {
        if (x == null)
        {
            if (y == null)
            {
                // If x is null and y is null, they're
                // equal. 
                return 0;
            }
            else
            {
                // If x is null and y is not null, y
                // is greater. 
                return -1;
            }
        }
        else
        {
            // If x is not null...
            //
            if (y == null)
            // ...and y is null, x is greater.
            {
                return 1;
            }
            else
            {
                // ...and y is not null, compare the 
                // lengths of the two strings.
                //
                return x.PawnIndex.CompareTo(y.PawnIndex);
            }
        }
    }

    /// <summary>
    /// Get the list of pawns owend by the player from the server
    /// </summary>
    [ClientRpc]
    public void RpcBuildPawnList()
    {
        Pawns = gMaster.getPawnOfAColor(PlayerColor);
        Pawns.Sort(ComparePawnsByPawnIndex);
    }

    /// <summary>
    /// Test if all the pawns of the player are in home (finish line)
    /// </summary>
    /// <returns></returns>
    private bool hasWin()
    {
        int inHouse = 0;
        foreach (Pawn pawnSelected in Pawns)
        {
            if (pawnSelected.Progress > board.NB_CASES) inHouse++;
        }
        return inHouse == 4;
    }

    /// <summary>
    /// Command to move a Pawn
    /// </summary>
    /// <param name="pawnIndex"></param>
    [Command]
    public void CmdMovePawn(int pawnIndex, int nbMoves)
    {
        for (int i = 0; i<nbMoves;i++)
        {
            this.Pawns[pawnIndex].Move();

        }
    }

    /// <summary>
    /// Command to make a pawn enter the board
    /// </summary>
    /// <param name="pawnIndex"></param>
    public void CmdEnterPawn(int pawnIndex)
    {
        this.Pawns[pawnIndex].Enter();
    }
}
