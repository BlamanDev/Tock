using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Card : NetworkBehaviour
{
    public CardsColorsEnum Color;
    public CardsValuesEnum Value;

    public delegate void CardEffect(Pawn target);
    public CardEffect Effect;

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
        this.name = value.ToString() + "_" + color.ToString();
        Effect = getCardEffect(value);
        
        this.gameObject.GetComponentInChildren<MeshRenderer>().material = Resources.Load<Material>("\\Materials\\"+ this.name);
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
        throw new NotImplementedException();
    }

    private void KING(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void QUEEN(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void JACK(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void TEN(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void NINE(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void EIGHT(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void SEVEN(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void SIX(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void FIVE(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void FOUR(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void THREE(Pawn target)
    {
        throw new NotImplementedException();
    }

    private void TWO(Pawn target)
    {
        throw new NotImplementedException();
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
