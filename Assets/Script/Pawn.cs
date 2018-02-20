using cakeslice;
using System.Collections;
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

    public float EntryFrame = 54f;

    //Index of the Pawn
    [SyncVar]
    public int PawnIndex;

    //Pawn on the board ?
    [SyncVar(hook = "OnChangeOnBoard")]
    public bool OnBoard = false;

    //Owning player of this Pawn
    [SyncVar(hook = "OnChangeOwningPlayerIndex")]
    public int OwningPlayerIndex;

    [SyncVar(hook = "OnChangePlayerColor")]
    public Color PlayerColor;

    private Color selectionColor;

    [SyncVar(hook = "OnChangeStatus")]
    private PawnStatusEnum status = PawnStatusEnum.OUT;

    public PawnMoveEnum MoveType = PawnMoveEnum.NORMAL;

    //Spawn positions for the pawns
    private SpawnPositions spawnPositions;
    private GameObject outPosition;

    #region Components
    //Components of the pawn
    private Animator pawnAnimator;
    private MeshRenderer pawnMeshRenderer;
    private MeshRenderer selectionMeshRenderer;
    private Light selectableLight;
    private Outline outliner;
    public GameObject Selection;

    #endregion
    static private GameMaster gMaster;

    #region Hash
    //Hash of Animator parameters
    static private int enterHash = Animator.StringToHash("EnterBoard");
    static private int exitHash = Animator.StringToHash("ExitBoard");
    static private int StateHash = Animator.StringToHash("ProgressOnBoard");
    static private int speedHash = Animator.StringToHash("Speed");
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

    public MeshRenderer SelectionMeshRenderer
    {
        get
        {
            if (selectionMeshRenderer == null)
            {
                selectionMeshRenderer = Selection.GetComponentInChildren<MeshRenderer>(true);
                selectionColor = selectionMeshRenderer.material.color;
                float h, s, v;
                Color.RGBToHSV(selectionColor, out h, out s, out v);
                if (h > (65f / 255f) && h < (155f / 255f))
                {
                    selectionColor = Color.HSVToRGB(1 - h, 0, v);

                }
            }
            return selectionMeshRenderer;
        }

        set
        {
            selectionMeshRenderer = value;
        }
    }

    public Outline Outliner
    {
        get
        {
            if (outliner == null)
            {
                outliner = this.GetComponentInChildren<Outline>(true);
                if (SystemInfo.deviceType != DeviceType.Handheld)
                {
                    outliner.enabled = true;
                }
            }
            return outliner;
        }

        set
        {
            outliner = value;
        }
    }

    /*public GameObject Selection
    {
        get
        {
            if (selection == null)
            {
                selection = this.GetComponentsInChildren<GameObject>(true)[1];
            }
            return selection;
        }

        set
        {
            selection = value;
        }
    }*/
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


    public void OnChangePlayerColor(Color newColor)
    {
        PlayerColor = newColor;
        PawnMeshRenderer.material.color = newColor;
        //Selection.GetComponentInChildren<MeshRenderer>().material.color = 
    }

    public void OnChangeOwningPlayerIndex(int newIndex)
    {
        OwningPlayerIndex = newIndex;
        this.name = newIndex.ToString() + "-" + PawnIndex.ToString();
        outPosition = SpawnPositions.GetOutPosition(this.name);

    }

    /// <summary>
    /// Fire the OnChangePlayerColor
    /// </summary>
    public override void OnStartClient()
    {
        OnChangeOwningPlayerIndex(OwningPlayerIndex);
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
            Transform startTransform = SpawnPositions.getStartPosition(OwningPlayerIndex).transform;
            this.transform.position = startTransform.position;
            this.transform.rotation = startTransform.rotation;
            
            PawnAnimator.SetTrigger(enterHash);
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
        if (isServer && MoveType != PawnMoveEnum.WIPENONE && other.name == "PawnModel")
        {
            Pawn pawnCollided = other.GetComponentInParent<Pawn>();
            //IF this pawn is moving and the other isn't moving
            //Useful when pawns are switching positions
            if (this.Status == PawnStatusEnum.MOVING && pawnCollided.Status == PawnStatusEnum.IDLE)
            {
                //IF wipeAllPawns is true or if the other pawn is at the same postion in the progressDico
                if (MoveType == PawnMoveEnum.WIPEALL || (pawnCollided.ProgressInDictionnary == this.ProgressInDictionnary))
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
            if (Progress > 70)
            {
                Status = PawnStatusEnum.IN_HOUSE;

            }
            else
            {
                if (Progress < -1)
                {
                    Progress = 72 + Progress;
                    
                }
                Status = PawnStatusEnum.IDLE;
            }
            MoveType = PawnMoveEnum.NORMAL;
        }
    }

    public void OnChangeStatus(PawnStatusEnum newStatus)
    {
        Status = newStatus;
        //Debug.Log(this.name + " - new status : " + newStatus);
    }
    #endregion

    #region methods
    /// <summary>
    /// Set the color, index, owning player and out position of the pawn
    /// </summary>
    /// <param name="color"></param>
    /// <param name="pawnIndex"></param>
    public void Initialize(TockPlayer player, int pawnIndex)
    {
        this.PawnIndex = pawnIndex;
        this.OwningPlayerIndex = player.PlayerIndex;
        PlayerColor = player.PlayerColor;
        this.transform.position = outPosition.transform.position;

    }


    /// <summary>
    /// Get the pawn on the board
    /// </summary>
    public void Enter()
    {
        OnBoard = true;
        this.Progress = 0;
        
        GMaster.LocalPlayer.CmdAddtoProgressDictionnary(this.name);
    }

    /// <summary>
    /// Move the Pawn
    /// </summary>
    /// <param name="nbCell">int : unsigned number of cell</param>
    /// <param name="wipeAllPawns">bool (default : false) : if true, all pawns encoutered will be wiped off the board</param>
    /// <param name="speed">int : speed of the animation</param>
    public void Move(int nbCell, bool wipeAllPawns = false, int speed = 1)
    {
        

        PawnAnimator.SetFloat(speedHash, (nbCell > 0 ? speed : -speed));
        Progress += nbCell;
        if (wipeAllPawns)
        {
            MoveType = PawnMoveEnum.WIPEALL;
        }
        //Update the position of the pawn in the ProgressDico
        GMaster.LocalPlayer.CmdMoveinProgressDictionnary(this.name, nbCell);
        if (Status == PawnStatusEnum.ENTRY)
        {
            Status = PawnStatusEnum.MOVING;
            PawnAnimator.PlayInFixedTime(StateHash, 0, EntryFrame / 60f);
        }
        else
        {
            Status = PawnStatusEnum.MOVING;
            PawnAnimator.PlayInFixedTime(StateHash);
        }
    }

    /// <summary>
    /// Exchange the pawn position with another one
    /// </summary>
    /// <param name="otherPawn"></param>
    public void Exchange(Pawn otherPawn)
    {
        int[] cellBetween = GMaster.ExchangeCompute(this, otherPawn);
        MoveType = PawnMoveEnum.WIPENONE;
        this.Move(cellBetween[0]);
        otherPawn.MoveType = PawnMoveEnum.WIPENONE;
        otherPawn.Move(cellBetween[1]);
    }

    /// <summary>
    /// Get the pawn out of the board
    /// </summary>
    public void Exit()
    {
        OnBoard = false;
        this.Progress = 0;
        this.ProgressInDictionnary = 0;
        GMaster.LocalPlayer.CmdRemovefromProgressDictionnary(this.name);
    }

    /// <summary>
    /// Place the Pawn at its starting position
    /// </summary>
    public void PlacePawnOut()
    {
        this.transform.position = outPosition.transform.position;
        Status = PawnStatusEnum.OUT;
    }
    #region Selection Halo
    /// <summary>
    /// Switch on/off the selection halo with the given color
    /// </summary>
    /// <param name="on"></param>
    /// <param name="playerColor"></param>
    public void SwitchHalo(bool on, Color playerColor)
    {
        /*if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            Outliner.enabled = on;

        }
        Outliner.color = on?1:0;*/
        Selection.SetActive(on);
        SelectionMeshRenderer.material.color = selectionColor;

    }

    /// <summary>
    /// OnClick, fire the event PawnSelected
    /// </summary>
    public void OnMouseDown()
    {
        //if (Outliner.color > 0)
        if (Selection.activeInHierarchy)
        {
            EventOnPawnSelected(this);
        }
    }

    /// <summary>
    /// Paint the halo in grey when mouse hover the pawn
    /// </summary>
    public void OnMouseOver()
    {
        //if (Outliner.color > 0)
        if (Selection.activeInHierarchy)
        {
            //Outliner.color = 2;
            SelectionMeshRenderer.material.color = PlayerColor;
        }
    }

    /// <summary>
    /// repaint the halo in previous color when stop hovering
    /// </summary>
    public void OnMouseExit()
    {
        /*if (Outliner.color > 0)
        {
            Outliner.color = 1;
        }*/
        if (Selection.activeInHierarchy)
        {
            SelectionMeshRenderer.material.color = selectionColor;
        }

    }
    #endregion
    #endregion
    public IEnumerator waitForFinishedMoving()
    {
        yield return new WaitWhile(() => Status == PawnStatusEnum.MOVING);
    }
}
