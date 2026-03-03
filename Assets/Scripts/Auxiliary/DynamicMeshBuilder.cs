using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.AI.Navigation;
using UnityEngine;
using Debug = UnityEngine.Debug;

public sealed class DynamicMeshBuilder : MonoBehaviorSingleton<DynamicMeshBuilder>
{
    private readonly Queue<NavMeshSurface> _queue = new();

    private void Awake()
    {
        SetInstance(this);
    }

    private void Start()
    {
        StartCoroutine(BuildingCoroutine());
    }

    private IEnumerator BuildingCoroutine()
    {
        WaitForEndOfFrame wait = new();

        yield return new WaitForSeconds(1.5f);

        Debug.Log($"DynamicMeshBuilder: Got {_queue.Count} meshes.");

        Stopwatch stopwatch = Stopwatch.StartNew();

        while (_queue.Count > 0)
        {
            NavMeshSurface surface = _queue.Dequeue();
            surface.BuildNavMesh();
            yield return wait;
        }

        stopwatch.Stop();
        Debug.Log($"DynamicMeshBuilder: built for {stopwatch.ElapsedMilliseconds}ms");
    }

    public void Enqueue(NavMeshSurface surface)
    {
        _queue.Enqueue(surface);
    }
}
