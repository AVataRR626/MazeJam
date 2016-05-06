//Flicker class! By Timo;
//slightly modified by Matt
using UnityEngine;
using System.Collections;
public class Flicker : MonoBehaviour
{   


    [SerializeField]
    float speed;

    [SerializeField]
    float amount;

    [SerializeField]
    float startIntensity;

    [SerializeField]
    float startRange;

    [SerializeField]
    float rangeFactor;

    float flicker;

    Light light;
    Light globLight;

    void Awake()
    {
        light = GetComponent<Light>();
        globLight = FindObjectOfType<MazeLightManager>().GetComponent<Light>();
    }
    void Update()
    {
        flicker = Mathf.Sin(Time.time * speed);
        light.intensity = (startIntensity-globLight.intensity) + Time.deltaTime * amount * flicker;
        light.range = startRange + flicker * rangeFactor;
    }
}