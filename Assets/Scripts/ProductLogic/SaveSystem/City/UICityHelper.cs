using UnityEngine;

public sealed class UICityHelper : UISavesHelper<MLSaveData>
{
    public override void CreateNewFields(UIManager manager)
    {
        data = new MLSaveData();
        Debug.Log($"UICityHelper: creating save");

        manager.Create(data.GetFieldTypes());
        MLSaver.GetInstance().SetCurrentSave<MLSaveData>(data);
    }

    public override void LoadSave(string name)
    {
        Debug.Log($"UICityHelper: loading save {name}");

        data = MLSaver.GetInstance().LoadSpecificSave<MLSaveData>(name);
        _manager.ReloadButtons(data.GetFieldTypes());
        shouldReload = true;
    }
}
