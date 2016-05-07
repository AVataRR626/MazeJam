using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CubeRotater : MonoBehaviour {

	void Awake () {
        for (int k = transform.childCount - 1; k >= 0; k--)
        {
            float rotation = 90;
            rotation *= (int)Random.Range(0, 4);
            Debug.Log(rotation);
            transform.GetChild(k).Rotate(new Vector3(0, rotation, 0));
        }
    }
}
