using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CrackSpawner : MonoBehaviour
{
    public static CrackSpawner Instance { get; private set; }

    private readonly Queue<CrackSpawnData> _queue = new();

    private static void SetInstance(CrackSpawner crackSpawner)
    {
        if (Instance == null) Instance = crackSpawner;
    }

    private void Awake()
    {
        SetInstance(this);
    }

    public void EnqueueCrack(CrackSpawnData crack)
    {
        _queue.Enqueue(crack);
        Debug.Log("Enqueue " + crack.Crack.name);
    }

    private void Start()
    {
        StartCoroutine(GenerateCoroutine());
    }

    private IEnumerator GenerateCoroutine()
    {
        WaitForEndOfFrame wait = new();

        yield return new WaitForSeconds(0.2f);

        while (_queue.Count >= 1)
        {
            CrackSpawnData data = _queue.Dequeue();
            Instantiate(data.Crack, data.WantedPosition.AddY(0.008f), data.Crack.transform.rotation);
            yield return wait;
        }
    }
}
