using System;
using UnityEngine;

[RequireComponent(typeof(PyModule))]
public sealed class EntryPoint : MonoBehaviour
{
    private PyModule _module;

    public static event Action OnApplicationStarted;

    private void Start()
    {
        Application.targetFrameRate = 90;
        _module = GetComponent<PyModule>();

        _module.Init();
        OnApplicationStarted?.Invoke();
    }
}
