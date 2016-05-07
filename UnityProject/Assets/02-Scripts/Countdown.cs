using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public float time;
    public Text timeDisplay;
    public string format = "0.0";
    public GameObject deactivateObject;
    public GameObject activateObject;

    // Use this for initialization
    void Start ()
    {
        if (timeDisplay == null)
            timeDisplay = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(timeDisplay != null)
        {
            timeDisplay.text = time.ToString(format);
        }

        if(time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {   
            time = 0;

            if(deactivateObject != null)
                deactivateObject.SetActive(false);

            if(activateObject != null)
                activateObject.SetActive(true);
        }

	}
}
