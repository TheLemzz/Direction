using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public sealed class UISavesLoader : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private UIManager _manager;
    [SerializeField] private GameObject _createPanel;
    [SerializeField] private GameObject _loadPanel;
    [SerializeField] private HelpersData[] _data;

    private readonly Dictionary<string, ISaveHelper> _saveHelpers = new();

    private string _fileType = Constants.ROAD_SAVE;

    private ISaveHelper _saveHelper;

    private void Awake()
    {
        if (_saveHelper == null) LoadKeys();
    }

    private void OnEnable()
    {
        if (MLSaver.Instance.GetAvailableSaves().Where(x => x.Key.ToLower().Contains(_fileType)).Count() == 0)
        {
            Debug.Log($"UISavesLoader: found no valid saves for {_fileType}. Create new, used {_saveHelper.GetType()} | {_fileType}");
            _createPanel.SetActive(true);
            _loadPanel.SetActive(false);

            _saveHelper.CreateNewFields(_manager);

            return;
        }
        Debug.Log("UISavesLoader: reload options.");
        ReloadOptions();
    }

    public void SetFileType(string fileType)
    {
        if (_saveHelper == null) LoadKeys();

        Debug.Log($"UISavesLoader: set {fileType}");

        _fileType = fileType.ToLower();
        _saveHelper = _saveHelpers[_fileType];

        _manager.CelarFields();
        _createPanel.SetActive(false);
        _loadPanel.SetActive(true);
    }

    public void Select()
    {
        if (_dropdown.options.Count == 0) return;

        string selectedSaveName = _dropdown.options[_dropdown.value].text;

        _saveHelper.LoadSave(selectedSaveName);
    }

    public void Delete()
    {
        if (_dropdown.options.Count == 0) return;

        string selectedSaveName = _dropdown.options[_dropdown.value].text;

        MLSaver.Instance.DeleteSave(selectedSaveName);

        _dropdown.value = 0;
        ReloadOptions();
    }

    private void ReloadOptions()
    {
        _dropdown.ClearOptions();

        List<KeyValuePair<string, string>> saves = MLSaver.Instance.GetAvailableSaves().Where(x => x.Key.ToLower().Contains(_fileType)).ToList();

        if (saves != null && saves.Count > 0)
        {
            List<string> saveNames = saves.Select(x => x.Key).ToList();
            _dropdown.AddOptions(saveNames);
        }
        else
        {
            _dropdown.AddOptions(new List<string> { "Нет сохранений..." });
        }
    }

    private void LoadKeys()
    {
        foreach (HelpersData item in _data)
        {
            _saveHelpers.Add(item.FileType, item.Component.GetComponent<ISaveHelper>());
        }
    }
}