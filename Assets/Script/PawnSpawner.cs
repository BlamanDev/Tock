using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// GameObject handling the pawns creation and spawning
/// </summary>
public class PawnSpawner : NetworkBehaviour
{

    //The prefab used for the Pawn GameObject
    public GameObject PawnPrefab;
    //List of all the players in the Game
    public TockPlayer[] playerList;
    //Used for debugging
    public Text text;

    private GameMaster gMaster;

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




    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Create and spawn a new Pawn
    /// </summary>
    /// <param name="player">Owner of the pawn</param>
    /// <param name="pawnIndex"></param>
    /// <returns></returns>
    private Pawn CreatePawn(TockPlayer player, int pawnIndex)
    {
        GameObject newPawn = Instantiate(PawnPrefab);
        Pawn retour = newPawn.GetComponent<Pawn>();

        retour.Initialize(player, pawnIndex);

        NetworkServer.Spawn(newPawn);

        return retour;
    }

    /// <summary>
    /// Create all pawns for all players
    /// </summary>
    public void PopulatePawns()
    {
        //For debugging
        if (text == null)
        {
            text = GameObject.Find("TextPawnSpawner").GetComponent<Text>();
        }

        //FOR EACH player, create 4 pawns
        //foreach (String color in Enum.GetNames(typeof(PlayerColorEnum)))
        foreach (TockPlayer player in GameMaster.players)
        {
            for (int i = 1; i <= 4; i++)
            {
                CreatePawn(player, i);
            }
        }
    }
}
