using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(AnimationController))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour {

    public float speed = 2f;

    private InputController inputController;
    private AnimationController animController;
    private Rigidbody2D rigidbody;

	void Awake () {
        inputController = GetComponent<InputController>();
        animController = GetComponent<AnimationController>();
        rigidbody = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
        rigidbody.velocity = inputController.directionInput.normalized * speed;
	}

    void Update()
    {
        animController.SetHorizontalInput(inputController.GetDirection().x);
        animController.SetVerticalInput(inputController.GetDirection().y);
    }
}
