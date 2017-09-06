using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Card : NetworkBehaviour
{
    [SyncVar]
    public CardsColorsEnum Color;
    [SyncVar(hook ="OnChangeValue")]
    public CardsValuesEnum Value;

    public delegate void CardEffect(Pawn target);
    public CardEffect Effect;

    public Material Illustration;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize(CardsColorsEnum color, CardsValuesEnum value)
    {
        Color = color;
        Value = value;

    }

    public void OnChangeValue(CardsValuesEnum value)
    {
        Value = value;
        this.name = value.ToString() + "_" + Color.ToString();
        Effect = getCardEffect(value);
        Illustration = Resources.Load<Material>("Materials/Cards/" + this.name);
        this.gameObject.transform.GetChild(1).GetComponentInChildren<MeshRenderer>().material = Illustration;

    }

    #region Card Effect
    private CardEffect getCardEffect(CardsValuesEnum value)
    {
        CardEffect methodChosen = null;
        switch (value)
        {
            case CardsValuesEnum.ACE:
                methodChosen = ACE;
                break;
            case CardsValuesEnum.TWO:
                methodChosen = TWO;
                break;
            case CardsValuesEnum.THREE:
                methodChosen = THREE;
                break;
            case CardsValuesEnum.FOUR:
                methodChosen = FOUR;
                break;
            case CardsValuesEnum.FIVE:
                methodChosen = FIVE;
                break;
            case CardsValuesEnum.SIX:
                methodChosen = SIX;
                break;
            case CardsValuesEnum.SEVEN:
                methodChosen = SEVEN;
                break;
            case CardsValuesEnum.EIGHT:
                methodChosen = EIGHT;
                break;
            case CardsValuesEnum.NINE:
                methodChosen = NINE;
                break;
            case CardsValuesEnum.TEN:
                methodChosen = TEN;
                break;
            case CardsValuesEnum.JACK:
                methodChosen = JACK;
                break;
            case CardsValuesEnum.QUEEN:
                methodChosen = QUEEN;
                break;
            case CardsValuesEnum.KING:
                methodChosen = KING;
                break;
            case CardsValuesEnum.JOKER:
                methodChosen = JOKER;
                break;
            default:
                break;
        }
        return methodChosen;
    }

    private void JOKER(Pawn target)
    {
    }

    private void KING(Pawn target)
    {
    }

    private void QUEEN(Pawn target)
    {
    }

    private void JACK(Pawn target)
    {
    }

    private void TEN(Pawn target)
    {
    }

    private void NINE(Pawn target)
    {
    }

    private void EIGHT(Pawn target)
    {
    }

    private void SEVEN(Pawn target)
    {
    }

    private void SIX(Pawn target)
    {
    }

    private void FIVE(Pawn target)
    {
    }

    private void FOUR(Pawn target)
    {
    }

    private void THREE(Pawn target)
    {
    }

    private void TWO(Pawn target)
    {
    }

    private void ACE(Pawn target)
    {

    }
    #endregion

public void Move(Pawn target)
    {
        target.Move((int)Value);
        Effect(target);
    }
}
