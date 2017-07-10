using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InventoryNamespace;

public class ButtonActions : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler, IDropHandler {

    [HideInInspector] public InventoryController belongedIinventory;
    [HideInInspector] public List<GameObject> slotsBeingOn = null;
    [HideInInspector] public Item buttonItem;
    [HideInInspector] public bool isItemPlaced = true;

    private Transform startParent;
    private Vector3 startPosition;
    private Vector3 relativePosition;

    public void SetItemButton(InventoryController inventory, Item item)
    {
        belongedIinventory = inventory;
        buttonItem = item;
        float buttonSizeX = inventory.slotSize.x * buttonItem.slotProportions.x;
        float buttonSizeY = inventory.slotSize.y * buttonItem.slotProportions.y;
        GetComponent<RectTransform>().sizeDelta = new Vector2(buttonSizeX, buttonSizeY);
        transform.SetAsLastSibling();
        buttonItem.slotSize++;
        SetButtonUI();
    }

    public void SetButtonUI()
    {
        FindComponentInChildWithName<Text>("Name").text = buttonItem.itemName;
        FindComponentInChildWithName<Image>("Icon").sprite = buttonItem.itemIcon;
        FindComponentInChildWithName<Text>("Slot Size").text = buttonItem.slotSize + "/" + buttonItem.slotMaxSize;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        belongedIinventory.itemBeingDragged = gameObject;
        startPosition = transform.position;
        relativePosition = Input.mousePosition - transform.position;
        transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform);

        isItemPlaced = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition - relativePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (isItemPlaced == false)
        {
            transform.position = startPosition;
            transform.SetParent(belongedIinventory.scrollviewContent);
            transform.SetAsLastSibling();
            isItemPlaced = true;
        }
        belongedIinventory.isInventoryChanged = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Item draggedItem = eventData.pointerDrag.gameObject.GetComponent<ButtonActions>().buttonItem;
        if (draggedItem.itemName == buttonItem.itemName)
        {
            if ((draggedItem.slotSize + buttonItem.slotSize) <= buttonItem.slotMaxSize)
            {
                buttonItem.slotSize = buttonItem.slotSize + draggedItem.slotSize;
                eventData.pointerDrag.gameObject.GetComponent<ButtonActions>().ClearOldSlots();
                Destroy(eventData.pointerDrag.gameObject);
            }
            else
            {
                int decreaseAmount = buttonItem.slotMaxSize - buttonItem.slotSize;
                buttonItem.slotSize = buttonItem.slotSize + decreaseAmount;
                draggedItem.slotSize = draggedItem.slotSize - decreaseAmount;
                eventData.pointerDrag.gameObject.GetComponent<ButtonActions>().SetButtonUI();
            }
            SetButtonUI();
        }
    }

    public void ClearOldSlots()
    {
        if (slotsBeingOn != null)
        {
            for (int i=0; i<slotsBeingOn.Count; i++)
            {
                slotsBeingOn[i].GetComponent<SlothController>().SetIsFull(false);
                slotsBeingOn[i].GetComponent<SlothController>().slothItem = null;
            }
        }
    }

    public T FindComponentInChildWithName<T>(string childName) where T : Component
    {
        foreach (Transform _transform in transform)
        {
            if (_transform.name == childName)
            {
                return _transform.GetComponent<T>();
            }
        }
        return null;
    }

}