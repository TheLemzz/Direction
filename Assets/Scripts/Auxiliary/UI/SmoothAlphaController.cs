using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothAlphaController : MonoBehaviour
{
    private readonly Dictionary<Image, float> originalAlphas = new Dictionary<Image, float>();
    private readonly Dictionary<Image, float> velocities = new Dictionary<Image, float>();

    [SerializeField] private float currentMultiplier = 1f;
    [SerializeField] private float smoothTime = 0.3f;

    private void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>(true);

        foreach (Image img in images)
        {
            if (!originalAlphas.ContainsKey(img))
            {
                originalAlphas[img] = img.color.a;
                velocities[img] = 0f;
            }
        }

        ApplyMultiplierImmediate(currentMultiplier);
    }

    private void Update()
    {
        var keys = new List<Image>(originalAlphas.Keys);

        foreach (var img in keys)
        {
            if (img == null)
            {
                originalAlphas.Remove(img);
                velocities.Remove(img);
                continue;
            }

            float originalAlpha = originalAlphas[img];
            float currentVelocity = velocities[img];

            float targetAlpha = originalAlpha * currentMultiplier;
            float currentAlpha = img.color.a;

            currentAlpha = Mathf.SmoothDamp(
                currentAlpha,
                targetAlpha,
                ref currentVelocity,
                smoothTime
            );

            velocities[img] = currentVelocity;

            Color color = img.color;
            color.a = currentAlpha;
            img.color = color;
        }
    }

    public void SetMultiplier(float multiplier)
    {
        currentMultiplier = Mathf.Clamp01(multiplier);
    }

    public void ReturnToOriginal(float transitionTime = 0.3f)
    {
        smoothTime = transitionTime;
        currentMultiplier = 1f;
    }

    // ≈¡¿Õ€… FADE IN
    public void FadeIn(float duration = 0.3f)
    {
        smoothTime = duration;
        currentMultiplier = 1f;
    }

    // ≈¡¿Õ€… FADE OUT
    public void FadeOut(float duration = 0.3f)
    {
        smoothTime = duration;
        currentMultiplier = 0f;
    }

    public void SetMultiplierSmooth(float multiplier, float duration = 0.3f)
    {
        smoothTime = duration;
        currentMultiplier = Mathf.Clamp01(multiplier);
    }

    private void ApplyMultiplierImmediate(float multiplier)
    {
        currentMultiplier = Mathf.Clamp01(multiplier);

        foreach (var kvp in originalAlphas)
        {
            Image img = kvp.Key;
            if (img == null) continue;

            float targetAlpha = kvp.Value * currentMultiplier;
            Color color = img.color;
            color.a = targetAlpha;
            img.color = color;

            if (velocities.ContainsKey(img))
                velocities[img] = 0f;
        }
    }

    public void AddImage(Image newImage)
    {
        if (newImage != null && !originalAlphas.ContainsKey(newImage))
        {
            originalAlphas[newImage] = newImage.color.a;
            velocities[newImage] = 0f;
        }
    }

    public void RemoveImage(Image image)
    {
        if (originalAlphas.ContainsKey(image))
        {
            originalAlphas.Remove(image);
        }
        if (velocities.ContainsKey(image))
        {
            velocities.Remove(image);
        }
    }

    public void RefreshImages()
    {
        originalAlphas.Clear();
        velocities.Clear();

        Image[] images = GetComponentsInChildren<Image>(true);

        foreach (Image img in images)
        {
            originalAlphas[img] = img.color.a;
            velocities[img] = 0f;
        }

        ApplyMultiplierImmediate(currentMultiplier);
    }
}