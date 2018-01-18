using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;

/// <summary>
/// Card script
/// </summary>
public class Card : NetworkBehaviour
{
    /*public const String DESCRIPTIONFILE = "Texts/Descriptions";
    public const String DESC_MOVE = "Move";
    public const String DESC_MOVEORENTER = "MoveOrEnter";
    public const String DESC_MOVEOTHER = "MoveOther";
    public const String DESC_MOVEBACKWARD = "MoveBackWard";
    public const String DESC_EXCHANGE = "Exchange";
    public const String DESC_MOVEWIPEALL = "MoveWipeAll";
    public const String DESC_MOVEMANY = "MoveMany";
    public const String VALUEINTEXT = "[VALUE]";

    private static Dictionary<String, String> dicoDescription;
    */
    [SyncVar]
    public CardsColorsEnum Color;
    [SyncVar(hook = "OnChangeValue")]
    public CardsValuesEnum Value;

    public SelectionFilterEnum ColorFilter;

    private String description;
    public Sprite EffectImage;

    private Sprite colorImage;

    public delegate void CardEffect(Pawn target, Pawn otherTarget = null);
    public CardEffect Effect;

    public delegate bool CardProjection(Pawn target);
    public List<CardProjection> Projections = new List<CardProjection>();

    public Material Illustration;

    //Pawn which can be played by this card after projection
    public List<Pawn> possibleTargets;
    public SyncListString possibleTargetsS;

    static private GameMaster gMaster;

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

    public string Description
    {
        get
        {
            return description;
        }

        set
        {
            description = value;
        }
    }

