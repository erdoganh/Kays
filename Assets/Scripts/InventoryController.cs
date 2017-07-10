using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using InventoryNamespace;

public class InventoryController : MonoBehaviour {

    //silinecek
    // [SerializeField] private Dropdown itemDropdown;

    public bool isPlayer = false;
    
    public InventoryDatabase inventoryDatabase;

    [HideInInspector] public GameObject itemBeingDragged = null;
    [HideInInspector] public Transform scrollviewContent;

    public Vector2 inventorySize;
    public int seenHeight;
    public Vector2 slotSize;

    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject itemButtonPrefab;

    private GameObject[,] inventorySlotMatrix;

    public bool isInventoryChanged = false;

    void Awake()
    {
        /*
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i=0; i< inventoryDatabase.itemList.Count; i++)
        {
            options.Add(new Dropdown.OptionData(inventoryDatabase.itemList[i].itemName));
        }
        if(itemDropdown)
            itemDropdown.options = options;
        */
        //Init();
    }

    void Update()
    {
        if (isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                AddItem(inventoryDatabase.itemList[4].Copy());
                AddItem(inventoryDatabase.itemList[4].Copy());
                AddItem(inventoryDatabase.itemList[4].Copy());
                AddItem(inventoryDatabase.itemList[4].Copy());

                AddItem(inventoryDatabase.itemList[5].Copy());
                AddItem(inventoryDatabase.itemList[5].Copy());
                AddItem(inventoryDatabase.itemList[5].Copy());
                AddItem(inventoryDatabase.itemList[5].Copy());
            }
        }
    }

    public void Init()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(inventorySize.y * slotSize.x, seenHeight * slotSize.y);

        scrollviewContent.GetComponent<GridLayoutGroup>().cellSize = slotSize;
        scrollviewContent.GetComponent<GridLayoutGroup>().constraintCount = (int)inventorySize.y;

        itemButtonPrefab.SetActive(false);

        inventorySlotMatrix = new GameObject[(int)inventorySize.x, (int)inventorySize.y];
        for (int i = 0; i < inventorySize.x; i++)
        {
            for (int j = 0; j < inventorySize.y; j++)
            {
                inventorySlotMatrix[i, j] = Instantiate(slotPrefab) as GameObject;
                inventorySlotMatrix[i, j].transform.SetParent(scrollviewContent);
                inventorySlotMatrix[i, j].GetComponent<RectTransform>().sizeDelta = slotSize;
                inventorySlotMatrix[i, j].GetComponent<SlothController>().SetSlot(this);
            }
        }
        slotPrefab.SetActive(false);
    }

    public void ChangeInventoryPosition(Vector3 newPosition)
    {
        Vector3 positionOffset = transform.position - newPosition;
        transform.position = newPosition;
        ButtonActions[] allButtons = GetComponentsInChildren<ButtonActions>();
        for (int i=0; i<allButtons.Length; i++)
        {
            //allButtons[i].transform.position += positionOffset;
        }
    }
    
    /*
    public void OnAddPressed()
    {
        int itemIndex = itemDropdown.value;
        Item item = new Item(inventoryDatabase.itemList[itemIndex]);
        AddItem(item);
    }
    */

    public void AddItem(Item item)
    {
        isInventoryChanged = true;
        bool createNew = ShouldCreateNew(item.itemName);
        if (createNew)
        {
            CreateItemButton(item);
        }
        else
        {
            List<ButtonActions> buttons = GetButtons(item.itemName);
            ButtonActions suitableButton = GetSuitableButton(item.itemName, buttons);
            suitableButton.buttonItem.slotSize++;
            suitableButton.SetButtonUI();
        }
    }

    public void RemoveItem(Item item, int amount)
    {
        List<ButtonActions> buttons = GetButtons(item.itemName);
        for (int i=0; i< buttons.Count; i++)
        {
            if (buttons[i].buttonItem.slotSize <= amount)
            {
                amount = amount - buttons[i].buttonItem.slotSize;
                buttons[i].ClearOldSlots();
                Destroy(buttons[i].gameObject);
            }
            else
            {
                buttons[i].buttonItem.slotSize = buttons[i].buttonItem.slotSize - amount;
                buttons[i].SetButtonUI();
            }

            if (amount <= 0) break;
        }
    }

    public void CreateItemButton(Item item)
    {
        GameObject itemButton = Instantiate(itemButtonPrefab) as GameObject;
        itemButton.SetActive(true);
        itemButton.transform.SetParent(scrollviewContent);
        itemButton.GetComponent<ButtonActions>().SetItemButton(this, item);

        bool isItemPlaced = false;

        for (int i=0; i<inventorySize.x; i++)
        {
            for (int j=0; j<inventorySize.y; j++)
            {
                if (IsSlotAvailable(itemButton, new Vector2(0f, 0f), inventorySlotMatrix[i, j]))
                {
                    PlaceItemToSlot(itemButton, new Vector2(0f, 0f), inventorySlotMatrix[i, j]);
                    itemButton.GetComponent<ButtonActions>().ClearOldSlots();
                    SetNewSlots(itemButton, new Vector2(0f, 0f), inventorySlotMatrix[i, j]);
                    isItemPlaced = true;
                }
                if (isItemPlaced) break;
            }
            if (isItemPlaced) break;
        }

    }

    public bool ShouldCreateNew(string itemName)
    {
        List<ButtonActions> buttons = GetButtons(itemName);
        if (buttons == null) { return true; }
        else
        {
            for (int i=0; i<buttons.Count; i++)
            {
                if(buttons[i].buttonItem.slotSize < buttons[i].buttonItem.slotMaxSize) { return false; }
            }
        }
        return true;
    }

    public bool IsSlotAvailable(GameObject item, Vector2 clickedButtonPos, GameObject slot)
    {
        Vector2 proportions = item.GetComponent<ButtonActions>().buttonItem.slotProportions;
        List<GameObject> slots = GetSlotsForItem(clickedButtonPos, proportions, slot);
        if (slots == null) return false;
        for (int i=0; i<slots.Count; i++)
        {
            if (slots[i].GetComponent<SlothController>().slothItem != item)
            {
                if (slots[i].GetComponent<SlothController>().GetIsFull())
                    return false;
            }
        }
        return true;
    }

    public void PlaceItemToSlot(GameObject item, Vector2 clickedButtonPos, GameObject slot)
    {
        Vector2 buttonProportions = item.GetComponent<ButtonActions>().buttonItem.slotProportions;
        Vector2 droppedSlotPos = GetDropppedSlotPos(slot);

        float x = (droppedSlotPos.x - clickedButtonPos.y);
        float y = (droppedSlotPos.y - clickedButtonPos.x);

        float addedAmountX = slotSize.x * ((buttonProportions.x / 2f - 0.5f));
        float addedAmountY = -slotSize.y * ((buttonProportions.y / 2f - 0.5f));

        RectTransform transformOfSlot = inventorySlotMatrix[(int)x, (int)y].GetComponent<RectTransform>();
        item.transform.position = transformOfSlot.position + new Vector3(addedAmountX, addedAmountY, 0f);
    }

    public void SetNewSlots(GameObject item, Vector2 clickedButtonPos, GameObject slot)
    {
        Vector2 proportions = item.GetComponent<ButtonActions>().buttonItem.slotProportions;
        List<GameObject> slots = GetSlotsForItem(clickedButtonPos, proportions, slot);
        item.GetComponent<ButtonActions>().slotsBeingOn = slots;
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<SlothController>().SetIsFull(true);
            slots[i].GetComponent<SlothController>().slothItem = item;
        }
    }

    public Vector2 GetClickedButtonPos(GameObject item)
    {
        Vector2 buttonProportions = item.GetComponent<ButtonActions>().buttonItem.slotProportions;

        Vector3 startPosOfButton = item.transform.position + new Vector3(-slotSize.x * buttonProportions.x, slotSize.y * buttonProportions.y, 0f) / 2f;
        Vector3 distance = Input.mousePosition - startPosOfButton;
        distance.y = distance.y * -1f;

        int indexX = (int)(distance.x / slotSize.x);
        int indexY = (int)(distance.y / slotSize.y);

        return new Vector2(indexX, indexY);
    }

    public List<Item> GetItemsInInventory()
    {
        List<Item> itemsInInventory = new List<Item>();

        for (int i = 0; i < inventorySize.x; i++)
        {
            for (int j = 0; j < inventorySize.y; j++)
            {
                GameObject item = inventorySlotMatrix[i, j].GetComponent<SlothController>().slothItem;
                if (item)
                {
                    bool isInList = false;
                    for (int k=0; k<itemsInInventory.Count; k++)
                    {
                        if (itemsInInventory[k] == item.GetComponent<ButtonActions>().buttonItem)
                            isInList = true;
                    }
                    if (!isInList)
                        itemsInInventory.Add(item.GetComponent<ButtonActions>().buttonItem);
                }

            }
        }
        return itemsInInventory;
    }

    private Vector2 GetDropppedSlotPos(GameObject slot)
    {
        for (int i = 0; i < inventorySize.x; i++)
        {
            for (int j = 0; j < inventorySize.y; j++)
            {
                if (inventorySlotMatrix[i, j] == slot)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return Vector2.zero;
    }

    private List<GameObject> GetSlotsForItem(Vector2 clickedButtonPos, Vector2 buttonProportions, GameObject slot)
    {
        List<GameObject> slots = new List<GameObject>();
        Vector2 droppedSlotPos = GetDropppedSlotPos(slot);

        float x = (droppedSlotPos.x - clickedButtonPos.y);
        float y = (droppedSlotPos.y - clickedButtonPos.x);

        for (int i=0; i<buttonProportions.y; i++)
        {
            for (int j = 0; j < buttonProportions.x; j++)
            {
                if ((x + i) >= inventorySize.x || (x + i) < 0f || (y + j) >= inventorySize.y || (y + j) < 0f)
                {
                    return null;
                }
                slots.Add(inventorySlotMatrix[(int)(x + i),(int)(y + j)]);
            }
        }
        return slots;
    }

    private List<ButtonActions> GetButtons(string itemName)
    {
        List<ButtonActions> returnedButtons = new List<ButtonActions>();
        ButtonActions[] buttons = GetComponentsInChildren<ButtonActions>();
        for (int i=0; i<buttons.Length; i++)
        {
            if (buttons[i].buttonItem.itemName == itemName)
            {
                returnedButtons.Add(buttons[i]);
            }
        }
        return returnedButtons;
    }

    private ButtonActions GetSuitableButton(string itemName, List<ButtonActions> buttons)
    {
        ButtonActions returnedButton = buttons[0];
        for (int i=1; i<buttons.Count; i++)
        {
            if (returnedButton.buttonItem.slotSize > buttons[i].buttonItem.slotSize)
            {
                returnedButton = buttons[i];
            }
        }
        return returnedButton;
    }

}
