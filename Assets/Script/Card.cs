using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Card : NetworkBehaviour
{
    [SyncVar]
    public CardsColorsEnum Color;
    [SyncVar(hook = "OnChangeValue")]
    public CardsValuesEnum Value;

    public SelectionFilterEnum Filter;

    public delegate void CardEffect(Pawn target);
    public CardEffect Effect;

    public delegate bool CardProjection(Pawn target);
    public List<CardProjection> Projections;

    public Material Illustration;

    public List<Pawn> possibleTargets;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Illustration == null && this.Value != 0)
        {
            OnChangeValue(this.Value);
        }
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
        initCard(value);
        Illustration = Resources.Load<Material>("Materials/Cards/" + this.name);
        this.gameObject.transform.GetChild(1).GetComponentInChildren<MeshRenderer>().material = Illustration;

    }

    #region Card Effect
    private void initCard(CardsValuesEnum value)
    {
        switch (value)
        {
            case CardsValuesEnum.ACE:
                Effect = ACE;
                Filter = SelectionFilterEnum.OWNPAWNS;
                break;
            case CardsValuesEnum.TWO:
                Effect = TWO;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.THREE:
                Effect = THREE;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.FOUR:
                Effect = FOUR;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.FIVE:
                Effect = FIVE;
                Filter = SelectionFilterEnum.OTHERPAWNS;

                break;
            case CardsValuesEnum.SIX:
                Effect = SIX;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.SEVEN:
                Effect = SEVEN;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.EIGHT:
                Effect = EIGHT;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.NINE:
                Effect = NINE;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.TEN:
                Effect = TEN;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.JACK:
                Effect = JACK;
                Filter = SelectionFilterEnum.ALLPAWNS;

                break;
            case CardsValuesEnum.QUEEN:
                Effect = QUEEN;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.KING:
                Effect = KING;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            case CardsValuesEnum.JOKER:
                Effect = JOKER;
                Filter = SelectionFilterEnum.OWNPAWNS;

                break;
            default:
                break;
        }
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
    #region cardFilter
    #endregion

    public void Move(Pawn target)
    {
        target.Move((int)Value);
        Effect(target);
    }
}
