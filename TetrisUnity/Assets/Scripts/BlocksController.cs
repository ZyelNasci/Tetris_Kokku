using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksController : MonoBehaviour
{
#region Variables
    [Header("Components")]
    public IndividualBlock blockPrefab;
    [Tooltip("Materials for blocks")]
    public Material[] pieceMaterials;
    private GridManager grid;

    [Header("Attributes")]
    [Tooltip("Piece Fall Time")]
    public float fallTime;

    private float currentTime;
    private float curFallTime;
    private float curMoveTime;
    private bool moving;
    private bool hasGhost;

    private struct Block
    {
        public int row;
        public int col;
        public IndividualBlock ob;

        public Block (int _row, int _col, IndividualBlock ob)
        {
            this.row = _row;
            this.col = _col;
            this.ob = ob;
        }
    }
    private Block[] piece               = new Block[4];
    private Block[] ghostPiece          = new Block[4];
    private List<int> pieceNumbList     = new List<int>();
    private int[,] shape                = new int[,] //Matrix Shape
    {
        {1,3,5,7}, //l
        {2,4,5,7}, //Z
        {3,4,5,6}, //S
        {3,4,5,7}, //T
        {2,3,5,7}, //L
        {3,5,6,7}, //J
        {2,3,4,5}  //O
    };
    private Coroutine curCoroutine;
#endregion

 #region UnityFunctions
    void Awake()
    {
        grid = GetComponent<GridManager>();
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            ghostPiece[i].ob = ControlPooling.Instance.GetBlock(true);//Instantiate(brickPrefab, new Vector2(ghostPiece[i].row, ghostPiece[i].col), Quaternion.identity);
            ghostPiece[i].ob.SwitchOff();                
        }
        UIController.Instance.StopEvent += StopGameEvent;
    }

    private void Update()
    {
        if (moving == true)
        {
            Inputs();
            FallCooldown();
        }
    }
#endregion

 #region Initialize Methods

    /// <summary>
    /// Called by the grid manager after grid creates to start the game
    /// </summary>
    public void StartPieces()
    {     
        for (int i = 0; i < 7; i++)
        {
            pieceNumbList.Add(i);
        }
        pieceNumbList.Randomize();
        Generate();
    }

    /// <summary>
    /// Create and place a new piece on the grid
    /// </summary>
    public void Generate()
    {
        currentTime = Time.time;
        curFallTime = fallTime;
        int n = pieceNumbList[0];
        pieceNumbList.RemoveAt(0);

        for (int i = 0; i < grid.previousController.Length; i++)
        {
            GeneratePrevious(pieceNumbList[i], grid.previousController[i]);
        }

        pieceNumbList.Add(Random.Range(0, 7));
        Color pieceColor = GetColorPiece(n);

        for (int i = 0; i < 4; i++)
        {
            piece[i].row = shape[n, i] % 2;
            piece[i].col = shape[n, i] / 2 + (Mathf.FloorToInt(grid.columnsCount / 2) - 2);

            ghostPiece[i].row = shape[n, i] % 2;
            ghostPiece[i].col = shape[n, i] / 2 + (Mathf.FloorToInt(grid.columnsCount / 2) - 2);
        }

        for (int i = 0; i < 4; i++)
        {
            piece[i].ob = ControlPooling.Instance.GetBlock();
            piece[i].ob.blockMesh.material = pieceMaterials[n];
            grid.gridMatriz[piece[i].row, piece[i].col].SetBrick(piece[i].ob);
        }

        SetGhostPiece();
        curCoroutine = StartCoroutine(DelayToStartFall());
    }

    /// <summary>
    /// Delay to start fall the piece
    /// </summary>
    /// <returns></returns>
    public IEnumerator DelayToStartFall()
    {
        yield return new WaitForSeconds(0.2f);
        moving = true;
    }

    /// <summary>
    /// Create and place a new piece on the previous grid
    /// </summary>
    /// <param name="n"></param>
    /// <param name="_previous"></param>
    public void GeneratePrevious(int n, PreviousPiece _previous)
    {
        _previous.ClearAllTiles();
        Block[] newPiece = new Block[4];

        Color pieceColor = GetColorPiece(n);

        for (int i = 0; i < 4; i++)
        {
            newPiece[i].row = shape[n, i] % 2;
            newPiece[i].col = shape[n, i] / 2;
        }

        for (int i = 0; i < 4; i++)
        {
            newPiece[i].ob = ControlPooling.Instance.GetBlock();
            newPiece[i].ob.blockMesh.material = pieceMaterials[n];
            _previous.gridMatriz[newPiece[i].row, newPiece[i].col].PlaceBlock(newPiece[i].ob);
        }
    }

    /// <summary>
    /// returns a color based on the shape of the piece
    /// </summary>
    /// <param name="numb"></param>
    /// <returns></returns>
    public Color GetColorPiece(int numb)
    {
        Color curColor = Color.red;
        switch (numb)
        {
            case 0:
                curColor = Color.red;
                break;
            case 1:
                curColor = Color.blue;
                break;
            case 2:
                curColor = Color.yellow;
                break;
            case 3:
                curColor = Color.green;
                break;
            case 4:
                curColor = Color.cyan;
                break;
            case 5:
                curColor = Color.magenta;
                break;
            case 6:
                curColor = Color.grey;
                break;
        }
        return curColor;
    }
    #endregion

