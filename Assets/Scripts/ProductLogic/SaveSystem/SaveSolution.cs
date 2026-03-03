using System.Collections.Generic;
using UnityEngine;

public abstract class SaveSolution : MonoBehaviour
{
    public abstract void Save<T>(T data, string saveName) where T : BaseSaveData;

    public abstract T Load<T>(string saveName) where T : BaseSaveData;

    public abstract IReadOnlyDictionary<string, string> GetSaves();
    public abstract bool Delete(string saveName);
}