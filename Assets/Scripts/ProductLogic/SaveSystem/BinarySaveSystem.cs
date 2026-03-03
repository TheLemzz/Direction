using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class JsonSaveSystem : SaveSolution
{
    private const string FILE_EXTENSION = ".json";
    private string _savePath;

    private void Awake()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "presets");
        if (!Directory.Exists(_savePath))
        {
            Directory.CreateDirectory(_savePath);
        }
    }

    public override void Save<T>(T data, string saveName)
    {
        string json = JsonUtility.ToJson(data, true);

        string fileName = string.IsNullOrEmpty(saveName)
            ? $"{data.SaveKey}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}"
            : $"{saveName}.{data.SaveKey}";

        string fullPath = Path.Combine(_savePath, fileName + FILE_EXTENSION);

        File.WriteAllText(fullPath, json);
        Debug.Log($"Successfully saved to: {fullPath}");
    }

    public override T Load<T>(string saveName)
    {
        string fullPath = Path.Combine(_savePath, saveName + FILE_EXTENSION);

        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Save file not found: {fullPath}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(fullPath);

            T data = JsonUtility.FromJson<T>(json);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load save {saveName}: {e.Message}");
            return null;
        }
    }

    public override IReadOnlyDictionary<string, string> GetSaves()
    {
        Dictionary<string, string> saves = new();
        if (!Directory.Exists(_savePath)) return saves;

        foreach (string filePath in Directory.GetFiles(_savePath, $"*{FILE_EXTENSION}"))
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            saves[fileName] = filePath;
        }
        return saves;
    }

    public override bool Delete(string saveName)
    {
        string fullPath = Path.Combine(_savePath, saveName + FILE_EXTENSION);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return true;
        }

        return false;
    }
}