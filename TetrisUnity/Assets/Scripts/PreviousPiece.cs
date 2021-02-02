using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviousPiece : MonoBehaviour
{
    [Header("Components")]
    public Tile tilePrefab;

    [Header("Attributes")]
    [Tooltip("Number of rows on the grid")]
    public int rowsCount = 2;
    [Tooltip("Number of columns on the grid")]
    public int columnsCount = 4;
    [Tooltip("Size of the grid")]
    public float tileSize = 0.8f;

    public Tile[,] gridMatriz;

    private void Start()
    {
        UIController.Instance.StopEvent += StopGameEvent;
    }

    public void GenerateGrid()
    {
        gridMatriz = new Tile[rowsCount, columnsCount];

        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                float posX = j * tileSize;
                float posY = i * -tileSize;

                Tile tile = Instantiate(tilePrefab, transform);
                tile.line = i;
                tile.column = j;
                tile.transform.localPosition = new Vector2(posX, posY);

                gridMatriz[i, j] = tile;
                tile.mesh.enabled = false;
            }
        }
    }

    /// <summary>
    /// Method called by the Game Over Event 
    /// </summary>
    public void StopGameEvent()
    {
        ClearAllTiles();
        SwitchGrid(false);
    }

    /// <summary>
    /// Clear all blocks from the grid
    /// </summary>
    public void ClearAllTiles()
    {
        for (int i = 0; i <rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                if(gridMatriz[i,j].hasBlock)
                    gridMatriz[i, j].ClearTileBlock();
            }
        }
    }

    /// <summary>
    /// Show/Hide the grid
    /// </summary>
    /// <param name="OnOrFalse"></param>
    public void SwitchGrid(bool OnOrFalse)
    {
        for (int i = 0; i < rowsCount; i++)
        {
            for (int j = 0; j < columnsCount; j++)
            {
                gridMatriz[i, j].mesh.enabled = OnOrFalse;
            }
        }
    }
}