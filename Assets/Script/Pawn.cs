using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Pawn Script
/// 
/// </summary>
public class Pawn : NetworkBehaviour
{
    #region Attributes
    //Progress of the Pawn on its path
    [SyncVar]
    public int Progress = 0;

    public int ProgressInDictionnary;

    //Index of the Pawn
    [SyncVar]
    public int PawnIndex;

    //Pawn on the board ?
    [SyncVar(hook = "OnChangeOnBoard")]
    public bool OnBoard = false;

    private bool wipeAllPawns = false;

    //Owning player of this Pawn
    [SyncVar(hook = "OnChangePlayerColor")]
    public PlayerColorEnum PlayerColor;

    [SyncVar]
    private PawnStatusEnum status=PawnStatusEnum.OUT;

    //Spawn positions for the pawns
    private SpawnPositions spawnPositions;
    private GameObject outPosition;

    #region Components
    //Components of the pawn
    private Animator pawnAnimator;
    private MeshRenderer pawnMeshRenderer;
    private Light selectableLight;
    #endregion
    private GameMaster gMaster;

    #region Hash
    //Hash of Animator parameters
    private int enterHash = Animator.StringToHash("EnterBoard");
    private int exitHash = Animator.StringToHash("ExitBoard");
    private int StateHash = Animator.StringToHash("ProgressOnBoard");
    private int speedHash = Animator.StringToHash("Speed");
    #endregion


    //public GameObject GhostPawnPrefab;

    private Color actualHaloColor;

    #region properties
    public Color ActualHaloColor
    {
        get
        {
            return actualHaloColor;
        }

        set
        {
            actualHaloColor = value;
            SelectableLight.color = value;
        }
    }

    public Light SelectableLight
    {
        get
        {
            if (selectableLight == null)
            {
                selectableLight = this.GetComponentInChildren<Light>();

            }
            return selectableLight;
        }
        set
        {
            selectableLight = value;
        }
    }

    public GameMaster GMaster
    {
        get
        {
            if (gMaster == null)
            {
                gMaster = FindObjectOfType<GameMaster>();
            }
            return gMaster;
        }
        set
        {
            gMaster = value;
        }
    }

    public Animator PawnAnimator
    {
        get
        {
            if (pawnAnimator == null)
            {
                pawnAnimator = GetComponent<Animator>();
            }
            return pawnAnimator;
        }
        set
        {
            pawnAnimator = value;
        }
    }

    public MeshRenderer PawnMeshRenderer
    {
        get
        {
            if (pawnMeshRenderer == null)
            {
                pawnMeshRenderer = this.GetComponentInChildren<MeshRenderer>();
            }
            return pawnMeshRenderer;
        }
        set
        {
            pawnMeshRenderer = value;
        }
    }

    public SpawnPositions SpawnPositions
    {
        get
        {
            if (spawnPositions == null)
            {
                spawnPositions = FindObjectOfType<SpawnPositions>();

            }
            return spawnPositions;
        }
        set
        {
            spawnPositions = value;
        }
    }

    public PawnStatusEnum Status
    {
        get
        {
            return status;
        }
        set
        {
            if (value == PawnStatusEnum.IDLE && this.Progress > 70)
            {
                status = PawnStatusEnum.IN_HOUSE;
            }
            else
            {
                status = value;
            }
        }
    }
    #endregion

    #endregion

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {


    }

    #region Events
    //Event fired when a pawn is selected
    public delegate void OnPawnSelected(Pawn pawnSelected);
    public event OnPawnSelected EventOnPawnSelected;

    /// <summary>
    /// Event called when the pawn is created
    /// Get the component attached to the pawn
    /// </summary> 
    private void OnEnable()
    {
    }


    /// <summary>
    /// Event called when changing the owning Player
    /// </summary>
    /// <param name="newColor"></param>
    public void OnChangePlayerColor(PlayerColorEnum newColor)
    {
        PlayerColor = newColor;
        //Change the material color of the pawn
        PawnMeshRenderer.material.color = PlayerColorEnumToColor(newColor);

        //Pawn named after its color and index
        this.name = newColor.ToString() + PawnIndex.ToString();
        //Get the out position for this pawn
        outPosition = SpawnPositions.getOutPosition(newColor, PawnIndex);
    }

    /// <summary>
    /// Fire the OnChangePlayerColor
    /// </summary>
    public override void OnStartClient()
    {
        OnChangePlayerColor(PlayerColor);
    }

    /// <summary>
    /// Convert a PlayerColorEnum into a Color
    /// </summary>
    /// <param name="newColor"></param>
    /// <returns></returns>
    private Color PlayerColorEnumToColor(PlayerColorEnum newColor)
    {
        Color color = Color.clear;
        switch (newColor)
        {
            case PlayerColorEnum.Blue:
                color = Color.blue;
                break;
            case PlayerColorEnum.Green:
                color = Color.green;
                break;
            case PlayerColorEnum.Red:
                color = Color.red;
                break;
            case PlayerColorEnum.Yellow:
                color = Color.yellow;
                break;
        }
        return color;
    }

