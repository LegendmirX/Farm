using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile
{
    public enum TileType
    {
        EMPTY,
        GRASS,
        TILLED,
        SAND,
        WATER
    }

    private TileType _type;
    public TileType Type
    {
        get { return _type; }
        set
        {
            TileType oldType = _type;
            _type = value;

            if (cbTileTypeChanged != null && oldType != _type)
            {

                switch (_type)
                {
                    case TileType.GRASS:
                        LinksToNeighbour = false;
                        UpdateSubType();
                        cbTileTypeChanged(this);
                        break;
                    case TileType.TILLED:
                        LinksToNeighbour = true;
                        UpdateSubType();
                        cbTileTypeChanged(this);
                        break;
                }
            }
        }
    }

    public string TileSubType;

    public int X { get; protected set; }
    public int Z { get; protected set; }
    public int Floor { get; protected set; }
    public Vector3 vector3 { get; protected set; }

    public bool LinksToNeighbour { get; protected set; }
    public bool Watered { get; protected set; }

    Action<Tile> cbTileTypeChanged;

    public Tile(int z, int x, int f)
    {
        Z = z;
        X = x;
        Floor = f;

        vector3 = new Vector3(x, f, z);
    }

    public void UpdateSubType()
    {
        if(LinksToNeighbour == true)
        {
            TileSubType = GetSubType();
            //Debug.Log("TileSubType: " + TileSubType);
        }
        else if( LinksToNeighbour == false)
        {
            TileSubType = "";
        }

    }

    public void SetWatered(bool value)
    {
        Watered = value;
        cbTileTypeChanged(this);
    }

    string GetSubType()
    {
        string subType = "";
        Tile t;

        int x = X;
        int z = Z;
        int f = Floor;

        TileManager tileManager = WorldController.instance.tileManager;
        
        bool n = false;
        bool e = false;
        bool s = false;
        bool w = false;

        t = tileManager.GetTileAt(x, z + 1, f);
        if (t != null && t.Type == this.Type)
        {
            subType += "_N";
            n = true;
        }
        t = tileManager.GetTileAt(x + 1, z, f);
        if (t != null && t.Type == this.Type)
        {
            subType += "_E";
            e = true;
        }
        t = tileManager.GetTileAt(x, z - 1, f);
        if (t != null && t.Type == this.Type)
        {
            subType += "_S";
            s = true;
        }
        t = tileManager.GetTileAt(x - 1, z, f);
        if (t != null && t.Type == this.Type)
        {
            subType += "_W";
            w = true;
        }
        t = tileManager.GetTileAt(x + 1, z + 1, f);
        if (t != null && t.Type == this.Type && n == true && e == true)
        {
            subType += "_NE";
        }
        t = tileManager.GetTileAt(x + 1, z - 1, f);
        if (t != null && t.Type == this.Type && e == true && s == true)
        {
            subType += "_SE";
        }
        t = tileManager.GetTileAt(x - 1, z - 1, f);
        if (t != null && t.Type == this.Type && s == true && w == true)
        {
            subType += "_SW";
        }
        t = tileManager.GetTileAt(x - 1, z + 1, f);
        if (t != null && t.Type == this.Type && w == true && n == true)
        {
            subType += "_NW";
        }

        return subType;
    }

    #region CallBacks
    public void RegisterTileTypeChangedCallback(Action<Tile> function)
    {
        cbTileTypeChanged += function;
    }
    public void UnregisterTileTypeChangedCallback(Action<Tile> function)
    {
        cbTileTypeChanged -= function;
    }
    #endregion
}
