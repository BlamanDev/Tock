using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositions : MonoBehaviour {
    private int currentPosition = 0;
    private List<Transform> positionsList;

    public List<Transform> PositionsList
    {
        get
        {
            if (positionsList == null)
            {
                positionsList = new List<Transform>();
                this.GetComponentsInChildren<Transform>(PositionsList);
                positionsList.RemoveAt(0);
                
            }
            return positionsList;
        }

        set
        {
            positionsList = value;
        }
    }

    public KeyValuePair<Vector3,Quaternion> NextCamera()
    {
        currentPosition++;
        if (currentPosition == PositionsList.Count)
        {
            currentPosition = 0;
        }
        return new KeyValuePair<Vector3, Quaternion>(PositionsList[currentPosition].position, PositionsList[currentPosition].rotation);
    }

    public KeyValuePair<Vector3, Quaternion> PreviousCamera()
    {
        currentPosition--;
        if (currentPosition < 0)
        {
            currentPosition = positionsList.Count -1;
        }
        return new KeyValuePair<Vector3, Quaternion>(PositionsList[currentPosition].position, PositionsList[currentPosition].rotation);
    }

    public KeyValuePair<Vector3, Quaternion> GetCameraforPlayer(int playerIndex)
    {
        currentPosition = playerIndex * 2;
        return new KeyValuePair<Vector3, Quaternion>(PositionsList[currentPosition].position, PositionsList[currentPosition].rotation);

    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
