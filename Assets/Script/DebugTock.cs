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
        foreach (Pawn item in cardInspected.possibleTargets)
        {
            Debug.Log("name : " + item.name);
            Debug.Log("status : " + item.Status);
            Debug.Log("Progress : " + item.Progress);
            Debug.Log("ProgressInDico : " + item.ProgressInDictionnary);
        }
    }


}
