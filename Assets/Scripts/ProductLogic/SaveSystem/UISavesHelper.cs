using System.Collections.Generic;
using UnityEngine;

public abstract class UISavesHelper<T> : MonoBehaviour, ISaveHelper where T : BaseSaveData
{
    [SerializeField] protected UIManager _manager;
    protected T data;

    protected bool shouldReload = false;

    public IReadOnlyDictionary<BaseFieldType, VariableReference> GetVariables()
    {
        return data.GetFieldTypes();
    }

    public abstract void LoadSave(string name);

    public abstract void CreateNewFields(UIManager manager);
}
