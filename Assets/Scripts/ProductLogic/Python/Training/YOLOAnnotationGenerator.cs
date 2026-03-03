using System.Collections.Generic;
using UnityEngine;

public static class YOLOAnnotationGenerator
{
    /// <summary>
    /// debug = true включает визуализацию и подробные логи.
    /// debugDuration = сколько секунд держать линии на экране.
    /// </summary>
    public static bool TryGetAnnotation(
        TrainingObject trainingObject,
        Camera cam,
        LayerMask mask,
        out YOLOAnnotation annotation,
        int classId,
        bool debug = true,
        float debugDuration = 1f)
    {
        annotation = new YOLOAnnotation();
        //List<Vector3> worldPoints = GenerateColliderPoints(trainingObject.GetCollider(), trainingObject.GetPointsPerAxis());
        List<Vector3> worldPoints = trainingObject.GetCompositeWorldPoints();

        if (worldPoints == null || worldPoints.Count == 0)
        {
            Debug.Log($"YOLO: null composite world points for {trainingObject.gameObject.name}");
            return false;
        }

        float minX = 1f, maxX = 0f;
        float minY = 1f, maxY = 0f;
        bool hasVisiblePoints = false;

        Vector3 camPos = cam.transform.position;
        int visibleCount = 0;
        int occludedCount = 0;
        int outOfFrameCount = 0;

        HashSet<string> blockingObjects = new();

        foreach (var point in worldPoints)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(point);

            bool isInFrame = viewportPoint.z > 0 &&
                             viewportPoint.x > -0.1f && viewportPoint.x < 1.1f &&
                             viewportPoint.y > -0.1f && viewportPoint.y < 1.1f;

            if (!isInFrame)
            {
                outOfFrameCount++;
                if (debug) Debug.DrawRay(point, Vector3.up * 0.2f, Color.grey, debugDuration);
                continue;
            }

            if (CheckVisibility(camPos, point, trainingObject.gameObject, mask, out RaycastHit hitInfo))
            {
                if (debug) Debug.DrawLine(camPos, point, Color.green, debugDuration);

                if (viewportPoint.x < minX) minX = viewportPoint.x;
                if (viewportPoint.x > maxX) maxX = viewportPoint.x;
                if (viewportPoint.y < minY) minY = viewportPoint.y;
                if (viewportPoint.y > maxY) maxY = viewportPoint.y;

                hasVisiblePoints = true;
                visibleCount++;
            }
            else
            {
                if (debug)
                {
                    Debug.DrawLine(camPos, hitInfo.point, Color.red, debugDuration);
                    Debug.DrawRay(hitInfo.point, Vector3.up * 0.5f, Color.red, debugDuration);
                    Debug.DrawRay(hitInfo.point, Vector3.right * 0.5f, Color.red, debugDuration);

                    occludedCount++;
                    blockingObjects.Add(hitInfo.collider.name);
                }
            }
        }

        if (debug)
        {
            string statusColor = hasVisiblePoints ? "green" : "red";
            string statusText = hasVisiblePoints ? "PARTIAL/FULL" : "OCCLUDED";

            string blockersList = blockingObjects.Count > 0
                ? string.Join(", ", blockingObjects)
                : "None";

            string logMessage = $"<color={statusColor}><b>[YOLO {statusText}]</b></color> Object: <b>{trainingObject.name}</b>\n" +
                                $"Stats: Visible: {visibleCount} | Occluded: {occludedCount} | OutOfFrame: {outOfFrameCount}\n" +
                                $"Blockers: {blockersList}";

            if (hasVisiblePoints)
                Debug.Log(logMessage);
            else
                Debug.LogWarning(logMessage);
        }

        if (!hasVisiblePoints)
            return false;

        minX = Mathf.Clamp01(minX);
        maxX = Mathf.Clamp01(maxX);
        minY = Mathf.Clamp01(minY);
        maxY = Mathf.Clamp01(maxY);

        float invertedMinY = 1f - maxY;
        float invertedMaxY = 1f - minY;
        float width = maxX - minX;
        float height = invertedMaxY - invertedMinY;
        float centerX = minX + (width / 2f);
        float centerY = invertedMinY + (height / 2f);

        if (width < 0.01f || height < 0.01f)
        {
            if (debug) Debug.LogWarning($"[YOLO SKIP] {trainingObject.name} слишком мал на экране! W:{width:F4} H:{height:F4}");
            return false;
        }

        annotation.ClassId = classId;
        annotation.CenterX = centerX;
        annotation.CenterY = centerY;
        annotation.Width = width;
        annotation.Height = height;

        if (debug)
        {
            DrawDebugBox(cam, minX, maxX, minY, maxY, debugDuration);
        }

        return true;
    }

    private static void DrawDebugBox(Camera cam, float minX, float maxX, float minY, float maxY, float duration)
    {
        float zDepth = 5.0f;

        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(minX, minY, zDepth));
        Vector3 tl = cam.ViewportToWorldPoint(new Vector3(minX, maxY, zDepth));
        Vector3 br = cam.ViewportToWorldPoint(new Vector3(maxX, minY, zDepth));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(maxX, maxY, zDepth));

        Color boxColor = Color.yellow;

        Debug.DrawLine(bl, tl, boxColor, duration);
        Debug.DrawLine(tl, tr, boxColor, duration);
        Debug.DrawLine(tr, br, boxColor, duration);
        Debug.DrawLine(br, bl, boxColor, duration);
        Debug.DrawLine(bl, tr, boxColor * 0.7f, duration);
    }

    private static List<Vector3> GenerateColliderPoints(BoxCollider col, int pointsPerAxis)
    {
        List<Vector3> points = new();
        Transform tr = col.transform;
        Vector3 center = col.center;
        Vector3 size = col.size;
        float extX = size.x / 2f; float extY = size.y / 2f; float extZ = size.z / 2f;

        for (int i = 0; i <= pointsPerAxis; i++)
        {
            float t1 = (float)i / pointsPerAxis;
            float l1 = Mathf.Lerp(-1f, 1f, t1);
            for (int j = 0; j <= pointsPerAxis; j++)
            {
                float t2 = (float)j / pointsPerAxis;
                float l2 = Mathf.Lerp(-1f, 1f, t2);
                points.Add(tr.TransformPoint(center + new Vector3(l1 * extX, l2 * extY, extZ)));
                points.Add(tr.TransformPoint(center + new Vector3(l1 * extX, l2 * extY, -extZ)));
                points.Add(tr.TransformPoint(center + new Vector3(extX, l1 * extY, l2 * extZ)));
                points.Add(tr.TransformPoint(center + new Vector3(-extX, l1 * extY, l2 * extZ)));
                points.Add(tr.TransformPoint(center + new Vector3(l1 * extX, extY, l2 * extZ)));
                points.Add(tr.TransformPoint(center + new Vector3(l1 * extX, -extY, l2 * extZ)));
            }
        }
        points.Add(tr.TransformPoint(center));
        return points;
    }

    private static bool CheckVisibility(Vector3 cameraPos, Vector3 targetPoint, GameObject targetObject, LayerMask mask, out RaycastHit hitInfo)
    {
        Vector3 direction = targetPoint - cameraPos;
        float distance = direction.magnitude;

        if (Physics.Raycast(cameraPos, direction, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.gameObject == targetObject || hitInfo.transform.IsChildOf(targetObject.transform)) return true;

            return false;
        }
        hitInfo = new RaycastHit();
        return true;
    }
}