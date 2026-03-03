using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class TrainingObject : MonoBehaviour
{
    [Header("Training Settings")]
    [SerializeField] private bool useForTraining = true;
    [SerializeField] private TrainingData trainingData;

    [Header("Visualization")]
    [SerializeField] private bool showBoundingBox = true;

    [SerializeField] private BoxCollider boxCollider;

    [Tooltip("Если объект не имеет анимации (машина, столб) - ставь галочку. Скрипт закэширует точки 1 раз.\nЕсли объект анимирован (человек) - убери галочку. Скрипт будет сканировать позу каждый кадр.")]
    [SerializeField] private bool isStatic = true;
    [SerializeField] private bool useAnimationDebug = true;

    [SerializeField] private bool _useCullingSettings = true;

    private readonly List<StaticMeshData> _staticParts = new();
    private readonly List<DynamicSkinnedData> _dynamicParts = new();

    private Mesh _tempBakeMesh;
    private bool _isInitialized = false;

    private const int TARGET_TOTAL_POINTS = 250;

    private void OnDrawGizmosSelected()
    {
        if (!_isInitialized || !useAnimationDebug) return;

        Gizmos.color = Color.yellow;

        List<Vector3> points = GetCompositeWorldPoints();

        foreach (Vector3 p in points)
        {
            Gizmos.DrawSphere(p, 0.03f);
        }
    }

    private void OnValidate()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null && boxCollider.isTrigger)
        {
            Debug.LogWarning($"BoxCollider for training shoud be a trigger! {gameObject.name}");
        }
    }

    private void Awake()
    {
        InitializeCompositeMesh();
    }

    private void OnDestroy()
    {
        if (_tempBakeMesh != null) Destroy(_tempBakeMesh);
    }

    public void InitializeCompositeMesh()
    {
        if (_isInitialized) return;

        _staticParts.Clear();
        _dynamicParts.Clear();

        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        SkinnedMeshRenderer[] skinneds = GetComponentsInChildren<SkinnedMeshRenderer>();

        int totalVerts = 0;
        foreach (MeshFilter f in filters)
            if (f.sharedMesh != null) totalVerts += f.sharedMesh.vertexCount;

        foreach (SkinnedMeshRenderer s in skinneds)
            if (s.sharedMesh != null) totalVerts += s.sharedMesh.vertexCount;

        if (totalVerts == 0) return;

        int globalStep = Mathf.Max(1, totalVerts / TARGET_TOTAL_POINTS);

        int filteredCount = 0;

        foreach (MeshFilter filter in filters)
        {
            Mesh m = filter.sharedMesh;
            if (m == null) continue;

            if (_useCullingSettings && m.bounds.size.sqrMagnitude < 0.5f)
            {
                filteredCount++;
                continue;
            }

            Vector3[] sampled = SampleVertices(m, globalStep);

            _staticParts.Add(new StaticMeshData
            {
                Transform = filter.transform,
                SampledVertices = sampled
            });
        }

        foreach (SkinnedMeshRenderer skinned in skinneds)
        {
            Mesh m = skinned.sharedMesh;
            if (m == null) continue;

            if (_useCullingSettings && m.bounds.size.sqrMagnitude < 0.5f)
            {
                filteredCount++;
                continue;
            }

            if (isStatic)
            {
                Mesh baked = new();
                skinned.BakeMesh(baked);

                Vector3[] sampled = SampleVertices(baked, globalStep);

                _staticParts.Add(new StaticMeshData
                {
                    Transform = skinned.transform,
                    SampledVertices = sampled
                });

                DestroyImmediate(baked);
            }
            else
            {
                _dynamicParts.Add(new DynamicSkinnedData
                {
                    Renderer = skinned,
                    SamplingStep = globalStep
                });
            }
        }

        if (_dynamicParts.Count > 0)
        {
            _tempBakeMesh = new Mesh();
            _tempBakeMesh.MarkDynamic();
        }

        _isInitialized = true;
        //Debug.Log($"[MeshInit] {gameObject.name}: Static Parts: {_staticParts.Count}, Dynamic Parts: {_dynamicParts.Count}. Filtered small: {filteredCount}");
    }

    /// <summary>
    /// Возвращает актуальное облако точек.
    /// Для статичных объектов - из кэша.
    /// Для анимированных - запекает SkinnedMesh "на лету".
    /// </summary>
    public List<Vector3> GetCompositeWorldPoints()
    {
        if (!_isInitialized) InitializeCompositeMesh();

        List<Vector3> worldPoints = new(TARGET_TOTAL_POINTS + 50);

        foreach (StaticMeshData part in _staticParts)
        {
            if (part.Transform == null || !part.Transform.gameObject.activeInHierarchy) continue;

            Transform t = part.Transform;
            foreach (var localV in part.SampledVertices)
            {
                worldPoints.Add(t.TransformPoint(localV));
            }
        }

        foreach (DynamicSkinnedData dyn in _dynamicParts)
        {
            if (dyn.Renderer == null || !dyn.Renderer.gameObject.activeInHierarchy) continue;

            dyn.Renderer.BakeMesh(_tempBakeMesh);

            Vector3[] bakedVerts = _tempBakeMesh.vertices;
            Transform t = dyn.Renderer.transform;
            int step = dyn.SamplingStep;

            for (int i = 0; i < bakedVerts.Length; i += step)
            {
                worldPoints.Add(t.TransformPoint(bakedVerts[i]));
            }

            if (worldPoints.Count == 0 && bakedVerts.Length > 0)
            {
                worldPoints.Add(t.TransformPoint(bakedVerts[0]));
            }

            _tempBakeMesh.Clear();
        }

        return worldPoints;
    }

    private Vector3[] SampleVertices(Mesh m, int step)
    {
        if (!m.isReadable)
        {
            if (!m.isReadable) return new Vector3[0];
        }

        Vector3[] allVerts = m.vertices;
        if (allVerts.Length == 0) return new Vector3[0];

        List<Vector3> sampled = new((allVerts.Length / step) + 2);

        for (int i = 0; i < allVerts.Length; i += step)
        {
            sampled.Add(allVerts[i]);
        }

        if (sampled.Count < 4 && allVerts.Length > 0)
        {
            sampled.Add(allVerts[0]);
            sampled.Add(allVerts[^1]);
        }

        return sampled.ToArray();
    }

    public string GetClassification()
    {
        return trainingData.Classification;
    }

    public float GetVisibilityRange()
    {
        return trainingData.Range;
    }

    public TrainingData GetTrainingData()
    {
        return trainingData;
    }

    public BoxCollider GetCollider()
    {
        return boxCollider;
    }

    public bool IsUsingForTraining()
    {
        return useForTraining && gameObject.activeInHierarchy;
    }

    private void OnDrawGizmos()
    {
        if (showBoundingBox && boxCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}