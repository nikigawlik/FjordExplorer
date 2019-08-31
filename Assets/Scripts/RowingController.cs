using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RowingController : MonoBehaviour
{
    [SerializeField]
    private float force;

    [SerializeField]
    private float forceOffsetToCenter;

    [SerializeField]
    private float maxRowTime;

    [SerializeField]
    private float dragCoefficient;

    private Rigidbody boat;
    private float timeLeftPressed = 0;
    private float timeRightPressed = 0;

    // Start is called before the first frame update
    void Start()
    {
        boat = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            timeLeftPressed += Time.deltaTime;
        }
        else
        {
            timeLeftPressed = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            timeRightPressed += Time.deltaTime;
        }
        else
        {
            timeRightPressed = 0;
        }
    }

    void FixedUpdate()
    {
        float leftForce = force * Mathf.Sin(timeLeftPressed / maxRowTime * Mathf.PI);
        float rightForce = force * Mathf.Sin(timeRightPressed / maxRowTime * Mathf.PI);

        boat.AddForceAtPosition(this.gameObject.transform.forward * leftForce,
            this.gameObject.transform.TransformPoint(-forceOffsetToCenter, 0, 0));
        boat.AddForceAtPosition(this.gameObject.transform.forward * rightForce,
            this.gameObject.transform.TransformPoint(forceOffsetToCenter, 0, 0));
        boat.AddForce(DragOf(boat.velocity));
    }

    private Vector3 DragOf(Vector3 velocity)
    {
        float scalarDrag = dragCoefficient * velocity.magnitude * velocity.magnitude;
        return velocity.normalized * -scalarDrag;
    }
}
