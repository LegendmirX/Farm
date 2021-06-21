using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileVisuals : MonoBehaviour
{
    public Dictionary<Tile, GameObject> TileGameObjectMap;
    public Dictionary<string, GameObject> Floors;

    public bool HasBuiltTiles { get; protected set; } = false;

    public void BuildTiles()
    {

        Floors = new Dictionary<string, GameObject>();

        GameObject world = new GameObject("World");
        world.transform.position = Vector3.zero;

        WorldController.instance.RegisterFloorChanged(OnFloorChanged);

        for (int i = 0; i < Area.current.Floors; i++)
        {
            GameObject floor = new GameObject("Floor" + i);
            floor.transform.position = new Vector3(0, i, 0);

            Floors.Add("Floor" + i, floor);
            //Debug.Log("Floor" + i);

            if (i > 0)
            {
                floor.SetActive(false);
            }
        }

        TileGameObjectMap = new Dictionary<Tile, GameObject>();
        foreach (Tile tile in WorldController.instance.tileManager.Tiles)
        {
            GameObject go = new GameObject();
            go.transform.position = new Vector3(tile.X, tile.Floor, tile.Z);
            go.transform.Rotate(90f, go.transform.rotation.y, go.transform.rotation.z, Space.World);
            go.name = "Tile_" + tile.X + "_" + tile.Z + "_" + tile.Floor;
            go.transform.SetParent(Floors["Floor" + tile.Floor].transform);


            SpriteRenderer r = go.AddComponent<SpriteRenderer>();
            r.sprite = SpriteManager.current.GetSprite(SpriteManager.SpriteCatagory.Tiles, tile.Type.ToString() + tile.TileSubType);
            string layerName = "";
            if (tile.Floor > 0)
            {
                go.layer = 9;
                layerName = "Upper";
            }
            else
            {
                go.layer = 8;
            }
            layerName += "Floor";
            r.sortingLayerName = layerName;

            TileGameObjectMap.Add(tile, go);
        }

        HasBuiltTiles = true;
    }

    public void OnTileChanged(Tile tile)
    {
        if(HasBuiltTiles == false)
        {
            return;
        }

        GameObject tileObj = TileGameObjectMap[tile];

        SpriteRenderer r = tileObj.GetComponent<SpriteRenderer>();
        r.sprite = SpriteManager.current.GetSprite(SpriteManager.SpriteCatagory.Tiles, tile.Type.ToString() + tile.TileSubType);
        if(tile.Watered == true)
        {
            r.color = Color.blue;
        }
        else
        {
            r.color = Color.white;
        }
    }

    void OnFloorChanged(int f)
    {
        List<GameObject> floorsList = new List<GameObject>();
        for (int i = 0; i < Floors.Count; i++)
        {
            floorsList.Add(Floors["Floor" + i]);
        }

        if (f == 0)
        {
            GameObject activeFloor = Floors["Floor" + f];
            activeFloor.SetActive(true);
            floorsList.Remove(activeFloor);
        }
        else
        {
            List<GameObject> activeFloors = new List<GameObject>();
            activeFloors.Add(Floors["Floor" + f]);
            activeFloors.Add(Floors["Floor0"]);
            foreach (GameObject floor in activeFloors)
            {
                floor.SetActive(true);
                floorsList.Remove(floor);
            }

        }

        foreach (GameObject floor in floorsList)
        {
            floor.SetActive(false);
        }
        
    }
}
