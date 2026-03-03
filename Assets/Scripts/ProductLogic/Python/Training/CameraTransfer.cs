using UnityEngine;

public sealed class CameraTransfer : MonoBehaviour
{
    [SerializeField] private Transform[] _positions;

    private void Update()
    {
        if (Random.Range(0, 1000) >= 5) return;

        Transform element = _positions.PickRandomElement();

        transform.position = element.position;
        transform.rotation = element.rotation;
    }
}
