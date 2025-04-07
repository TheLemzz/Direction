using UnityEngine;

[System.Serializable]
[Icon("Assets/Design/Python.png")]
public class PythonData
{
    [Tooltip("Полный путь к .py скрипту.")] public string ScriptPath;
    [Tooltip("Выводить output после завершения работы симуляции? Editor only.")] public bool ShowOutput;
    [Tooltip("Запускать скрипт без окна консоли?")] public bool HideConsole = true;
    [Tooltip("Выполняется ли скрипт паралелльно на протяжении всей симуляции? Если да, то требуется указать рабочую директорию, куда будет отправлен файл-остановки.")] public bool Eternal;
    [Tooltip("Рабочая директория, куда будет отправлен файл-остановки для завершения работы скрипта. Можно оставить пустым, если Eternal - false.")] public string WorkPath;
    [Space, TextArea, Tooltip("Аргументы, требуемые для запуска Python-скрипта. Передается через консоль.")] public string Args;
}
