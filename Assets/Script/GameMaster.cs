using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for the GameMaster
/// 
/// </summary>
public class GameMaster : NetworkBehaviour
{
    //Used for the Singleton
    public static GameMaster GMaster;
    //Prefab used for the Pawn
    public GameObject PawnPrefab;
    //to avoid giving the same color to different players
    private int nextColor = 0;

    //For debugging
    public Text text;

    //Contains the list of Pawns sorted by color
    public Dictionary<PlayerColorEnum, List<Pawn>> AllPawns;



    // Use this for initialization
    void Start()
    {
        //Attach to Event AllPawnCreated
        PawnSpawner.EventAllPawnsCreated += buildPawnList;
        AllPawns = new Dictionary<PlayerColorEnum, List<Pawn>>();
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

    
    private void OnLevelWasLoaded(int level)
    {
        if (level==1) text = GameObject.Find("TextGameMaster").GetComponent<Text>();

    }

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
            if (!AllPawns.ContainsKey(item.Player))
            {
                AllPawns[item.Player] = new List<Pawn>();
            }
            AllPawns[item.Player].Add(item);
        }
    }

    /// <summary>
    /// Return the list of pawns corresponding to the given color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public List<Pawn> getPawnOfAColor(PlayerColorEnum color)
    {
        if (AllPawns.Count==0)
        {
            buildPawnList();
        }
        return AllPawns[color];
    }

    /// <summary>
    /// Cycle between colors and return the next one
    /// </summary>
    /// <returns></returns>
    public PlayerColorEnum CmdGiveNewPlayerColor()
    {
        nextColor++;
        //if nextcolor > number of possible color
        if (nextColor > 4)
        {
            nextColor = 1;
        }
        text.text += "Giving new color : " + ((PlayerColorEnum)nextColor).ToString() + " ";
        return (PlayerColorEnum)nextColor;
    }

    [Command]
    public void CmdTestEnter(string player)
    {
        TockPlayer blop = GameObject.FindGameObjectWithTag(player + "_Player").GetComponent<TockPlayer>();
        blop.CmdEnterPawn(1);
    }

    [Command]
    public void CmdTestMove(string player)
    {
        TockPlayer blop = GameObject.FindGameObjectWithTag(player + "_Player").GetComponent<TockPlayer>();
        blop.CmdMovePawn(1, 3);

    }


}
