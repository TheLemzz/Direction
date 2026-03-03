using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseFieldHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _tooltipText;
    [SerializeField] private Image _icon;

    protected VariableReference _reference;

    protected void InternalInit(BaseFieldType baseInfo, Sprite sprite, VariableReference reference)
    {
        if (_description != null) _description.text = baseInfo.Description;
        if (_tooltipText != null) _tooltipText.text = baseInfo.TooltipText;
        if (_title != null) _title.text = baseInfo.Name;
        if (_icon != null) _icon.sprite = sprite;

        _reference = reference;
    }

    public abstract void OnValueChanged();
}
