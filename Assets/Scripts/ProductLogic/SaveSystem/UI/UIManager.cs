using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _saveName;
    [SerializeField] private FieldSettingController _fieldController;
    [SerializeField] private Button _startButton;
    [SerializeField] private TextMeshProUGUI _warningText;

    public void Save()
    {
        Debug.Log($"UIManager: called save with {_saveName.text} value.");
        MLSaver.GetInstance().SaveCurrentData(_saveName.text);
    }

    public void ReloadButtons(IReadOnlyDictionary<BaseFieldType, VariableReference> fields)
    {
        Debug.Log($"UIManager: called ReloadButtons with {fields.Count} values.");

        _fieldController.ClearFields();

        foreach (KeyValuePair<BaseFieldType, VariableReference> field in fields)
        {
            _fieldController.AddField(field.Key, field.Value);
        }

        string fileName = MLSaver.GetInstance().CurrentSaveName;
        _saveName.text = fileName == null ? "new_save" : fileName.Split('.')[0];
    }

    public void CelarFields()
    {
        _fieldController.ClearFields();
    }

    public void Create(IReadOnlyDictionary<BaseFieldType, VariableReference> fields)
    {
        Debug.Log("UIManager: called create");

        ReloadButtons(fields);
    }

    private void FixedUpdate()
    {
        bool value = MLSaver.GetInstance().HasSave();

        _warningText.gameObject.SetActive(!value);
        _startButton.gameObject.SetActive(value);
    }
}
