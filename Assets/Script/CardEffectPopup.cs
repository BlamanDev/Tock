using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectPopup : MonoBehaviour {
    public RectTransform CardEffectList;
    private DicoDescription dicoDesc;
    public GameObject DescriptionLinePrefab;

    public DicoDescription DicoDesc
    {
        get
        {
            if (dicoDesc == null)
            {
                dicoDesc = GameObject.FindObjectOfType<DicoDescription>();
            }
            return dicoDesc;
        }

        set
        {
            dicoDesc = value;
        }
    }

    // Use this for initialization
    void Start () {
        BuildList();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BuildList()
    {
        foreach (string item in DicoDescription.DicoText.Keys)
        {
            GameObject descLine = Instantiate(DescriptionLinePrefab, CardEffectList);
            descLine.GetComponent<DescriptionLine>().Description.text = DicoDescription.DicoText[item];
            descLine.GetComponent<DescriptionLine>().Icon.sprite = DicoDescription.DicoImage[item];
            //descLine.transform.SetParent(CardEffectList);
            descLine.transform.SetAsLastSibling();
        }
    }
}
