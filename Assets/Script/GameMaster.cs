using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour
{
    public static GameMaster GMaster;
    public GameObject PawnPrefab;
    private int nextColor = 0;

    // Use this for initialization
    void Start()
    {

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

    public PlayerColorEnum CmdGiveNewPlayerColor()
    {
        nextColor++;
        if (nextColor > 4) nextColor = 1;
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


    public override void OnStartServer()
    {
    }
}
