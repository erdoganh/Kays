using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InventoryNamespace;

public class CraftController : MonoBehaviour {

    [SerializeField] private InventoryController mainInventory;
    [SerializeField] private GameObject resultField;
    [SerializeField] private GameObject itemButton;

    private InventoryController craftInventory;
    private InventoryDatabase inventoryDatabase;

    private List<CraftItem> availableItems = new List<CraftItem>();
    private List<CraftItem> craftableItems = new List<CraftItem>();

    private int showenItemIndex = 0;

    void Start()
    {
        craftInventory = GetComponent<InventoryController>();
        craftInventory.Init();
        //craftInventory.AddItem(inventoryDatabase.itemList[0]);
        inventoryDatabase = craftInventory.inventoryDatabase;
        resultField.SetActive(false);
    }

    void Update()
    {
        if (craftInventory.isInventoryChanged)
        {
            craftInventory.isInventoryChanged = false;
            availableItems = GetAvailableItems();
            craftableItems = GetCraftableItems(availableItems);

            if (craftableItems.Count != 0)
            {
                resultField.SetActive(true);
                showenItemIndex = 0;
                ShowResultItem(showenItemIndex);
            }
            else
            {
                resultField.SetActive(false);
            }
        }
    }

    public void OnLeftArrowPressed()
    {
        showenItemIndex--;
        showenItemIndex = showenItemIndex < 0 ? showenItemIndex + craftableItems.Count : showenItemIndex;
        showenItemIndex = showenItemIndex % craftableItems.Count;
        ShowResultItem(showenItemIndex);
    }

    public void OnRightArrowPressed()
    {
        showenItemIndex++;
        showenItemIndex = showenItemIndex % craftableItems.Count;
        ShowResultItem(showenItemIndex);
    }

    public void OnCraftPressed()
    {
        CraftItem();
    }

    private void ShowResultItem(int index)
    {
        itemButton.GetComponent<ButtonActions>().FindComponentInChildWithName<Text>("Name").text = craftableItems[index].item.itemName;
        itemButton.GetComponent<ButtonActions>().FindComponentInChildWithName<Image>("Icon").sprite = craftableItems[index].item.itemIcon;
        itemButton.GetComponent<ButtonActions>().FindComponentInChildWithName<Text>("Slot Size").gameObject.SetActive(false);
    }

    private void CraftItem()
    {
        List<CraftItem> neededItems = GetNeededItems(craftableItems[showenItemIndex]);
        for (int i=0; i<neededItems.Count; i++)
        {
            craftInventory.RemoveItem(neededItems[i].item, neededItems[i].count);
        }
        mainInventory.AddItem(new Item(craftableItems[showenItemIndex].item));
        craftInventory.isInventoryChanged = true;
    }

    private List<CraftItem> GetNeededItems(CraftItem cItem)
    {
        List<CraftItem> neededItems = new List<CraftItem>();
        for (int i = 0; i < inventoryDatabase.craftBlueprints.Count; i++)
        {
            if (cItem.item.itemName == inventoryDatabase.craftBlueprints[i].resultItem.item.itemName &&
                cItem.count == inventoryDatabase.craftBlueprints[i].resultItem.count)
            {
                neededItems = inventoryDatabase.craftBlueprints[i].neededItems;
                return neededItems;
            }
        }
        return neededItems;
    }

    private List<CraftItem> GetCraftableItems(List<CraftItem> availableItems)
    {
        List<CraftItem> craftableItems = new List<CraftItem>();

        for (int i=0; i<inventoryDatabase.craftBlueprints.Count; i++)
        {
            if (IsCraftable(i, availableItems))
            {
                CraftItem cItem = new CraftItem( inventoryDatabase.craftBlueprints[i].resultItem);
                craftableItems.Add(cItem);
            }
        }
        return craftableItems;
    }

    private bool IsCraftable(int craftItemIndex, List<CraftItem> availableItems)
    {
        List<CraftItem> neededItems = inventoryDatabase.craftBlueprints[craftItemIndex].neededItems;

        int count = 0;
        for (int i=0; i<neededItems.Count; i++)
        {
            for (int j = 0; j<availableItems.Count; j++)
            {
                if (neededItems[i].item.itemName == availableItems[j].item.itemName)
                    if(neededItems[i].count <= availableItems[j].count) {
                        count++;
                        break;
                    }
            }
        }

        if (neededItems.Count == count)
            return true;
        else
            return false;
    }

    private List<CraftItem> GetAvailableItems()
    {
        List<CraftItem> availableItems = new List<CraftItem>();

        List<Item> inventoryItems = craftInventory.GetItemsInInventory();
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            bool isInList = false;
            for (int j=0; j< availableItems.Count; j++) {
                if (availableItems[j].item.itemName == inventoryItems[i].itemName)
                {
                    isInList = true;
                    availableItems[j].count += inventoryItems[i].slotSize;
                }
            }
            if (!isInList)
            {
                availableItems.Add(new CraftItem(inventoryItems[i], inventoryItems[i].slotSize));
            }
        }

        return availableItems;
    }

}
