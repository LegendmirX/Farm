using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    InventoryVisualsController invVisuals;
    ToolBarManager toolBarManager;

    public List<Inventory> Inventories;
    public Inventory ActiveInventory;
    public List<Inventory> TradeInventories;
    public Dictionary<string, InventoryItem> ItemPrototypes;

    public bool isInvOpen = false;
    bool isTradeInv = false;

    public int MaxInvs { get { return 30; } set { Debug.LogError("InventoryManager- MaxInvs: Cannot be set"); } }
    public bool IsToolBar { get { return true; } set { Debug.LogError("InventoryManager- IsToolBar: Cannot be set"); } }
    public int ToolBarSize { get { return 10; } set { Debug.LogError("InventoryManager- ToolBarSize: Cannot be set"); } }

    Action<Inventory> cbOnInventoryChanged;

    public void SetUp()
    {
        invVisuals = FindObjectOfType<InventoryVisualsController>();
        RegisterOnInventoryChanged(invVisuals.OnInventoryChanged);
        TradeInventories = new List<Inventory>();
        ItemPrototypes = new Dictionary<string, InventoryItem>();

        if(Inventories == null)
        {
            Inventories = new List<Inventory>();
        }

        invVisuals.SetUp(MaxInvs, this);

        if(IsToolBar == true)
        {
            toolBarManager = FindObjectOfType<ToolBarManager>();

            toolBarManager.SetUp(ToolBarSize);
        }
    }

    public Inventory CreateInventory(int capacity, string name = "NoName")
    {
        if (Inventories == null)
        {
            Inventories = new List<Inventory>();
        }

        Inventory inv = new Inventory(capacity, name);
        Inventories.Add(inv);
        return inv;
    }

    public void InventoryChanged(Inventory inv_Data)
    {
        cbOnInventoryChanged(inv_Data);
    }

    public void ToolBarOnChanged()
    {
        toolBarManager.UpdateSlots();
    }

    public void OpenInventory(Inventory inv_data)
    {
        ActiveInventory = inv_data;
        isTradeInv = false;
        invVisuals.OpenInventory(inv_data);
        isInvOpen = true;
    }

    public void OpenTradeWindow(Inventory invA, Inventory InvB) //Inventory A is normally the player
    {
        TradeInventories.Add(invA);
        TradeInventories.Add(InvB);
        isTradeInv = true;
        invVisuals.OpenTradeWindow(invA, InvB);
        isInvOpen = true;
    }

    public void CloseInventory()
    {
        invVisuals.CloseInventory(isTradeInv);
        isTradeInv = false;
        TradeInventories = new List<Inventory>();
        ActiveInventory = null;
        isInvOpen = false;
    }

    #region InventoryManagement
    public InventoryItem AddItem(Inventory inv, InventoryItem item, int slotID)
    {        
        InventoryItem slot = inv.items[slotID];

        if(slot.Name == item.Name)
        {
            InventoryItem itemA = StackItems(inv, item, slotID);

            if(itemA == null)
            {
                return null;
            }
            else
            {
                return itemA;
            }
        }
        else if(slot.Name == null || slot.Name == "")
        {
            inv.items.Remove(slotID);
            inv.items.Add(slotID, item);
            cbOnInventoryChanged(inv);
            return null;
        }
        else
        {
            Debug.LogError("Given slot was not empty and did not match item given?");
            return null;
        }
    }

    bool InvHasStackableSpace(Inventory inv, InventoryItem item, out int slotID)
    {
        for (int i = 0; i < inv.Capacity; i++)
        {
            InventoryItem slot = inv.items[i];

            if (slot.InventorySubType == item.InventorySubType && slot.Quantity < slot.MaxQuantity)
            {
                slotID = i;
                return true;
            }
        }
        slotID = -1;
        return false;
    }

    public bool DoseInvHaveSpace(Inventory inv, InventoryItem item, out int slotID)
    {
        if(item.IsStackable == true)
        {
            int ID;
            if( InvHasStackableSpace(inv, item, out ID) == true)
            {
                slotID = ID;
                return true;
            }
        }

        for (int i = 0; i < inv.Capacity; i++)
        {
            InventoryItem slot = inv.items[i]; //check each slot for a null item
            
            if(slot.Name == null || slot.Name == "")
            {
                slotID = i;
                return true;
            }
        }
        slotID = -1;
        return false;
    }

    public InventoryItem StackItems(Inventory inv, InventoryItem itemA, int slotID)
    {
        InventoryItem itemB = inv.items[slotID];

        int overflow = itemB.QuantityChange(itemA.Quantity);

        if(overflow <= 0)
        {

            return null;
        }
        else
        {
            itemA.SetQuantity(overflow);
            return itemA;
        }
    }

    public void SwapSlots(int fieldA, int slotA, int fieldB, int slotB)
    {
        if(fieldA == fieldB)
        {
            Inventory inv;
            if(fieldA == -1)
            {
                inv = ActiveInventory;
            }
            else
            {
                inv = TradeInventories[fieldA];
            }
            ArrangingInv(inv, slotA, slotB);
        }
        else
        {
            Inventory invA = TradeInventories[fieldA];
            Inventory invB = TradeInventories[fieldB];

            TradeItems(invA, invB, slotA, slotB);
        }
    }

    void ArrangingInv(Inventory inv, int slotA, int slotB)
    {
        InventoryItem itemA = inv.items[slotA]; //get items
        InventoryItem itemB = inv.items[slotB];

        if(itemA.InventorySubType == itemB.InventorySubType)
        {
            InventoryItem stackItemA = StackItems(inv, itemA, slotB);
            if(stackItemA == null)
            {
                inv.items.Remove(slotA);
                inv.items.Add(slotA, new InventoryItem());
            }
            cbOnInventoryChanged(inv);
            return;
        }

        inv.items.Remove(slotA); //remove items
        inv.items.Remove(slotB);

        itemA.Slot = slotB; //swap slot references(Do i need slot references?)
        itemB.Slot = slotA;

        inv.items.Add(slotA, itemB); //add to new slots
        inv.items.Add(slotB, itemA);

        cbOnInventoryChanged(inv); //update images
    }

    void TradeItems(Inventory invA, Inventory invB, int slotA, int slotB)
    {
        InventoryItem itemA = invA.items[slotA]; //get items
        InventoryItem itemB = invB.items[slotB];

        if (itemA.InventorySubType == itemB.InventorySubType)
        {
            InventoryItem stackItemA = StackItems(invB, itemA, slotB);
            if (stackItemA == null)
            {
                invA.items.Remove(slotA);
                invA.items.Add(slotA, new InventoryItem());
            }
            cbOnInventoryChanged(invA);
            cbOnInventoryChanged(invB);
            return;
        }

        invA.items.Remove(slotA); //remove items
        invB.items.Remove(slotB);

        itemA.Slot = slotB; //swap slot references(Do i need slot references?)
        itemB.Slot = slotA;

        invA.items.Add(slotA, itemB); //add to new slots
        invB.items.Add(slotB, itemA);

        cbOnInventoryChanged(invA); //update images
        cbOnInventoryChanged(invB);
    }

    public void SlotHasItem(int field, int itemSlotNum, out bool hasItem, out string itemSubType)
    {
        if(isTradeInv == false)
        {
            InventoryItem i = ActiveInventory.items[itemSlotNum];

            if(i.ItemID != -1)
            {
                hasItem = true;
                itemSubType = i.InventorySubType;
                return;
            }
        }
        else
        {
            InventoryItem i = TradeInventories[field].items[itemSlotNum];

            if (i.ItemID != -1)
            {
                hasItem = true;
                itemSubType = i.InventorySubType;
                return;
            }
        }
        hasItem = false;
        itemSubType = null;
    }

    public Inventory GetItemLocation(int field, int itemSlotNum, out int slotID)
    {
        if (isTradeInv == false)
        {
            InventoryItem i = ActiveInventory.items[itemSlotNum];

            if (i.ItemID != -1)
            {
                slotID = itemSlotNum;
                return ActiveInventory;
            }
        }
        else
        {
            InventoryItem i = TradeInventories[field].items[itemSlotNum];

            if (i.ItemID != -1)
            {
                slotID = itemSlotNum;
                return TradeInventories[field];
            }
        }
        slotID = -1;
        return null;
    }

    public bool DoseInvHave(Inventory inv, string itemSubType, out int amount, int quantityToLookFor = 0)
    { //send a -1 to get how many of that item the inv has
        int amountFound = 0;

        for (int i = 0; i < inv.items.Count; i++)
        {
            InventoryItem item = inv.items[i];

            if(item.InventorySubType == itemSubType)
            {
                if(quantityToLookFor > 0)
                {
                    amountFound += item.Quantity;

                    if(amountFound >= quantityToLookFor)
                    {
                        amount = amountFound;
                        return true;
                    }
                }
                else if(quantityToLookFor < 0)
                {
                    amountFound += item.Quantity;
                }
                else
                {
                    amount = amountFound;
                    return true;
                }
            }
        }

        if(amountFound > 0)
        {
            amount = amountFound;
            return true;
        }

        amount = amountFound;
        return false;
    }

    public int QuantityInSlot(Inventory inv, int slotID)
    {
        int quantity = inv.items[slotID].Quantity;

        return quantity;
    }

    public InventoryItem GetReferenceToItem(Inventory inv, int slotID)
    {
        return inv.items[slotID];
    }

    public void RemoveSpecificItem(Inventory inv, int slotID, int quantity)
    {
        int removeAmount = 0;

        if(quantity > 0)
        {
            removeAmount = -quantity;
            inv.items[slotID].QuantityChange(removeAmount); //reduce the quantity
        }
        else
        {
            InventoryItem item = inv.items[slotID];
            
            inv.items.Remove(slotID);
            inv.items.Add(slotID, new InventoryItem());

            item.Remove();
        }
        cbOnInventoryChanged(inv);

    }

    public void RemoveItems(Inventory inv, string itemType, int quantity)
    {
        int removeAmount = -quantity;

        for (int i = 0; i < inv.items.Count; i++)
        {
            InventoryItem item = inv.items[i];

            if( item.InventorySubType == itemType)
            {
                removeAmount = item.QuantityChange(removeAmount);
            }

            if(item.Quantity == 0)
            {
                inv.items.Remove(i);
                inv.items.Add(i, new InventoryItem());
            }

            if(removeAmount == 0)
            {
                return;
            }
        }
    }
    #endregion

    public void AddToToolBar(Inventory playerInv, int slot, int toolBarSlot)
    {
        InventoryItem item = playerInv.items[slot];

        toolBarManager.AddItemToBar(item, toolBarSlot);
    }

    public void UseItem(int field, int slot)
    {
        Inventory inv;
        if (field == -1)
        {
            inv = ActiveInventory;
        }
        else
        {
            inv = TradeInventories[field];
        }

        if (inv.items[slot].hasUseAction() == true)
        {
            inv.items[slot].UseAction();
            if (inv.items[slot].Quantity <= 0)
            {
                inv.items[slot] = new InventoryItem();
                cbOnInventoryChanged(inv);
            }
        }
    }

    public void UseToolBarItem(int slot)
    {
        toolBarManager.UseItem(slot);
    }

    public void RemoveInventory(Inventory inv)
    {
        Inventories.Remove(inv);
    }

    #region Call Backs + Call Back Funcs

    public void RegisterOnInventoryChanged(Action<Inventory> callBackFunc)
    {
        cbOnInventoryChanged += callBackFunc;
    }
    public void UnregisterOnInventoryChanged(Action<Inventory> callBackFunc)
    {
        cbOnInventoryChanged -= callBackFunc;
    }
    #endregion
}