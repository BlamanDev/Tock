﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using cakeslice;

/// <summary>
/// Script for the GameMaster
/// </summary>
public class GameMaster : NetworkBehaviour
{
    private const String RULESFILE = "Texts/Rules";

    #region properties
    //Used for the Singleton
    static public GameMaster GMaster = null;
    //List of players
    private List<TockPlayer> players ;

    public GrpCardEffect GrpCardEffect;

    private NetworkLobbyManager lobbyby; 

    public Canvas RulesPopup;
    public Canvas GameCanvas;

    //Prefab used for the Pawn
    static public GameObject PawnPrefab;
    static public GameObject CardPrefab;
    //to avoid giving the same color to different players
    static private int nextColor = -1;

    //For debugging
    public Text DebugText;

    static private Text txtRules;

    //Contains the list of Pawns sorted by color
    static public Dictionary<int, List<Pawn>> AllPawns;

    public SyncListString ProgressList = new SyncListString();
    public SyncListString HouseList = new SyncListString();

    private Rect fullscreenRect;
    private bool displayRuleFullscreen= false;
    private String rulesText = "";

    [SyncVar]
    private int numberOfPlayers;

    static private int activePlayerIndex = -1;


    static private TockPlayer localPlayer;

    public TockPlayer LocalPlayer
    {
        get
        {
            if (localPlayer == null)
            {
                localPlayer = findLocalPlayer();
                Camera.main.GetComponent<OutlineEffect>().lineColor2 = localPlayer.PlayerColor;

                Color tempColor = localPlayer.PlayerColor;
                float h, s, v;
                Color.RGBToHSV(tempColor, out h, out s, out v);
                if (h>(65f/255f) && h < (155f/255f))
                {
                    tempColor = Color.HSVToRGB(1-h, 0, v);
                    Camera.main.GetComponent<OutlineEffect>().lineColor1 = tempColor;


                }
            }
            return localPlayer;
        }

        set
        {
            localPlayer = value;
        }
    }

    public static Text TxtRules
    {
        get
        {
            if (txtRules == null)
            {
                txtRules = GameObject.Find("TxtRules").GetComponent<Text>();
            }
            return txtRules;
        }

        set
        {
            txtRules = value;
        }
    }

    public string RulesText
    {
        get
        {
            if (String.IsNullOrEmpty(rulesText))
            {
                try
                {
                    rulesText = Resources.Load<TextAsset>(RULESFILE).text;
                }
                catch (Exception e)
                {

                    Debug.Log(e.Message);
                }
            }
            return rulesText;
        }

        set
        {
            rulesText = value;
        }
    }

    public Rect FullscreenRect
    {
        get
        {
            if (fullscreenRect.height == 0)
            {
                fullscreenRect = new Rect(5, 5, Screen.width - 10, Screen.height - 10);
            }
            return fullscreenRect;
        }

        set
        {
            fullscreenRect = value;
        }
    }

    public List<TockPlayer> Players
    {
        get
        {
            if (players == null)
            {
                players = new List<TockPlayer>();
            }
            return players;
        }

        set
        {
            players = value;
        }
    }

    public int NumberOfPlayers
    {
        get
        {
            if (isServer)
            {
                numberOfPlayers = Lobbyby.numPlayers;
            }
            return numberOfPlayers;
        }

        set
        {
            numberOfPlayers = value;
        }
    }

    public NetworkLobbyManager Lobbyby
    {
        get
        {
            if (isServer && lobbyby == null)
            {
                lobbyby = GameObject.FindObjectOfType<NetworkLobbyManager>();
                NumberOfPlayers = lobbyby.numPlayers;
            }
            return lobbyby;
        }

        set
        {
            lobbyby = value;
        }
    }

