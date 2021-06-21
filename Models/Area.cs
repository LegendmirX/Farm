using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Area
{
    public static Area current { get; protected set; }

    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public int Floors { get; protected set; }

    private Func<int, int, int, Tile> getTileAt;

    public Dictionary<Tile, Plant> PlantMap;
    public Dictionary<Tile, InstalledObject> InstalledObjectMap;
    public Dictionary<InstalledObject, Needs.Need> NeedsMap;
    public Dictionary<InstalledObject, Inventory> StorageMap;

    public GridUtil<HeatMapGridObject> grid;
    public Pathfinding PathFindingGrid;

    public Area(int width, int height, int floors, Func<int, int, int, Tile> getTileAtFunc = null)
    {
        BuildWorld(width, height, floors);
        getTileAt = getTileAtFunc;
    }

    void BuildWorld(int width, int height, int floors)
    {
        current = this;

        Width = width;
        Height = height;
        Floors = floors;

        PlantMap            = new Dictionary<Tile, Plant>();
        InstalledObjectMap  = new Dictionary<Tile, InstalledObject>();
        NeedsMap            = new Dictionary<InstalledObject, Needs.Need>();
        StorageMap          = new Dictionary<InstalledObject, Inventory>();

        WorldController.instance.tileManager.BuildTiles(width, height, floors);

        PathFindingGrid = new Pathfinding(Width, Height, 1, true, isEnterable);

        //grid = new GridUtil<HeatMapGridObject>(20, 20, 1, new Vector3(45, 0 , 45), createHeatMapObject);
        //WorldController.instance.heatMapManager.SetUp(grid);
        //MeshUtil mesh = new MeshUtil();
    }

    public void Update(float deltaTime)
    {
        //Used to handle time here. may not need this anymore
    }

    //private HeatMapGridObject createHeatMapObject(GridUtil<HeatMapGridObject> g, int x, int z)
    //{
    //    return new HeatMapGridObject(g, x, z);
    //}

    private PathNode.Enterability isEnterable(PathNode node)
    {
        Tile tile = WorldController.instance.currentArea.getTileAt(node.x, node.z, 0); //TODO: havent we already passed this func to the area?

        if (node.isWalkable == false) //Not walkable
        {
            Debug.Log("This Node Should not be in our path");
            return PathNode.Enterability.Never;
        }

        //Node is walkable
        InstalledObject obj = null;

        if (WorldController.instance.currentArea.InstalledObjectMap.ContainsKey(tile) == true)
        {
            obj = WorldController.instance.currentArea.InstalledObjectMap[tile];
        }
        else
        {
            // Node Dose not contain an OBJ
            return PathNode.Enterability.Yes;
        }

        //if we are here tile has an obj
        if (obj.Type == InstalledObject.ObjectType.Door)
        {
            bool isOpening = (bool)obj.Paramaters["isDoorOpening"];

            if(isOpening == false)
            {
                obj.Paramaters["isDoorOpening"] = true;
                obj.Paramaters["hasPassedOver"] = false;
                obj.RegisterUpdateAction(InstalledObjectActions.DoorUpdateAction);
                return PathNode.Enterability.Soon;
            }
            //If we are here door is opening and door update action is registered

            float openAmount = (float)obj.Paramaters["openAmount"];

            if(openAmount >= 1)
            {
                return PathNode.Enterability.Yes;
            }
            else
            {
                return PathNode.Enterability.Soon;
            }
        }

        return PathNode.Enterability.Yes;
    }
}
