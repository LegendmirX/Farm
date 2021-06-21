using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileManager : MonoBehaviour
{
    public Tile[,,] Tiles; // z, x, floor

    public List<Tile> wateredTiles;

    TileVisuals tileVisuals;

    public int Height { get; protected set; }
    public int Width { get; protected set; }
    public int Floors { get; protected set; }

    Action<Tile> OnTileCreated;

    public void SetUp()
    {
        tileVisuals = FindObjectOfType<TileVisuals>();
        wateredTiles = new List<Tile>();
    }

    public void BuildTiles(int height, int width, int floors)
    {
        Height = height;
        Width = width;
        Floors = floors;

        Tiles = new Tile[height, width, floors];

        int tilesCreated = 0;

        for (int f = 0; f < floors; f++)
        {
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Tiles[z, x, f] = new Tile(z, x, f);
                    Tiles[z, x, f].RegisterTileTypeChangedCallback(OnTileChanged);
                    if (f > 0)
                    {
                        Tiles[z, x, f].Type = Tile.TileType.EMPTY;
                    }
                    else
                    {
                        Tiles[z, x, f].Type = Tile.TileType.GRASS;
                    }

                    tilesCreated++;
                }
            }
        }
        Debug.Log(tilesCreated + " Tiles Created");
        tileVisuals.BuildTiles();
    }

    void OnTileChanged(Tile tile)
    {
        if(tile.LinksToNeighbour == true)
        {
            UpdateNeighbours(tile);
        }
        tileVisuals.OnTileChanged(tile);
    }

    void UpdateNeighbours(Tile tile)
    {
        Tile t;
        int x = tile.X - 1;
        int z = tile.Z - 1;
        int f = tile.Floor;

        for (int zCheck = z; zCheck < z + 3; zCheck++)
        {
            for (int xCheck = x; xCheck < x + 3; xCheck++)
            {
                t = WorldController.instance.tileManager.GetTileAt(xCheck, zCheck, f);

                if (t != null && t.Type == tile.Type)
                {
                    t.UpdateSubType();
                    tileVisuals.OnTileChanged(t);
                }
            }
        }
    }

    #region HelperFuncs
    public Tile GetTileAt(int x, int z, int f)
    {
        if (z >= Height || z < 0 || x >= Width || x < 0 ||  f < 0 || f > Floors)
        {
            return null;
        }
        return Tiles[z, x, f];
    }

    public void EndDayCleanUp()
    {
        foreach(Tile t in wateredTiles)
        {
            t.SetWatered(false);
        }
        wateredTiles = new List<Tile>();
    }

    public void SetWateredTile(Tile tile, bool value)
    {
        tile.SetWatered(value);
        wateredTiles.Add(tile);
    }
    #endregion

    #region CallBacks
    public void RegisterOnTileCreated(Action<Tile> func)
    {
        OnTileCreated += func;
    }
    public void UnregisterOnTileCreated(Action<Tile> func)
    {
        OnTileCreated -= func;
    }
    #endregion
}
