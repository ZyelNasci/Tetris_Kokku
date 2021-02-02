using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualBlock : MonoBehaviour
{
    public MeshRenderer blockMesh;
    public MeshRenderer ghostMesh;

    public void SwitchGhost(bool _switch)
    {
        ghostMesh.enabled = _switch;
    }

    public void SwitchOff()
    {
        blockMesh.enabled = false;
        ghostMesh.enabled = false;
    }
}
