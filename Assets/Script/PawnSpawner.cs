using Assets.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PawnSpawner : NetworkBehaviour
{
    public delegate void OnAllPawnsCreation();
    [SyncEvent]
    public static event OnAllPawnsCreation EventAllPawnsCreated;

    public GameObject PawnPrefab;
    public TockPlayer[] playerList;
    public Text text;


    private void OnEnable()
    {


    }

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }


    private Pawn CreatePawn(PlayerColorEnum player, int pawnIndex)
    {
        GameObject newPawn = Instantiate(PawnPrefab);

        Pawn retour = newPawn.GetComponent<Pawn>();
        retour.Initialise(player, pawnIndex);
        NetworkServer.Spawn(newPawn);
        return retour;
    }

    public void PopulatePawns()
    {
        if (text == null)
        {
            text = GameObject.Find("TextPawnSpawner").GetComponent<Text>();
        }

        playerList = FindObjectsOfType<TockPlayer>();
        foreach (TockPlayer player in playerList)
        {
            text.text += "Populating " + player.PlayerColor.ToString() + " Pawn : ";
            for (int i = 0; i < 4; i++)
            {
                text.text += CreatePawn(player.PlayerColor, i).Player.ToString() + " ";
            }

        }
        EventAllPawnsCreated();
        foreach (TockPlayer player in playerList)
        {
            player.RpcBuildPawnList();

        }
    }
    }
