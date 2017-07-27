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

    
    public List<Card> Hand;

    //Color of the player
    [SyncVar]
    public PlayerColorEnum PlayerColor;

    //Prefab used for the Pawn
    public GameObject PawnPrefab;
    //references to the the component used by the script
    private GameMaster gMaster;
    private TockBoard board;

    //for debugging
    public Text text;


    /// <summary>
    /// Find the references, add tag, colorize player
    /// </summary>
    void Start()
    {
        //Find references
        FindReferences();

        //colorize player
        PlayerColor = gMaster.CmdGiveNewPlayerColor();

        //add tag to the player
        String blop = PlayerColor.ToString();
        this.tag = blop + "_Player";


    }

    private void FindReferences()
    {
        if (text == null)
        {
            text = GameObject.Find("TextTockPlayer").GetComponent<Text>();
        }

        if (gMaster == null)
        {
            gMaster = GameObject.Find("NetworkGameMaster").GetComponent<GameMaster>();
        }
        if (board == null)
        {
            board = GameObject.Find("toc").GetComponent<TockBoard>();
        }
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        FindReferences();
        if (isLocalPlayer)
        {
            gMaster.localPlayer = this;
        }
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
            this.Pawns[pawnIndex].Move(nbMoves);
    }

    /// <summary>
    /// Command to make a pawn enter the board
    /// </summary>
    /// <param name="pawnIndex"></param>
    [Command]
    public void CmdEnterPawn(int pawnIndex)
    {
        this.Pawns[pawnIndex].Enter();
    }

    [Command]
    public void CmdMoveOtherColor(String otherPlayer, int PawnIndex, int nbMoves)
    {
        TockPlayer otherTockPlayer = GameObject.FindGameObjectWithTag(otherPlayer + "_Player").GetComponent<TockPlayer>();
        otherTockPlayer.CmdMovePawn(PawnIndex, nbMoves);

    }

    public IEnumerator<List<Pawn>> Projection(int nbCells)
    {
        List<Pawn> PlayablePawns = new List<Pawn>();
        foreach (Pawn item in Pawns)
        {
            item.MakeProjection(nbCells);
            while (item.Status == PawnTestedEnum.UNTESTED)
            {
                yield return null;
            }
            if (item.Status == PawnTestedEnum.CAN_MOVE)
            {
                PlayablePawns.Add(item);
            }
            item.Status = PawnTestedEnum.UNTESTED;
        }
        yield return PlayablePawns;
    }
}
