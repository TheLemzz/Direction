using System;
using System.Collections.Generic;
using UnityEngine;

public class MLSaver : MonoBehaviorSingleton<MLSaver>
{
    [SerializeField] private SaveSolution _solution;

    public string CurrentSaveName { get; private set; }

    private BaseSaveData _currentSaveData;

    private void Awake()
    {
        SetInstance(this, true);

        if (_solution.gameObject.scene.name == null)
        {
            _solution = Instantiate(_solution, transform);
        }
    }

    public void SetCurrentSave(string name)
    {
        Debug.Log($"MLSaver: Set {name}");
        CurrentSaveName = name;
    }

    public void SetCurrentSave<T>(T data) where T : BaseSaveData
    {
        Debug.Log($"MLSaver: Set {data.SaveKey}");
        _currentSaveData = data;
    }

    public void SaveCurrentData<T>(string saveName) where T : BaseSaveData
    {
        if (_currentSaveData is T dataToSave)
        {
            _solution.Save(dataToSave, saveName);
        }
        else
        {
            Debug.LogError($"Current data is not of type {typeof(T).Name}");
        }
    }

    public void Save(BaseSaveData data, string saveName)
    {
        _solution.Save(data, saveName);
    }

    public void SaveCurrentData(string name)
    {
        _solution.Save(_currentSaveData, name);
    }

    public void SaveNewData<T>(T data, string saveName) where T : BaseSaveData
    {
        _solution.Save(data, saveName);
        _currentSaveData = data;
    }

    public T GetCurrentSaveData<T>() where T : BaseSaveData
    {
        return _currentSaveData as T;
    }

    public IReadOnlyDictionary<string, string> GetAvailableSaves()
    {
        if (_solution == null) throw new Exception("MLSaver doesn't have any SaveSolution.");

        return _solution.GetSaves();
    }

    public T LoadSpecificSave<T>(string saveName) where T : BaseSaveData
    {
        T loadedData = _solution.Load<T>(saveName);

        if (loadedData != null)
        {
            _currentSaveData = loadedData;
            CurrentSaveName = saveName;
            Debug.Log($"Loaded save type: {typeof(T).Name}");

            return loadedData;
        }

        throw new NullReferenceException($"No valid saves with name {saveName}.");
    }

    public bool HasSave()
    {
        return _currentSaveData != null;
    }

    public void DeleteSave(string saveName)
    {
        if (_solution != null)
        {
            _solution.Delete(saveName);
        }
    }
}