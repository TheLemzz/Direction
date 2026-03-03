using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public sealed class BadSymbolsInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;

    public void OnValueChanged()
    {
        _inputField.text = Regex.Replace(_inputField.text, @"[^a-zA-Z0-9 ]", "");
    }
}
