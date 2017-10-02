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

    public SelectionFilterEnum ColorFilter;

    public delegate void CardEffect(Pawn target);
    public CardEffect Effect;

    public delegate bool CardProjection(Pawn target);
    public List<CardProjection> Projections = new List<CardProjection>();

    public Material Illustration;

    public List<Pawn> possibleTargets;

    private GameMaster gMaster;

    public GameMaster GMaster
    {
        get
        {
            if (gMaster == null)
            {
                GMaster = GameObject.Find("NetworkGameMaster").GetComponent<GameMaster>();
            }
            return gMaster;
        }

        set
        {
            gMaster = value;
        }
    }


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
            case CardsValuesEnum.KING:
                Effect = MoveOrEnter;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(ParteuFilter);
                break;
            case CardsValuesEnum.TWO:
            case CardsValuesEnum.THREE:
            case CardsValuesEnum.SIX:
            case CardsValuesEnum.EIGHT:
            case CardsValuesEnum.NINE:
            case CardsValuesEnum.TEN:
            case CardsValuesEnum.QUEEN:
                Effect = Move;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(MoveFilter);
                Projections.Add(OnBoardFilter);
                break;
            case CardsValuesEnum.FOUR:
                Effect = Move;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(OnBoardFilter);
                break;
            case CardsValuesEnum.FIVE:
                Effect = Move;
                ColorFilter = SelectionFilterEnum.OTHERPAWNS;
                Projections.Add(OnBoardFilter);
                Projections.Add(MoveFilter);
                break;
            case CardsValuesEnum.SEVEN:
                Effect = SEVEN;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(OnBoardFilter);
                break;
            case CardsValuesEnum.JACK:
                Effect = JACK;
                ColorFilter = SelectionFilterEnum.ALLPAWNS;
                Projections.Add(OnBoardFilter);
                break;
            case CardsValuesEnum.JOKER:
                Effect = JOKER;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;

                break;
            default:
                break;
        }
    }

    private void JOKER(Pawn target)
    {
    }


    private void JACK(Pawn target)
    {

    }


    private void SEVEN(Pawn target)
    {
    }


    private void FOUR(Pawn target)
    {
        target.Move(-(int)Value);
    }


    private void Move(Pawn target)
    {
        target.Move((int)Value);
    }

    private void MoveOrEnter(Pawn target)
    {
        if (target.OnBoard)
        {
            target.Move((int)Value);
        }
        else
        {
            target.Enter();
        }
    }
    #endregion
    #region cardFilter
    public bool MoveFilter(Pawn target)
    {
        bool Playable = true;
        if (target.Progress + (int)Value > 74)
        {
            Playable = false;
        }
        else
        {
            int progressToCheck = target.Progress + (int)Value + 18 * (int)target.PlayerColor;
            if (GMaster.progressDictionnary.ContainsValue(progressToCheck))
            {
                if (GMaster.progressDictionnary.GetPawn(progressToCheck).Progress == 0)
                {
                    Playable = false;
                }
            }

        }
        return Playable;
    }

    public bool OnBoardFilter(Pawn target)
    {
        return target.OnBoard;
    }

    public bool ParteuFilter(Pawn target)
    {
        bool Playable = true;
        if (target.OnBoard)
        {
            Playable = MoveFilter(target);
        }
        return Playable;
    }
    #endregion
    public void MakeProjections(List<Pawn> listToTest)
    {
        bool playable;
        foreach (Pawn item in listToTest)
        {
            playable = true;
            foreach (CardProjection projection in this.Projections)
            {
                if (!projection(item))
                {
                    playable = false;
                    break;
                }
            }
            if (playable)
            {
                possibleTargets.Add(item);
            }
        }
    }


    public void Play(Pawn target)
    {
        Effect(target);
    }
}
