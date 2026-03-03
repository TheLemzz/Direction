using UnityEngine;

public sealed class DestroySelf : MonoBehaviour
{
    public float time = 1f;

    public void Start()
    {
        Destroy(gameObject, time);
    }
}