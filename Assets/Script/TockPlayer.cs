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
    #region attributes
    //Player name
    [SyncVar]
    public String PlayerName = "Player";
    //List of the pawns owned by the player
    public List<Pawn> Pawns;

    private PlayerHand playerHand;

    public ColorBlock playCardColors;
    public ColorBlock discardCardColors;
    #region temp attributes
    //Number of playable card in the player's hand
    private int nbPlayableCards = 0;

    private Card cardSelected = null;
    private Pawn firstPawnSelected = null;
    private Pawn pawnSelected = null;
    #endregion
    //Color of the player
    [SyncVar]
    public PlayerColorEnum PlayerColor = PlayerColorEnum.Clear;

    #region References
    private Deck gameDeck;

    //Prefab used for the Pawn
    public GameObject PawnPrefab;
    //references to the the component used by the script
    private GameMaster gMaster;
    private TockBoard board;

    public Image[] DisplayedHand;
    #endregion
    #region Seven card
    //Number of movement left for the Seven card
    private int movementLeft = 0;
    //Display the movement left for the Seven card
    private GameObject displayMovementLeft;
    #endregion
    //for debugging
    private Text text;
    #region EndGame
    private Rect windowRect;
    private String winnerName = "";
    #endregion
    #region properties
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
                gMaster = GameObject.Find("NetworkGameMaster").GetComponent<GameMaster>();
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

    public int MovementLeft
    {
        get
        {
            return movementLeft;
        }

        set
        {
            movementLeft = value;
            DisplayMovementLeft.GetComponentsInChildren<Text>()[1].text = value.ToString();
        }
    }

    public GameObject DisplayMovementLeft
    {
        get
        {
            if (displayMovementLeft == null)
            {
                displayMovementLeft = GameObject.Find("MovementLeft");
            }
            return displayMovementLeft;
        }

        set
        {
            displayMovementLeft = value;
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

    public Rect WindowRect
    {
        get
        {
            if (windowRect.height == 0)
            {
                windowRect = new Rect(((float)(Screen.width - Screen.width / 5) / 2), (float)((Screen.height - Screen.height / 5) / 2), Screen.width / 5, Screen.height / 5);

            }

            return windowRect;
        }

        set
        {
            windowRect = value;
        }
    }
    #endregion
    #endregion
    #region initialization
    /// <summary>
    /// Find the references, add tag, name player
    /// </summary>
    void Start()
    {
        //add tag to the player
        String nameTag = PlayerColor.ToString();
        this.tag = nameTag + "_Player";
        this.name = nameTag + "_Player";

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Ask for a new Color to the GameMaster and add this Player to the player list of the GameMaster
    /// </summary>
    public void Awake()
    {
        //colorize player
        PlayerColor = GameMaster.GiveNewPlayerColor();

        GameMaster.players.Add(this);
    }

    /// <summary>
    /// Find the reference of the displayed hand and subscribe to the event for displaying/removing cards in hand
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (isLocalPlayer)
        {
            DisplayMovementLeft.SetActive(false);

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

    #endregion
    #region projection
    /// <summary>
    /// Test the if the cards in the Player's Hand can be played
    /// </summary>
    public void Projection()
    {
        nbPlayableCards = 0;
        //Test each card in the Player's Hand
        foreach (Card item in playerHand)
        {
            //IF card is not playable, disable the card button
            if (!item.MakeProjections(GMaster.getPawnsFiltered(item.ColorFilter, this.PlayerColor)))
            {
                DisplayedHand[playerHand.IndexOf(item)].GetComponent<Button>().enabled = false;
            }
            //ELSE enable the card button and increment nbPlayableCards
            else
            {
                nbPlayableCards++;
                DisplayedHand[playerHand.IndexOf(item)].GetComponent<Button>().enabled = true;
            }
        }
        //IF no cards are playable, enable all card button
        if (nbPlayableCards == 0)
        {
            this.switchAllCardsClick(true);
        }
    }
    #endregion
    #region Card
    #region Displaying
    /// <summary>
    /// Discard a card from Hand and re-order other cards
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClearCard(object sender, EventArgs e)
    {
        HandEventArgs HEA = (HandEventArgs)e;
        //Re-order other cards
        for (int i = HEA.CardPosition; i < 4; i++)
        {
            DisplayedHand[i].material = DisplayedHand[i + 1].material;
        }
    }

    /// <summary>
    /// Display the card drawed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisplayCard(object sender, EventArgs e)
    {
        HandEventArgs HEA = (HandEventArgs)e;
        DisplayedHand[HEA.CardPosition].material = HEA.Card.Illustration;
        DisplayedHand[HEA.CardPosition].GetComponent<Button>().enabled = false;
    }

    private void switchAllCardsClick(bool enable = false)
    {
        foreach (Card item in playerHand)
        {
            DisplayedHand[playerHand.IndexOf(item)].GetComponent<Button>().enabled = enable;
        }

    }
    #endregion
    #region Drawing
    /// <summary>
    /// Force the targetted client build his first Hand
    /// </summary>
    /// <param name="target"></param>
    [TargetRpc]
    public void TargetBuildFirstHand(NetworkConnection target)
    {
        for (int i = 0; i < 5; i++)
        {
            this.PickACard();
        }
    }

    /// <summary>
    /// Draw a new card
    /// </summary>
    public void PickACard()
    {
        if (Hand.Count < 5)
        {
            //TockPlayer.EventOnCardDrawed += RpcCardDrawed;
            CmdPickACard();
        }
    }

    /// <summary>
    /// Ask a new card to the server
    /// </summary>
    [Command]
    public void CmdPickACard()
    {
        Card newCard = GameDeck.DrawACard();
        RpcCardDrawed(newCard.Color, newCard.Value);
    }

    /// <summary>
    /// Force the client to wait for the new card
    /// </summary>
    /// <param name="CardColor"></param>
    /// <param name="CardValue"></param>
    [ClientRpc]
    public void RpcCardDrawed(CardsColorsEnum CardColor, CardsValuesEnum CardValue)
    {
        StartCoroutine(waitForCard(CardColor, CardValue));
        //TockPlayer.EventOnCardDrawed -= RpcCardDrawed;
    }

    /// <summary>
    /// Wait for the new card drawed and add it to the player's hand
    /// </summary>
    /// <param name="CardColor">CardsColorsEnum - Color of the card waited</param>
    /// <param name="CardValue">CardsValuesEnum - Value of the card waited</param>
    /// <returns></returns>
    IEnumerator waitForCard(CardsColorsEnum CardColor, CardsValuesEnum CardValue)
    {
        String CardName = CardValue.ToString() + "_" + CardColor.ToString();
        {
            //Wait for the new card to arrive
            yield return new WaitWhile(() => GameObject.Find(CardName) == null);
        }
        Card newCard = GameObject.Find(CardName).GetComponent<Card>();
        //Add the new card to the Player's hand
        Hand.Add(newCard);
    }
    #endregion
    #region Playing
    /// <summary>
    /// Begin the process of playing a card, used by the the onclick on the card
    /// </summary>
    /// <param name="cardPlayed">index in the hand of the card played</param>
    public void PlayCard(int cardPlayed)
    {
        if (cardPlayed < Hand.Count)
        {
            cardSelected = Hand[cardPlayed];
            //IF no cards are playable, discard the selected card
            if (nbPlayableCards == 0)
            {
                this.switchAllCardsClick();
                EndTurn();
            }
            else
            {
                Text.text = "Card Selected : " + Hand[cardPlayed].name + " : " + (int)Hand[cardPlayed].Value + " cases";   //debug
                if (Hand[cardPlayed].Value == CardsValuesEnum.SEVEN)
                {
                    MovementLeft = 7;
                    DisplayMovementLeft.SetActive(true);
                }
                this.SelectPawn();
            }
        }

    }

    /// <summary>
    /// Discard the Selected Card then draw a new card
    /// </summary>
    private void DiscardSelectedCard()
    {
        Hand.Remove(cardSelected);
        CmdReturnCard(cardSelected.name);
        if (isClient)
        {
            NetworkServer.UnSpawn(cardSelected.gameObject);
        }
        this.PickACard();
        Text.text = "Turn Finished for Me";



    }

    /// <summary>
    /// Return Card to the Server's Deck
    /// </summary>
    /// <param name="cardname"></param>
    [Command]
    private void CmdReturnCard(string cardname)
    {
        GameDeck.CardsInDeck.Add(GameDeck.StrToCard(cardname));
    }

    /// <summary>
    /// Ask the server to play the card Selected
    /// </summary>
    /// <param name="cardPlayed"></param>
    /// <param name="pawnTarget"></param>
    /// <param name="otherPawnTarget"></param>
    [Command]
    public void CmdPlayCard(String cardPlayed, String pawnTarget, String otherPawnTarget)
    {
        //Retrieve the card played from the Deck
        Card card = GameDeck.StrToCard(cardPlayed);
        Pawn otherTarget = null;
        //IF there is another target, find it
        if (!string.IsNullOrEmpty(otherPawnTarget))
        {
            otherTarget = GameObject.Find(otherPawnTarget).GetComponent<Pawn>();
        }
        //Find the targetted pawn and play the Card on it
        card.Play(GameObject.Find(pawnTarget).GetComponent<Pawn>(), otherTarget);
        Text.text = "Card Played : " + card.name + " : " + (int)card.Value + " cases";   //debug

    }
    #endregion
    #endregion
    #region selection & play pawn
    /// <summary>
    /// Start the selection process for the pawns identified as playable by the card Projection
    /// </summary>
    /// <param name="secondTarget">boolean - true if it is for choosing a second target (ex : Jack Card)</param>
    public void SelectPawn(bool secondTarget = false)
    {
        //Subscribe  on the Event OnClick on every pawns then turn on the halo selection
        foreach (Pawn item in cardSelected.possibleTargets)
        {
            item.EventOnPawnSelected += PawnSelection;
            item.SwitchHalo(true, PlayerColor);
        }
        //IF this the first selection
        if (!secondTarget)
        {
            StartCoroutine(waitForPawn());
        }
        //IF this the second selection process
        else
        {
            StartCoroutine(waitForSecondPawn());
        }
    }

    /// <summary>
    /// Function called by the event OnClick on the pawn, register the pawn selected and disable the selection process
    /// </summary>
    /// <param name="Selection">Pawn - Pawn Selected</param>
    private void PawnSelection(Pawn Selection)
    {
        pawnSelected = Selection;
        foreach (Pawn item in cardSelected.possibleTargets)
        {
            unSelect(item);
        }

    }

    /// <summary>
    /// Disable the selection process for one pawn 
    /// </summary>
    /// <param name="item">Pawn - pawn to unselect</param>
    private void unSelect(Pawn item)
    {
        //unsuscribe the event OnClick
        item.EventOnPawnSelected -= PawnSelection;
        //turn off the halo selection
        item.SwitchHalo(false, PlayerColor);
    }

    /// <summary>
    /// Wait for a pawn to be selected and apply card effect on the pawn selected
    /// </summary>
    /// <returns></returns>
    IEnumerator waitForPawn()
    {
        {
            yield return new WaitWhile(() => pawnSelected == null);
        }
        this.switchAllCardsClick();

        switch (cardSelected.Value)
        {
            case CardsValuesEnum.SEVEN:
                PlaySevenCard();
                break;
            case CardsValuesEnum.JACK:
                PlayJackCard();
                MovementLeft++;
                break;
            default:
                CmdPlayCard(cardSelected.name, pawnSelected.name, "");
                MovementLeft = 0;
                break;
        }
        Text.text = "Turn Finished for Me";

        StartCoroutine(waitForPawnMoved());
    }

    /// <summary>
    /// Play the Seven Card, divide 7 movement on 1 or more pawns
    /// </summary>
    private void PlaySevenCard()
    {
        //Move the selected pawn for 1 cell
        CmdPlayCard(cardSelected.name, pawnSelected.name, "");
        //Test if this pawn can still move
        if (cardSelected.MoveFiltering(pawnSelected, 1))
        {
            unSelect(pawnSelected);
        }
        MovementLeft--;

        pawnSelected = null;
        //Relaunch the selection process if there is still movement to play
        if (MovementLeft > 0)
        {
            SelectPawn();
        }
    }

    /// <summary>
    /// Play the Jack Card, switch position between two pawns
    /// </summary>
    private void PlayJackCard()
    {
        firstPawnSelected = pawnSelected;
        //Compute the selection of pawns to be switched
        cardSelected.MakeProjections(GMaster.getPawnsFiltered(SelectionFilterEnum.ALLPAWNS, firstPawnSelected.PlayerColor));
        cardSelected.possibleTargets.Remove(firstPawnSelected);
        pawnSelected = null;
        //Relaunch the selection process 
        SelectPawn(true);
    }

    /// <summary>
    /// Wait for a second pawn to be selected and apply the Jack card effect
    /// </summary>
    /// <returns></returns>
    IEnumerator waitForSecondPawn()
    {
        {
            yield return new WaitWhile(() => pawnSelected == null);
        }
        CmdPlayCard(cardSelected.name, firstPawnSelected.name, pawnSelected.name);
        MovementLeft = 0;
        StartCoroutine(waitForPawnMoved());
    }

    /// <summary>
    /// Wait for end of pawn move 
    /// </summary>
    /// <returns></returns>
    IEnumerator waitForPawnMoved()
    {
        if (pawnSelected != null)
        {
            yield return new WaitWhile(() => pawnSelected.Status == PawnStatusEnum.MOVING);
        }
        Debug.Log("Pawn finished moving");
        if (MovementLeft == 0)
        {
            this.EndTurn();
        }
    }
    #endregion
    #region ProgressDictionnary
    /// <summary>
    /// Add a new pawn to the ProgressDictionnary on the client
    /// </summary>
    /// <param name="target">string - name of the pawn to be added</param>
    [ClientRpc]
    public void RpcAddtoProgressDictionnary(string target)
    {
        int newProgress = GMaster.progressDictionnary.Add(target);
        GameObject.Find(target).GetComponent<Pawn>().ProgressInDictionnary = newProgress;
    }

    /// <summary>
    /// Update the position of a pawn in the ProgressDictionnary on the client
    /// </summary>
    /// <param name="target">string - name of the pawn to move</param>
    /// <param name="nbMoves">int - number of moves</param>
    [ClientRpc]
    public void RpcMoveinProgressDictionnary(string target, int nbMoves)
    {
        int newProgress = GMaster.progressDictionnary.Move(target, nbMoves);
        GameObject.Find(target).GetComponent<Pawn>().ProgressInDictionnary = newProgress;
    }

    /// <summary>
    /// Remove a pawn from the ProgressDictionnary on the client
    /// </summary>
    /// <param name="target">string - name of the pawn to remove</param>
    [ClientRpc]
    public void RpcRemovefromProgressDictionnary(string target)
    {
        GMaster.progressDictionnary.Remove(target);
        Debug.Log("Removed : " + target + " from ProgressDico");

    }
    #endregion
    #region Turn begin/end
    /// <summary>
    /// TargetRpc - Begin turn for the targetted player
    /// </summary>
    /// <param name="Target"></param>
    [TargetRpc]
    public void TargetBeginTurn(NetworkConnection Target)
    {
        Text.text = "It's my turn, i'm the player : " + this.PlayerName + " - Color : " + this.PlayerColor.ToString();
        Debug.Log("Beginning turn of player : " + this.PlayerColor);
        Projection();
    }

    /// <summary>
    /// End the turn for the player
    /// </summary>
    public void EndTurn()
    {
        DiscardSelectedCard();

        pawnSelected = null;
        firstPawnSelected = null;
        DisplayMovementLeft.SetActive(false);

        //Disable click on cards
        foreach (Card item in playerHand)
        {
            DisplayedHand[playerHand.IndexOf(item)].GetComponent<Button>().enabled = false;
        }
        if (hasWin())
        {
            CmdVictory();
        }
        CmdEndTurn();
    }

    /// <summary>
    /// Tell the server to switch player
    /// </summary>
    [Command]
    public void CmdEndTurn()
    {
        GMaster.NextPlayer();
    }
    #endregion
    #region End Game
    /// <summary>
    /// Tell the client there is a winner
    /// </summary>
    [Command]
    private void CmdVictory()
    {
        RpcEndGame(this.name);
    }

    /// <summary>
    /// Trigger the display of the end game popup by setting a winnerName
    /// </summary>
    /// <param name="winnerName"></param>
    [ClientRpc]
    private void RpcEndGame(string winnerName)
    {
        this.winnerName = winnerName;

    }

    /// <summary>
    /// Tell the client to return to the Lobby
    /// </summary>
    [Command]
    private void CmdReturnToLobby()
    {
        RpcReturnToLobby();
        SceneManager.LoadScene("Lobby");

    }

    /// <summary>
    /// Load the lobby scene
    /// </summary>
    [ClientRpc]
    private void RpcReturnToLobby()
    {
        SceneManager.LoadScene("Lobby");

    }

    #endregion
    #region Popup
    private void OnGUI()
    {
        if (winnerName != "")
        {
            WindowRect = GUI.Window(0, WindowRect, DoMyWindow, winnerName == GMaster.LocalPlayer.name ? "Victory !!!" : "Defeat...");
        }
    }

    private void DoMyWindow(int id)
    {
        Rect buttonRect = new Rect((WindowRect.width - (WindowRect.width / 4)) / 2, (WindowRect.height - (WindowRect.height / 4)) / 2, WindowRect.width / 4, WindowRect.height / 4);
        if (GUI.Button(buttonRect, "Ok"))
        {
            CmdReturnToLobby();
        }
    }

    #endregion
}
