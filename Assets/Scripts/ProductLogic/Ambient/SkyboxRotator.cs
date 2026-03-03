using UnityEngine;

public sealed class SkyboxRotator : MonoBehaviour
{
    [Header("Settings:")]
    [Space(20)]
    [Tooltip("Rotate speed")]
    public float rotationSpeed = 1.0f;

    [Tooltip("Auto rotate enabled?")]
    public bool autoRotate = true;

    [Tooltip("Current angle")]
    [SerializeField] private float currentRotation = 0f;

    private void Update()
    {
        if (autoRotate)
        {
            RotateSkybox(rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Вращает skybox на указанный угол
    /// </summary>
    /// <param name="angle">Угол вращения в градусах</param>
    public void RotateSkybox(float angle)
    {
        currentRotation += angle;
        currentRotation %= 360f; // Сохраняем значение в диапазоне 0-360

        RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
    }

    /// <summary>
    /// Устанавливает конкретный угол вращения
    /// </summary>
    /// <param name="angle">Целевой угол в градусах</param>
    public void SetSkyboxRotation(float angle)
    {
        currentRotation = angle % 360f;
        RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
    }

    /// <summary>
    /// Включает/выключает автоматическое вращение
    /// </summary>
    public void ToggleAutoRotation()
    {
        autoRotate = !autoRotate;
    }

    /// <summary>
    /// Устанавливает скорость вращения
    /// </summary>
    /// <param name="speed">Новая скорость вращения</param>
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    // Сохраняем вращение при выходе из play mode
    private void OnApplicationQuit()
    {
        // Сбрасываем вращение при выходе
        RenderSettings.skybox.SetFloat("_Rotation", 0f);
    }

    // Для сброса вращения при отключении скрипта
    private void OnDisable()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0f);
    }
}