using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour {
    public static GameMaster GMaster;
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

}
