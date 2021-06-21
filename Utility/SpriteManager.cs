using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System;

public class SpriteManager : MonoBehaviour
{
    public enum SpriteCatagory
    {
        Tiles,
        Item,
        Plants,
        InstalledObjects
    }
    public enum SpriteRotation
    {
        Forward,
        Left,
        Backward,
        Right
    }
    public int currRotation = 0;
    public int enumLength = Enum.GetValues(typeof(SpriteRotation)).Length;

    static public SpriteManager current;

    Dictionary<string, Sprite> tileSprites;
    Dictionary<string, Sprite> itemSprites;
    Dictionary<string, Sprite> plantSprites;
    Dictionary<string, Sprite> installedObjectSprites;

    Sprite ErrorSprite;

    public void SetUp()
    {
        current = this;
    }

    #region HelperFuncs   
    string LoadImgsPath()
    {
        return "Imgs";
    }

    public int GetSpriteListLength(SpriteCatagory cat)
    {
        int length = 0;

        switch (cat)
        {
            case SpriteCatagory.Tiles:
                length = tileSprites.Count;
                break;
            case SpriteCatagory.Item:
                length = itemSprites.Count;
                break;
            case SpriteCatagory.Plants:
                length = plantSprites.Count;
                break;
            case SpriteCatagory.InstalledObjects:
                length = installedObjectSprites.Count;
                break;
        }

        return length;
    }

    public List<string> GetSpriteNames(SpriteCatagory cat)
    {
        List<string> NameList = new List<string>();

        switch (cat)
        {
            case SpriteCatagory.Tiles:
                foreach (string name in tileSprites.Keys)
                {
                    NameList.Add(name);
                }
                break;
            case SpriteCatagory.Item:
                foreach (string name in itemSprites.Keys)
                {
                    NameList.Add(name);
                }
                break;
            case SpriteCatagory.Plants:
                foreach (string name in plantSprites.Keys)
                {
                    NameList.Add(name);
                }
                break;
            case SpriteCatagory.InstalledObjects:
                foreach (string name in installedObjectSprites.Keys)
                {
                    NameList.Add(name);
                }
                break;

        }

        return NameList;
    }

    public Sprite GetSprite(SpriteCatagory cat, string spriteName)
    {
        Sprite sprite = null;

        switch (cat)
        {
            case SpriteCatagory.Tiles:
                if (tileSprites.ContainsKey(spriteName) == false)
                {
                    if(spriteName == "EMPTY")
                    {
                        return null;
                    }
                    Debug.LogError("SpriteManager - TileSprites: Dose not contain " + spriteName);
                    sprite = ErrorSprite;
                }
                else
                {
                    sprite = tileSprites[spriteName];
                }
                break;
            case SpriteCatagory.Item:
                if (itemSprites.ContainsKey(spriteName) == false)
                {
                    Debug.LogError("SpriteManager - ItemSprites: Dose not contain " + spriteName);
                    sprite = ErrorSprite;
                }
                else
                {
                    sprite = itemSprites[spriteName];
                }
                break;
            case SpriteCatagory.Plants:
                if (plantSprites.ContainsKey(spriteName) == false)
                {
                    Debug.LogError("SpriteManager - PlantSprites: Dose not contain " + spriteName);
                    sprite = ErrorSprite;
                }
                else
                {
                    sprite = plantSprites[spriteName];
                }
                break;
            case SpriteCatagory.InstalledObjects: 
                if (installedObjectSprites.ContainsKey(spriteName) == false)
                {
                    Debug.LogError("SpriteManager - InstalledObjectSprites: Dose not contain " + spriteName);
                    sprite = ErrorSprite;
                }
                else
                {
                    sprite = installedObjectSprites[spriteName];
                }
                break;
        }
        return sprite;
    }
    #endregion

    #region LoadSpriteFuncs
    public void LoadTileSprites()
    {
        //Debug.Log("-TileSprites:");
        string filePath = LoadImgsPath();

        string path = Path.Combine(filePath, "Tiles");
        tileSprites = new Dictionary<string, Sprite>();

        List<Sprite> sprites = LoadSpritesFromPath(path);

        if (sprites != null && sprites.Count > 0)
        {
            foreach (Sprite s in sprites)
            {
                //Debug.Log(s.name);
                tileSprites.Add(s.name, s);
            }
        }
        else
        {
            Debug.LogError("Somthing went terribly wrong");
        }
    }

    public void LoadItemSprites()
    {
        //Debug.Log("-ItemSprites:");
        string filePath = LoadImgsPath();

        string path = Path.Combine(filePath, "Items");
        itemSprites = new Dictionary<string, Sprite>();

        List<Sprite> sprites = LoadSpritesFromPath(path);

        if (sprites != null && sprites.Count > 0)
        {
            foreach (Sprite s in sprites)
            {
                //Debug.Log(s.name);
                itemSprites.Add(s.name, s);
            }
        }
        else
        {
            Debug.LogError("Somthing went terribly wrong");
        }
    }

    public void LoadPlantSprites()
    {
        //Debug.Log("-PlantSprites:");
        string filePath = LoadImgsPath();

        string path = Path.Combine(filePath, "Plants");
        plantSprites = new Dictionary<string, Sprite>();

        List<Sprite> sprites = LoadSpritesFromPath(path);

        if (sprites != null && sprites.Count > 0)
        {
            foreach (Sprite s in sprites)
            {
                //Debug.Log(s.name);
                plantSprites.Add(s.name, s);
            }
        }
        else
        {
            Debug.LogError("Somthing went terribly wrong");
        }
    }

    public void LoadInstalledObjectSprites()
    {
        //Debug.Log("-InstalledObjectSprites:");
        string filePath = LoadImgsPath();

        string path = Path.Combine(filePath, "InstalledObjects");
        installedObjectSprites = new Dictionary<string, Sprite>();

        List<Sprite> sprites = LoadSpritesFromPath(path);

        if (sprites != null && sprites.Count > 0)
        {
            foreach (Sprite s in sprites)
            {
                //Debug.Log(s.name);
                installedObjectSprites.Add(s.name, s);
            }
        }
        else
        {
            Debug.LogError("Somthing went terribly wrong");
        }
    }

    List<Sprite> LoadSpritesFromPath(string path)
    {
        List<Sprite> ls = new List<Sprite>();
        //Sprite[] sprites = (Sprite[])Resources.LoadAll(path, typeof(Sprite));

        foreach (Sprite loadedsprite in Resources.LoadAll(path, typeof(Sprite)))
        {
            ls.Add(loadedsprite);
        }

        //foreach (Sprite s in sprites)
        //{
        //    ls.Add(s);
        //}

        return ls;
    }

    #endregion
}