    /// <summary>
    /// Update the position of the pawn regarding if it is entering or exiting the board
    /// </summary>
    /// <param name="onBoard"></param>
    public void OnChangeOnBoard(bool onBoard)
    {
        OnBoard = onBoard;
        if (!PawnAnimator.enabled)
        {
            PawnAnimator.enabled = true;
        }
        if (onBoard)
        {
            //Place the pawn in its entry position et fire the Entry animation
            Transform startTransform = SpawnPositions.getStartPosition(PlayerColor).transform;
            this.transform.position = startTransform.position;
            this.transform.rotation = startTransform.rotation;

            PawnAnimator.SetTrigger(enterHash);
            Status = PawnStatusEnum.ENTRY;
        }
        else
        {
            PawnAnimator.SetTrigger(exitHash);
            Status = PawnStatusEnum.OUT;
        }
    }

    /// <summary>
    /// Collision detection
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //IF object collided is a Pawn
        if (other.name == "PawnModel")
        {
            Pawn pawnCollided = other.GetComponentInParent<Pawn>();
            //IF this pawn is moving and the other isn't moving
            //Useful when pawns are switching positions
            if (this.Status == PawnStatusEnum.MOVING && pawnCollided.Status == PawnStatusEnum.IDLE)
            {
                //IF wipeAllPawns is true or if the other pawn is at the same postion in the progressDico
                if (wipeAllPawns || (GMaster.progressDictionnary[pawnCollided] == GMaster.progressDictionnary[this]))
                {
                    //THEN remove the other pawn from the board
                    other.GetComponentInParent<Pawn>().Exit();
                }
            }
        }
    }

    /// <summary>
    /// Check if the pawn is arrived at destination
    /// </summary>
    /// <param name="animationProgress"></param>
    public void CheckProgress(int animationProgress)
    {
        if (animationProgress == Progress)
        {
            PawnAnimator.SetFloat(speedHash, 0);
            if (Progress>70)
            {
                Status = PawnStatusEnum.IN_HOUSE;

            }
            else
            {
                Status = PawnStatusEnum.IDLE;
            }
            wipeAllPawns = false;
        }
    }
    #endregion

    #region methods
    /// <summary>
    /// Set the color, index, layer used and out position of the pawn
    /// </summary>
    /// <param name="color"></param>
    /// <param name="pawnIndex"></param>
    public void Initialize(PlayerColorEnum color, int pawnIndex)
    {
        this.PawnIndex = pawnIndex;
        PlayerColor = color;
        this.transform.position = outPosition.transform.position;

    }

    /// <summary>
    /// Get the pawn on the board
    /// </summary>
    public void Enter()
    {
        OnBoard = true;
        GMaster.LocalPlayer.RpcAddtoProgressDictionnary(this.name);
    }

    /// <summary>
    /// Move the Pawn
    /// </summary>
    /// <param name="nbCell">int : unsigned number of cell</param>
    /// <param name="wipeAllPawns">bool (default : false) : if true, all pawns encoutered will be wiped off the board</param>
    /// <param name="speed">int : speed of the animation</param>
    public void Move(int nbCell, bool wipeAllPawns = false, int speed = 1)
    {
        Status = PawnStatusEnum.MOVING;

        PawnAnimator.SetFloat(speedHash, (nbCell > 0 ? speed : -speed));
        Progress += nbCell;
        this.wipeAllPawns = wipeAllPawns;
        //Update the position of the pawn in the ProgressDico
        GMaster.LocalPlayer.RpcMoveinProgressDictionnary(this.name, nbCell);

        PawnAnimator.PlayInFixedTime(StateHash);
    }

    /// <summary>
    /// Exchange the pawn position with another one
    /// </summary>
    /// <param name="otherPawn"></param>
    public void Exchange(Pawn otherPawn)
    {
        int[] cellBetween = GMaster.progressDictionnary.ExchangeCompute(this, otherPawn);
        this.Move(cellBetween[0]);
        otherPawn.Move(cellBetween[1]);
    }

    /// <summary>
    /// Get the pawn out of the board
    /// </summary>
    public void Exit()
    {
        OnBoard = false;
        this.Progress = 0;
        GMaster.LocalPlayer.RpcRemovefromProgressDictionnary(this.name);
    }

    /// <summary>
    /// Place the Pawn at its starting position
    /// </summary>
    public void PlacePawnOut()
    {
        this.transform.position = outPosition.transform.position;
    }
    #region Selection Halo
    /// <summary>
    /// Switch on/off the selection halo with the given color
    /// </summary>
    /// <param name="on"></param>
    /// <param name="playerColor"></param>
    public void SwitchHalo(bool on, PlayerColorEnum playerColor)
    {
        ActualHaloColor = PlayerColorEnumToColor(playerColor);
        SelectableLight.enabled = on;
    }

    /// <summary>
    /// OnClick, fire the event PawnSelected
    /// </summary>
    public void OnMouseDown()
    {
        if (SelectableLight.enabled)
        {
            EventOnPawnSelected(this);
        }
    }

    /// <summary>
    /// Paint the halo in grey when mouse hover the pawn
    /// </summary>
    public void OnMouseOver()
    {
        if (SelectableLight.enabled)
        {
            SelectableLight.color = Color.grey;
        }
    }

    /// <summary>
    /// repaint the halo in previous color when stop hovering
    /// </summary>
    public void OnMouseExit()
    {
        if (SelectableLight.enabled)
        {
            SelectableLight.color = ActualHaloColor;
        }
    }
    #endregion
    #endregion
}
