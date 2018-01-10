using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTock : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    static public void DebugCard(Card cardInspected)
    {
        Debug.Log("Card : " + cardInspected.name);
        Debug.Log("Possible targets : ");
        foreach (String item in cardInspected.possibleTargetsS)
        {
            Pawn pawnTarget = GameObject.Find(item).GetComponent<Pawn>();

            Debug.Log("name : " + pawnTarget.name);
            Debug.Log("status : " + pawnTarget.Status);
            Debug.Log("Progress : " + pawnTarget.Progress);
            Debug.Log("ProgressInDico : " + pawnTarget.ProgressInDictionnary);
        }
    }


}
