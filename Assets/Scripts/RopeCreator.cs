using UnityEngine;
using System.Collections;

public class RopeCreator : MonoBehaviour {

    [SerializeField] private GameObject ropeEnd;
    [SerializeField] private int partNumber;
    [SerializeField] private GameObject partPrefab;

    private Vector3 localPos;

	void Start () {
        CreateRope();
        localPos = transform.localPosition;
    }
	
	private void CreateRope()
    {
        if (ropeEnd.GetComponent<Rigidbody2D>() == null) ropeEnd.AddComponent<Rigidbody2D>();
        if (ropeEnd.GetComponent<HingeJoint2D>() == null) ropeEnd.AddComponent<HingeJoint2D>();

        GameObject ropeContainer = GameObject.FindGameObjectWithTag("RopeContainer");

        GameObject lastObject = gameObject;
        for (int i = 0; i < partNumber; i++)
        {
            GameObject part = Instantiate(partPrefab) as GameObject;
            part.transform.SetParent(ropeContainer.transform);

            part.GetComponent<HingeJoint2D>().connectedBody = lastObject.GetComponent<Rigidbody2D>();
            part.GetComponent<HingeJoint2D>().anchor = new Vector2(0f, 0.5f);
            part.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(-0f, -0.3f); //new Vector2(-0.125f, -1.3f);
            lastObject = part;
        }
        ropeEnd.GetComponent<HingeJoint2D>().connectedBody = lastObject.GetComponent<Rigidbody2D>();
        ropeEnd.GetComponent<HingeJoint2D>().anchor = new Vector2(0f, 0f);
        ropeEnd.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(-0f, -0f);
    }

    void FixedUpdate()
    {
        transform.localPosition = localPos;
    }

}
