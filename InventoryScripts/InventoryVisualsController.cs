using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class InventoryVisualsController : MonoBehaviour
{
    InventoryManager invManager;

    GameObject inventoryVis;
    Text title;
    GameObject InvField;

    GameObject tradeVis;
    List<GameObject> tradeFields;

    GameObject slotPrefab;
    
    Dictionary<int, GameObject> slots;
    Dictionary<string, GameObject> tradeSlots;
    public Dictionary<InventoryItem, GameObject> InventoryItemGOMap;
    int maxInvs;

    private void OnEnable()
    {

    }

    void Start()
    {

    }

    public void SetUp(int MaxInvs, InventoryManager invManager)
    {
        this.invManager = invManager;
        slots = new Dictionary<int, GameObject>();
        InventoryItemGOMap = new Dictionary<InventoryItem, GameObject>();
        tradeFields = new List<GameObject>();
        tradeSlots = new Dictionary<string, GameObject>();

        slotPrefab = UIReferences.i.Slot;

        inventoryVis = UIReferences.i.Inventory;
        InvField = UIReferences.i.InventoryItemField;
        //title = inventoryVis.transform.Find("Title").GetComponent<Text>();

        maxInvs = MaxInvs;

        for (int i = 0; i < MaxInvs; i++) //20 being max inv slots
        {
            GameObject s = Instantiate(slotPrefab, InvField.transform);
            Slot slotData = s.GetComponent<Slot>();
            slotData.TradeField = -1;
            slotData.Number = i;
            slots.Add(i, s);
        }
        inventoryVis.SetActive(false);

        tradeVis = UIReferences.i.TradeInv;
        tradeFields.Add(UIReferences.i.TradeInvItemFieldA);
        tradeFields.Add(UIReferences.i.TradeInvItemFieldB);

        for (int i = 0; i < tradeFields.Count; i++)
        {
            for (int s = 0; s < MaxInvs; s++)
            {
                GameObject sGO = Instantiate(slotPrefab, tradeFields[i].transform);
                Slot slotData = sGO.GetComponent<Slot>();
                slotData.TradeField = i;
                slotData.Number = s;
                tradeSlots.Add(GetTradeSlotString(i,s), sGO);
            }
        }

        tradeVis.SetActive(false);
    }

    public void OnLoad()
    {
        
    }


    public void OnInventoryChanged(Inventory inv_Data) 
    {
        if(inventoryVis.activeSelf == true)
        {
            for (int i = 0; i < inv_Data.Capacity; i++)
            {
                Image img = slots[i].transform.Find("Img").GetComponent<Image>();
                GameObject counter = slots[i].transform.Find("Img").Find("Counter").gameObject;
                InventoryItem item = inv_Data.items[i];
                SetSlotImg(img, counter, item);
            }
        }
        else if(tradeVis.activeSelf == true)
        {
            int field = invManager.TradeInventories.IndexOf(inv_Data);
            for (int i = 0; i < inv_Data.Capacity; i++)
            {
                Image img = tradeSlots[GetTradeSlotString(field, i)].transform.Find("Image").GetComponent<Image>();
                GameObject counter = tradeSlots[GetTradeSlotString(field, i)].transform.Find("Image").Find("Counter").gameObject;
                InventoryItem item = inv_Data.items[i];
                SetSlotImg(img, counter, item);
            }
        }
    }

    void SetSlotImg(Image img, GameObject counter, InventoryItem item)
    {
        Color c = Color.white;
        if (item.InventorySubType != null && item.InventorySubType != "" && item.InventorySubType != "null")
        {
            img.sprite = SpriteManager.current.GetSprite(SpriteManager.SpriteCatagory.Item, item.InventorySubType);
            c.a = 255;
            img.color = c;
        }
        else
        {
            img.sprite = null;
            c.a = 0;
            img.color = c;
        }

        if (item.IsStackable == true)
        {
            counter.SetActive(true);
            counter.GetComponent<TextMeshProUGUI>().SetText(item.Quantity.ToString());
        }
        else
        {
            counter.SetActive(false);
        }
    }

    string GetTradeSlotString(int tradeField, int slot)
    {
        string s = tradeField + "-" + slot;
        return s;
    }

    public void OpenInventory(Inventory inv_Data)
    {
        inventoryVis.SetActive(true);
        tradeVis.SetActive(false);
        //title.text = inv_Data.Name;

        for (int i = 0; i < slots.Count; i++)
        {
            if(i < inv_Data.Capacity)
            {
                slots[i].SetActive(true);
            }
            else
            {
                slots[i].SetActive(false);
            }
        }
        OnInventoryChanged(inv_Data);
    }

    public void OpenTradeWindow(Inventory invA, Inventory invB)
    {
        tradeVis.SetActive(true);
        inventoryVis.SetActive(false);

        UIReferences.i.TradeTextA.text = invA.Name;
        UIReferences.i.TradeTextB.text = invB.Name;

        for (int i = 0; i < invManager.TradeInventories.Count; i++)
        {
            Inventory inv = invManager.TradeInventories[i];
            for (int s = 0; s < invManager.MaxInvs; s++) 
            {
                if(s < inv.Capacity)
                {
                    tradeSlots[GetTradeSlotString(i,s)].SetActive(true);
                }
                else
                {
                    tradeSlots[GetTradeSlotString(i,s)].SetActive(false);
                }
            }
            OnInventoryChanged(inv);
        }

    }

    public void CloseInventory(bool isTrade)
    {
        if(isTrade == true)
        {
            tradeVis.SetActive(false);
        }
        else
        {
            inventoryVis.SetActive(false);
        }
    }
    
    void OnInventoryRemoved(InventoryItem inv)
    {
        GameObject obj = InventoryItemGOMap[inv];

        InventoryItemGOMap.Remove(inv);
        Destroy(obj);

        //inv.UnregisterOnChangedCallback(OnInventoryChanged);
        inv.UnegisterOnRemovedCallback(OnInventoryRemoved);
    }
}
