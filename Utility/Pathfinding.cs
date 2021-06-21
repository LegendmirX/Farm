using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding 
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private bool NoCuttingCorners = true;

    private GridUtil<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public Pathfinding (int width, int height, int cellSize, bool walkable = true, Func<PathNode, PathNode.Enterability> enterableFunc = null)
    {
        grid = new GridUtil<PathNode>(width, height, cellSize, Vector3.zero, makePathNode, true, enterableFunc);
    }

    private PathNode makePathNode(GridUtil<PathNode> g, int x, int z, bool w = true, Func<PathNode, PathNode.Enterability> enterable = null )
    {
        return new PathNode(g, x, z, w, enterable);
    }

    public void SetIsWalkable(Vector3 position, bool value)
    {
        PathNode pathNode = grid.GetGridObject(position);
        if(pathNode == null)
        {
            Debug.Log("Node Not Found");
            return;
        }
        pathNode.SetIsWalkable(value);
    }

    public List<PathNode> FindPath(Vector3 start, Vector3 end)
    {
        grid.GetXZ(start, out int startX, out int startZ);
        grid.GetXZ(end, out int endX, out int endZ);

        return FindPath(startX, startZ, endX, endZ);
    }

    public List<PathNode> FindPath(int startX, int startZ, int endX, int endZ)
    {
        PathNode startNode  = grid.GetGridObject(startX, startZ);
        PathNode endNode    = grid.GetGridObject(endX, endZ);

        if (startNode == null || endNode == null)
        {
            // Invalid Path
            return null;
        }

        if(endNode.isWalkable == false)
        {
            PathNode node = FindWalkableNearNode(endNode);
            if(node == null)
            {

                //Invalid path
                return null;
            }
            endNode = node;
        }
        
        openList            = new List<PathNode> { startNode }; //instantiating list with a start node in the list
        closedList          = new List<PathNode>();

        SetUp_GnF_Costs();

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
        startNode.Calculate_F_Cost();
        //SetUp Compleate

        while(openList.Count > 0) //Start Pathfinding
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if(currentNode == endNode)
            {
                //GoalReached
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode, NoCuttingCorners))
            {
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                if(neighbourNode.isWalkable == false)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);
                    neighbourNode.Calculate_F_Cost();

                    if (openList.Contains(neighbourNode) == false)
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        //out of nodes on open list

        return null;
    }

    #region PathFindingFuncs

    private PathNode FindWalkableNearNode(PathNode node)
    {
        List<PathNode> neighbours = GetNeighbourList(node);

        foreach(PathNode nodeCheck in neighbours)
        {
            if(nodeCheck.isWalkable == true)
            {
                return nodeCheck;
            }
        }

        return null;
    }

    private void SetUp_GnF_Costs()
    {
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                PathNode pathNode = grid.GetGridObject(x, z);
                pathNode.gCost = int.MaxValue;
                pathNode.Calculate_F_Cost();
                pathNode.cameFromNode = null;
            }
        }
    }

    private int CalculateDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int zDistance = Mathf.Abs(a.z - b.z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];

        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode, bool noCuttingCorners = false)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        bool n = false;
        bool e = false;
        bool s = false;
        bool w = false;
        //Check N,E,S,W
        if (currentNode.z + 1 < grid.GetHeight())//N
        {
            PathNode node = GetNode(currentNode.x, currentNode.z + 1);
            neighbourList.Add(node);

            if(noCuttingCorners == true && node.isWalkable == false)
            {
                n = false;
            }
            else
            {
                n = true;
            }

        }
        if(currentNode.x + 1 < grid.GetWidth())//E
        {
            PathNode node = GetNode(currentNode.x + 1, currentNode.z);
            neighbourList.Add(node);

            if (noCuttingCorners == true && node.isWalkable == false)
            {
                e = false;
            }
            else
            {
                e = true;
            }
        }
        if(currentNode.z - 1 >= 0)//S
        {
            PathNode node = GetNode(currentNode.x, currentNode.z - 1);
            neighbourList.Add(node);

            if (noCuttingCorners == true && node.isWalkable == false)
            {
                s = false;
            }
            else
            {
                s = true;
            }
        }
        if(currentNode.x - 1 >= 0)//W
        {
            PathNode node = GetNode(currentNode.x - 1, currentNode.z);
            neighbourList.Add(node);

            if (noCuttingCorners == true && node.isWalkable == false)
            {
                w = false;
            }
            else
            {
                w = true;
            }
        }

        //CheckDiagonals
        if(n == true && e == true)//NE
        {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z + 1));
        }
        if(e == true && s == true)//SE
        {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.z - 1));
        }
        if(s == true && w == true)//SW
        {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z - 1));
        }
        if(w == true && n == true)//NW
        {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.z + 1));
        }

        return neighbourList;
    }

    private PathNode GetNode(int x, int z)
    {
        return grid.GetGridObject(x, z);
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();

        path.Add(endNode);
        PathNode currentNode = endNode;

        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();

        return path;
    }

    #endregion
}
