using CityGen;
using System.Collections;
using UnityEngine;

public class PoliceSiren : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Transform[] redLights;
    [SerializeField] private Transform[] blueLights;

    [Header("Siren Settings")]
    [SerializeField] private float patternInterval = 0.5f;
    [SerializeField] private float burstInterval = 0.1f;

    private TrafficCar _car;

    private bool siren;

    private void Start()
    {
        InvokeRepeating(nameof(Switch), Random.Range(10f, 60f), Random.Range(60f, 90f));
        StartCoroutine(SirenCycle());

        _car = GetComponent<TrafficCar>();
    }

    private void Switch()
    {
        siren = !siren;
        _car.carSetting.limitSpeed += siren ? 5 : -5;
    }

    private IEnumerator SirenCycle()
    {
        WaitForSeconds patternWait = new(patternInterval);

        while (true)
        {
            if (siren == false)
            {
                foreach (Transform item in redLights)
                {
                    item.gameObject.SetActive(false);
                }
                foreach (Transform item in blueLights)
                {
                    item.gameObject.SetActive(false);
                }
                yield return patternWait;
                continue;
            }

            SetLights(redLights, true);
            SetLights(blueLights, false);
            yield return patternWait;

            yield return StartCoroutine(BurstFlash(blueLights, 3));

            SetLights(blueLights, true);
            SetLights(redLights, false);
            yield return patternWait;

            yield return StartCoroutine(BurstFlash(redLights, 3));
        }
    }

    private IEnumerator BurstFlash(Transform[] lights, int flashes)
    {
        for (int i = 0; i < flashes; i++)
        {
            SetLights(lights, true);
            yield return new WaitForSeconds(burstInterval);
            SetLights(lights, false);
            yield return new WaitForSeconds(burstInterval);
        }
    }

    private void SetLights(Transform[] lights, bool enabled)
    {
        foreach (Transform light in lights)
        {
            light.gameObject.SetActive(enabled);
        }
    }
}