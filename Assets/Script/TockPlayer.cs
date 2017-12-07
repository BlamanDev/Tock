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
    #region attributes
    //List of the pawns owned by the player
    public List<Pawn> Pawns;

    private PlayerHand playerHand;

    #region temp attributes
    //Number of playable card in the player's hand
    private int nbPlayableCards = 0;

    public Card CardSelected = null;
    private Pawn firstPawnSelected = null;
    private Pawn pawnSelected = null;

    private Coroutine waitfor = null;
    #endregion
    //Color of the player
    [SyncVar]
    public PlayerColorEnum PlayerColorE = PlayerColorEnum.Clear;

    [SyncVar]
    public Color PlayerColor;


    [SyncVar]
    public int PlayerIndex;

    #region References
    static private Deck gameDeck;

    //Prefab used for the Pawn
    static public GameObject PawnPrefab;
    //references to the the component used by the script
    static private GameMaster gMaster;
    static private TockBoard board;


    public Image[] DisplayedHand;
    #endregion
    #region Seven card
    //Number of movement left for the Seven card
    private int movementLeft = 0;
    //Display the movement left for the Seven card
    static private GameObject displayMovementLeft;
    #endregion
    //for debugging
    private Text debugPlayerText;
    #region Popup
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

    public Text DebugPlayerText
    {
        get
        {
            if (debugPlayerText == null)
            {
                debugPlayerText = GameObject.Find("TextTockPlayer").GetComponent<Text>();
            }
            return debugPlayerText;
        }
        set
        {
            debugPlayerText = value;
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
        //PlayerColor = GameMaster.GiveNewPlayerColor();

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
            DisplayedHand = GameObject.Find("CardsInHand").GetComponentsInChildren<Image>();
            Hand.OnAdd += DisplayCard;
            Hand.OnRemove += ClearCard;
            //Camera.main.enabled = false;
            //Camera playerCam = GameObject.Find("Camera_" + this.PlayerIndex).GetComponent<Camera>();
            Camera.main.backgroundColor = this.PlayerColor;
            KeyValuePair<Vector3, Quaternion> cameraPosition = GameMaster.CameraPositions.GetCameraforPlayer(PlayerIndex);
            Camera.main.transform.SetPositionAndRotation(cameraPosition.Key, cameraPosition.Value);
            //playerCam.enabled = true;
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
    public void RpcBuildLists()
    {
        Pawns = GMaster.GetPawnsOfAPlayer(PlayerIndex);
        Pawns.Sort(ComparePawnsByPawnIndex);
    }

    /// <summary>
    /// Test if all the pawns of the player are in home (finish line)
    /// </summary>
    /// <returns></returns>
    public bool hasWin()
    {
        int inHouse = 0;
        //Debug.Log("Test HasWin() :");
        foreach (Pawn pawnSelected in Pawns)
        {
            //Debug.Log("Pawn : " + pawnSelected.name + " - status : " + pawnSelected.Status + " - progress : " + pawnSelected.Progress);
            if (pawnSelected.Status == PawnStatusEnum.IN_HOUSE)
            {
                inHouse++;
                //Debug.Log(pawnSelected.name + " is IN_HOUSE, there is " + inHouse +" Pawns IN_HOUSE");
            }
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
            if (!item.MakeProjections(GMaster.GetPawnsFiltered(item.ColorFilter, PlayerIndex)))
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
        GameObject btnGameBegin = GameObject.Find("BtnGameBegin");
        if (btnGameBegin != null)
        {
            btnGameBegin.SetActive(false);
        }
        GameObject txtClientWait = GameObject.Find("TxtClientWait");
        if (txtClientWait != null)
        {
            txtClientWait.SetActive(false);
        }


    }

    /// <summary>
    /// Draw a new card
    /// </summary>
    public void PickACard()
    {
        if (Hand.Count < 5)
        {
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
            CardSelected = Hand[cardPlayed];
            //IF no cards are playable, discard the selected card
            if (nbPlayableCards == 0)
            {
                this.switchAllCardsClick();
                EndTurn();
            }
            else
            {
                //DebugPlayerText.text = "Card Selected : " + Hand[cardPlayed].name + " : " + (int)Hand[cardPlayed].Value + " cases";   //debug
                /*if (Hand[cardPlayed].Value == CardsValuesEnum.SEVEN)
                {
                    MovementLeft = 7;
                    DisplayMovementLeft.SetActive(true);
                }*/
                this.SelectPawn();
            }
        }
        
    }

    /// <summary>
    /// Discard the Selected Card then draw a new card
    /// </summary>
    private void DiscardSelectedCard()
    {
        Hand.Remove(CardSelected);
        CmdReturnCard(CardSelected.name);
        if (isClient)
        {
            NetworkServer.UnSpawn(CardSelected.gameObject);
        }
        this.PickACard();
        CardSelected = null;
        //DebugPlayerText.text = "Turn Finished for Me";
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
        //DebugPlayerText.text = "Card Played : " + card.name + " : " + (int)card.Value + " cases";   //debug
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
        if (waitfor != null)
        {
            StopCoroutine(waitfor);
        }
        DisplayProjection(CardSelected);
        //IF this the first selection
        if (!secondTarget)
        {
            waitfor = StartCoroutine(waitForPawn());
        }
        //IF this the second selection process
        else
        {
            waitfor = StartCoroutine(waitForSecondPawn());
        }
    }

    public void DisplayProjection(Card cardToDisplay)
    {
        //Subscribe  on the Event OnClick on every pawns then turn on the halo selection
        foreach (Pawn item in cardToDisplay.possibleTargets)
        {
            item.EventOnPawnSelected += PawnSelection;
            item.SwitchHalo(true, PlayerColor);
        }

    }

    /// <summary>
    /// Function called by the event OnClick on the pawn, register the pawn selected and disable the selection process
    /// </summary>
    /// <param name="Selection">Pawn - Pawn Selected</param>
    private void PawnSelection(Pawn Selection)
    {
        pawnSelected = Selection;
        UnSelectAll();
    }

    public void UnSelectAll()
    {
        foreach (Pawn item in GameObject.FindObjectsOfType<Pawn>())
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

        switch (CardSelected.Value)
        {
            /*case CardsValuesEnum.SEVEN:
                PlayMoveMany();
                break;*/
            case CardsValuesEnum.JACK:
                PlayExchange();
                MovementLeft++;
                break;
            default:
                CmdPlayCard(CardSelected.name, pawnSelected.name, "");
                MovementLeft = 0;
                break;
        }
        //DebugPlayerText.text = "Turn Finished for Me";

        StartCoroutine(waitForPawnMoved());
    }

    /// <summary>
    /// Play the Seven Card, divide 7 movement on 1 or more pawns
    /// </summary>
    private void PlayMoveMany()
    {
        //Move the selected pawn for 1 cell
        CmdPlayCard(CardSelected.name, pawnSelected.name, "");
        //Test if this pawn can still move
        if (CardSelected.MoveFiltering(pawnSelected, 1))
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
    private void PlayExchange()
    {
        firstPawnSelected = pawnSelected;
        UnSelectAll();
        //Compute the selection of pawns to be switched
        CardSelected.MakeProjections(GMaster.GetPawnsFiltered(SelectionFilterEnum.ALLPAWNS, firstPawnSelected.OwningPlayerIndex));
        CardSelected.possibleTargets.Remove(firstPawnSelected);
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
        CmdPlayCard(CardSelected.name, firstPawnSelected.name, pawnSelected.name);
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
            yield return new WaitWhile(() => (pawnSelected.Status == PawnStatusEnum.MOVING));
        }
        //Debug.Log("Pawn finished moving");
        if (MovementLeft == 0)
        {
            this.EndTurn();
        }
    }

    IEnumerator CheckPawnsStillMoving()
    {
        foreach (List<Pawn> pawnList in GameMaster.AllPawns.Values)
        {
            foreach (Pawn item in pawnList)
            {
                if (item.Status == PawnStatusEnum.MOVING)
                {
                    //Debug.Log(item.name + " haven't finished moving");
                    yield return item.waitForFinishedMoving();
                    //Debug.Log(item.name + " have finished moving");

                    break;
                }
            }
        }
       // Debug.Log("All pawn have finished moving");
        GMaster.TestVictory();
        Projection();
        //Debug.Assert(!allPawnIdling, "All Pawn are idling");
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

    [Command]
    public void CmdAddtoProgressDictionnary(string target)
    {
        int newProgress = GMaster.ProgressListAdd(target);
        GameObject.Find(target).GetComponent<Pawn>().ProgressInDictionnary = newProgress;
    }

    [Command]
    public void CmdMoveinProgressDictionnary(string target, int nbMoves)
    {
        int newProgress = GMaster.ProgressListMovePawn(target, nbMoves);
        GameObject.Find(target).GetComponent<Pawn>().ProgressInDictionnary = newProgress;
    }

    [Command]
    public void CmdRemovefromProgressDictionnary(string target)
    {
        GMaster.ProgressListRemove(target);
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
        //DebugPlayerText.text = "It's my turn, i'm the player : " + this.name + " - Color : " + this.PlayerColor.ToString();
        //Debug.Log("Beginning turn of player : " + this.PlayerColor);
        StartCoroutine(CheckPawnsStillMoving());
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
        CmdTestVictory();
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
    public void CmdTestVictory()
    {
        GMaster.TestVictory();
    }

    /// <summary>
    /// Trigger the display of the end game popup by setting a winnerName
    /// </summary>
    /// <param name="winnerNetId"></param>
    [ClientRpc]
    public void RpcEndGame(string winnerNetId)
    {
        this.winnerName = winnerNetId;

    }

    /// <summary>
    /// Tell the client to return to the Lobby
    /// </summary>
    [Command]
    public void CmdReturnToLobby()
    {
        RpcReturnToLobby();
        GMaster.QuitSession();

    }

    /// <summary>
    /// Load the lobby scene
    /// </summary>
    [ClientRpc]
    private void RpcReturnToLobby()
    {
        GMaster.QuitSession();

    }

    #endregion
    #region Popup
    private void OnGUI()
    {
        if (winnerName != "")
        {
            WindowRect = GUI.Window(0, WindowRect, DoEndWindow, winnerName == GMaster.LocalPlayer.netId.ToString() ? "Victory !!!" : "Defeat...");
        }

    }

    private void DoEndWindow(int id)
    {
        Rect buttonRect = new Rect((WindowRect.width - (WindowRect.width / 4)) / 2, (WindowRect.height - (WindowRect.height / 4)) / 2, WindowRect.width / 4, WindowRect.height / 4);
        if (GUI.Button(buttonRect, "Ok"))
        {
            CmdReturnToLobby();
        }
    }


    #endregion
}
