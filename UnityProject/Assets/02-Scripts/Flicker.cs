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

    [SerializeField]
    bool isDimmedByGlobalLight = false;

    float flicker;

    Light light;
    Light globLight;

    float offset;

    void Awake()
    {
        light = GetComponent<Light>();
        globLight = FindObjectOfType<MazeLightManager>().GetComponent<Light>();
        offset = Random.Range(0,10);
    }
    void Update()
    {
        flicker = Mathf.Sin(offset + Time.time * speed);

        light.intensity = startIntensity + Time.deltaTime * amount * flicker;

        if (isDimmedByGlobalLight)
        {
            light.intensity -= globLight.intensity;
        } else
        {
            light.intensity += globLight.intensity;
        }

        light.range = startRange + flicker * rangeFactor;
    }
}