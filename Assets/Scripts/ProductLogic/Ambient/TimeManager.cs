using System;
using System.Collections;
using UnityEngine;

public sealed class TimeManager : MonoBehaviour
{
    [SerializeField] private Light _sun;
    [SerializeField] private Light _moon;

    [SerializeField, Tooltip("График зависимости.")] private AnimationCurve _sunAnimationCurve;
    [SerializeField] private AnimationCurve _moonAnimationCurve;

    [SerializeField] private Material _daySkyBox;
    [SerializeField] private Material _nightSkyBox;

    [SerializeField, Range(1, 30), Tooltip("Полный день в минутах. Ночь = день = 1/2 суток.")] private float _dayDuration;
    [SerializeField, Range(0f, 1f), Tooltip("Время на старте игры.")] private float time;

    public event Action<bool> OnNightStarted;

    private float _sunIntensity;
    private float _moonIntensity;

    private bool _night;
    private bool _started = false;

    public void StartCycle()
    {
        if (_started || _sun == null || _moon == null) return;

        _started = true;
        StartCoroutine(DayCycleCoroutine());
        Debug.Log("Start TimeManager.");
    }

    private void Start()
    {
        _sunIntensity = _sun.intensity;
        _moonIntensity = _moon.intensity;
        _sun.transform.localRotation = Quaternion.Euler(time * 360f, 180, 0);
        StartCycle();
    }

    private void Update()
    {
        if (!_started) return;

        time += Time.deltaTime / (_dayDuration * 60);
        if (time >= 1) time -= 1;

        RenderSettings.skybox.Lerp(_nightSkyBox, _daySkyBox, _sunAnimationCurve.Evaluate(time));

        _sun.transform.localRotation = Quaternion.Euler(time * 360f, 180, 0);
        _sun.intensity = _sunIntensity * _sunAnimationCurve.Evaluate(time);
        _moon.transform.localRotation = Quaternion.Euler((time * 360f) + 180f, 180, 0);
        _moon.intensity = _moonIntensity * _moonAnimationCurve.Evaluate(time);
    }

    private IEnumerator DayCycleCoroutine()
    {
        WaitForSeconds wait = new(1.5f);
        while (true)
        {
            yield return wait;
            DynamicGI.UpdateEnvironment();

            if (_night != (_sunAnimationCurve.Evaluate(time) < 0.2f))
            {
                _night = _sunAnimationCurve.Evaluate(time) < 0.2f;
                OnNightStarted?.Invoke(_night);
            }
        }
    }

    public void OnDestroy()
    {
        DynamicGI.UpdateEnvironment();
    }
}
