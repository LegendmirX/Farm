using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct DOTSPathNode : IComponentData
{
    public int x;
    public int z;

    public int index;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;

    public int cameFromNodeIndex;

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}