    public Sprite ColorImage
    {
        get
        {
            if (colorImage==null)
            {
                colorImage = Resources.Load<Sprite>("Icons/Cards/Colors/" + Color.ToString());
            }
            return colorImage;
        }

        set
        {
            colorImage = value;
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

    /// <summary>
    /// Set the color and value of this card
    /// </summary>
    /// <param name="color"></param>
    /// <param name="value"></param>
    public void Initialize(CardsColorsEnum color, CardsValuesEnum value)
    {
        Color = color;
        
        Value = value;

    }

    /// <summary>
    /// Update the card attributes according to the new value
    /// </summary>
    /// <param name="value"></param>
    public void OnChangeValue(CardsValuesEnum value)
    {
        Value = value;
        this.name = value.ToString() + "_" + Color.ToString();
        initCard(value);
        Illustration = Resources.Load<Material>("Materials/Cards/" + this.name);
        this.gameObject.transform.GetChild(1).GetComponentInChildren<MeshRenderer>().material = Illustration;


    }

    #region Card Effect
    /// <summary>
    /// Initialise the card according to the new value, Effect and Filter
    /// </summary>
    /// <param name="value"></param>
    private void initCard(CardsValuesEnum value)
    {
        switch (value)
        {
            case CardsValuesEnum.ACE:
            case CardsValuesEnum.KING:
                Effect = MoveOrEnter;
                Description = DicoDescription.DESC_MOVEORENTER;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(ParteuFilter);
                break;
            /*case CardsValuesEnum.TWO:
            case CardsValuesEnum.THREE:
            case CardsValuesEnum.SIX:
            case CardsValuesEnum.EIGHT:
            case CardsValuesEnum.NINE:
            case CardsValuesEnum.TEN:
            case CardsValuesEnum.QUEEN:
                Effect = Move;
                Description = DESC_MOVE;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(OnBoardFilter);
                Projections.Add(MoveFilter);
                break;*/
            case CardsValuesEnum.FOUR:
                Effect = MoveBackward;
                Description = DicoDescription.DESC_MOVEBACKWARD;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(OnBoardFilter);
                Projections.Add(MoveFilter);
                break;
            case CardsValuesEnum.FIVE:
                Effect = Move;
                Description = DicoDescription.DESC_MOVEOTHER;
                ColorFilter = SelectionFilterEnum.OTHERPAWNS;
                Projections.Add(OnBoardFilter);
                Projections.Add(IdleFilter);
                Projections.Add(MoveFilter);
                break;
            /*case CardsValuesEnum.SEVEN:
                Effect = MoveMany;
                Description = DESC_MOVEMANY;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(OnBoardFilter);
                break;*/
            case CardsValuesEnum.JACK:
                Effect = Exchange;
                Description = DicoDescription.DESC_EXCHANGE;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(OnBoardFilter);
                Projections.Add(IdleFilter);
                break;
            case CardsValuesEnum.JOKER:
                Effect = JOKER;
                Description = DicoDescription.DESC_MOVEWIPEALL;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(ParteuFilter);
                break;
            default:
                Effect = Move;
                Description = DicoDescription.DESC_MOVE;
                ColorFilter = SelectionFilterEnum.OWNPAWNS;
                Projections.Add(OnBoardFilter);
                Projections.Add(MoveFilter);

                break;
        }

    }

    /// <summary>
    /// Make the target enter the board or move the target according to the value of the card wiping all pawns in the way
    /// </summary>
    /// <param name="target"></param>
    /// <param name="otherTarget"></param>
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

    /// <summary>
    /// Exchange the places of the targets
    /// </summary>
    /// <param name="target"></param>
    /// <param name="otherTarget"></param>
    public void Exchange(Pawn target, Pawn otherTarget)
    {
        target.Exchange(otherTarget);
    }

    /// <summary>
    /// Not used, Seven logic is done in the TockPlayer class
    /// </summary>
    /// <param name="target"></param>
    /// <param name="otherTarget"></param>
    public void MoveMany(Pawn target, Pawn otherTarget = null)
    {
        target.Move(1);
    }

    /// <summary>
    /// Move the target backward
    /// </summary>
    /// <param name="target"></param>
    /// <param name="otherTarget"></param>
    public void MoveBackward(Pawn target, Pawn otherTarget = null)
    {
        target.Move(-(int)Value);
    }

    /// <summary>
    /// Move the target forward
    /// </summary>
    /// <param name="target"></param>
    /// <param name="otherTarget"></param>
    public void Move(Pawn target, Pawn otherTarget = null)
    {
        target.Move((int)Value);
    }

    /// <summary>
    /// Make the target enter the board or move the target according to the value of the card
    /// </summary>
    /// <param name="target"></param>
    /// <param name="otherTarget"></param>
    public void MoveOrEnter(Pawn target, Pawn otherTarget = null)
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
    /// <summary>
    /// Test if the pawn target can move according the value of the card
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool MoveFilter(Pawn target)
    {
        return MoveFiltering(target, (int)this.Value);
    }

    /// <summary>
    /// Test if the pawn target can move according the value of the card or the specified number of cell
    /// </summary>
    /// <param name="target"></param>
    /// <param name="nbMoves">int - if -1, test with value of the card, else test with the specified number</param>
    /// <returns></returns>
    public bool MoveFiltering(Pawn target, int nbMoves)
    {
        //Debug.Log("MoveFiltering - target : " + target.name + " nbMoves : " + nbMoves.ToString());
        bool Playable = true;
        //Debug.Log("Card Value : " + this.Value + " - ProgressToCheck before : " + progressToCheck);
        int progressToCheck = target.Progress + (nbMoves * (this.Effect == MoveBackward ? -1 : 1));

        //IF target has finished => not playable
        if (progressToCheck > 74 || progressToCheck < 0  )
        {
            //Debug.Log(target.name + " : Progress > 74 OR < 0 => can't move");
            Playable = false;
        }
        else
        {

            //Compute the progress according to the color of the pawn and the value of the card
            //progressToCheck += 18 * target.OwningPlayerIndex;
            //Debug.Log("Card Value : " + this.Value + " - ProgressToCheck after : " + progressToCheck);

            //Get the list of pawn to be tested
            List<Pawn> pawnEncoutered = GMaster.ProgressListGetPawnsInRange(target.Progress + 1 + 18 * target.OwningPlayerIndex, progressToCheck + (18 * target.OwningPlayerIndex));

            //Test if there is pawn on its starting position
            if (pawnEncoutered.Count > 0)
            {
                //Debug.Log(this.name + " - " + "Projection : " + target.name + " encoutered " + pawnEncoutered.Count + " Pawns :");

                foreach (Pawn item in pawnEncoutered)
                {
                    //Debug.Log(this.name + " - " + target.name + " - " + item.name + " : progressDico = " + item.ProgressInDictionnary + " ; progress = " + item.Progress + " ; Status : " + item.Status.ToString());
                    if (item.Progress == 0)
                    {
                        //Debug.Log(this.name + " - " + target.name + " : " + item.name + " is in ENTRY => can't move");
                        Playable = false;
                    }
                }
            }
            //Test if there is a pawn on the destination and if it is in house
            if ((progressToCheck > 70) && (GMaster.ProgressListTestHouseFree(progressToCheck, target.OwningPlayerIndex)))
            {
                //Debug.Log(target.name + " : " + "A Pawn is in this HOUSE Cell => can't move");
                Playable = false;
            }
        }
        //Debug.Log(target.name + " playable : " + Playable.ToString());
        return Playable;
    }

    /// <summary>
    /// Test if the target is on the board
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool OnBoardFilter(Pawn target)
    {
        return target.OnBoard;
    }

    public bool IdleFilter(Pawn target)
    {
        return target.Status == PawnStatusEnum.IDLE;
    }

    /// <summary>
    /// Test if the target is on the board, and if can make the move
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool ParteuFilter(Pawn target)
    {
        bool Playable = true;
        if (target.OnBoard)
        {
            Playable = MoveFilter(target);
        }
        return Playable;
    }

    public bool NotOnEntryFilter(Pawn target)
    {
        return target.Progress != 0;
    }
    #endregion
    #region Projection
    /// <summary>
    /// Test if every pawn in the given list can be played with the card
    /// </summary>
    /// <param name="listToTest"></param>
    /// <returns></returns>
    public bool MakeProjections(List<Pawn> listToTest)
    {
        bool playable = true;
        possibleTargetsS.Clear();
        //IF the card is Seven, use the projection function specific to the seven
        if (this.Effect == MoveMany)
        {
            playable = ProjectionMoveMany(listToTest);
        }
        else
        {
            foreach (Pawn item in listToTest)
            {
                playable = true;
                //test the pawn with each filter of the card
                foreach (CardProjection projection in this.Projections)
                {
                    if (!projection(item))
                    {
                        playable = false;
                        //Debug.Assert(!playable, item.name + " not playable : " + projection.Method.ToString());

                        break;
                    }
                }
                if (playable)
                {
                    possibleTargetsS.Add(item.name);
                }
            }
        }
        if (this.Effect == this.Exchange && getNbOfPawnOnBoard() < 2)
        {
            possibleTargetsS.Clear();
        }

        return possibleTargetsS.Count > 0;
    }

    /// <summary>
    /// Test if Seven Card can be played
    /// </summary>
    /// <param name="listToTest"></param>
    /// <returns></returns>
    private bool ProjectionMoveMany(List<Pawn> listToTest)
    {
        bool playable = true;
        int indexPawn = 0;
        int movementAdded = 1;
        int movemenTotal = 0;

        for (indexPawn = 0; indexPawn < listToTest.Count; indexPawn++)
        {
            Pawn pawnTested = listToTest[indexPawn];
            if (pawnTested.OnBoard && GMaster.ProgressList.Contains(pawnTested.name))
            {
                movementAdded = 1;
                //Compute movement max for the pawn
                while (String.IsNullOrEmpty(GMaster.ProgressList[pawnTested.ProgressInDictionnary + movementAdded]) && (movementAdded < 8))
                {
                    movementAdded++;
                    movemenTotal++;
                }
                //IF the pawn can move for minimum 1 cell, add it to possibles argets
                if (movementAdded > 1)
                {
                    possibleTargetsS.Add(pawnTested.name);
                }
            }
        }
        //IF movement total is inferior to the card value
        if (movemenTotal < (int)Value)
        {
            playable = false;
        }
        return playable;
    }
    #endregion
    /// <summary>
    /// Apply the card effect on the targetted pawn
    /// </summary>
    /// <param name="target"></param>
    /// <param name="otherTarget"></param>
    public void Play(Pawn target, Pawn otherTarget = null)
    {
        Effect(target, otherTarget);
    }

    private int getNbOfPawnOnBoard()
    {
        Pawn[] pawnsOnBoard = GameObject.FindObjectsOfType<Pawn>();
        pawnsOnBoard = Array.FindAll(pawnsOnBoard, x => x.OnBoard);
        return pawnsOnBoard.Length;
    }
}
