using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{

    int startField = -1;
    int startSlot = -1;
    int releaseField = -1;
    int releaseSlot = -1;

    public GameObject itemDrag;

    public string currBuildItem = "Wall";
    

    public void SetUp()
    {
        itemDrag = Instantiate(UIReferences.i.ItemDrag);
        itemDrag.transform.SetParent(UIReferences.i.InGameUICanavas.transform);
        itemDrag.SetActive(false);
    }
    
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) //if over UI bail
        {
            InventoryClick();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition   = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(WorldController.instance.DebugMode == false)
            {
                if(currBuildItem.Length > 0)
                {
                    WorldController.instance.CreateInstalledObject(currBuildItem, mousePosition);
                }
            }
            else
            {
                WorldController.instance.npcManager.DebugPathTest(mousePosition);

                //List<PathNode> path = WorldController.instance.world.PathFind.FindPath(WorldController.instance.playerController.PlayerPosition(), mousePosition);

                //if (path != null)
                //{
                //    for (int i = 0; i < path.Count - 1; i++)
                //    {
                //        PathNode node = path[i];
                //        Vector3 start = new Vector3(path[i].x, 0, path[i].z);
                //        Vector3 end = new Vector3(path[i + 1].x, 0, path[i + 1].z);

                //        Debug.DrawLine(start, end, Color.red, 1f);
                //    }
                //}
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            WorldController.instance.RemoveInstalledObject(mousePosition);
        }

        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Backslash))
        {
            //Debug.Log("Comboooooooooooooooooooooo");
            if (WorldController.instance.DebugMode == false)
            {
                WorldController.instance.DebugMode = true;
                
            }
            else
            {
                WorldController.instance.DebugMode = false;
            }
        }
    }

    public List<RaycastResult> RayCast()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> rayResult = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, rayResult);

        return rayResult;
    }

    void InventoryClick()
    {
        if (WorldController.instance.inventoryManager == null)
        {
            return;
        }


        #region LeftClick
        if (Input.GetMouseButtonDown(0)) //Click
        {
            List<RaycastResult> rayResult = RayCast();

            for (int i = 0; i < rayResult.Count; i++)
            {
                GameObject GO = rayResult[i].gameObject;
                //Debug.Log(rayResult[i].gameObject.name);
                if (GO.layer == 12)
                {
                    Slot slotData = GO.GetComponent<Slot>();
                    if(slotData.IsToolBar == true)
                    {
                        return;
                    }

                    startField = slotData.TradeField;
                    startSlot = slotData.Number;

                    bool hasItem;
                    string item;
                    WorldController.instance.inventoryManager.SlotHasItem(startField, startSlot, out hasItem, out item);

                    if (hasItem == true)
                    {
                        itemDrag.GetComponent<Image>().sprite = SpriteManager.current.GetSprite(SpriteManager.SpriteCatagory.Item, item);
                        itemDrag.SetActive(true);
                        itemDrag.transform.position = Input.mousePosition;
                    }
                    else
                    {
                        startSlot = -1;
                        startField = -1;
                    }
                }
            }
        }

        if (Input.GetMouseButton(0)) //Drag
        {
            itemDrag.transform.position = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) //Release
        {
            List<RaycastResult> rayResult = RayCast();
            itemDrag.SetActive(false);

            for (int i = 0; i < rayResult.Count; i++)
            {
                GameObject GO = rayResult[i].gameObject;
                //Debug.Log(rayResult[i].gameObject.name);

                if (GO.layer == 13)
                {
                    return;
                }

                if (GO.layer == 12)
                {
                    Slot slotData = GO.GetComponent<Slot>();
                    releaseField = slotData.TradeField;
                    releaseSlot = slotData.Number;

                    if (startSlot == -1 || releaseSlot == -1)
                    {
                        return;
                    }

                    if (startSlot == releaseSlot && startField == releaseField)
                    {
                        Debug.Log("SamePos as start");
                        //TODO: if we click and release on the same spot are we using a consumable?
                        //WorldController.instance.inventoryManager.UseItem(startField, startSlot);
                        return;
                    }

                    if(slotData.IsToolBar == true)
                    {
                        //Debug.Log("IsToolBar");
                        Inventory playerInv = WorldController.instance.npcManager.Player.inventory;
                        WorldController.instance.inventoryManager.AddToToolBar(playerInv, startSlot, releaseSlot);
                    }
                    else
                    {
                        WorldController.instance.inventoryManager.SwapSlots(startField, startSlot, releaseField, releaseSlot);
                    }

                }
            }


            startField = -1;
            startSlot = -1;
            releaseField = -1;
            releaseSlot = -1;
        }
        #endregion

        #region RightClick
        //    if (Input.GetMouseButtonUp(1))
        //    {
        //        List<RaycastResult> rayResult = RayCast();

        //        for (int i = 0; i < rayResult.Count; i++)
        //        {
        //            GameObject GO = rayResult[i].gameObject;
        //            //Debug.Log(rayResult[i].gameObject.name);
        //            if (GO.layer == 12)
        //            {
        //                startField = GO.GetComponent<Slot>().TradeField;
        //                startSlot = GO.GetComponent<Slot>().Number;

        //                bool hasItem;
        //                string itemSubType;
        //                WorldController.instance.inventoryManager.SlotHasItem(startField, startSlot, out hasItem, out itemSubType);



        //                if (hasItem == true)
        //                {
        //                    int slotID;
        //                    Inventory itemInv = WorldController.instance.inventoryManager.GetItemLocation(startField, startSlot, out slotID);

        //                    WorldController.instance.popUpManager.SetUp(itemInv, slotID);
        //                }
        //                else
        //                {
        //                    startSlot = -1;
        //                    startField = -1;
        //                }
        //            }
        //        }
        //    }
        #endregion

    }
}
