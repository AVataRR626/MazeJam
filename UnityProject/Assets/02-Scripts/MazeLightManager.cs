using UnityEngine;
using System.Collections;


public class MazeLightManager : MonoBehaviour
{
    public float introTime;
    public float darknessTime;
    public float flashIntensity = 4;
    public float flashRate = 1f;
    public float introDarkenRate = 0.2f;

    public int mode;//0 intro time, 1: darkening mode, 2:darkness, 3: flash mode,

    private Light myLight;
    private float clock = 0;

	// Use this for initialization
	void Start ()
    {
        clock = introTime;
        myLight = GetComponent<Light>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //introMode
	    if(mode == 0)
        {
            if (clock > 0)
            {
                clock -= Time.deltaTime;
            }
            else
            {
                mode = 1;
            }

        }

        //darkening mode
        if(mode == 1)
        {
            if (myLight.intensity > 0)
            {
                myLight.intensity -= introDarkenRate * Time.deltaTime;
            }
            else
            {
                mode = 2;
                clock = darknessTime;
            }
        }

        //darkness mode
        if(mode == 2)
        {
            if(clock > 0)
            {
                clock -= Time.deltaTime;
            }
            else
            {   
                mode = 3;
                myLight.intensity = flashIntensity;
            }
        }

        if(mode == 3)
        {
            if(myLight.intensity > 0)
            {
                myLight.intensity -= flashRate * Time.deltaTime;
            }
            else
            {
                mode = 2;
                clock = darknessTime;
            }
        }
	}
}
