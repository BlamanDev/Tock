using DFTGames.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class DicoDescription : MonoBehaviour {
    public const String DESCRIPTION_SUFFIX = "_cardeffects";
    public const String CARDEFFECTSPRITEFOLDER = "Icons/Cards/Effects";
    public const String DESC_MOVE = "Move";
    public const String DESC_MOVEORENTER = "MoveOrEnter";
    public const String DESC_MOVEOTHER = "MoveOther";
    public const String DESC_MOVEBACKWARD = "MoveBackWard";
    public const String DESC_EXCHANGE = "Exchange";
    public const String DESC_MOVEWIPEALL = "MoveWipeAll";
    public const String DESC_MOVEMANY = "MoveMany";
    public const String VALUEINTEXT = "[VALUE]";

    private static Dictionary<String, String> dicoText;
    private static Dictionary<String, Sprite> dicoImage;

    public static Dictionary<string, Sprite> DicoImage
    {
        get
        {
            if (dicoImage == null)
            {
                buildDicoImage();
            }
            return dicoImage;
        }

        set
        {
            dicoImage = value;
        }
    }

    private static void buildDicoImage()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(CARDEFFECTSPRITEFOLDER);
        dicoImage = new Dictionary<string, Sprite>();
        if (sprites.Length>0)
        {
            foreach (Sprite item in sprites)
            {
                dicoImage.Add(item.name, item);
            }

        }
    }

    public static Dictionary<string, string> DicoText
    {
        get
        {
            if (dicoText == null)
            {
                buildDicoText();
            }
            return dicoText;
        }

        set
        {
            dicoText = value;
        }
    }

    private static void buildDicoText()
    {
        dicoText = new Dictionary<string, string>();
        TextAsset allLines = new TextAsset();
        try
        {
            allLines = Resources.Load<TextAsset>(Locale.STR_LOCALIZATION_PREFIX + Locale.currentLanguage + DESCRIPTION_SUFFIX);
                // We wplit on newlines to retrieve the key pairs
                string[] lines = allLines.text.Split(new string[] { "\r\n", "\n\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] pairs = lines[i].Split(new char[] { '\t', '=', ';' }, 2);
                    if (pairs.Length == 2)
                    {
                        DicoText.Add(pairs[0].Trim(), pairs[1].Trim());
                    }
                }
            

            /*using (XmlReader reader = XmlReader.Create(new StringReader(allLines.text)))
            {
                reader.Read();
                reader.ReadSubtree();
                while (reader.Read())
                {
                    dicoText.Add(reader.GetAttribute("name"), reader.GetAttribute("description"));
                }
            }*/


        }
        catch (Exception e)
        {
            Debug.Log("Problem building dicoDescription : " + e.Message);
        }
    }

    public KeyValuePair<String,Sprite> GetEffect(String effect)
    {
        return new KeyValuePair<string, Sprite>(DicoText[effect], DicoImage[effect]);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
