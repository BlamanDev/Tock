using System;

using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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

    private int nbPlayableCards=0;
    private PlayerHand playerHand;
    public List<Card> liste;

    private Card cardSelected=null;
    private Pawn pawnSelected = null;
    //Color of the player
    [SyncVar]
    public PlayerColorEnum PlayerColor=PlayerColorEnum.Clear;

    private Deck gameDeck;

    //Prefab used for the Pawn
    public GameObject PawnPrefab;
    //references to the the component used by the script
    private GameMaster gMaster;
    private TockBoard board;

    public Image[] DisplayedHand;

    //Card Event 
    public delegate void OnCardDrawed(CardsColorsEnum CardColor, CardsValuesEnum CardValue);
    [SyncEvent]
    public static event OnCardDrawed EventOnCardDrawed;

    //for debugging
    private Text text;

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

    public GameMaster GMaster
    {
        get
        {
            if (gMaster == null)
            {
                GMaster = GameObject.Find("NetworkGameMaster").GetComponent<GameMaster>();
            }
            return gMaster;
        }

        set
        {
            gMaster = value;
        }
    }

    public TockBoard Board
    {
        get
        {
            if (board == null)
            {
                board = GameObject.Find("toc").GetComponent<TockBoard>();
            }

            return board;
        }

        set
        {
            board = value;
        }
    }

    public Text Text
    {
        get
        {
            if (text == null)
            {
                text = GameObject.Find("TextTockPlayer").GetComponent<Text>();
            }
            return text;
        }

        set
        {
            text = value;
        }
    }
    #endregion
    #region initialization

    /// <summary>
    /// Find the references, add tag, colorize player
    /// </summary>
    void Start()
    {

        //colorize player
        PlayerColor = GMaster.GiveNewPlayerColor();

        //add tag to the player
        String nameTag = PlayerColor.ToString();
        this.tag = nameTag + "_Player";

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (isLocalPlayer)
        {
            GMaster.localPlayer = this;
            DisplayedHand = GameObject.Find("Cards").GetComponentsInChildren<Image>();
            
            Hand.OnAdd += DisplayCard;
            Hand.OnRemove += ClearCard;
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
        Pawns = GMaster.getPawnsOfAColor(PlayerColor);
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
            if (pawnSelected.Progress > Board.NB_CASES) inHouse++;
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
        //EventOnPawnEntered(this.Pawns[pawnIndex].name);
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
    /// <summary>
    /// Test the if the cards in the Player's Hand can be played
    /// </summary>
    public void Projection()
    {
        nbPlayableCards = 0;
        foreach (Card item in playerHand)
        {
            item.MakeProjections(GMaster.getPawnsFiltered(item.ColorFilter, this.PlayerColor));
            if (item.possibleTargets.Count == 0)
            {
                DisplayedHand[playerHand.IndexOf(item)].GetComponent<Button>().enabled = false;
            }
            else
            {
                nbPlayableCards++;
                DisplayedHand[playerHand.IndexOf(item)].GetComponent<Button>().enabled = true;
            }
        }

        


    }
    #endregion
    #region Card
    #region Drawing
    private void ClearCard(object sender, EventArgs e)
    {
        HandEventArgs HEA = (HandEventArgs)e;
        for (int i = HEA.CardPosition; i < 4; i++)
        {
            DisplayedHand[i].material = DisplayedHand[i + 1].material;
        }
    }

    private void DisplayCard(object sender, EventArgs e)
    {
        HandEventArgs HEA = (HandEventArgs)e;
        DisplayedHand[HEA.CardPosition].material = HEA.Card.Illustration;
    }

    public void PickACard()
    {
        if (Hand.Count < 5)
        {
            TockPlayer.EventOnCardDrawed += RpcCardDrawed;
            CmdPickACard();
        }
    }

    [Command]
    public void CmdPickACard()
    {
        Card newCard = GameDeck.DrawACard();
        RpcCardDrawed(newCard.Color, newCard.Value);
    }


    [ClientRpc]
    public void RpcCardDrawed(CardsColorsEnum CardColor, CardsValuesEnum CardValue)
    {
        StartCoroutine(waitForCard(CardColor, CardValue));
        TockPlayer.EventOnCardDrawed -= RpcCardDrawed;
    }

    IEnumerator waitForCard(CardsColorsEnum CardColor, CardsValuesEnum CardValue)
    {

        String CardName = CardValue.ToString() + "_" + CardColor.ToString();
        {
            yield return new WaitWhile(() => GameObject.Find(CardName) == null);
        }
        Card newCard = GameObject.Find(CardName).GetComponent<Card>();
        Hand.Add(newCard);
    }
    #endregion
    #region Playing
    public void PlayCard(int cardPlayed)
    {
        if (cardPlayed < Hand.Count)
        {
            cardSelected = Hand[cardPlayed];

            if (nbPlayableCards==0)
            {
                DiscardSelectedCard();
            }
            else
            {
                Text.text = "Card Selected : " + Hand[cardPlayed].name + " : " + (int)Hand[cardPlayed].Value + " cases";   //debug
                this.SelectPawn();

            }
        }

    }

    private void DiscardSelectedCard()
    {
        if (isClient)
        {
            Destroy(cardSelected.gameObject);
        }

        Hand.Remove(cardSelected);
        cardSelected = null;
        this.PickACard();
    }

    [Command]
    public void CmdPlayCard(String cardPlayed, String pawnTarget)
    {
        Card card = GameDeck.StrToCard(cardPlayed);
        card.Play(GameObject.Find(pawnTarget).GetComponent<Pawn>());
        Text.text = "Card Played : " + card.name + " : " + (int)card.Value + " cases";   //debug

        GameDeck.CardsInDeck.Add(card);
    }
    #endregion
    #endregion
    #region selection & play pawn
    public void SelectPawn()
    {
        foreach (Pawn item in cardSelected.possibleTargets)
        {
            item.EventOnPawnSelected += PawnSelected;
            item.SwitchHalo(true, PlayerColor);
        }
        StartCoroutine(waitForPawn());
    }

    private void PawnSelected(Pawn Selection)
    {
        pawnSelected = Selection;
        foreach (Pawn item in cardSelected.possibleTargets)
        {
            item.EventOnPawnSelected -= PawnSelected;
            item.SwitchHalo(false, PlayerColor);
        }

    }

    IEnumerator waitForPawn()
    {
        {
            yield return new WaitWhile(() => pawnSelected == null);
        }
        CmdPlayCard(cardSelected.name, pawnSelected.name);
        pawnSelected = null;
        DiscardSelectedCard();

    }
    #endregion
    #region ProgressDictionnary
    [ClientRpc]
    public void RpcAddtoProgressDictionnary(string target)
    {
        int newProgress = GMaster.progressDictionnary.Add(target);
        Text.text = "Added : " + target + "- to ProgressDico at position : " + newProgress;

    }

    [ClientRpc]
    public void RpcMoveinProgressDictionnary(string target, int nbMoves)
    {
        int newProgress = GMaster.progressDictionnary.Move(target, nbMoves);
        Text.text = "Moved : " + target + "- for " + nbMoves + " cells in ProgressDico, new position : " + newProgress;
    }

    [ClientRpc]
    public void RpcRemovefromProgressDictionnary(string target)
    {
        GMaster.progressDictionnary.Remove(target);
    }
#endregion
    public void RpcBeginTurn()
    {
        Projection();
    }
}
