using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class SlidebarVisualizer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Slider _slider;

    [SerializeField] private string _symbol;

    private void Awake()
    {
        _text.text = $"{_slider.value}{_symbol}";
    }

    public void OnValueChanged()
    {
        _text.text = $"{_slider.value}{_symbol}";
    }
}
