using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

public struct PathGrid : IComponentData
{
    public int2 gridSize;

    public NativeArray<DOTSPathNode> pathNodeArray;
}