    #endregion
    #region initialisation
    // Use this for initialization
    void Start()
    {
        //Attach to Event AllPawnCreated
        AllPawns = new Dictionary<int, List<Pawn>>();
        if (isServer)
        {
            GameObject.Find("BtnGameBegin").SetActive(true);
            GameObject.Find("TxtClientWait").SetActive(false);
            for (int i = 0; i < 76; i++)
            {
                ProgressList.Add("");
            }
            for (int i = 0; i < 4 * GameObject.FindObjectOfType<NetworkLobbyManager>().numPlayers; i++)
            {
                HouseList.Add("");
            }
            //Debug.Log("HouseList has " + HouseList.Count + " space");
        }
        else
        {
            GameObject.Find("BtnGameBegin").SetActive(false);
            GameObject.Find("TxtClientWait").SetActive(true);

        }

        //GameBegin();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Singleton business
    /// </summary>
    void Awake()
    {
        if (GMaster == null)
        {
            GMaster = this;
        }
        else if (GMaster != this)
        {
            Destroy(gameObject);
        }
        Players.Clear();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    /// <summary>
    /// Begin the Game !
    /// </summary>
    public void GameBegin()
    {
        StartCoroutine(waitForAllPlayers(true));
    }
    #endregion
    #region Pawns Methods
    /// <summary>
    /// Build the AllPawns dictionnary
    /// </summary>
    private void buildPawnList()
    {
        //Get all the pawns present in the game
        Pawn[] listPawn = GameObject.FindObjectsOfType<Pawn>();
        //FOR EACH pawn, add it to the list corresponding its color
        foreach (Pawn item in listPawn)
        {
            if (!AllPawns.ContainsKey(item.OwningPlayerIndex))
            {
                AllPawns[item.OwningPlayerIndex] = new List<Pawn>();
                MeshRenderer cellOfPlayer = GameObject.Find("Zone_J" + item.OwningPlayerIndex).GetComponent<MeshRenderer>();
                cellOfPlayer.material.color = item.PlayerColor;
            }
            AllPawns[item.OwningPlayerIndex].Add(item);

        }
    }

    /// <summary>
    /// Return the list of pawns corresponding to the given color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public List<Pawn> GetPawnsOfAPlayer(int playerIndex)
    {
        if (AllPawns.Count == 0)
        {
            buildPawnList();
        }
        return AllPawns[playerIndex];
    }

    /// <summary>
    /// Return the list of pawn a color filtered with the card Colorfilter
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public List<Pawn> GetPawnsFiltered(SelectionFilterEnum filter, int playerIndex)
    {
        List<Pawn> listeRetour = new List<Pawn>();
        switch (filter)
        {
            case SelectionFilterEnum.OWNPAWNS:
                listeRetour = AllPawns[playerIndex];
                break;

            case SelectionFilterEnum.ALLPAWNS:
                foreach (int item in AllPawns.Keys)
                {
                    listeRetour.AddRange(AllPawns[item]);
                }
                break;
            case SelectionFilterEnum.OTHERPAWNS:
                foreach (int item in AllPawns.Keys)
                {
                    if (item != playerIndex)
                    {
                        listeRetour.AddRange(AllPawns[item]);
                    }
                }

                break;
            default:
                break;
        }
        return listeRetour;
    }

    #endregion
    #region Players Methods
    /// <summary>
    /// Find the local player
    /// </summary>
    /// <returns></returns>
    private TockPlayer findLocalPlayer()
    {
        TockPlayer localP = Players.Find(x => x.isLocalPlayer);
        
        return localP;
    }

    public void buildPlayersList()
    {
        StartCoroutine(waitForAllPlayers());
    }

    /// <summary>
    /// Wait for all players in the lobby to be connected to the scene
    /// </summary>
    /// <returns></returns>
    IEnumerator waitForAllPlayers(bool beginGame=false)
    {
        while (players.Count != NumberOfPlayers)
        {
            players = new List<TockPlayer>(GameObject.FindObjectsOfType<TockPlayer>());
            yield return new WaitForSeconds(0.1f);
        }
        if (beginGame)
        {
            PawnSpawner spawner = GameObject.FindObjectOfType<PawnSpawner>();

            //If there is a PawnSpawner in the scene, create the pawns
            if (spawner != null)
            {
                spawner.PopulatePawns();
            }
            //Build the first hand for each players
            foreach (TockPlayer item in Players)
            {
                item.RpcBuildLists(Players.Count, item.name);
                item.TargetBuildFirstHand(NetworkServer.objects[item.netId].connectionToClient);
                yield return new WaitUntil(() => item.Hand.Count == 5);
            }
            Color tempColor = LocalPlayer.PlayerColor;
            tempColor.a = 190 / 255f;

            GameObject.Find("PlayerColorDisplayed").GetComponent<Image>().color = tempColor;
            NextPlayer();
        }
    }
    #endregion
    /// <summary>
    /// Switch to the next player
    /// </summary>
    public void NextPlayer()
    {
        GrpCardEffect.SelectedCard = null;
        this.ClearDescription();
        if (isServer)
        {
            activePlayerIndex++;
            if (activePlayerIndex == Players.Count)
            {
                activePlayerIndex = 0;
            }
            Players[activePlayerIndex].TargetBeginTurn(NetworkServer.objects[Players[activePlayerIndex].netId].connectionToClient);
        }
    }

    public void TestVictory()
    {
        if (isServer)
        {
            foreach (int player in AllPawns.Keys)
            {
                int nbPawnsInHouse = 0;
                foreach (Pawn item in AllPawns[player])
                {
                    if (item.Status == PawnStatusEnum.IN_HOUSE)
                    {
                        nbPawnsInHouse++;
                    }
                }
                if (nbPawnsInHouse == 4)
                {
                    LocalPlayer.RpcEndGame(Players[player].netId.ToString()); ;
                }
            }
        }
    }

    public void PlayCard(int indexCard)
    {

        if (!displayRuleFullscreen)
        {
            GrpCardEffect.SelectedCard = LocalPlayer.Hand[indexCard];
            LocalPlayer.PlayCard(indexCard);

        }
    }

    public void OnCardHovered(int indexCardHovered)
    {
        if (LocalPlayer.ActivePlayer && LocalPlayer.Hand != null && LocalPlayer.Hand.Count  == 5 && !displayRuleFullscreen )
        {
            DisplayDescription(LocalPlayer.Hand[indexCardHovered]);
        }
        
    }

    public void DisplayDescription(Card cardHovered)
    {
        GrpCardEffect.DisplayCardEffect(cardHovered);
        LocalPlayer.DisplayProjection(cardHovered);
        
    }

    public void OnCardExit(int cardExited)
    {
        if (LocalPlayer.ActivePlayer)
        {
            if(LocalPlayer.CardSelected != null)
            {
                LocalPlayer.DisplayProjection();

            }
            else
            {
                LocalPlayer.UnSelectAllPawns();
            }

            if (LocalPlayer.Hand != null && LocalPlayer.Hand.Count == 5)
            {
                this.ClearDescription();
            }
            

        }
    }

    public void ClearDescription()
    {
        GrpCardEffect.ClearCardEffect();

    }

    #region ProgressList
    public int ProgressListAdd(string pawnTarget)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();

        int position = 18 * target.OwningPlayerIndex;
        ProgressList[position] = pawnTarget;
        //Debug.Log("Added : " + pawnTarget + "- to ProgressDico at position : " + position);

        return position;
    }

    public int[] ExchangeCompute(Pawn target1, Pawn target2)
    {
        int[] nbMoves = new int[2];

        nbMoves[0] = (TestPosition(ProgressList.IndexOf(target2.name) - target1.OwningPlayerIndex * 18)) - target1.Progress;
        nbMoves[1] = (TestPosition(ProgressList.IndexOf(target1.name) - target2.OwningPlayerIndex * 18)) - target2.Progress;

        return nbMoves;
    }

    public List<Pawn> ProgressListGetPawnsInRange(int startIndex, int lastIndex)
    {
        //Debug.Log("Check between " + startIndex + " and " + lastIndex);
        List<Pawn> returnList = new List<Pawn>();
        for (int i = startIndex; i <= lastIndex; i++)
        {
            if (!String.IsNullOrEmpty(ProgressList[TestPosition(i)]))
            {
                returnList.Add(GameObject.Find(ProgressList[TestPosition(i)]).GetComponent<Pawn>());
            }
        }
        return returnList;
    }

    public int ProgressListMovePawn(string pawnTarget, int nbCell)
    {
        Pawn target = GameObject.Find(pawnTarget).GetComponent<Pawn>();
        int newPosition = target.ProgressInDictionnary + nbCell;

        ProgressListRemove(pawnTarget);

        if (target.Progress < 71)
        {
            newPosition = TestPosition(newPosition);
            ProgressList[newPosition] = pawnTarget;
            //Debug.Log("Moved : " + target + " for " + nbCell + " cells in ProgressDico, new position : " + newPosition);
        }
        else
        {
            HouseList[(74 - target.Progress) + (target.OwningPlayerIndex * 4)] = pawnTarget;
            //Debug.Log("Moved : " + target + " for " + nbCell + " cells in ProgressDico, entering House at : " + HouseList[HouseList.IndexOf(pawnTarget)] + " , new Position : " + newPosition);
        }
        return newPosition;
    }

    public void ProgressListRemove(string pawnTarget)
    {
        if (ProgressList.Contains(pawnTarget))
        {
            ProgressList[ProgressList.IndexOf(pawnTarget)] = String.Empty;

        }
        else
        {
            if (HouseList.Contains(pawnTarget))
            {
                HouseList[HouseList.IndexOf(pawnTarget)] = String.Empty;
            }
        }
    }

    public bool ProgressListTestHouseFree(int progress, int playerIndex)
    {
        //Debug.Log("Test House Free - progress tested : " + progress + " - playerindex tested : " + playerIndex + " - position tested : " + ((74 - progress) + 4 * playerIndex));
        return !String.IsNullOrEmpty(HouseList[(74-progress) + 4 * playerIndex]);
    }

    static public int TestPosition(int position)
    {
        int rPosition = position;

        if (position > 71) rPosition -= 72;
        if (position < 0) rPosition += 72;

        return rPosition;
    }

    #endregion
    #region Popup Window
    public void DisplayRuleWindow()
    {
        GameCanvas.enabled = false;

        RulesPopup.enabled = true;
    }

#endregion
    public void DebugSelectedCard()
    {
        DebugTock.DebugCard(LocalPlayer.CardSelected);
    }

    public void QuitSession()
    {
        if (isServer)
        {          
            GameObject.FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
        }
        else
        {
            GameObject.FindObjectOfType<NetworkLobbyManager>().SendReturnToLobby();
        }

    }

    
}
