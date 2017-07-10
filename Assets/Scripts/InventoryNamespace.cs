using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace InventoryNamespace
{

    [System.Serializable]
    public class Item
    {
        public int itemID;
        public string itemName;
        public string itemDescription;
        public Sprite itemIcon;
        public ItemType itemType;
        public Vector2 slotProportions;
        public int slotSize;
        public int slotMaxSize;

        //Map Item Values
        public int minAmountInMap;
        public int maxAmountInMap;
        public float percentageInMap;

        public Item() {
            itemName = "";
            itemDescription = "";
            itemIcon = null;
            itemType = ItemType.None;
            slotProportions = Vector2.zero;
            slotSize = 0;
            slotMaxSize = 0;

            minAmountInMap = 0;
            maxAmountInMap = 0;
            percentageInMap = 0f;
        }

        public Item(Item item)
        {
            itemName = item.itemName;
            itemDescription = item.itemDescription;
            itemIcon = item.itemIcon;
            itemType = item.itemType;
            slotProportions = item.slotProportions;
            slotSize = 0;
            slotMaxSize = item.slotMaxSize;
        }

        public Item Copy()
        {
            Item item = new Item();
            item.itemName = itemName;
            item.itemDescription = itemDescription;
            item.itemIcon = itemIcon;
            item.itemType = itemType;
            item.slotProportions = slotProportions;
            item.slotSize = slotSize;
            item.slotMaxSize = slotMaxSize;
            return item;
        }

    }

    [System.Serializable]
    public class Blueprint
    {
        public CraftItem resultItem;
        public List<CraftItem> neededItems;

        public Blueprint()
        {
            resultItem = new CraftItem();
            neededItems = new List<CraftItem>();
        }

        public Blueprint( CraftItem _resultItem, List<CraftItem> _neededItems)
        {
            resultItem = _resultItem;
            neededItems = _neededItems;
        }

        public void Clear()
        {
            resultItem = new CraftItem();
            neededItems = new List<CraftItem>();
        }

    }

    [System.Serializable]
    public class CraftItem
    {
        public Item item;
        public int count;

        public CraftItem() {
            item = new Item();
            count = 0;
        }

        public CraftItem(Item _item, int _count)
        {
            item = _item;
            count = _count;
        }

        public CraftItem(CraftItem craftItem)
        {
            item = craftItem.item;
            count = craftItem.count;
        }
    }

    public enum ItemType
    {
        None,
        Weapon,
        Consumable,
        Quest,
        Head,
        Shoes,
        Chest,
        Trousers,
        Earrings,
        Necklace,
        Rings,
        Hands
    };
}