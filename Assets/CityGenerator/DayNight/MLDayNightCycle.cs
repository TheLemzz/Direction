using UnityEngine;

public sealed class MLDayNightCycle : MonoBehaviour
{
    [SerializeField] private DayNight _dayNight;

    private void OnValidate()
    {
        if (_dayNight == null) _dayNight = GetComponent<DayNight>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(Switch), 120, Random.Range(120, 200));
    }

    private void Switch()
    {
        _dayNight.isNight = !_dayNight.isNight;
        _dayNight.ChangeMaterial();
    }
}
