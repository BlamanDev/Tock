using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : NetworkBehaviour
{
    public static GameMaster GMaster;
    public GameObject PawnPrefab;
    private int nextColor = 0;

    public Text text;

    public Dictionary<PlayerColorEnum, List<Pawn>> AllPawns;



    // Use this for initialization
    void Start()
    {
        PawnSpawner.EventAllPawnsCreated += buildPawnList;
        AllPawns = new Dictionary<PlayerColorEnum, List<Pawn>>();
    }

    // Update is called once per frame
    void Update()
    {

    }

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

    private void buildPawnList()
    {
        Pawn[] listPawn = GameObject.FindObjectsOfType<Pawn>();
        foreach (Pawn item in listPawn)
        {
            if (!AllPawns.ContainsKey(item.Player))
            {
                AllPawns[item.Player] = new List<Pawn>();
            }
            AllPawns[item.Player].Add(item);
        }
    }

    public List<Pawn> getPawnOfAColor(PlayerColorEnum color)
    {
        if (AllPawns.Count==0)
        {
            buildPawnList();
        }
        return AllPawns[color];
    }

    public PlayerColorEnum CmdGiveNewPlayerColor()
    {
        nextColor++;
        if (nextColor > 4)
        {
            nextColor = 1;
        }
        text.text += "Giving new color : " + ((PlayerColorEnum)nextColor).ToString() + " ";
        return (PlayerColorEnum)nextColor;
    }

    [Command]
    public void CmdTestEnter()
    {
        TockPlayer blop = GameObject.FindGameObjectWithTag("Blue_Player").GetComponent<TockPlayer>();
        blop.CmdEnterPawn(1);
    }

    [Command]
    public void CmdTestMove()
    {
        TockPlayer blop = GameObject.FindGameObjectWithTag("Blue_Player").GetComponent<TockPlayer>();
        blop.CmdMovePawn(1);

    }


}
