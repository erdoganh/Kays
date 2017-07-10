using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SlothController : MonoBehaviour, IDropHandler {

    [HideInInspector] public GameObject slothItem;

    private InventoryController belongedIinventory;
    private bool isFull = false;

    public void OnDrop(PointerEventData eventData)
    {
        belongedIinventory.itemBeingDragged = eventData.pointerDrag.gameObject;

        GameObject item = belongedIinventory.itemBeingDragged;
        Vector2 clickedButtonPos = belongedIinventory.GetClickedButtonPos(item);
        if (belongedIinventory.IsSlotAvailable(item, clickedButtonPos, gameObject))
        {
            item.GetComponent<ButtonActions>().belongedIinventory.isInventoryChanged = true;
            item.GetComponent<ButtonActions>().belongedIinventory = belongedIinventory;
            item.transform.SetParent(belongedIinventory.scrollviewContent);
            item.GetComponent<ButtonActions>().isItemPlaced = true;

            belongedIinventory.PlaceItemToSlot(item, clickedButtonPos, gameObject);
            item.GetComponent<ButtonActions>().ClearOldSlots();
            belongedIinventory.SetNewSlots(item, clickedButtonPos, gameObject);

            belongedIinventory.itemBeingDragged = null;
            belongedIinventory.isInventoryChanged = true;
        }
    }

    public void SetSlot(InventoryController inventory)
    {
        belongedIinventory = inventory;
    }

    public void SetIsFull(bool value)
    {
        isFull = value;
    }

    public bool GetIsFull()
    {
        return isFull;
    }
}
