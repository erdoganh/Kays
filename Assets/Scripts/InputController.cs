using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

    [HideInInspector] public Vector2 directionInput;

    [SerializeField] InventoryController playerInventory;

    void Start()
    {
        playerInventory.Init();
    }

	void Update () {
        directionInput.x = Input.GetAxis("Horizontal");
        directionInput.y = Input.GetAxis("Vertical");
    }

    public Vector2 GetDirection()
    {
        Vector2 direction = Vector2.zero;
        if (directionInput.x != 0f)
            direction.x = Mathf.Sign(directionInput.x);
        if (directionInput.y != 0f)
            direction.y = Mathf.Sign(directionInput.y);
        return direction;
    }
}