#region Inputs and movements methods
    /// <summary>
    /// Get all inputs     
    /// </summary>
    public void Inputs()
    {

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKey(KeyCode.LeftArrow) && Time.time >= curMoveTime + 0.12f)
        {
            MovePiece(0, -1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.RightArrow) && Time.time >= curMoveTime + 0.12f)
        {
            MovePiece(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }


        if (Input.GetKey(KeyCode.DownArrow))
        {
            curFallTime = fallTime * 0.0001f;
        }
        else
        {
            curFallTime = fallTime;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SetOnBottom();

        }
    }

    /// <summary>
    /// Moves the piece on the grid
    /// </summary>
    /// <param name="_rowDirection"></param>
    /// <param name="_colDirection"></param>
    public void MovePiece(int _rowDirection, int _colDirection)
    {
        ClearPieceTiles();

        for (int i = 0; i < 4; i++)
        {
            if (grid.CheckNextTile(piece[i].row + _rowDirection, piece[i].col + _colDirection) == false)
            {
                if (_rowDirection > 0)
                {
                    SoundController.Instance.PlayPlacePiece();
                    StopPiece();
                }
                return;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            piece[i].col += _colDirection;
            piece[i].row += _rowDirection;
            grid.gridMatriz[piece[i].row, piece[i].col].SetBrick(piece[i].ob);
        }
        if (_rowDirection == 0)
        {
            curMoveTime = Time.time;
            SoundController.Instance.PlayMove();
            SetGhostPiece();
        }
    }

    /// <summary>
    /// Change the position of each block to rotate the piece
    /// </summary>
    public void Rotate()
    {
        ClearPieceTiles();
        //ClearPieceTiles();
        Block[] original = new Block[4];
        Block p = piece[1];

        for (int i = 0; i < 4; i++)
        {
            int col = piece[i].row - p.row;
            int row = piece[i].col - p.col;
            piece[i].col = p.col - col;
            piece[i].row = p.row + row;
        }

        if(CheckAndSetRotate(original))
            SoundController.Instance.PlayMove();
        SetGhostPiece();
    }

    /// <summary>
    /// Moves all blocks for verification
    /// </summary>
    /// <param name="rowDirection"></param>
    /// <param name="colDirection"></param>
    public void MoveAllBlocks(int rowDirection, int colDirection)
    {
        for (int i = 0; i < 4; i++)
        {
            piece[i].row += rowDirection;
            piece[i].col += colDirection;
        }
    }

    /// <summary>
    /// Checks and move the ghost piece
    /// </summary>
    public void SetGhostPiece()
    {
        //ghostPiece = piece.Clone() as Block[];

        List<int> temp = new List<int>();
        int numb = 0;
        for (int i = 0; i < 4; i++)
        {
            ghostPiece[i].ob.ghostMesh.enabled = true;
            temp.Add(grid.CheckGhostPosition(piece[i].row, piece[i].col));
        }

        for (int i = 0; i < temp.Count; i++)
        {
            if (i == 0)
            {
                numb = temp[i] - piece[i].row;
            }
            else if (numb > (temp[i] - piece[i].row))
            {
                numb = temp[i] - piece[i].row;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (piece[i].row < piece[i].row + numb)
            {
                grid.gridMatriz[piece[i].row + numb, piece[i].col].SetGhost(ghostPiece[i].ob);

                ghostPiece[i].row = piece[i].row + numb;
                ghostPiece[i].col = piece[i].col;
                hasGhost = true;
            }
            else
            {
                hasGhost = false;
                ghostPiece[i].ob.SwitchGhost(false);
            }
        }
    }

    //public void SetOnBottom()
    //{
    //    if (hasGhost)
    //    {
    //        moving = false;

    //        for (int i = 0; i < 4; i++)
    //        {
    //            piece[i].row = ghostPiece[i].row;
    //            piece[i].col = ghostPiece[i].col;
    //        }
    //        //SetAllBlockOnTile();
    //        StopPiece();
    //    }
    //}

    /// <summary>
    /// Place all the blocks on the grid
    /// </summary>
    public void PlaceAllBlocks()
    {
        for (int i = 0; i < 4; i++)
        {
            //grid.gridMatriz[piece[i].row, piece[i].col].SetBrick(piece[i].ob);
            grid.gridMatriz[piece[i].row, piece[i].col].PlaceBlock(piece[i].ob);
        }
    }

    /// <summary>
    /// Check if can rotate the piece
    /// </summary>
    /// <param name="ori"></param>
    /// <returns></returns>
    private bool CheckAndSetRotate(Block[] ori)
    {

        bool set = true;
        for (int i = 0; i < 4; i++)
        {
            if (piece[i].col < 0)
            {
                MoveAllBlocks(0, 1);
            }
            else if (piece[i].col >= grid.columnsCount)
            {
                MoveAllBlocks(0, -1);
            }

            if (piece[i].row < 0)
            {
                MoveAllBlocks(1, 0);
            }
            else if (piece[i].row >= grid.rowsCount)
            {
                MoveAllBlocks(-1, 0);
            }

            if (grid.gridMatriz[piece[i].row, piece[i].col].hasBlock == true)
            {
                //set = false;
                if (piece[i].col < piece[1].col)
                {
                    MoveAllBlocks(0, 1);
                }
                else if (piece[i].col > piece[1].col)
                {
                    MoveAllBlocks(0, -1);
                }

                if (piece[i].row < piece[1].row)
                {
                    MoveAllBlocks(1, 0);
                }
                else if (piece[i].row > piece[1].row)
                {
                    MoveAllBlocks(-1, 0);
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (grid.CheckNextTile(piece[i].row, piece[i].col) == false)
            {
                set = false;
                break;
            }
        }

        if (set)
        {
            //etAllBlockOnTile();
            for (int i = 0; i < 4; i++)
                grid.gridMatriz[piece[i].row, piece[i].col].SetBrick(piece[i].ob);
        }
        else
        {
            piece = ori;
            for (int i = 0; i < 4; i++)
                grid.gridMatriz[piece[i].row, piece[i].col].SetBrick(piece[i].ob);
        }
        return set;

    }
    #endregion

    #region Other Methods
    /// <summary>
    /// Method called by the Game Over Event 
    /// </summary>
    void StopGameEvent()
    {
        if (curCoroutine != null)
            StopCoroutine(curCoroutine);

        moving = false;
        for (int i = 0; i < 4; i++)
        {
            ghostPiece[i].ob.SwitchGhost(false);
            //ControlPooling.Instance.StoreBlock(ghostPiece[i].ob);            
        }
    }

    /// <summary>
    /// Stop the piece movements, check if do score or the game ended and call the next piece.
    /// </summary>
    public void StopPiece()
    {
        currentTime = Time.time;
        moving = false;
        PlaceAllBlocks();

        for (int i = 0; i < 4; i++)
        {
            hasGhost = false;
            ghostPiece[i].ob.SwitchGhost(false);
        }

        if(piece[1].row > 2)
        {
            List<int> rowScoresList = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                if (grid.CheckRowScore(piece[i].row) == true)
                {
                    if (!rowScoresList.Contains(piece[i].row))
                        rowScoresList.Add(piece[i].row);
                }
            }

            if (rowScoresList.Count > 0)
                grid.LineScore(rowScoresList);
            else
                Generate();

            //yield return new WaitForSeconds(0.2f);
            //Generate();
        }
        else
        {
            grid.GameOverGrid();            
            print("GameOver");
        }

    }

    /// <summary>
    /// Checks the time for the piece to fall
    /// </summary>
    public void FallCooldown()
    {
        if (Time.time >= currentTime + curFallTime)
        {
            MovePiece(1,0);
            currentTime = Time.time;
        }
    }

    /// <summary>
    /// Clears the tiles of the piece
    /// </summary>
    public void ClearPieceTiles()
    {
        for (int i = 0; i < 4; i++)
        {
            grid.gridMatriz[piece[i].row, piece[i].col].ClearTile();
        }
    }
#endregion

}