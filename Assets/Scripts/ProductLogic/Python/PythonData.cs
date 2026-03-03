using UnityEngine;

[System.Serializable]
public struct PythonData
{
    [Tooltip("Полный путь к .py скрипту.")] public string ScriptPath;
    [Tooltip("Выводить output после завершения работы симуляции? Editor only.")] public bool ShowOutput;
    [Tooltip("Запускать скрипт без окна консоли?")] public bool HideConsole;
    [Tooltip("Выполняется ли скрипт паралелльно на протяжении всей симуляции? Если да - требуется указать рабочую директорию, там будет файл-остановки.")]
    public bool Eternal;
    [Tooltip("Рабочая директория, куда будет отправлен файл-остановки для завершения работы скрипта. Можно оставить пустым при Eternal = false.")]
    public string WorkPath;
    [Space, TextArea, Tooltip("Аргументы, требуемые для запуска Python-скрипта. Передается через консоль. Можно оставить пустым")] public string Args;
}
