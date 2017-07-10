using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {

    [SerializeField] private Transform target;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float followDistance = 1f;
    [SerializeField] private float stopDistance = 0.5f;
    private Vector3 lastTargetPosition;

    void Start(){
        lastTargetPosition = target.position;
    }

	void FixedUpdate () {
        float distanceToPlayer = Vector3.Distance(target.position, lastTargetPosition);
        float distanceToTrasnform = Vector3.Distance(transform.position, lastTargetPosition);

        if (distanceToPlayer > followDistance){ lastTargetPosition = target.position; }

        if (distanceToTrasnform > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, lastTargetPosition, speed * Time.deltaTime);

            Vector2 moveDirection = lastTargetPosition - transform.position;
            Vector2 velocityForAC = GetVelocityForAnimator(moveDirection);

            GetComponent<AnimationController>().SetHorizontalInput(velocityForAC.x);
            GetComponent<AnimationController>().SetVerticalInput(velocityForAC.y);
        }
        else
        {
            GetComponent<AnimationController>().SetHorizontalInput(0f);
            GetComponent<AnimationController>().SetVerticalInput(0f);
        }
    }


    private Vector2 GetVelocityForAnimator(Vector2 velocity)
    {
        if (velocity.x == 0f) velocity.x = 0.01f;
        if (velocity.y == 0f) velocity.y = 0.01f;

        Vector2 velocityForAC = new Vector2(1f, 1f);

        float div = Mathf.Abs(velocity.x / velocity.y);
        if (div < 1.5f && div > 0.5f)
        {
            if (velocity.x < 0f) velocityForAC.x = -1f;
            if (velocity.y < 0f) velocityForAC.y = -1f;
        }
        else if (div >= 1.5f)
        {
            if (velocity.x < 0f) velocityForAC.x = -1f;
            velocityForAC.y = 0f;
        }
        else if (div <= 0.5f)
        {
            velocityForAC.x = 0f;
            if (velocity.y < 0f) velocityForAC.y = -1f;
        }

        return velocityForAC;
    }

}
