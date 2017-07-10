using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour {

    private Animator animator;
    [SerializeField] private string horizontalInputName;
    [SerializeField] private string verticalInputName;

	void Awake () {
        animator = GetComponent<Animator>();
    }

    public void SetHorizontalInput(float value)
    {
        animator.SetFloat(horizontalInputName, value);
    }

    public void SetVerticalInput(float value)
    {
        animator.SetFloat(verticalInputName, value);
    }

}
