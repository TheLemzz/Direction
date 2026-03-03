using System.Collections;
using UnityEngine;

public sealed class OptimizedDistanceCulling : MonoBehaviour
{
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float checkInterval = 0.3f;

    [SerializeField] private Transform renderTranform;
    private Transform cameraTransform;

    private Renderer[] _renderers;
    private Collider _collider;

    private bool status = true;

    private void Start()
    {
        _renderers = renderTranform.GetComponentsInChildren<Renderer>(true);
        _collider = GetComponentInChildren<BoxCollider>();

        cameraTransform = Camera.main.transform;
        StartCoroutine(CullCheck());
    }

    private bool IsColliderVisibility(Collider collider, Plane[] planes)
    {
        return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
    }



    private IEnumerator CullCheck()
    {
        WaitForSeconds wait = new(checkInterval);

        while (true)
        {
            status = IsColliderVisibility(_collider, GeometryUtility.CalculateFrustumPlanes(Camera.main)) || Vector3.Distance(transform.position, cameraTransform.position) >= maxDistance;

            if (_renderers[0].enabled != status)
            {
                foreach (Renderer renderer in _renderers)
                {
                    renderer.enabled = status;
                }
            }
            yield return wait;
        }
    }
}