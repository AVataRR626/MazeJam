using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SimpleControl : MonoBehaviour
{
    public float disableTimer = 30;
    Rigidbody myRigidbody;
    public KeyCode left;
    public KeyCode right;
    public KeyCode up;
    public KeyCode down;
    public float speed = 300;

	// Use this for initialization
	void Start ()
    {
        myRigidbody = GetComponent<Rigidbody>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(disableTimer <= 0)
        {
            ManageMove();
        }
        else
        {
            disableTimer -= Time.deltaTime;
        }

	}

    void ManageMove()
    {
        Vector3 delta = Vector3.zero;

        if (Input.GetKey(up))
        {
            delta += new Vector3(0, 0, 1);
        }

        if (Input.GetKey(down))
        {
            delta += new Vector3(0, 0, -1);
        }

        if (Input.GetKey(left))
        {
            delta += new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(right))
        {
            delta += new Vector3(1, 0, 0);
        }

        //Debug.Log(delta);
        myRigidbody.velocity = Vector3.zero;

        myRigidbody.MovePosition(transform.position + delta * speed * Time.deltaTime);
    }
}
