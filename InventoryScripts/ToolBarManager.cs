using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolBarManager : MonoBehaviour
{
    InventoryItem[] items;

    GameObject[] slotGOs;
    GameObject[] invSlotGOs;

    public void SetUp(int toolBarSize)
    {
        invSlotGOs = new GameObject[toolBarSize];
        slotGOs = new GameObject[toolBarSize];
        items = new InventoryItem[toolBarSize];

        for (int i = 0; i < toolBarSize; i++)
        {
            items[i] = new InventoryItem();

            GameObject slotGO = Instantiate(UIReferences.i.Slot, UIReferences.i.ToolBarItemField.transform);
            Slot slotData = slotGO.GetComponent<Slot>();
            slotData.Number = i;
            slotData.IsToolBar = true;
            slotGOs[i] = slotGO;

            GameObject invSlotGo = Instantiate(UIReferences.i.Slot, UIReferences.i.ToolBarInvField.transform);
            Slot invSlotData = invSlotGo.GetComponent<Slot>();
            invSlotData.Number = i;
            invSlotData.IsToolBar = true;
            invSlotGOs[i] = invSlotGo;
        }
        UpdateSlots();
    }

    public void AddItemToBar(InventoryItem item, int slot)
    {
        int previousSlot = 0;
        bool contains = false;
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] == item)
            {
                Debug.Log("Contains");
                contains = true;
                previousSlot = i;
            }
        }

        if(contains == true)
        {
            InventoryItem swapItem = items[slot];

            items[slot] = item;

            items[previousSlot] = swapItem;
        }
        else
        {
            items[slot] = item;
        }

        UpdateSlots();
    }

    public void UseItem(int slot)
    {
        items[slot].UseAction();
    }

    public void UpdateSlots()
    {
        for (int i = 0; i < slotGOs.Length; i++)
        {
            InventoryItem item = items[i];

            Image img = slotGOs[i].transform.Find("Img").GetComponent<Image>();
            GameObject counter = slotGOs[i].transform.Find("Img").Find("Counter").gameObject;
            SetSlotImg(img, counter, item);

            Image invImg = invSlotGOs[i].transform.Find("Img").GetComponent<Image>();
            GameObject invCounter = invSlotGOs[i].transform.Find("Img").Find("Counter").gameObject;
            SetSlotImg(invImg, invCounter, item);
        }
    }

    void SetSlotImg(Image img, GameObject counter, InventoryItem item)
    {
        Color color = Color.white;
        if (item.InventorySubType != null && item.InventorySubType != "" && item.InventorySubType != "null")
        {
            img.sprite = SpriteManager.current.GetSprite(SpriteManager.SpriteCatagory.Item, item.InventorySubType);
            color.a = 255;
            img.color = color;
        }
        else
        {
            img.sprite = null;
            color.a = 0;
            img.color = color;
        }

        if (item.IsStackable == true)
        {
            //Debug.Log("ItemStackable?");
            counter.SetActive(true);
            counter.GetComponent<TextMeshProUGUI>().SetText(item.Quantity.ToString());
        }
        else
        {
            counter.SetActive(false);
        }
    }
}
