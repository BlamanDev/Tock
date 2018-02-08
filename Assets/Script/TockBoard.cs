using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Script associated with the board, handle the camera
/// </summary>
public class TockBoard : NetworkBehaviour
{
    public int NB_CASES;
    private Coroutine cameraRotation;
    public float RotationWaitTime = 0.01f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Place the camera accordingly to the player index
    /// </summary>
    /// <param name="playerIndex"></param>
    public void SetInitialCameraPostion(int playerIndex)
    {
        Camera.main.transform.RotateAround(this.transform.position, Vector3.up, 90*playerIndex);
    }

    /// <summary>
    /// Start moving the camera
    /// </summary>
    /// <param name="angle">angle step</param>
    public void StartMovingCamera(float angle = 45)
    {
        cameraRotation = StartCoroutine(rotateCamera(angle));
    }

    /// <summary>
    /// Stop the camera rotation
    /// </summary>
    public void StopMovingCamera()
    {
        StopCoroutine(cameraRotation);
    }

    /// <summary>
    /// Make the camera rotate endlessly
    /// </summary>
    /// <param name="angle">angle step</param>
    /// <returns></returns>
    IEnumerator rotateCamera(float angle=45)
    {
        while(true)
        {
            Camera.main.transform.RotateAround(this.transform.position, Vector3.up, angle);
            yield return new WaitForSecondsRealtime(RotationWaitTime);
        }
    }
}
