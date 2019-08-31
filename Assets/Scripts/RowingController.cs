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
    private int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        boat = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            if(direction != -1) {
                timeLeftPressed = timeRightPressed = 0;
            }
            direction = -1;
        } else {
            if(direction != 1) {
                timeLeftPressed = timeRightPressed = 0;
            }
            direction = 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            timeLeftPressed = Mathf.Min(timeLeftPressed + Time.deltaTime, maxRowTime);
        }
        else
        {
            timeLeftPressed = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            timeRightPressed = Mathf.Min(timeRightPressed + Time.deltaTime, maxRowTime);
        }
        else
        {
            timeRightPressed = 0;
        }
    }

    void FixedUpdate()
    {
        float leftForce = direction * force * Mathf.Sin(timeLeftPressed / maxRowTime * Mathf.PI);
        float rightForce = direction * force * Mathf.Sin(timeRightPressed / maxRowTime * Mathf.PI);
        // Debug.Log(leftForce + " " + rightForce);

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
