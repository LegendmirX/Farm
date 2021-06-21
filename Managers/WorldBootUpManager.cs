using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldBootUpManager : MonoBehaviour
{
    public static WorldBootUpManager current;

    public bool IsLoading = false;
    //bool hasUpdate = false;
    public static bool LoadFromSave = false;
    static string fileName;

    float LoadAmount;

    Action callback;
    Action cbLoadSaveList;
    static List<Action> onLoaderCallback;
    Dictionary<string, Action> onLoaderCallbackDiction;

    private void Awake()
    {
        current = this;
        onLoaderCallbackDiction = new Dictionary<string, Action>();
        onLoaderCallback = new List<Action>();

        onLoaderCallbackDiction.Add("GameAssets",                               GameAssets.i.SetUp);
        onLoaderCallbackDiction.Add("UIReferences",                             FindObjectOfType<UIReferences>().SetUp);
        onLoaderCallbackDiction.Add("SpriteManager.SetUp",                      FindObjectOfType<SpriteManager>().SetUp);
        onLoaderCallbackDiction.Add("SpriteManager.LoadTileSprites" ,           FindObjectOfType<SpriteManager>().LoadTileSprites);
        onLoaderCallbackDiction.Add("SpriteManager.LoadItemSprites",            FindObjectOfType<SpriteManager>().LoadItemSprites);
        onLoaderCallbackDiction.Add("SpriteManager.LoadPlantSprites",           FindObjectOfType<SpriteManager>().LoadPlantSprites);
        onLoaderCallbackDiction.Add("SpriteManager.LoadInstalledObjectSprites", FindObjectOfType<SpriteManager>().LoadInstalledObjectSprites);
        onLoaderCallbackDiction.Add("TileManager",                              FindObjectOfType<TileManager>().SetUp);
        onLoaderCallbackDiction.Add("InstalledObjects",                         FindObjectOfType<InstalledObjectManager>().SetUp);
        onLoaderCallbackDiction.Add("CharacterManager",                         FindObjectOfType<NPCManager>().SetUp);
        onLoaderCallbackDiction.Add("InventoryManager",                         FindObjectOfType<InventoryManager>().SetUp);
        onLoaderCallbackDiction.Add("PlantManager",                             FindObjectOfType<PlantManager>().SetUp);
        onLoaderCallbackDiction.Add("MouseManager",                             FindObjectOfType<MouseManager>().SetUp);
        onLoaderCallbackDiction.Add("WorldController",                          FindObjectOfType<WorldController>().SetUp);
    }

    private void Start()
    {
        foreach(string funcName in onLoaderCallbackDiction.Keys)
        {
            Debug.Log(funcName);
            Action action = onLoaderCallbackDiction[funcName];
            action.Invoke();
        }
        foreach(Action func in onLoaderCallback)
        {
            Debug.Log(func.Method.Name);
            func.Invoke();
        }
        UIReferences.i.ToolBar.SetActive(true);
    }

    //void UpdateCallBack(Action callback)
    //{
    //    hasUpdate = false;
    //    Bar.fillAmount += LoadAmount;
    //    callback.Invoke();
    //    //Debug.Log(callback.Method.ToString());
    //}

    //void Update()
    //{
    //    if (IsLoading == true)
    //    {
    //        if (onLoaderCallback.Count == 0)
    //        {
    //            WorldController.instance.LateSetUp();
    //            LoadScreen.SetActive(false);
    //            IsLoading = false;
    //        }
    //        else
    //        {
    //            if (hasUpdate == false)
    //            {
    //                hasUpdate = true;
    //                Loading();
    //            }
    //        }
    //    }
    //}

    #region CallBacks
    public void RegisterOnLoadCallBack(Action func)
    {
        onLoaderCallback.Add(func);
    }
    public void UnregisterOnLoadCallBack(Action func)
    {
        onLoaderCallback.Remove(func);
    }
    #endregion
}
