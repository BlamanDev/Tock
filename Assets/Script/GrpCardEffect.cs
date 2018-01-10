using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrpCardEffect : MonoBehaviour
{
    public Text LblSelectedCard;
    public Text CardValueText;
    public Image CardColorImage;
    public Image CardEffectImage;
    public Text MoveText;
    public Text BackText;
    private DicoDescription dicoDescription;
    private Card selectedCard;

    public DicoDescription DicoDesc
    {
        get
        {
            if (dicoDescription == null)
            {
                dicoDescription = GameObject.FindObjectOfType<DicoDescription>();
            }
            return dicoDescription;
        }

        set
        {
            dicoDescription = value;
        }
    }

    public Card SelectedCard
    {
        get
        {
            return selectedCard;
        }

        set
        {
            selectedCard = value;
            DisplayCardEffect(value);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayCardEffect(Card cardToDisplay)
    {
        if (cardToDisplay != null)
        {
            KeyValuePair<String, Sprite> effect = DicoDesc.GetEffect(cardToDisplay.Description);

            
            LblSelectedCard.enabled = (cardToDisplay == SelectedCard);
            CardValueText.text = cardToDisplay.Value.ToString();
            CardColorImage.enabled = true;
            CardColorImage.sprite = cardToDisplay.ColorImage;

            CardEffectImage.sprite = effect.Value;
            CardEffectImage.color = Color.white;

            switch (cardToDisplay.Description)
            {
                case DicoDescription.DESC_MOVE:
                case DicoDescription.DESC_MOVEORENTER:
                case DicoDescription.DESC_MOVEOTHER:
                case DicoDescription.DESC_MOVEWIPEALL:
                    MoveText.enabled = true;
                    BackText.enabled = false;
                    MoveText.text = ((int)cardToDisplay.Value).ToString();
                    BackText.text = "";
                    break;
                case DicoDescription.DESC_MOVEBACKWARD:
                    MoveText.enabled = false;
                    BackText.enabled = true;
                    MoveText.text = "";
                    BackText.text = ((int)cardToDisplay.Value).ToString();
                    break;
                default:
                    MoveText.enabled = false;
                    BackText.enabled = false;
                    MoveText.text = "";
                    BackText.text = "";
                    break;
            }

        }
        else
        {
            ClearCardEffect();

        }
    }

    public void ClearCardEffect()
    {
        if (selectedCard != null)
        {
            DisplayCardEffect(selectedCard);

        }
        else
        {
            LblSelectedCard.enabled = false;
            MoveText.enabled = false;
            BackText.enabled = false;
            MoveText.text = "";
            BackText.text = "";
            CardValueText.text = "";
            CardEffectImage.sprite = null;
            CardEffectImage.color = Color.clear;
            CardColorImage.enabled = false;
        }
    }
}
