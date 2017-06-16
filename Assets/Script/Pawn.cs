using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;

public class Pawn : NetworkBehaviour
{
    public int Progress = -1;
    private PlayerColorEnum Player;
    private GameObject PawnObject;
    private LayerMask PawnLayer;
    private Animator PawnAnimator;
    private int progressHash;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((this.Progress > -1)&&(PawnAnimator!=null))
        {
            PawnAnimator.SetInteger(progressHash, this.Progress);
        }

    }

    public void Initialise(PlayerColorEnum color,int pawnIndex)
    {
        Player = color;
        switch (Player)
        {
            case PlayerColorEnum.Blue:
                this.GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
            case PlayerColorEnum.Green:
                this.GetComponent<MeshRenderer>().material.color = Color.green;
                break;
            case PlayerColorEnum.Red:
                this.GetComponent<MeshRenderer>().material.color = Color.red;
                break;
            case PlayerColorEnum.Yellow:
                this.GetComponent<MeshRenderer>().material.color = Color.yellow;
                break;
        }
        PawnAnimator = GetComponent<Animator>();
        PawnAnimator.SetLayerWeight((int)Player,1);
        progressHash = Animator.StringToHash("Progress");
        this.name = color.ToString() + pawnIndex.ToString();
    }

    public void Enter()
    {
        this.Progress = 1;
       // PawnAnimator.SetInteger("Progress",this.Progress);
    }

    public void Move(bool recul = false)
    {
        if (recul) this.Progress--;
        else this.Progress++;
        //PawnAnimator.SetInteger("Progress", this.Progress);

    }

    public void Exit()
    {
        this.Progress = -1;
        //PawnAnimator.SetInteger("Progress", this.Progress);

    }
}
