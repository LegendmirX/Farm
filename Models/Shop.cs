using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop
{
    public enum Type
    {
        General
    }
    public Type ShopType;
    public bool isShopOpen = false;
    List<string> ShopItemsList;

    public Shop(Type shopType, List<string> shopItems)
    {
        ShopType = shopType;
        ShopItemsList = shopItems;
    }

    
}
