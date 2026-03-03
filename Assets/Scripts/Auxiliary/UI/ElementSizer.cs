using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ElementSizer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Настройки масштабирования")]
    [SerializeField] private float scaleAmount = 0.1f;
    [SerializeField] private float scaleSpeed = 8f;

    [Space(20), SerializeField] private AudioClip _hoveringClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool _onlyOnAwake;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private RectTransform rectTransform;
    private bool isHovering = false;

    private float timer;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        targetScale = originalScale;

        if (_onlyOnAwake)
        {
            Scale();
        }
    }

    private void Update()
    {
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );

        timer += Time.deltaTime;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_onlyOnAwake) return;

        Scale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_onlyOnAwake) return;

        isHovering = false;
        targetScale = originalScale;
    }

    public void Scale()
    {
        isHovering = true;
        targetScale = originalScale * (1 + scaleAmount);

        if (timer >= 0.05f)
        {
            audioSource.PlayOneShot(_hoveringClip);
            timer = 0;
        }
    }

    public void ResetScale()
    {
        isHovering = false;
        targetScale = originalScale;
        rectTransform.localScale = originalScale;
    }

    public void SetOriginalScale(Vector3 newScale)
    {
        originalScale = newScale;

        if (!isHovering)
        {
            targetScale = originalScale;
        }
    }
}