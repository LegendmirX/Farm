using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

public class PathfindingDOTS : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private NativeArray<DOTSPathNode> DOTSPathNodeArray;
    private int2 gridSize;

    public void SetUp(int gridSizeX, int gridSizeZ)
    {
        Debug.Log("Pathfinding SetUp");



        gridSize = new int2(gridSizeX, gridSizeZ);
        DOTSPathNodeArray = new NativeArray<DOTSPathNode>(gridSize.x * gridSize.y, Allocator.Persistent);

        //build grid
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                DOTSPathNode DOTSPathNode = new DOTSPathNode();
                DOTSPathNode.x = x;
                DOTSPathNode.z = z;
                DOTSPathNode.index = CalculateIndex(x, z, gridSize.x);

                DOTSPathNode.gCost = int.MaxValue;
                DOTSPathNode.CalculateFCost();

                DOTSPathNode.isWalkable = true;
                DOTSPathNode.cameFromNodeIndex = -1;

                DOTSPathNodeArray[DOTSPathNode.index] = DOTSPathNode;
            }
        }

        //Debug
        DOTSPathNode notWalkableNode;
        int horizontalPos = 50;
        for (int i = 2; i < (gridSize.y - 2); i++)
        {
            notWalkableNode = DOTSPathNodeArray[CalculateIndex(horizontalPos, i, gridSize.x)];
            notWalkableNode.SetIsWalkable(false);
            DOTSPathNodeArray[notWalkableNode.index] = notWalkableNode;
        }
        //Debug
    }

    public void FindPath(int startX, int startY, int endX, int endY)
    {
        FindPathJob findPathJob = new FindPathJob
        {
            startPos = new int2(startX, startY),
            endPos = new int2(endX, endY),
            gridSize = gridSize,
            DOTSPathNodeArray = DOTSPathNodeArray
        };
        findPathJob.Schedule();
    }

    public void FindPathTest(int startX, int startY, int endX, int endY, int multiplyer)
    {
        float startTime = Time.realtimeSinceStartup;

        int pathTestCount = multiplyer;
        NativeArray<JobHandle> jobHandleArray = new NativeArray<JobHandle>(pathTestCount, Allocator.Temp);
        
        for (int i = 0; i < pathTestCount; i++)
        {
            FindPathJob findPathJob = new FindPathJob
            {
                startPos = new int2(startX, startY),
                endPos = new int2(endX, endY),
                gridSize = this.gridSize,
                DOTSPathNodeArray = this.DOTSPathNodeArray
            };
            jobHandleArray[i] = findPathJob.Schedule();
        }

        JobHandle.CompleteAll(jobHandleArray);
        jobHandleArray.Dispose();

        Debug.Log((Time.realtimeSinceStartup - startTime) * 1000f);

    }

    [BurstCompile]
    private struct FindPathJob : IJob
    {
        public int2 startPos;
        public int2 endPos;
        public int2 gridSize;
        public NativeArray<DOTSPathNode> DOTSPathNodeArray;

        public void Execute()
        {



            //offset to find neighbours
            //TODO: work on this so we dont cut corners
            NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Allocator.Temp);
            neighbourOffsetArray[0] = new int2(0, +1); //north
            neighbourOffsetArray[1] = new int2(+1, 0); //east
            neighbourOffsetArray[2] = new int2(0, -1); // south
            neighbourOffsetArray[3] = new int2(-1, 0); // west
            neighbourOffsetArray[4] = new int2(+1, +1); //N,E
            neighbourOffsetArray[5] = new int2(-1, +1); //S,E
            neighbourOffsetArray[6] = new int2(-1, -1); //S,W
            neighbourOffsetArray[7] = new int2(+1, -1); //N,W

            //Get our end node so we know when we arrive
            int endNodeIndex = CalculateIndex(endPos.x, endPos.y, gridSize.x);

            //set up nodes
            //for (int x = 0; x < gridSize.x; x++)
            //{
            //    for (int z = 0; z < gridSize.y; z++)
            //    {
            //        int nodeIndex = CalculateIndex(x, z, gridSize.x);
            //        DOTSPathNode DOTSPathNode = DOTSPathNodeArray[nodeIndex];
            //        DOTSPathNode.gCost = int.MaxValue;
            //        DOTSPathNode.CalculateFCost();
            //        DOTSPathNode.cameFromNodeIndex = -1;
            //        DOTSPathNodeArray[nodeIndex] = DOTSPathNode;
            //    }
            //}

            DOTSPathNode startNode = DOTSPathNodeArray[CalculateIndex(startPos.x, startPos.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startPos, endPos);
            startNode.CalculateFCost();
            DOTSPathNodeArray[startNode.index] = startNode;

            //prep lists
            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            //loop time
            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestCostFNodeIndex(openList, DOTSPathNodeArray);
                DOTSPathNode currentNode = DOTSPathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex)
                {
                    //We are there
                    break;
                }

                //Remove current node from list
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPos = new int2(currentNode.x + neighbourOffset.x, currentNode.z + neighbourOffset.y);

                    if (IsPositionInsideGrid(neighbourPos, gridSize) == false)
                    {
                        //Outside of grid
                        continue;
                    }

                    int neighbourNodeIndex = CalculateIndex(neighbourPos.x, neighbourPos.y, gridSize.x);

                    if (closedList.Contains(neighbourNodeIndex) == true)
                    {
                        //Already searched
                        continue;
                    }

                    DOTSPathNode neighbourNode = DOTSPathNodeArray[neighbourNodeIndex];
                    if (neighbourNode.isWalkable == false)
                    {
                        //Not walkable
                        continue;
                    }

                    int2 currentNodePostition = new int2(currentNode.x, currentNode.z);

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePostition, neighbourPos);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNodeIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(new int2(neighbourNode.x, neighbourNode.z), endPos);
                        neighbourNode.CalculateFCost();
                        DOTSPathNodeArray[neighbourNodeIndex] = neighbourNode;

                        if (openList.Contains(neighbourNode.index) == false)
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }
                }
            }

            DOTSPathNode endNode = DOTSPathNodeArray[endNodeIndex];
            if (endNode.cameFromNodeIndex == -1)
            {
                //Path not found
                Debug.Log("Path Not Found");
            }
            else
            {
                //Path Found
                NativeList<int2> path = CalculatePath(DOTSPathNodeArray, endNode);

                //foreach (int2 pos in path)
                //{
                //    Debug.Log(pos);
                //}

                path.Dispose();
            }

            neighbourOffsetArray.Dispose();
            DOTSPathNodeArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }


        private void FindPath(int2 startPos, int2 endPos)
        {


        }

        private NativeList<int2> CalculatePath(NativeArray<DOTSPathNode> DOTSPathNodeArray, DOTSPathNode endNode)
        {
            if (endNode.cameFromNodeIndex == -1)
            {
                //No path
                return new NativeList<int2>(Allocator.Temp);
            }
            else
            {
                //Path 
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                path.Add(new int2(endNode.x, endNode.z));

                DOTSPathNode currentNode = endNode;
                while (currentNode.cameFromNodeIndex != -1)
                {
                    DOTSPathNode cameFromNode = DOTSPathNodeArray[currentNode.cameFromNodeIndex];
                    path.Add(new int2(cameFromNode.x, cameFromNode.z));
                    currentNode = cameFromNode;
                }

                return path;

            }
        }

        private bool IsPositionInsideGrid(int2 gridPos, int2 gridSize)
        {
            return gridPos.x >= 0 && gridPos.y >= 0 && gridPos.x < gridSize.x && gridPos.y < gridSize.y;
        }

        private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<DOTSPathNode> DOTSPathNodeArray)
        {
            DOTSPathNode lowestCostDOTSPathNode = DOTSPathNodeArray[openList[0]];
            for (int i = 0; i < openList.Length; i++)
            {
                DOTSPathNode testDOTSPathNode = DOTSPathNodeArray[openList[i]];
                if (testDOTSPathNode.fCost < lowestCostDOTSPathNode.fCost)
                {
                    lowestCostDOTSPathNode = testDOTSPathNode;
                }
            }

            return lowestCostDOTSPathNode.index;
        }

        private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);
            int remaining = math.abs(xDistance - yDistance);

            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        private int CalculateIndex(int x, int z, int cellWidth)
        {
            return x + z * cellWidth;
        }


    }

    private int CalculateIndex(int x, int z, int cellWidth)
    {
        return x + z * cellWidth;
    }
}
