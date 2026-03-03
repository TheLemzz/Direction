using System.Collections.Generic;

[System.Serializable]
public class MLSaveData : BaseSaveData
{
    public override string SaveKey => Constants.ROAD_SAVE;

    public override IReadOnlyDictionary<BaseFieldType, VariableReference> GetFieldTypes()
    {
        return GetBaseFieldTypes();
    }
}