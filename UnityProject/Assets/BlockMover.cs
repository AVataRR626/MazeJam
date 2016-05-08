using UnityEngine;
using System.Collections;

public class BlockMover : MonoBehaviour
{
    [SerializeField]
    float speed = 1.0f;

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z);
    }
}
