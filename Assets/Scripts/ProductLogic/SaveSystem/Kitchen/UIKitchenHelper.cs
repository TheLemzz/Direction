using UnityEngine;

public sealed class UIKitchenHelper : UISavesHelper<KitchenSaveData>
{
    public override void CreateNewFields(UIManager manager)
    {
        data = new KitchenSaveData();
        Debug.Log($"UIKitchenHelper: creating save");

        manager.Create(data.GetFieldTypes());
        MLSaver.GetInstance().SetCurrentSave<KitchenSaveData>(data);
    }

    public override void LoadSave(string name)
    {
        Debug.Log($"UIKitchenHelper: loading save {name}");

        data = MLSaver.GetInstance().LoadSpecificSave<KitchenSaveData>(name);
        _manager.ReloadButtons(data.GetFieldTypes());
        shouldReload = true;
    }
}
