using System.Collections.Generic;

[System.Serializable]
public class KitchenSaveData : BaseSaveData
{
    public float ItemSize;

    public override string SaveKey => Constants.KITCHEN_SAVE;

    public override IReadOnlyDictionary<BaseFieldType, VariableReference> GetFieldTypes()
    {
        Dictionary<BaseFieldType, VariableReference> dict = GetBaseFieldTypes();
        dict.Add(new SliderFieldType(
                "Размер объектов",
                "Регулирует размер объектов, во сколько раз больше может стать размер.",
                false,
                FieldIconType.Gear,
                1,
                5,
                2,
                false)
                ,
                new VariableReference(() => ItemSize, v => { ItemSize = (int)v; }));

        return dict;
    }
}