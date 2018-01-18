using UnityEngine;

public class BtnGameBegin : MonoBehaviour {
    public Canvas GameCanvas;
    public GameObject Toc;
    public GameObject Deck;
    public GameMaster NetworkGameMaster;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GameBegin()
    {
        DisplayGameCanvas();
        Deck.GetComponentInChildren<MeshRenderer>().enabled = true;
        NetworkGameMaster.GameBegin();
    }

    public void DisplayGameCanvas()
    {
        GameCanvas.enabled = true;
        Toc.SetActive(true);
        this.GetComponentInParent<Canvas>().enabled = false;

    }
}
