using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TockBoard : NetworkBehaviour
{
    public int NB_CASES;
    private Coroutine cameraRotation;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetInitialPostion(int playerIndex)
    {
        Camera.main.transform.RotateAround(this.transform.position, Vector3.up, 90*playerIndex);
    }

    public void StartMovingCamera(float angle = 45)
    {
        cameraRotation = StartCoroutine(rotateCamera(angle));
    }

    public void StopMovingCamera()
    {
        StopCoroutine(cameraRotation);
    }

    IEnumerator rotateCamera(float angle=45)
    {
        while(true)
        {
            Camera.main.transform.RotateAround(this.transform.position, Vector3.up, angle);
            yield return new WaitForEndOfFrame();
        }
    }
}
