using UnityEngine;
using System.Collections;

public class Persistent : MonoBehaviour
{
    public bool singleton = true;

    public static Persistent instance;

	// Use this for initialization
	void Start ()
    {
        if(singleton)
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        
        }

        DontDestroyOnLoad(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
