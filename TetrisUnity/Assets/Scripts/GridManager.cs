using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
#region Variables
    [Header("Components")]
    public Tile tilePrefab;
    public Transform gridGroup;
    [HideInInspector]
    public ScoreController scoreControl;
    public ParticleSystem particle;
    public PreviousPiece[] previousController = new PreviousPiece[3];

    [Header("Attributes")]
    [Tooltip("Number of rows on the grid")]
    public int rowsCount;
    [Tooltip("Number of columns on the grid")]
    public int columnsCount;
    [Tooltip("Size of the grid")]
    public float tileSize;

    public Tile[,] gridMatriz;
    private BlocksController bricks;
    private Coroutine curCoroutine;
#endregion

 #region Unity Functions
    // Start is called before the first frame update    
    void Awake()
    {
        bricks          = GetComponent<BlocksController>();
        scoreControl    = GetComponent<ScoreController>();
    }
    private void Start()
    {
        UIController.Instance.StartEvent += StartGameEvent;
        UIController.Instance.StopEvent += StopGameEvent;
        GenerateGrid();
        scoreControl = UIController.Instance.scoreControl;
    }
#endregion

#region Initialize Methods
    /// <summary>
    /// Method called by the Start Game Event
    /// </summary>
    public void StartGameEvent()
    {
        SwitchGrid(true);
        bricks.StartPieces();
    }

    public void GenerateGrid()
    {
        gridMatriz = new Tile[rowsCount + 2, columnsCount];

        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {

                float posX = j * tileSize;
                float posY = i * -tileSize;

                Tile tile = Instantiate(tilePrefab, gridGroup);
                tile.line = i;
                tile.column = j;
                tile.transform.localPosition = new Vector2(posX, posY);

                gridMatriz[i, j] = tile;

                tile.mesh.enabled = false;
            }
        }

        for (int i = 0; i < previousController.Length; i++)
        {
            previousController[i].GenerateGrid();
        }
    }
#endregion

#region CheckMethods
    /// <summary>
    /// Check if the tile is free
    /// </summary>
    /// <param name="_line"></param>
    /// <param name="_column"></param>
    /// <returns></returns>
    public bool CheckNextTile(int _line, int _column)
    {
        bool canMove = false;
        if (_line < rowsCount && _line >= 0 && _column < columnsCount && _column >= 0)
        {
            canMove = !gridMatriz[_line, _column].hasBlock;
        }
        else
        {
            canMove = false;
        }

        return canMove;
    }

    /// <summary>
    /// Check if you score on that line
    /// </summary>
    /// <param name="_line"></param>
    /// <returns></returns>
    public bool CheckRowScore(int _line)
    {
        bool scored = true;

        for (int i = 0; i < columnsCount; i++)
        {
            if (gridMatriz[_line, i].hasBlock == false)
            {
                //bricks.CallNewBrick();
                scored = false;
                break;
            }
        }
        //StartCoroutine(Score(_line));
        return scored;
    }

    public int CheckGhostPosition(int _rown, int _col)
    {
        int numb = rowsCount - 1;
        for (int i = _rown; i < rowsCount; i++)
        {
            if (gridMatriz[i, _col].hasBlock)
            {
                return i - 1;
            }
        }
        return numb;
    }
#endregion

#region Methods
    /// <summary>
    /// Method called by the Game Over Event 
    /// </summary>
    public void StopGameEvent()
    {
        if (curCoroutine != null)
            StopCoroutine(curCoroutine);

        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                Tile tile = gridMatriz[i, j];
                if (tile.curBlockTile != null)
                    tile.ClearTileBlock();
            }
        }
        SwitchGrid(false);
    }

    /// <summary>
    /// Show/Hide the grid
    /// </summary>
    /// <param name="OnOrOff"></param>
    public void SwitchGrid(bool OnOrOff)
    {        
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                if (gridMatriz[i,j].hasBlock)
                    gridMatriz[i, j].ClearTileBlock();

                gridMatriz[i, j].mesh.enabled = OnOrOff;

                if (i < 2)
                    gridMatriz[i, j].mesh.enabled = false;
            }
        }

        for (int i = 0; i < previousController.Length; i++)
        {
            previousController[i].SwitchGrid(OnOrOff);
        }
    }

    /// <summary>
    /// Do Score!
    /// </summary>
    /// <param name="_line"></param>
    public void LineScore(List<int> _line)
    {
        curCoroutine =  StartCoroutine(LineScoreDelay(_line));
    }

    /// <summary>
    /// Do the score animation
    /// </summary>
    /// <param name="_line"></param>
    /// <returns></returns>
    public IEnumerator LineScoreDelay(List<int> _line)
    {
        Debug.Log("Score!");
        int numb = 1;
        for (int i = _line.Count - 1; i >= 0; i--)
        {
            SoundController.Instance.PlayScore(numb);
            numb++;
            for (int j = columnsCount - 1; j >= 0; j--)
            {
                yield return new WaitForSeconds(0.02f);
                if(gridMatriz[_line[i], j].hasBlock)
                {
                    PlayParticle(gridMatriz[_line[i], j]);
                    gridMatriz[_line[i], j].ClearTileBlock();
                }
            }
        }

        scoreControl.AddPoints(_line.Count);

        int temp = 0;
        for (int i = 0; i < _line.Count; i++)
        {
            if(i == 0)
            {
                temp = _line[i];
            }
            else if(_line[i] > temp)
            {
                temp = _line[i];
            }
        }

        //yield return new WaitForSeconds(0.25f);
        for (int j = columnsCount - 1; j >= 0; j--)
        {
            for (int i = temp; i >= 0; i--)
            {
                if (gridMatriz[i, j].hasBlock)
                {
                    if (CheckNextTile(i + _line.Count, j) == true)
                    {
                        IndividualBlock brick = gridMatriz[i, j].GetBrick();
                        gridMatriz[i, j].ClearTile();
                        gridMatriz[i + _line.Count, j].PlaceBlock(brick);
                    }
                }
            }
            yield return new WaitForSeconds(0.02f);
        }
        //yield return new WaitForSeconds(0.5f);
        bricks.Generate();
    }

    /// <summary>
    /// Do the Game Over Animation
    /// </summary>
    /// <returns></returns>
    public IEnumerator ClearGrid()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                gridMatriz[i, j].mesh.enabled = false;
                if (gridMatriz[i, j].hasBlock)
                {
                    PlayParticle(gridMatriz[i, j]);
                    gridMatriz[i, j].ClearTileBlock();
                }
                yield return new WaitForSeconds(0.001f);
            }
        }
        for (int i = 0; i < previousController.Length; i++)
        {
            previousController[i].ClearAllTiles();
            previousController[i].SwitchGrid(false);
        }
        UIController.Instance.StopEvent();
        UIController.Instance.OpenGameOver();
    }

    public void GameOverGrid()
    {
        curCoroutine = StartCoroutine(ClearGrid());
    }  
    void PlayParticle(Tile _tile)
    {
        particle.transform.position = _tile.transform.position;
        particle.startColor = _tile.curBlockTile.blockMesh.material.color;
        particle.Play();
    }
#endregion
}