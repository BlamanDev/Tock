using System;

using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Player Script
/// </summary>
public class TockPlayer : NetworkBehaviour
{
    #region properties
    //Player name
    [SyncVar]
    public String PlayerName = "Player";
    //List of the pawns owned by the player
    public List<Pawn> Pawns;


    private PlayerHand playerHand;
    public List<Card> liste;

    //Color of the player
    [SyncVar]
    public PlayerColorEnum PlayerColor;

    private Deck gameDeck;

    //Prefab used for the Pawn
    public GameObject PawnPrefab;
    //references to the the component used by the script
    private GameMaster gMaster;
    private TockBoard board;

    public Image[] DisplayedHand;

    public delegate void OnCardDrawed(CardsColorsEnum CardColor, CardsValuesEnum CardValue);
    [SyncEvent]
    public static event OnCardDrawed EventOnCardDrawed;


    //for debugging
    public Text text;

    public PlayerHand Hand
    {
        get
        {
            if (playerHand == null)
            {
                playerHand = new PlayerHand();
            }
            return playerHand;
        }

        set
        {
            playerHand = value;
        }
    }

    public Deck GameDeck
    {
        get
        {
            if (gameDeck == null)
            {
                gameDeck = GameObject.FindObjectOfType<Deck>();

            }
            return gameDeck;
        }

        set
        {
            gameDeck = value;
        }
    }
    #endregion
    #region initialization
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
            DisplayedHand = GameObject.Find("Cards").GetComponentsInChildren<Image>();
            Hand.OnAdd += DisplayCard;
            Hand.OnRemoveAt += DiscardCard;
        }

    }
    #endregion
    #region Pawns
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

    #endregion
    #region projection
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
    #endregion
    #region Card
    private void DiscardCard(object sender, EventArgs e)
    {
        HandEventArgs HEA = (HandEventArgs)e;
        DisplayedHand[HEA.CardPosition].material = null;
    }

    private void DisplayCard(object sender, EventArgs e)
    {
        HandEventArgs HEA = (HandEventArgs)e;
        DisplayedHand[HEA.CardPosition].material = HEA.Card.Illustration;
    }

    public void PickACard()
    {
        TockPlayer.EventOnCardDrawed += RpcCardDrawed;
        CmdPickACard();
    }

    [Command]
    public void CmdPickACard()
    {
        //Hand.PickACard();
        if (Hand.Count < 5)
        {
            //Deck.EventOnCardDrawed += RpcCardDrawed;
            Card newCard = GameDeck.DrawACard();
            //EventOnCardDrawed(newCard.Color, newCard.Value);
            RpcCardDrawed(newCard.Color, newCard.Value);
        }
        else
        {
            liste.AddRange(Hand);

        }


    }

    [Command]
    public void CmdPlayCard(int cardPlayed, int pawnTarget)
    {
        if (cardPlayed < Hand.Count)
        {
            Hand[cardPlayed].Move(Pawns[pawnTarget]);
            Hand.RemoveAt(cardPlayed);

        }
    }

    [ClientRpc]
    public void RpcCardDrawed(CardsColorsEnum CardColor, CardsValuesEnum CardValue)
    {

        StartCoroutine(waitForCard(CardColor, CardValue));
        TockPlayer.EventOnCardDrawed -= RpcCardDrawed;
    }

    /*
    private IEnumerable<Card> waitForCard(CardsColorsEnum CardColor, CardsValuesEnum CardValue)
    {
        String CardName = CardValue.ToString() + "_" + CardColor.ToString();
        //List<Card> listReturned = new List<Card>();
        Card newCard = GameObject.Find(CardName).GetComponent<Card>();
        newCard = GameObject.Find(CardName).GetComponent<Card>();
        yield return new WaitWhile(() => newCard = GameObject.Find(CardName).GetComponent<Card>() );

        yield return newCard;
    }
    */
    IEnumerator waitForCard(CardsColorsEnum CardColor, CardsValuesEnum CardValue)
    {
        String CardName = CardValue.ToString() + "_" + CardColor.ToString();
        {
            yield return new WaitWhile(() => GameObject.Find(CardName) == null);
        }
        Hand.Add(GameObject.Find(CardName).GetComponent<Card>());

        
    }
    #endregion
}
