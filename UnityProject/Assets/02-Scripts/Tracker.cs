using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Tracker : MonoBehaviour
{
    NavMeshAgent nma;
    public Transform destination;

	// Use this for initialization
	void Start ()
    {
        nma = GetComponent<NavMeshAgent>();

        nma.destination = destination.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
