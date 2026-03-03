using UnityEngine;

public struct BoxStructure
{
    public float CenterX;
    public float CenterY;
    public float Width;
    public float Height;

    public override string ToString()
    {
        return $"Center: ({CenterX:F2}, {CenterY:F2}), Size: ({Width:F2}, {Height:F2})";
    }
}

public class DebugAnnots : MonoBehaviour
{
    [SerializeField] private TrainingObject _obj;
    [SerializeField] private Camera _camera;
    [SerializeField] private int _raysPerEdge = 2;

    private void Start()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    private void Update()
    {
        // Визуализация точек проверки видимости
        Vector3[] corners = GetBoundingBoxPoints(_obj.GetCollider().bounds);

        foreach (Vector3 point in corners)
        {
            Physics.Raycast(_camera.transform.position, (point - _camera.transform.position).normalized, out RaycastHit hit, 100);
            Color color = Vector3.Distance(hit.point, point) >= 0.5f ? Color.red : Color.green;
            Debug.DrawLine(_camera.transform.position, point, color);
            DebugUtilities.DrawPrimitive(hit.point, 0.45f, PrimitiveType.Sphere, color, 0.05f);
        }

        bool isVisible = IsVisibleFromCamera(_obj);
        Debug.Log("Current visibility: " + isVisible);

        if (isVisible)
        {
            BoxStructure boxStruct;
            if (TryGetBoxStructure(_obj, out boxStruct))
            {
                Debug.Log("Bounding Box: " + boxStruct.ToString());
            }
        }
    }

    public bool IsVisibleFromCamera(TrainingObject obj)
    {
        BoxCollider collider = obj.GetCollider();

        if (_camera == null)
            _camera = Camera.main;

        if (_camera == null || collider == null || !GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(_camera), collider.bounds))
            return false;

        Bounds bounds = collider.bounds;
        Vector3 cameraPos = _camera.transform.position;

        Vector3[] points = GetBoundingBoxPoints(bounds);

        int count = 0;
        foreach (Vector3 point in points)
        {
            if (IsPointVisible(cameraPos, point, obj.GetCollider()))
                count++;

            if (count >= 4)
                return true;
        }

