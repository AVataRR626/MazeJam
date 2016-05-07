using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class WinArea : MonoBehaviour
{
    public string playerTag = "PlayerA";
    public string winTreeTag = "BlueWinTree";
    public GameObject winTree;

	// Use this for initialization
	void Start ()
    {
        if (winTree == null)
        {
            winTree = GameObject.FindGameObjectWithTag(winTreeTag);            
        }

        if(winTree != null)
            winTree.SetActive(false);

        GetComponent<Collider>().isTrigger = true;
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
