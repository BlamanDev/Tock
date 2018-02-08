using DFTGames.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour {
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeLanguage()
    {
        if (Locale.CurrentLanguage.Equals(SystemLanguage.French.ToString()))
        {
            Localize.SetCurrentLanguage(SystemLanguage.English);
            
        }
        else
        {
            Localize.SetCurrentLanguage(SystemLanguage.French);
        }
        LocalizeImage.SetCurrentLanguage();

    }

}