        return false;
    }

    private Vector3[] GetBoundingBoxPoints(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3[] points = new Vector3[_raysPerEdge * _raysPerEdge * 6];
        int index = 0;

        // Генерируем точки на всех гранях бокса
        for (int i = 0; i < _raysPerEdge; i++)
        {
            for (int j = 0; j < _raysPerEdge; j++)
            {
                float u = i / (_raysPerEdge - 1f);
                float v = j / (_raysPerEdge - 1f);

                // Передняя и задняя грани
                points[index++] = new Vector3(
                    center.x + Mathf.Lerp(-extents.x, extents.x, u),
                    center.y + Mathf.Lerp(-extents.y, extents.y, v),
                    center.z + extents.z
                );
                points[index++] = new Vector3(
                    center.x + Mathf.Lerp(-extents.x, extents.x, u),
                    center.y + Mathf.Lerp(-extents.y, extents.y, v),
                    center.z - extents.z
                );

                // Верхняя и нижняя грани
                points[index++] = new Vector3(
                    center.x + Mathf.Lerp(-extents.x, extents.x, u),
                    center.y + extents.y,
                    center.z + Mathf.Lerp(-extents.z, extents.z, v)
                );
                points[index++] = new Vector3(
                    center.x + Mathf.Lerp(-extents.x, extents.x, u),
                    center.y - extents.y,
                    center.z + Mathf.Lerp(-extents.z, extents.z, v)
                );

                // Левая и правая грани
                points[index++] = new Vector3(
                    center.x + extents.x,
                    center.y + Mathf.Lerp(-extents.y, extents.y, u),
                    center.z + Mathf.Lerp(-extents.z, extents.z, v)
                );
                points[index++] = new Vector3(
                    center.x - extents.x,
                    center.y + Mathf.Lerp(-extents.y, extents.y, u),
                    center.z + Mathf.Lerp(-extents.z, extents.z, v)
                );
            }
        }

        return points;
    }

    private bool IsPointVisible(Vector3 from, Vector3 to, BoxCollider collider)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;

        Vector3 viewportPoint = _camera.WorldToViewportPoint(to);
        if (viewportPoint.x < 0 || viewportPoint.x > 1 ||
            viewportPoint.y < 0 || viewportPoint.y > 1 ||
            viewportPoint.z < 0)
            return false;

        if (Physics.Raycast(from, direction, out RaycastHit hit, distance))
        {
            if (Vector3.Distance(hit.point, to) <= 0.35f)
                return true;

            // Если попал в другой объект - точка не видима
            return false;
        }

        return true;
    }

    public bool TryGetBoxStructure(TrainingObject obj, out BoxStructure boxStructure)
    {
        boxStructure = new BoxStructure();

        BoxCollider collider = obj.GetCollider();
        if (_camera == null || collider == null)
            return false;

        // Проверяем видимость коллайдера
        if (!IsVisibleFromCamera(obj))
            return false;

        // Получаем bounding box коллайдера
        Bounds bounds = collider.bounds;

        // Получаем все углы bounding box'а
        Vector3[] corners = GetBoundingBoxCorners(bounds);

        // Преобразуем углы в viewport координаты (0-1)
        Vector2[] viewportCorners = new Vector2[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3 viewportPoint = _camera.WorldToViewportPoint(corners[i]);

            // Если точка позади камеры, используем проекцию на границы экрана
            if (viewportPoint.z < 0)
            {
                viewportPoint = ProjectPointToViewport(corners[i]);
            }

            viewportCorners[i] = new Vector2(viewportPoint.x, viewportPoint.y);
        }

        // Находим ограничивающий прямоугольник в viewport координатах
        Vector2 min = viewportCorners[0];
        Vector2 max = viewportCorners[0];

        for (int i = 1; i < 8; i++)
        {
            min = Vector2.Min(min, viewportCorners[i]);
            max = Vector2.Max(max, viewportCorners[i]);
        }

        // Ограничиваем значения в диапазоне [0, 1]
        min = Vector2.Max(Vector2.zero, min);
        max = Vector2.Min(Vector2.one, max);

        // Если прямоугольник вырожденный (не видим), возвращаем false
        if (max.x - min.x <= 0.001f || max.y - min.y <= 0.001f)
            return false;

        // Заполняем структуру
        boxStructure.CenterX = (min.x + max.x) / 2f;
        boxStructure.CenterY = (min.y + max.y) / 2f;
        boxStructure.Width = max.x - min.x;
        boxStructure.Height = max.y - min.y;

        return true;
    }

    private Vector3[] GetBoundingBoxCorners(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3[] corners = new Vector3[8];

        // Все 8 углов bounding box'а
        corners[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
        corners[1] = center + new Vector3(-extents.x, -extents.y, extents.z);
        corners[2] = center + new Vector3(-extents.x, extents.y, -extents.z);
        corners[3] = center + new Vector3(-extents.x, extents.y, extents.z);
        corners[4] = center + new Vector3(extents.x, -extents.y, -extents.z);
        corners[5] = center + new Vector3(extents.x, -extents.y, extents.z);
        corners[6] = center + new Vector3(extents.x, extents.y, -extents.z);
        corners[7] = center + new Vector3(extents.x, extents.y, extents.z);

        return corners;
    }

    private Vector3 ProjectPointToViewport(Vector3 worldPoint)
    {
        // Преобразуем точку в viewport space
        Vector3 viewportPoint = _camera.WorldToViewportPoint(worldPoint);

        // Если точка позади камеры, инвертируем координаты
        if (viewportPoint.z < 0)
        {
            viewportPoint.x = 1 - viewportPoint.x;
            viewportPoint.y = 1 - viewportPoint.y;
            viewportPoint.z = -viewportPoint.z;
        }

        // Ограничиваем в диапазоне [0, 1]
        viewportPoint.x = Mathf.Clamp01(viewportPoint.x);
        viewportPoint.y = Mathf.Clamp01(viewportPoint.y);

        return viewportPoint;
    }
}