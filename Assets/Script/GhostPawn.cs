using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GhostPawn : MonoBehaviour {
    private Animator PawnAnimator;
    private int StateHash = Animator.StringToHash("ProgressOnBoard");

    public int Progress;
    public List<Pawn> PawnsEncoutered;
    public float Normaltime;

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

    public List<Pawn> Projection(int nbCells)
    {
        Progress += nbCells;
        StartCoroutine(playProjection());
        return PawnsEncoutered;
    }

    IEnumerator playProjection()
    {
        PawnAnimator.enabled = true;
        PawnAnimator.Play(StateHash,0,Normaltime);
        yield return new WaitWhile(()=>PawnAnimator.enabled == true);

    }

    public void CheckProgress(int animationProgress)
    {
        if (animationProgress == Progress)
        {
            PawnAnimator.enabled = false;
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
