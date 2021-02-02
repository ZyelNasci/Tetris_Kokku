using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Variables
    [Header("Components")]
    public MeshRenderer mesh;
    
    [Header("Positions")]
    [Tooltip("The tile position in the grid row")]
    public int line;
    [Tooltip("The tile position in the grid row")]
    public int column;

    [HideInInspector]
    public bool hasBlock = false;
    [HideInInspector]
    public IndividualBlock curBlockTile;
    #endregion

    #region Methods   
    /// <summary>
    /// Places a phantom block on the tile
    /// </summary>
    /// <param name="_tile"></param>
    public void SetGhost(IndividualBlock _tile)
    {
        _tile.transform.position = transform.position;
    }

    /// <summary>
    /// Set position for a block
    /// </summary>
    /// <param name="_tile"></param>
    public void SetBrick(IndividualBlock _tile)
    {
        _tile.transform.position = transform.position;
        curBlockTile = _tile;
    }

    /// <summary>
    /// Places a block on the tile
    /// </summary>
    /// <param name="_tile"></param>
    public void PlaceBlock(IndividualBlock _tile)
    {
        _tile.transform.position = transform.position;
        curBlockTile = _tile;
        hasBlock = true;
    }

    /// <summary>
    /// Returns the block that is on the tile
    /// </summary>
    /// <returns></returns>
    public IndividualBlock GetBrick()
    {
        return curBlockTile;
    }

    /// <summary>
    /// Removes the tile block
    /// </summary>
    public void ClearTile()
    {        
        curBlockTile = null;
        hasBlock = false;
    }

    /// <summary>
    /// Removes the tile block and gives back to the stack
    /// </summary>
    public void ClearTileBlock()
    {
        //if(curTile != null)
        //Destroy(curBlockTile.gameObject);
        ControlPooling.Instance.StoreBlock(curBlockTile);
        curBlockTile = null;
        hasBlock = false;
    }
    #endregion
}
