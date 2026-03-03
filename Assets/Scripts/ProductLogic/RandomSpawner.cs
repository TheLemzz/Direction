using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class SpawnSettings
{
    public GameObject[] spawnableObjects;
    public Transform[] spawnPoints;
    public Material[] tableMaterials;
    public Light directionalLight;
    public Transform tableTransform;

    [Header("Настройки спавна")]
    [Range(0.5f, 60f)] public float spawnInterval = 5f;
    [Range(0f, 1f)] public float minLightIntensityChange = -0.2f;
    [Range(0f, 1f)] public float maxLightIntensityChange = 0.2f;
    [Range(0f, 5f)] public float lightChangeDuration = 1f;
    [Range(0.1f, 2f)] public float backgroundChance = 1f;
    public bool animate = false;
}

public sealed class RandomSpawner : MonoBehaviour
{
    [SerializeField] private SpawnSettings settings;

    private readonly List<GameObject> spawnedObjects = new();
    private Coroutine lightChangeCoroutine;
    private float originalLightIntensity;

    private void Start()
    {
        if (settings.directionalLight != null)
        {
            originalLightIntensity = settings.directionalLight.intensity;
        }

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(settings.spawnInterval);

            ClearSpawnedObjects();

            SpawnObjects();

            ChangeTableMaterial();

            ChangeLightIntensity();
        }
    }

    private void SpawnObjects()
    {
        if (settings.spawnPoints == null || settings.spawnPoints.Length == 0)
        {
            Debug.LogWarning("Не заданы точки спавна!");
            return;
        }

        if (settings.spawnableObjects == null || settings.spawnableObjects.Length == 0)
        {
            Debug.LogWarning("Не заданы объекты для спавна!");
            return;
        }

        if (Random.Range(0f, 100f) >= settings.backgroundChance)
            foreach (Transform spawnPoint in settings.spawnPoints)
            {
                if (spawnPoint == null || Random.Range(0, 100) <= 65) continue;

                int randomIndex = Random.Range(0, settings.spawnableObjects.Length);
                GameObject objectToSpawn = settings.spawnableObjects[randomIndex];

                if (objectToSpawn != null)
                {
                    GameObject spawnedObject = Instantiate(
                        objectToSpawn,
                        spawnPoint.position.AddY(Random.Range(2f, 4f)),
                        objectToSpawn.transform.rotation
                    );
                    spawnedObject.transform.localScale = spawnedObject.transform.localScale * Random.Range(1, 1.55f);
                    float randomYRotation = Random.Range(0f, 360f);
                    spawnedObject.transform.rotation = Quaternion.Euler(
                        spawnedObject.transform.rotation.eulerAngles.x,
                        randomYRotation,
                        spawnedObject.transform.rotation.eulerAngles.z
                    );

                    spawnedObjects.Add(spawnedObject);

                    if (settings.animate)
                    {
                        StartCoroutine(AnimateSpawn(spawnedObject));
                    }
                }
            }
    }

    private IEnumerator AnimateSpawn(GameObject obj)
    {
        Vector3 originalScale = obj.transform.localScale;
        obj.transform.localScale = Vector3.zero;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            obj.transform.localScale = Vector3.Lerp(
                Vector3.zero,
                originalScale,
                Mathf.SmoothStep(0f, 1f, t)
            );

            yield return null;
        }

        obj.transform.localScale = originalScale;
    }

    private void ChangeTableMaterial()
    {
        if (settings.tableTransform == null ||
            settings.tableMaterials == null ||
            settings.tableMaterials.Length == 0)
        {
            Debug.LogWarning("Не задан стол или материалы для стола!");
            return;
        }

        Renderer tableRenderer = settings.tableTransform.GetComponent<Renderer>();
        if (tableRenderer == null)
        {
            Debug.LogWarning("У стола нет компонента Renderer!");
            return;
        }

        int randomMaterialIndex = Random.Range(0, settings.tableMaterials.Length);
        Material randomMaterial = settings.tableMaterials[randomMaterialIndex];

        if (randomMaterial != null)
        {
            tableRenderer.material = randomMaterial;
        }
    }

    private void ChangeLightIntensity()
    {
        if (settings.directionalLight == null)
        {
            Debug.LogWarning("Не задан Directional Light!");
            return;
        }

        if (lightChangeCoroutine != null)
        {
            StopCoroutine(lightChangeCoroutine);
        }

        lightChangeCoroutine = StartCoroutine(ChangeLightIntensityRoutine());
    }

    private IEnumerator ChangeLightIntensityRoutine()
    {
        float targetIntensity = originalLightIntensity +
            Random.Range(settings.minLightIntensityChange, settings.maxLightIntensityChange);

        Color originalColor = settings.directionalLight.color;
        Color targetColor = new Color(
            Mathf.Clamp01(originalColor.r + Random.Range(-0.2f, 0.2f)),
            Mathf.Clamp01(originalColor.g + Random.Range(-0.2f, 0.2f)),
            Mathf.Clamp01(originalColor.b + Random.Range(-0.2f, 0.2f)),
            originalColor.a
        );

        float startIntensity = settings.directionalLight.intensity;
        float elapsed = 0f;

        while (elapsed < settings.lightChangeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / settings.lightChangeDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            settings.directionalLight.intensity = Mathf.Lerp(
                startIntensity,
                targetIntensity,
                smoothT
            );

            settings.directionalLight.color = Color.Lerp(
                originalColor,
                targetColor,
                smoothT
            );

            yield return null;
        }

        elapsed = 0f;
        Color currentColor = settings.directionalLight.color;
        float currentIntensity = settings.directionalLight.intensity;

        while (elapsed < settings.lightChangeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / settings.lightChangeDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            settings.directionalLight.intensity = Mathf.Lerp(
                currentIntensity,
                originalLightIntensity,
                smoothT
            );

            settings.directionalLight.color = Color.Lerp(
                currentColor,
                originalColor,
                smoothT
            );

            yield return null;
        }

        settings.directionalLight.intensity = originalLightIntensity;
        settings.directionalLight.color = originalColor;
    }

    private void ClearSpawnedObjects()
    {
        List<GameObject> objectsToDestroy = new(spawnedObjects);
        spawnedObjects.Clear();

        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj != null)
            {
                if (settings.animate)
                {
                    StartCoroutine(AnimateDestroy(obj));
                }
                else
                {
                    Destroy(obj);
                }
            }
        }
    }

    private IEnumerator AnimateDestroy(GameObject obj)
    {
        Vector3 originalScale = obj.transform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            obj.transform.localScale = Vector3.Lerp(
                originalScale,
                Vector3.zero,
                Mathf.SmoothStep(0f, 1f, t)
            );

            yield return null;
        }

        Destroy(obj);
    }

    private void OnValidate()
    {
        if (settings != null)
        {
            if (settings.minLightIntensityChange > settings.maxLightIntensityChange)
            {
                settings.minLightIntensityChange = settings.maxLightIntensityChange;
            }

            if (settings.spawnInterval < 0.1f)
            {
                settings.spawnInterval = 0.1f;
            }
        }
    }

    public void ForceSpawn()
    {
        StopAllCoroutines();
        ClearSpawnedObjects();
        SpawnObjects();
        ChangeTableMaterial();
        ChangeLightIntensity();
        StartCoroutine(SpawnRoutine());
    }

    public void SetSpawnInterval(float newInterval)
    {
        if (newInterval > 0)
        {
            settings.spawnInterval = newInterval;
        }
    }

    public void SetAnimate(bool animate)
    {
        settings.animate = animate;
    }

    private void OnDestroy()
    {
        ClearSpawnedObjects();
    }
}