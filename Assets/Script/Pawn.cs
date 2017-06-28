using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;

public class Pawn : NetworkBehaviour
{
    [SyncVar(hook ="OnChangeProgress")]
    public int Progress = -1;

    [SyncVar]
    public int nbLayer = 0;

    [SyncVar]
    public String PawnName;

    public bool OnBoard = false;

    [SyncVar]
    public PlayerColorEnum Player;

    private LayerMask PawnLayer;
    private Animator PawnAnimator;
    private MeshRenderer PawnMeshRenderer;
    private int progressHash = Animator.StringToHash("Progress");


    private void OnEnable()
    {
        PawnAnimator = GetComponent<Animator>();
        PawnMeshRenderer = this.GetComponentInChildren<MeshRenderer>();

    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    void OnChangeProgress(int progress)
    {
        this.Progress = progress;
        if ((this.Progress > -1) && (PawnAnimator != null))
        {
            PawnAnimator.SetInteger(progressHash, this.Progress);
        }

    }

    public void Initialise(PlayerColorEnum color,int pawnIndex)
    {
        Player = color;
        this.name = color.ToString() + pawnIndex.ToString();

        switch (Player)
        {
            case PlayerColorEnum.Blue:
                PawnMeshRenderer.material.color = Color.blue;
                break;
            case PlayerColorEnum.Green:
                PawnMeshRenderer.material.color = Color.green;
                break;
            case PlayerColorEnum.Red:
                PawnMeshRenderer.material.color = Color.red;
                break;
            case PlayerColorEnum.Yellow:
                PawnMeshRenderer.material.color = Color.yellow;
                break;
        }
        PawnAnimator.SetLayerWeight((int)Player,1);
        PawnName = color.ToString() + pawnIndex.ToString();

    }

    public void Enter()
    {
        this.Progress = 1;
    }

    public void Move(bool recul = false)
    {
        if (recul) this.Progress--;
        else this.Progress++;

    }

    public void Exit()
    {
        this.Progress = -1;

    }
}
