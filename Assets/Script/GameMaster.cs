using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

/// <summary>
/// Script for the GameMaster
/// </summary>
public class GameMaster : NetworkBehaviour
{
    #region properties
    //Used for the Singleton
    static public GameMaster GMaster = null;
    //List of players, static to be accessible on players connection
    static public List<TockPlayer> players = new List<TockPlayer>();


    //Prefab used for the Pawn
    public GameObject PawnPrefab;
    public GameObject CardPrefab;
    //to avoid giving the same color to different players
    static private int nextColor = -1;

    //For debugging
    public Text text;

    //Contains the list of Pawns sorted by color
    public Dictionary<PlayerColorEnum, List<Pawn>> AllPawns;
    public ProgressDictionnary progressDictionnary;

    private int activePlayerIndex = -1;


    private TockPlayer localPlayer;

    public TockPlayer LocalPlayer
    {
        get
        {
            if (localPlayer == null)
            {
                localPlayer = findLocalPlayer();
            }
            return localPlayer;
        }

        set
        {
            localPlayer = value;
        }
    }


    #endregion
    #region initialisation
    // Use this for initialization
    void Start()
    {
        //Attach to Event AllPawnCreated
        PawnSpawner.EventAllPawnsCreated += buildPawnList;
        AllPawns = new Dictionary<PlayerColorEnum, List<Pawn>>();
        progressDictionnary = new ProgressDictionnary();
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
            DontDestroyOnLoad(gameObject);
            GMaster = this;
        }
        else if (GMaster != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game") text = GameObject.Find("TextGameMaster").GetComponent<Text>();

    }

    /// <summary>
    /// Begin the Game !
    /// </summary>
    public void GameBegin()
    {
        StartCoroutine(waitForAllPlayers());
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
            if (!AllPawns.ContainsKey(item.PlayerColor))
            {
                AllPawns[item.PlayerColor] = new List<Pawn>();
            }
            AllPawns[item.PlayerColor].Add(item);

        }
    }

    /// <summary>
    /// Return the list of pawns corresponding to the given color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public List<Pawn> getPawnsOfAColor(PlayerColorEnum color)
    {
        if (AllPawns.Count == 0)
        {
            buildPawnList();
        }
        return AllPawns[color];
    }

    /// <summary>
    /// Return the list of pawn a color filtered with the card Colorfilter
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public List<Pawn> getPawnsFiltered(SelectionFilterEnum filter, PlayerColorEnum color)
    {
        List<Pawn> listeRetour = new List<Pawn>();
        switch (filter)
        {
            case SelectionFilterEnum.OWNPAWNS:
                listeRetour = AllPawns[color];
                break;

            case SelectionFilterEnum.ALLPAWNS:
                foreach (PlayerColorEnum item in AllPawns.Keys)
                {
                    listeRetour.AddRange(AllPawns[item]);
                }
                break;
            case SelectionFilterEnum.OTHERPAWNS:
                foreach (PlayerColorEnum item in AllPawns.Keys)
                {
                    if (item != color)
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
    /// Cycle between colors and return the next one
    /// </summary>
    /// <returns></returns>
    static public PlayerColorEnum GiveNewPlayerColor()
    {
        nextColor++;
        //if nextcolor > number of possible color
        if (nextColor > 3)
        {
            nextColor = 0;
        }

        PlayerColorEnum colorReturned = (PlayerColorEnum)nextColor;
        return colorReturned;
    }

    /// <summary>
    /// Find the local player
    /// </summary>
    /// <returns></returns>
    private TockPlayer findLocalPlayer()
    {
        return players.Find(x => x.isLocalPlayer);
    }

    /// <summary>
    /// Wait for all players in the lobby to be connected to the scene
    /// </summary>
    /// <returns></returns>
    IEnumerator waitForAllPlayers()
    {
        NetworkLobbyManager lobbyby = GameObject.FindObjectOfType<NetworkLobbyManager>();
        yield  return new WaitUntil(() => players.Count == lobbyby.numPlayers);
        PawnSpawner spawner = GameObject.FindObjectOfType<PawnSpawner>();
        //If there is a PawnSpawner in the scene, create the pawns
        if (spawner != null)
        {
            spawner.PopulatePawns();
        }
        //Build the first hand for each players
        foreach (TockPlayer item in players)
        {
            item.TargetBuildFirstHand(NetworkServer.objects[item.netId].connectionToClient);
        }
    }
    #endregion
    /// <summary>
    /// Switch to the next player
    /// </summary>
    public void NextPlayer()
    {
        activePlayerIndex++;
        if (activePlayerIndex==players.Count)
        {
            activePlayerIndex = 0;
        }
        text.text = "Beginning turn of player : " + players[activePlayerIndex].name;
        players[activePlayerIndex].TargetBeginTurn(NetworkServer.objects[players[activePlayerIndex].netId].connectionToClient);
    }


}
