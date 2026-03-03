using System.Collections.Generic;

[System.Serializable]
public abstract class BaseSaveData
{
    public int ImageQuality;
    public int BackgroundPercent;
    public int Split;
    public int CriticalMemory;
    public string ImageCap;
    public string PyIntepretatorPath;
    public string[] PyScripts;
    public string[] SavePaths;

    public abstract string SaveKey { get; }

    public BaseSaveData() { }

    protected Dictionary<BaseFieldType, VariableReference> GetBaseFieldTypes()
    {
        Dictionary<BaseFieldType, VariableReference> dict = new()
        {
            //Хардкод названий\описаний переменных легко изменяемо - когда внедрится локализация, достаточно будет передавать вместо описания
            //ключ translate, который будет автоматически переводится.

            {
            new SliderFieldType(
                "Качество изображения",
                "Большие значения увеличивают качество изображения, но снижают производительность.",
                false,
                FieldIconType.Rocket,
                25,
                100,
                25,
                true)
                ,
                new VariableReference(() => ImageQuality, v => { ImageQuality = (int)v; })
            },
            {
            new SliderFieldType(
                "Процент background",
                "Процент сохранения изображений, которые не содержат в себе целевых объектов.",
                false,
                FieldIconType.Gear,
                0,
                100,
                1,
                true)
                ,
                new VariableReference(() => BackgroundPercent, v => { BackgroundPercent = (int)v; })
            },
            {
            new InputFieldType(
                "Лимит изображений",
                "После преодоления лимита - симуляция остановится. 0 - нет лимита.",
                false,
                FieldIconType.Layers,
                "Лимит изображений")
                ,
                new VariableReference(() => ImageCap, v => { ImageCap = (string)v; })
            },
            {
            new SliderFieldType(
                "Сплит",
                "Какой процент изображений будет использоваться в качестве train?",
                false,
                FieldIconType.Layers,
                1,
                95,
                60,
                true)
                ,
                new VariableReference(() => Split, v => { Split = (int)v; })
            },
            {
            new SliderFieldType(
                "Критическая память",
                "При каких значениях оставшийся свободной памяти на диске в МБ - менять его на следующий из списка?",
                false,
                FieldIconType.Trashcan,
                4,
                1024,
                512,
                true,
                "МБ")
                ,
                new VariableReference(() => CriticalMemory, v => { CriticalMemory = (int)v; })
            },
            {
            new InputFieldType(
                "Интерпретатор",
                "Путь до Python.exe",
                false,
                FieldIconType.LinkPlus,
                "Путь до Python.exe")
                ,
                new VariableReference(() => PyIntepretatorPath, v => { PyIntepretatorPath = (string)v; })
            },
            {
            new ScrollViewFieldType(
                "Python скрипты",
                "Формат - path;show-output(1/0);hide-console(1/0);eternal(1/0);args. Используется PyModule!",
                false,
                FieldIconType.LinkPlus,
                "Новый скрипт")
                ,
                new VariableReference(() => PyScripts, v => { PyScripts = (string[])v; })
            },
            {
            new ScrollViewFieldType(
                "Пути сохранения",
                "Укажи полный путь к папке, куда будет сохраняться датасет.",
                false,
                FieldIconType.Gear,
                "Новый путь")
                ,
                new VariableReference(() => SavePaths, v => { SavePaths = (string[])v; })
            }
        };

        return dict;
    }

    public abstract IReadOnlyDictionary<BaseFieldType, VariableReference> GetFieldTypes();
}