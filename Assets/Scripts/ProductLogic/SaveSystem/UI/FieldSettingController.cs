using System;
using UnityEngine;

public sealed class FieldSettingController : MonoBehaviour
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private RectTransform _extraParent;

    [SerializeField] private RectTransform _sliderHolder;
    [SerializeField] private RectTransform _inputHolder;
    [SerializeField] private RectTransform _scrollViewHolder;

    [SerializeField] private IconRegistry _iconRegistry;

    public void AddField(BaseFieldType type, VariableReference reference)
    {
        Sprite iconSprite = _iconRegistry.GetIcon(type.IconType);

        switch (type)
        {
            case SliderFieldType sliderFieldType:
                SliderFieldHolder sliderInstance = Instantiate(_sliderHolder, _parent).GetChild(0).GetComponent<SliderFieldHolder>();
                sliderInstance.Init(sliderFieldType, iconSprite, reference);
                break;

            case InputFieldType inputFieldType:
                InputFieldHolder inputInstance = Instantiate(_inputHolder, _parent).GetChild(0).GetComponent<InputFieldHolder>();
                inputInstance.Init(inputFieldType, iconSprite, reference);
                break;

            case ScrollViewFieldType scrollViewFieldType:
                ScrollViewFieldHolder scrollViewInstance = Instantiate(_scrollViewHolder, _extraParent).GetComponent<ScrollViewFieldHolder>();
                scrollViewInstance.Init(scrollViewFieldType, iconSprite, reference);
                break;

            default:
                throw new ArgumentException($"Type {type} doesn't implement yet.");
        }
    }

    public void ClearFields()
    {
        foreach (BaseFieldHolder holder in _parent.GetComponentsInChildren<BaseFieldHolder>(true))
        {
            Destroy(holder.transform.parent.gameObject);
        }

        foreach (BaseFieldHolder holder in _extraParent.GetComponentsInChildren<BaseFieldHolder>(true))
        {
            Destroy(holder.gameObject);
        }
    }
}