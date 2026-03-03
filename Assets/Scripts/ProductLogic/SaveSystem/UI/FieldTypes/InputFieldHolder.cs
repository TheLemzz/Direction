using TMPro;
using UnityEngine;

public class InputFieldHolder : BaseFieldHolder
{
    [SerializeField] private TextMeshProUGUI _input;
    [SerializeField] private TextMeshProUGUI _inputFieldText;
    [SerializeField] private TMP_InputField _InputField;

    public InputFieldHolder Init(InputFieldType info, Sprite sprite, VariableReference reference)
    {
        InternalInit(info, sprite, reference);

        _inputFieldText.text = info.DefaultText;
        _InputField.text = ((string)reference.Get()).Sanitize();
        return this;
    }

    public override void OnValueChanged()
    {
        Debug.Log($"set new value: {_input.text}");
        _reference.Set(_input.text.Sanitize());

        print(_reference.Get());
    }
}
