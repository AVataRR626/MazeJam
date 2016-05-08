using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SlideShowController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.anyKey)
        {
            string name = PlayerPrefs.GetString("LoadedLevel");
            SceneManager.LoadScene(name);

        }
	}
}
