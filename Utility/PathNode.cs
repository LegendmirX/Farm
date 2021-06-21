using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public enum Enterability
    {
        Yes,
        Never,
        Soon
    }
    Func<PathNode, Enterability> isEnterableFunc;
    private GridUtil<PathNode> grid;
    public int x { get; protected set; }
    public int z { get; protected set; }
    public Vector3 Position;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable { get; protected set; }

    public PathNode cameFromNode;

    public PathNode(GridUtil<PathNode> grid, int x, int z, bool isWalkable = true, Func<PathNode, Enterability> enterableFunc = null)
    {
        this.grid           = grid;
        this.x              = x;
        this.z              = z;
        this.isWalkable     = isWalkable;
        this.isEnterableFunc = enterableFunc;
        Position = new Vector3(x, 0, z);
    }

    public void Calculate_F_Cost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool value)
    {
        isWalkable = value;
    }

    public override string ToString()
    {
        return "X:" + x + "\n" + "  Z:" + z;
    }

    public Enterability IsEnterable()
    {
        return isEnterableFunc(this);
    }
}
