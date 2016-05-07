using UnityEngine;
using System.Collections;

public class AllPlayerFreeze : MonoBehaviour
{
    public float freezeTime = 10;
    public float delay = 0;
    public bool activateOnStart = true;

	// Use this for initialization
	void Start ()
    {
        if(activateOnStart)
        {

            Invoke("FreezeTimeTrigger", delay);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void FreezeTimeTrigger()
    {
        FreezePlayers(freezeTime);
    }

    public static void FreezePlayers(float ft)
    {
        SimpleControl[] players = FindObjectsOfType<SimpleControl>();
        foreach (SimpleControl c in players)
        {
            c.disableTimer = ft;
        }
    }
}
