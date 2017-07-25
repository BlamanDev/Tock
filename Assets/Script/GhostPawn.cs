using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class GhostPawn : MonoBehaviour {
    private Animator PawnAnimator;
    private int StateHash = Animator.StringToHash("ProgressOnBoard");

    public int Progress;
    public List<Pawn> PawnsEncoutered;
    public float Normaltime;

    public delegate void OnProjectionFinished(List<Pawn> pawnEncoutered);
    public static event OnProjectionFinished EventOnProjectionFinished;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(Pawn origin)
    {
        PawnsEncoutered = new List<Pawn>();
        this.transform.position = origin.transform.position;
        this.transform.rotation = origin.transform.rotation;
        GetComponentInChildren<MeshRenderer>().material.color = Color.clear;
        
        PawnAnimator = GetComponent<Animator>();
        
        PawnAnimator.enabled = false;
        Progress = origin.Progress;

        //PawnAnimator.Play(StateHash);
        //AnimatorStateInfo actualstate = PawnAnimator.GetCurrentAnimatorStateInfo(0);
        //PawnAnimator.PlayInFixedTime(StateHash, -1, originState.normalizedTime);
        Normaltime = origin.PawnAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void Projection(int nbCells)
    {
        Progress += nbCells;
        PawnAnimator.enabled = true;
        PawnAnimator.speed = 10f;
        PawnAnimator.PlayInFixedTime(StateHash, 0, Normaltime);
    }


    public void CheckProgress(int animationProgress)
    {
        if (animationProgress == Progress)
        {
            PawnAnimator.enabled = false;
            EventOnProjectionFinished(this.PawnsEncoutered);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PawnModel")
        {
            PawnsEncoutered.Add(other.GetComponentInParent<Pawn>());
        }

    }
}
