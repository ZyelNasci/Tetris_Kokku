using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPooling : Singleton<ControlPooling>
{
    public IndividualBlock blockPrefab;
    Stack<IndividualBlock> blockStack = new Stack<IndividualBlock>();

    public void Start()
    {
        CreateBlocks();
    }

    /// <summary>
    /// Create and save blocks in the stack
    /// </summary>
    public void CreateBlocks()
    {
        int count = GridManager.Instance.columnsCount * GridManager.Instance.rowsCount;
        for (int i = 0; i < count; i++)
        {
            IndividualBlock newBlock = Instantiate(blockPrefab, transform);
            newBlock.SwitchOff();
            blockStack.Push(newBlock);
        }
        Debug.Log("Stack Count: " + blockStack.Count);
    }

    /// <summary>
    /// Returns one block from the stack or creates if there are no blocks in the stack and returns
    /// </summary>
    public IndividualBlock GetBlock(bool ghost = false)
    {
        if(blockStack.Count > 0)
        {
            IndividualBlock newBlock = blockStack.Pop();
            if (!ghost)
                newBlock.blockMesh.enabled = true;
            else
                newBlock.ghostMesh.enabled = true;

            print("Stack Count: " + blockStack.Count);
            return newBlock;
        }
        else
        {
            IndividualBlock newBlock = Instantiate(blockPrefab, transform);
            if (!ghost)
                newBlock.blockMesh.enabled = true;
            else
                newBlock.ghostMesh.enabled = true;

            return newBlock;
        }
    }

    /// <summary>
    /// Saves the block to the stack
    /// </summary>
    public void StoreBlock(IndividualBlock _block)
    {
        _block.SwitchOff();
        blockStack.Push(_block);
        print("Stack Count: " + blockStack.Count);
    }
}
