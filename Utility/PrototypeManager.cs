using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrototypeManager
{
    public static Dictionary<string, InventoryItem> BuildItemPrototypes()
    {
        Dictionary<string, InventoryItem> itemProtos = new Dictionary<string, InventoryItem>();

        itemProtos.Add("WoodHoe",
            InventoryItem.CreatePrototype(
                InventoryItem.InventoryType.TOOL,
                "WoodHoe",
                1,
                false,
                1,
                null,
                "Hoe_OnUse"
                )
            );
        itemProtos.Add("CornSeeds",
            InventoryItem.CreatePrototype(
                InventoryItem.InventoryType.SEEDS,
                "CornSeeds",
                2,
                true,
                99,
                null,
                "Seeds_OnUse"
                )
            );
        itemProtos.Add("Corn",
            InventoryItem.CreatePrototype(
                InventoryItem.InventoryType.MATERIAL,
                "Corn",
                3,
                true,
                99
                )
            );
        itemProtos.Add("WateringCan",
            InventoryItem.CreatePrototype(
                InventoryItem.InventoryType.TOOL,
                "WateringCan",
                4,
                true,
                20,
                null,
                "WateringCan_OnUse"
                )
            );
        itemProtos.Add("Coin",
            InventoryItem.CreatePrototype(
                InventoryItem.InventoryType.Currency,
                "Coin",
                5,
                true,
                999
                )
            );

        return itemProtos;
    }

    public static Dictionary<string, Plant> BuildPlantPrototypes()
    {
        Dictionary<string, Plant> plantProtos = new Dictionary<string, Plant>();

        plantProtos.Add("Corn",
            Plant.CreatePrototype(
                "Corn",
                30,
                5,
                true,
                2,
                true
                )
            );

        return plantProtos;
    }

    public static Dictionary<string, NPC> BuildCharacterPrototypes()
    {
        Dictionary<string, NPC> protos = new Dictionary<string, NPC>();

        protos.Add("Kevin",
            NPC.CreatePrototype(
                "Kevin",
                ChatLines("Kevin")
                )
            );
        protos.Add("Jessica",
            NPC.CreatePrototype(
                "Jessica",
                ChatLines("Jessica")
                )
            );

        return protos;
    }

    public static Dictionary<string, InstalledObject> BuildInstalledObjectPrototypes()
    {
        Dictionary<string, InstalledObject> protos = new Dictionary<string, InstalledObject>();

        protos.Add("Wall",
            InstalledObject.CreatePrototype(
                InstalledObject.ObjectType.InstalledObject,
                "Wall",
                false
                )
            );
        protos.Add("Door",
            InstalledObject.CreatePrototype(
                InstalledObject.ObjectType.Door,
                "Door",
                true,
                default(Needs.Need),
                false,
                null,
                getInstalledObjectUpdateActions("Door"),
                getInstalledObjectParamaters("Door"),
                getInstalledObjectFunctions("Door")
                )
            );
        protos.Add("Storage",
            InstalledObject.CreatePrototype(
                InstalledObject.ObjectType.Storage,
                "Storage",
                false
                )
            );
        protos.Add("Fridge",
            InstalledObject.CreatePrototype(
                InstalledObject.ObjectType.InstalledObject,
                "Fridge",
                false,
                Needs.Need.Hunger,
                true,
                InstalledObjectActions.GetInteractAction("FridgeInteractAction")
                )
            );
        protos.Add("Toilet",
            InstalledObject.CreatePrototype(
                InstalledObject.ObjectType.InstalledObject,
                "Toilet",
                false,
                Needs.Need.Toilet,
                true,
                InstalledObjectActions.GetInteractAction("ToiletInteratAction")
                )
            );
        protos.Add("Sink",
            InstalledObject.CreatePrototype(
                InstalledObject.ObjectType.InstalledObject,
                "Sink",
                false,
                Needs.Need.Thirst,
                true,
                InstalledObjectActions.GetInteractAction("SinkInteractAction")
                )
            );
        protos.Add("Bed",
            InstalledObject.CreatePrototype(
                InstalledObject.ObjectType.InstalledObject,
                "Bed",
                false,
                Needs.Need.Sleep,
                true,
                InstalledObjectActions.GetInteractAction("BedInteractAction")
                )
            );
        protos.Add("Shop",
            InstalledObject.CreatePrototype(
                InstalledObject.ObjectType.Shop,
                "Shop",
                false,
                default,
                false,
                null,
                null,
                getInstalledObjectParamaters("Shop")
                )
            );

        return protos;
    }

    private static Dictionary<string, Relationship.RelationshipStatus> ChatLines(string name)
    {
        Dictionary<string, Relationship.RelationshipStatus> chatLines = new Dictionary<string, Relationship.RelationshipStatus>();

        switch (name)
        {
            case "Kevin":
                chatLines.Add("oh its you.",
                    Relationship.RelationshipStatus.Adversary);
                chatLines.Add("Hello there",
                    Relationship.RelationshipStatus.Acquaintance);
                chatLines.Add("Its nice to see you again",
                    Relationship.RelationshipStatus.Friend);
                chatLines.Add("Whats up. How gose the farming",
                    Relationship.RelationshipStatus.CloseFriend);
                chatLines.Add("We should go fishing some time",
                    Relationship.RelationshipStatus.LoveInterest);
                chatLines.Add("Hope your day gose well <3",
                    Relationship.RelationshipStatus.Lover);
                break;
            case "Jessica":
                chatLines.Add("oh its you.",
                    Relationship.RelationshipStatus.Adversary);
                chatLines.Add("Hello there",
                    Relationship.RelationshipStatus.Acquaintance);
                chatLines.Add("Its nice to see you again",
                    Relationship.RelationshipStatus.Friend);
                chatLines.Add("Whats up. How gose the farming",
                    Relationship.RelationshipStatus.CloseFriend);
                chatLines.Add("We should go fishing some time",
                    Relationship.RelationshipStatus.LoveInterest);
                chatLines.Add("Hope your day gose well <3",
                    Relationship.RelationshipStatus.Lover);
                break;
        }

        return chatLines;
    }

    public static List<string> ShopItems(Shop.Type type)
    {
        List<string> items = new List<string>();

        switch (type)
        {
            case Shop.Type.General:
                items.Add("CornSeeds");
                break;
        }

        return items;
    }

    private static List<Action<InstalledObject,float>> getInstalledObjectUpdateActions(string objType)
    {
        List<Action<InstalledObject, float>> udpateActions = new List<Action<InstalledObject, float>>();

        switch (objType)
        {
            case "Door":
                udpateActions.Add(InstalledObjectActions.DoorUpdateAction);
                break;
        }

        return udpateActions;
    }

    private static Dictionary<string, object> getInstalledObjectParamaters(string objType)
    {
        Dictionary<string, object> objParams = new Dictionary<string, object>();

        switch (objType)
        {
            case "Door":
                objParams.Add("openAmount", 0f);
                objParams.Add("isDoorOpening", false);
                objParams.Add("hasPassedOver", false);
                objParams.Add("doorOpenTime", 0.5f);
                break;
            case "Shop":
                objParams.Add("ShopScript", new Shop(Shop.Type.General, ShopItems(Shop.Type.General)) );
                break;
        }

        if(objParams.Count > 0)
        {
            return objParams;
        }

        return null;
    }

    private static Dictionary<string, Func<InstalledObject, object>> getInstalledObjectFunctions(string objType)
    {
        Dictionary<string, Func<InstalledObject, object>> funcs = new Dictionary<string, Func<InstalledObject, object>>();

        switch (objType)
        {
            case "Door":
                funcs.Add("IsEnterable", IsEnterable);
                break;
        }

        return funcs;
    }

    #region InstalledObjectFuncs
    private static object IsEnterable(InstalledObject obj)
    {
        float openAmount = (float)obj.Paramaters["openAmount"];
        if (openAmount == 1)
        {
            return true;
        }

        return false;
    }
    #endregion
}
