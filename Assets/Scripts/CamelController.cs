using UnityEngine;
using System.Collections;
using InventoryNamespace;

public class CamelController : MonoBehaviour {

    public InventoryController inventory;
    [SerializeField] private Vector3 inventoryShowOffset;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = GetComponentInChildren<InventoryController>();
        inventory.Init();

        inventory.gameObject.SetActive(false);

        //Vector3 newInventoryPos = Camera.main.WorldToScreenPoint(transform.position) + inventoryShowOffset;
        //inventory.GetComponent<InventoryController>().ChangeInventoryPosition(newInventoryPos);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && inventory.gameObject.activeSelf)
        {
            inventory.AddItem(inventory.inventoryDatabase.itemList[0].Copy());

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 0.5f)
            {
                inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
                Vector3 newInventoryPos = Camera.main.WorldToScreenPoint(transform.position) + inventoryShowOffset;
                inventory.GetComponent<InventoryController>().ChangeInventoryPosition(newInventoryPos);
            }
        }

        if (Vector3.Distance(transform.position, player.transform.position) > 0.5f) {
            //inventory.gameObject.SetActive(false);
        }     

    }

}

/*
                Item item1 = new Item(inventory.GetComponent<InventoryController>().inventoryDatabase.itemList[0]);

                inventory.GetComponent<InventoryController>().AddItem(item1);
                inventory.GetComponent<InventoryController>().AddItem(item1);
*/