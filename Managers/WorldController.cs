using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour
{
    public static WorldController instance;
    public Area currentArea;
    int Width = 100;
    int Height = 100;
    int Floors = 3;

    public bool DebugMode = false;

    public int currentFloor = 0;

    public int Day;
    public int TimeHours;
    public int TimeMins;

    private int minsPerHour = 60;
    private float secondsPerMin = 1.39f;
    private float secondsTimer = 0;

    public TileManager              tileManager;
    public PlayerController         playerController;
    public NPCManager               npcManager;
    public InventoryManager         inventoryManager;
    public PlantManager             plantManager;
    public MouseManager             mouseManager;
    public InstalledObjectManager   installedObjectManager;

    public PathfindingDOTS pathfindingDOTS;
    //public HeatMapManager   heatMapManager;
    //public HeatMapBoolManager heatMapBoolManager;

    Action<int> cbFloorChanged;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    public static bool levelBuildMode = false;   ////             True = Build Levels            ////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        
    }

    void OnEnable()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {
        if(DebugMode == true) //Check debug menu
        {
            if(UIReferences.i.DebugMainMenu.activeSelf == false)
            {
                UIReferences.i.DebugMainMenu.SetActive(true);
            }
        }
        else
        {
            if (UIReferences.i.DebugMainMenu.activeSelf == true)
            {
                UIReferences.i.DebugMainMenu.SetActive(false);
            }
        }

        float deltaTime = Time.deltaTime;

        if (currentArea == null)
        {
            return;
        }
        else
        {
            Area.current.Update(deltaTime);
        }
        //Pass all updates from here so we can impliment a pause.
        npcManager.Updateframe(deltaTime);
        installedObjectManager.UpdateFrame(deltaTime);

        secondsTimer += deltaTime;
        if (secondsTimer >= secondsPerMin) //Tick Tock
        {
            secondsTimer -= secondsPerMin;
            IncrimentMins(1);
            UpdateClock();
        }
    }

    public void SetUp()
    {
        tileManager             = FindObjectOfType<TileManager>();
        playerController        = FindObjectOfType<PlayerController>();
        npcManager              = FindObjectOfType<NPCManager>();
        inventoryManager        = FindObjectOfType<InventoryManager>();
        plantManager            = FindObjectOfType<PlantManager>();
        mouseManager            = FindObjectOfType<MouseManager>();
        installedObjectManager  = FindObjectOfType<InstalledObjectManager>();

        pathfindingDOTS         = FindObjectOfType<PathfindingDOTS>();
        //heatMapManager      = FindObjectOfType<HeatMapManager>();

        if (instance != null)
        {
            Debug.LogError("2 worldcontrollers??");
        }
        instance = this;

        npcManager.CharacterPrototypes                      = PrototypeManager.BuildCharacterPrototypes();
        inventoryManager.ItemPrototypes                     = PrototypeManager.BuildItemPrototypes();
        plantManager.PlantPrototypes                        = PrototypeManager.BuildPlantPrototypes();
        installedObjectManager.InstalledObjectPrototypes    = PrototypeManager.BuildInstalledObjectPrototypes();

        WorldBootUpManager.current.RegisterOnLoadCallBack(BuildWorld);
        //TODO: Impliment saving and loading
        //if(SaveFileController.LoadFromSave == true)
        //{
        //    SaveFileController.LoadFromSave = false;
        //    SaveFileController.current.RegisterLoaderCallback( SaveFileController.current.CreateWorldFromSaveFile );
        //}
        //else
        //{
        //    if(levelBuildMode == false)
        //    {
        //        SaveFileController.current.RegisterLoaderCallback(SaveFileController.current.LoadLevel);
        //    }
        //    else
        //    {
        //        SaveFileController.current.RegisterLoaderCallback( CreateEmptyWorld );
        //    }
        //}
    }

    public void LateSetUp()
    {
        //if (levelBuildMode == false)
        //{
        //    SaveFileController.current.SetBuildMenus(false);
        //    if (world.playerSpawns.Count <= 0 || world.playerSpawns == null)
        //    {
        //        Tile tile = GetTileAtWorldCoord(new Vector3((Width / 2), 0, (Height / 2)), 0);
        //        world.CreatePlayer(tile, Vector3.zero);
        //    }
        //    ActivateSpawns();
        //}
        //else
        //{
        //    SaveFileController.current.SetBuildMenus(true);
        //}
    }

    void BuildWorld()
    {
        //load or new? for now just make empty
        CreateEmptyWorld();
    }

    void CreateEmptyWorld()
    {
        currentArea = new Area(Width, Height, Floors, tileManager.GetTileAt);

        //Debug for DOTS test
        pathfindingDOTS.SetUp(100, 100);

        //creatPlayer
        Vector3 startPos = new Vector3(50, 0, 50); //temp solution
        CreatePlayer(startPos);
        CreateNPC("Kevin", startPos += new Vector3(0, 0, 5));
        CreateNPC("Jessica", startPos += new Vector3(5, 0, 0));
    }

    public void ChangeFloor(int f) //dont think i use this anymore
    {
        if (cbFloorChanged == null)
        {
            return;
        }
        cbFloorChanged(f);
    }

    public Tile GetTileAtWorldCoord(Vector3 coord, int floor = -1)
    {
        int x = Mathf.FloorToInt(coord.x + 0.5f); //floor to int and set to center of tile for the array
        int z = Mathf.FloorToInt(coord.z + 0.5f);
        int f = 0;

        if(floor == -1)
        {
            f = currentFloor;
        }
        //Debug.Log("X=" + x + " Z=" + z + " f=" + f);
        return tileManager.GetTileAt(x, z, f);
    }

    public void AddDays(int days)
    {
        Dictionary<Plant, bool> toAge = new Dictionary<Plant, bool>();
        foreach(Plant plant in plantManager.Plants) //Get all plants that are satisfied to be able to grow
        {
            Tile t = GetTileAtWorldCoord(plant.Position);
            if (t.Watered == true)
            {
                toAge.Add(plant, true);
            }
            else
            {
                toAge.Add(plant, false);
            }
        }
        plantManager.AgePlants(days, toAge);
        if(plantManager.PlantsToRemove != null && plantManager.PlantsToRemove.Count > 0) //check for dead plants
        {
            foreach(Plant plant in plantManager.PlantsToRemove)
            {
                RemovePlant(plant);
            }
        }

        tileManager.EndDayCleanUp(); //gets rid of watered tiles

        npcManager.EndDay(days);
    }

    public void UpdatePlayerInventory() //use this when numbers change or items move
    {
        Inventory playerInv = playerController.PlayerData.inventory;
        inventoryManager.InventoryChanged(playerInv);
        inventoryManager.ToolBarOnChanged();
    }

    public void Harvest(Tile tile)
    {
        Plant plant = currentArea.PlantMap[tile];

        if (plant == null)
        {
            return;
        }

        if(plant.ReadyToHarvest() == false)
        {
            Debug.Log("NotReady");
            return;
        }
        //plant can be harvested

        InventoryItem proto = inventoryManager.ItemPrototypes[plant.PlantType];
        InventoryItem item = InventoryItem.CreateItem(proto, plant.HarvestQuantity);
        Inventory playerInv = playerController.PlayerData.inventory;

        //add harvest stuff to inv if there is space
        int slotID;
        if (inventoryManager.DoseInvHaveSpace(playerInv, item, out slotID) == true) 
        {
            inventoryManager.AddItem(playerInv, item, slotID);
            plant.Harvest();
            plantManager.OnPlantChange(plant);
        }
        else
        {
            item.Remove();
            Debug.Log("NoSpace for item");
        }
    }

    public void InteractWithGameObject(GameObject GO, Player player)
    {
        InteractWithGameObject(GO, player, null);
    }

    public void InteractWithGameObject(GameObject GO, NPC npc)
    {
        InteractWithGameObject(GO, null, npc);
    }

    private void InteractWithGameObject(GameObject GO, Player player = null, NPC npc = null)
    {
        string n = "";
        if(player != null)
        {
            n = player.Name;
        }
        else
        {
            n = npc.Name;
        }
        Debug.Log(n + " interacted with " + GO.name);
        switch (GO.tag) //use tag to find out what we are playing with
        {
            case "NPC":
                foreach (NPC dude in npcManager.NPCS)
                {
                    if (dude.GO == GO)
                    {
                        if(player != null) //player is talking to someone
                        {
                            npcManager.PlayerInteract(dude);
                        }
                        else //NPC is talking to NPC
                        {
                            npcManager.NPCInteract(dude, npc);
                        }
                        return;
                    }
                }
                break;
            case "Interactable":
                foreach (InstalledObject obj in installedObjectManager.InstalledObjects)
                {
                    if (obj.GO == GO)
                    {
                        if(player != null) //player messing with obj
                        {
                            if (obj.Type == InstalledObject.ObjectType.Storage) //open inventory
                            {
                                Inventory objInv = currentArea.StorageMap[obj];
                                inventoryManager.OpenTradeWindow(player.inventory, objInv);
                                UIReferences.i.ToolBar.SetActive(false);
                                return;
                            }

                            installedObjectManager.Interact(obj, player); //other interaction
                        }
                        else //NPC messing with obj
                        {
                            installedObjectManager.Interact(obj, npc);
                        }
                        return;
                    }
                }
                break;
            case "Shop":
                //Check shop map
                //Call use shop
                //Can NPC's do this?
                break;
        }
    }

    public Vector3 FindWhatINeed(Needs.Need need, Vector3 position)
    {
        //TODO: for now we assume same area as current

        InstalledObject foundObj = null;
        float distance = Mathf.Infinity;

        foreach(InstalledObject obj in currentArea.NeedsMap.Keys)
        {
            if(currentArea.NeedsMap[obj] == need)
            {
                float d = Vector3.Distance(position, obj.Position);
                if(d < distance)
                {
                    distance = d;
                    foundObj = obj;
                }
            }
        }

        if(foundObj == null)
        {
            //We found nothing
            return Vector3.one * -1;
        }
        //we got one.
        return foundObj.Position;
    }

    #region CreateThings
    public void CreatePlayer(Vector3 startPos)
    {
        int startingInvCapactity = 10;
        Player p = npcManager.BuildPlayer();
        p.inventory = inventoryManager.CreateInventory(startingInvCapactity, p.Name);

        //Debug section: giving player items to start with
        InventoryItem can = InventoryItem.CreateItem(inventoryManager.ItemPrototypes["WateringCan"], 20);
        int canSlot;
        inventoryManager.DoseInvHaveSpace(p.inventory, can, out canSlot);
        inventoryManager.AddItem(p.inventory, can, canSlot);
        InventoryItem tool = InventoryItem.CreateItem(inventoryManager.ItemPrototypes["WoodHoe"], 1);
        int toolSlot;
        inventoryManager.DoseInvHaveSpace(p.inventory, tool, out toolSlot);
        inventoryManager.AddItem(p.inventory, tool, toolSlot);
        InventoryItem seeds = InventoryItem.CreateItem(inventoryManager.ItemPrototypes["CornSeeds"], 99);
        int seedsSlot;
        inventoryManager.DoseInvHaveSpace(p.inventory, seeds, out seedsSlot);
        inventoryManager.AddItem(p.inventory, seeds, seedsSlot);


        GameObject go = npcManager.BuildPlayerInstance(startPos);
        playerController.SetPlayer(p, go);
    }

    public bool CreatePlant(Tile tile, string type)
    {
        if(currentArea.PlantMap.ContainsKey(tile) == true)
        {
            Debug.Log("TileContains a plant");
            return false;
        }

        Plant plant = plantManager.CreatePlant(type, tile.vector3);

        if(plant == null)
        {
            return false;
        }

        currentArea.PlantMap.Add(tile, plant);
        return true;
    }

    public void CreateNPC(string name, Vector3 startPos)
    {
        npcManager.CreateNPC(name, startPos);
    }

    public void CreateInstalledObject(string type, Vector3 position)
    {
        InstalledObject proto = installedObjectManager.InstalledObjectPrototypes[type];
        if(proto == null)
        {
            Debug.Log("InstalledObjectProtos dose not contain " + type);
        }
        //We have a prototype

        Tile tile = GetTileAtWorldCoord(position); 
        if(tile == null || tile.Type != Tile.TileType.GRASS || currentArea.InstalledObjectMap.ContainsKey(tile) == true) //TODO: impliment concreat area to build on.
        {
            Debug.Log("Connot build here");
            return;
        }
        //build area is ok

        Vector3 pos = new Vector3(tile.X, 0, tile.Z);

        currentArea.PathFindingGrid.SetIsWalkable(pos, proto.IsWalkable);

        InstalledObject obj = installedObjectManager.CreateInstalledObject(type, pos);

        if(obj.Type == InstalledObject.ObjectType.Storage) //dose it need an inventory
        {
            Inventory inv = inventoryManager.CreateInventory(20, obj.SubType);
            currentArea.StorageMap.Add(obj, inv);
        }

        if(obj.SatisfiesNeed == true)
        {
            currentArea.NeedsMap.Add(obj, obj.NeedSatisfies);
        }

        currentArea.InstalledObjectMap.Add(tile, obj);
    }
    #endregion

    #region RemoveThings 
    public void RemovePlant(Plant plant)
    {
        Tile tile = GetTileAtWorldCoord(plant.Position);

        currentArea.PlantMap.Remove(tile);
        plantManager.OnPlantRemoved(plant);
    }

    private void RemoveInstalledObject(InstalledObject obj)
    {
        Tile tile = GetTileAtWorldCoord(obj.Position);
        currentArea.PathFindingGrid.SetIsWalkable(obj.Position, true);
        currentArea.InstalledObjectMap.Remove(tile);
        installedObjectManager.OnInstalledObjectRemoved(obj);
        if(obj.Type == InstalledObject.ObjectType.Storage)
        {
            currentArea.StorageMap.Remove(obj);
        }
    }
    public void RemoveInstalledObject(Vector3 position)
    {
        Tile tile = GetTileAtWorldCoord(position);
        if(currentArea.InstalledObjectMap.ContainsKey(tile) == false)
        {
            return;
        }
        InstalledObject obj = currentArea.InstalledObjectMap[tile];
        RemoveInstalledObject(obj);
    }

    #endregion

    #region ClockFuncs
    public void IncrimentMins(int value)
    {
        TimeMins += value;

        if (TimeMins >= minsPerHour)
        {
            TimeMins -= minsPerHour;
            IncrimentHours(1);
        }
    }

    public void IncrimentHours(int value)
    {
        TimeHours += value;
        WorldController.instance.npcManager.HourlyUpdate(1);
        if (TimeHours >= 24)
        {
            TimeHours = 0;
            IncrimentDays(1);
        }
    }

    public void IncrimentDays(int days)
    {
        Day += days;
        WorldController.instance.AddDays(days);
    }

    public void UpdateClock()
    {
        string time = TimeHours + ":" + TimeMins;
        string day = "Day " + Day;

        UIReferences.i.ClockDayField.GetComponent<Text>().text = day;
        UIReferences.i.ClockTimeField.GetComponent<Text>().text = time;
    }
    #endregion

    #region CallBacks
    public void RegisterFloorChanged(Action<int> callbackfunc)
    {
        cbFloorChanged += callbackfunc;
    }
    public void UnregisterFloorChanged(Action<int> callbackfunc)
    {
        cbFloorChanged -= callbackfunc;
    }
    #endregion
}
