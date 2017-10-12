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

    public delegate void CardEffect(Pawn target, Pawn otherTarget = null);
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
                Effect = FOUR;
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
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(OnBoardFilter);
                break;
            case CardsValuesEnum.JOKER:
                Effect = JOKER;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(ParteuFilter);
                break;
            default:
                break;
        }
    }

    private void JOKER(Pawn target, Pawn otherTarget = null)
    {
        if (target.OnBoard)
        {
            target.Move((int)Value, true);
        }
        else
        {
            target.Enter();
        }
    }


    private void JACK(Pawn target, Pawn otherTarget)
    {
        target.Exchange(otherTarget);
    }


    private void SEVEN(Pawn target, Pawn otherTarget = null)
    {

    }


    private void FOUR(Pawn target, Pawn otherTarget = null)
    {
        target.Move(-(int)Value);
    }


    private void Move(Pawn target, Pawn otherTarget = null)
    {
        target.Move((int)Value);
    }

    private void MoveOrEnter(Pawn target, Pawn otherTarget = null)
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
        return MoveFiltering(target);
    }

    public bool MoveFiltering(Pawn target, int nbMoves=-1)
    {
        bool Playable = true;
        if (nbMoves==-1)
        {
            nbMoves = (int)Value;
        }
        if (target.Progress + (nbMoves * (this.Value == CardsValuesEnum.FOUR ? -1 : 1)) > 74)
        {
            Playable = false;
        }
        else
        {
            int progressToCheck = target.Progress + nbMoves * (this.Value == CardsValuesEnum.FOUR ? -1 : 1) + 18 * (int)target.PlayerColor;
            List<Pawn> pawnEncoutered = GMaster.progressDictionnary.GetPawnsInRange(target.Progress + 18 * (int)target.PlayerColor, progressToCheck);
            if (pawnEncoutered.Count > 0)
            {
                foreach (Pawn item in pawnEncoutered)
                {
                    if (item.Status == PawnStatusEnum.ENTRY)
                    {
                        Playable = false;
                    }
                }
            }
            if (GMaster.progressDictionnary.ContainsValue(progressToCheck))
            {
                if (GMaster.progressDictionnary.GetPawn(progressToCheck).Status == PawnStatusEnum.IN_HOUSE)
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

    public bool SevenFilter(Pawn target)
    {
        bool Playable = true;



        return Playable;
    }
    #endregion
    public bool MakeProjections(List<Pawn> listToTest)
    {
        bool playable = true;
        possibleTargets.Clear();
        if (this.Value == CardsValuesEnum.SEVEN)
        {
            playable = ProjectionSeven(listToTest);
        }
        else
        {
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
        return playable;
    }

    private bool ProjectionSeven(List<Pawn> listToTest)
    {
        bool playable = true;
        int indexPawn = 0;
        int movementAdded = 1;
        int movemenTotal = 0;

        while (indexPawn < listToTest.Count)
        {
            Pawn pawnTested = listToTest[indexPawn];
            if (pawnTested.OnBoard)
            {
                movementAdded = 1;
                while (!GMaster.progressDictionnary.ContainsValue(GMaster.progressDictionnary[pawnTested] + movementAdded) && (movementAdded < 8))
                {
                    movementAdded++;
                    movemenTotal++;
                }
                if (movementAdded > 1)
                {
                    possibleTargets.Add(pawnTested);
                }
            }
            indexPawn++;
        }
        if (movemenTotal < 7)
        {
            playable = false;
        }
        return playable;
    }

    public void Play(Pawn target, Pawn otherTarget = null)
    {
        Effect(target, otherTarget);
    }
}
