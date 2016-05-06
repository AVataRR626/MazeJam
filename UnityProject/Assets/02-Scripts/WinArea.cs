using UnityEngine;
using System.Collections;

public class WinArea : MonoBehaviour
{
    public string playerTag = "PlayerA";
    public GameObject winTree;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(name + "I ENTERED!!");

        if(col.gameObject.tag == playerTag)
        {
            winTree.SetActive(true);

        }
    }
}
