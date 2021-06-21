using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridUtil<TGridObject>
{
    public const int HeatMap_Max = 100;
    public const int HeatMap_Min = 0;

    public event EventHandler<OnGridObjectChangedArgs> OnGridObjectChanged;
    public class OnGridObjectChangedArgs: EventArgs
    {
        public int x;
        public int z;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    Vector3 gridOffset;
    private TGridObject[,] gridMap;
    private TextMeshPro[,] debugTextArray;

    public GridUtil(int width, int height, float cellSize, Vector3 originPosition, Func<GridUtil<TGridObject>, int, int, bool, Func<PathNode, PathNode.Enterability>, TGridObject> createGridObject, bool walkable = true, Func<PathNode, PathNode.Enterability> enterableFunc = null)
    {
        this.width          = width;
        this.height         = height;
        this.cellSize       = cellSize;
        this.originPosition = originPosition;

        gridMap         = new TGridObject[width, height];
        debugTextArray  = new TextMeshPro[width, height];

        gridOffset = new Vector3(cellSize, 0, cellSize) * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridMap[x, z] = createGridObject(this, x, z, walkable, enterableFunc);
            }
        }

        bool showDebug = false;
        if(showDebug == true)
        {
            for (int x = 0; x < gridMap.GetLength(0); x++)
            {
                for (int z = 0; z < gridMap.GetLength(1); z++)
                {
                    debugTextArray[x, z] = TextMeshUtil.CreateWorldText(gridMap[x, z]?.ToString(), null, GetWorldPosition(x,z), new Vector3(90,0,0), 3, Color.white, TMPro.TextAlignmentOptions.Center, "Characters");

                    Debug.DrawLine(GetWorldPosition(x, z) - gridOffset, GetWorldPosition(x, z + 1) - gridOffset, Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z) - gridOffset, GetWorldPosition(x + 1, z) - gridOffset, Color.white, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, height) - gridOffset, GetWorldPosition(width, height) - gridOffset, Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0) - gridOffset, GetWorldPosition(width, height) - gridOffset, Color.white, 100f);

            OnGridObjectChanged += (object sender, OnGridObjectChangedArgs eventArgs) => { debugTextArray[eventArgs.x, eventArgs.z].text = gridMap[eventArgs.x, eventArgs.z]?.ToString(); };
        }
    }

    public void TriggerGridObjectChanged(int x, int z)
    {
        if(OnGridObjectChanged != null)
        {
            OnGridObjectChanged(this, new OnGridObjectChangedArgs { x = x, z = z });
        }
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        worldPosition = (worldPosition - originPosition) + gridOffset;
        x = Mathf.FloorToInt(worldPosition.x );
        z = Mathf.FloorToInt(worldPosition.z );
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        if(x >= 0 && z >= 0 && x< width && z < height)
        {
            gridMap[x, z] = value;
            if (OnGridObjectChanged != null)
            {
                OnGridObjectChanged(this, new OnGridObjectChangedArgs { x = x, z = z });
            }
        }
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        SetGridObject(x, z, value);
    }

    //public void AddValue(int x, int z, TGridObject value)
    //{
    //    SetValue(x, z, GetValue(x, z) + value);
    //}

    //public void AddValueSquare(Vector3 worldPosition, TGridObject value, int range)
    //{
    //    GetXZ(worldPosition, out int originX, out int originZ);
    //    for (int x = 0; x < range; x++)
    //    {
    //        for (int z = 0; z < range; z++)
    //        {
    //            AddValue(originX + x, originZ + z, value);
    //        }
    //    }
    //}

    //public void AddValueDiamond(Vector3 worldPosition, TGridObject value, int range)
    //{
    //    GetXZ(worldPosition, out int originX, out int originZ);
    //    for (int x = 0; x < range; x++)
    //    {
    //        for (int z = 0; z < range - x; z++)
    //        {
    //            AddValue(originX + x, originZ + z, value);
    //            if(x != 0)
    //            {
    //                AddValue(originX - x, originZ + z, value);
    //            }
    //            if(z != 0)
    //            {
    //                AddValue(originX + x, originZ - z, value);
    //                if(x != 0)
    //                {
    //                    AddValue(originX - x, originZ - z, value);
    //                }
    //            }
    //        }
    //    }
    //}

    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridMap[x, z];
        }
        return default(TGridObject);
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }
}
