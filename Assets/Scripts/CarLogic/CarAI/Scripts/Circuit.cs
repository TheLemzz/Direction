using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    [SerializeField] private Color _color = Color.red;

    public List<Transform> waypoints = new List<Transform>();

    private void OnDrawGizmos()
    {
        if(waypoints.Count > 1)
        {
            Vector3 prev = waypoints[0].position;
            for (int i = 1; i < waypoints.Count; i++)
            {
                Vector3 next = waypoints[i].position;
                Debug.DrawLine(prev, next, _color);
                prev = next;
            }
            Debug.DrawLine(prev, waypoints[0].position, Color.green);
        }
    }

    private void OnValidate()
    {
        foreach (Transform childTransform in GetComponentsInChildren<Transform>())
        {
            if (!waypoints.Contains(childTransform) && gameObject.transform != childTransform) waypoints.Add(childTransform);
        }
    }
}
