using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class InventoryItemActions
{
    public static Action<float, InventoryItem> GetAction(string name) //When adding new ItemAction dont forget to add it to the switch
    {
        Action<float, InventoryItem> action = null;

        switch (name)//Gets actions from here
        {
            case "Hoe_OnUse":
                action = Hoe_OnUse;
                break;
            case "Seeds_OnUse":
                action = Seeds_OnUse;
                break;
            case "WateringCan_OnUse":
                action = WateringCan_OnUse;
                break;
        }

        if (action != null)
        {
            return action;
        }
        else
        {
            Debug.LogError("GetAction: Has no action for - " + name);
            return null;
        }
    }

    public static Tile GetTileInFrontOfPlayer()
    {
        PlayerController player = WorldController.instance.playerController;
        Vector3 direction = player.MovementInput;

        Tile t = WorldController.instance.GetTileAtWorldCoord(player.PlayerPosition() + (direction * 0.75f));

        return t;
    }

    public static void Hoe_OnUse(float deltaTime, InventoryItem item)
    {
        Tile t = GetTileInFrontOfPlayer();

        if (t == null)
        {
            Debug.Log("Tile Null");
        }

        if(t.Type == Tile.TileType.GRASS)
        {
            t.Type = Tile.TileType.TILLED;
        }
    }

    public static void Seeds_OnUse(float deltaTime, InventoryItem item)
    {
        Tile t = GetTileInFrontOfPlayer();

        if (t == null)
        {
            Debug.Log("Tile Null");
        }

        if(WorldController.instance.CreatePlant(t, item.InventorySubType.Replace("Seeds","")) == true)
        {
            //Debug.Log("Trying to update invs");
            item.QuantityChange(-1);
            WorldController.instance.UpdatePlayerInventory();
        }
    }

    public static void WateringCan_OnUse(float deltaTime, InventoryItem item)
    {
        if(item.Quantity > 1)
        {
            Tile t = GetTileInFrontOfPlayer();
            WorldController.instance.tileManager.SetWateredTile(t, true);
            item.QuantityChange(-1);
            WorldController.instance.UpdatePlayerInventory();
        }
        else
        {
            Debug.Log("WateringCan Empty");
        }

    }
}
