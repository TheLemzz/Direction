using UnityEngine;
using UnityEngine.UI;

public class SliderFieldHolder : BaseFieldHolder
{
    [SerializeField] private Slider _slider;

    public SliderFieldHolder Init(SliderFieldType info, Sprite sprite, VariableReference reference)
    {
        InternalInit(info, sprite, reference);

        _slider.minValue = info.Min;
        _slider.maxValue = info.Max;
        _slider.wholeNumbers = info.WholeNumbers;

        try
        {
            _slider.value = (int)reference.Get();
        }
        catch (System.InvalidCastException)
        {
            _slider.value = (float)reference.Get();
        }

        return this;
    }

    public override void OnValueChanged()
    {
        Debug.Log($"set new value: {_slider.value}");
        _reference.Set((int)_slider.value);
    }
}
