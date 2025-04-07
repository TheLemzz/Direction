using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

[Icon("Assets/Design/Python.png")]
public sealed class PyModule : MonoBehaviour
{
    [SerializeField, TextArea] private string _interpretatorPath;
    [SerializeField, Tooltip("Список .py скриптов, которые будут запущены по порядку перед стартом симуляции.")] private PythonData[] _pythonData;

    private readonly List<string> _workPaths = new();

    private bool init = false;

    private static PyModule _instance;

    public static PyModule Instance => _instance;

    public void Init()
    {
        if (init) return;

        init = true;
        _instance = this;

        Debug.Log($"{gameObject.name}: PyModule успешно запущен. Запуск скриптов...");

        foreach (PythonData data in _pythonData)
        {
            if (data.Eternal)
            {
                _workPaths.Add(data.WorkPath);
                File.Delete(data.WorkPath + "stop");
            }
            StartScript(data.ScriptPath, data.Args, data.HideConsole, data.ShowOutput);
        }
    }

    private void OnValidate()
    {
        foreach (PythonData data in _pythonData)
        {
            if (!data.WorkPath.EndsWith(@"\")) Debug.LogError(@$"{data.ScriptPath}: Путь workPath не оканчивается на символ \. ");
            if (!data.ScriptPath.EndsWith(".py")) Debug.LogError(@$"{data.ScriptPath}: Путь ScriptPath не оканчивается на .py");
        }
    }

    private (string, string) StartScript(string scriptName, string args = "", bool createNoWindow = true, bool showOutput = true)
    {
        Debug.Log($"Получен к запуску скрипт {scriptName} с аргументами {args}, стартуем...");

        Task.Factory.StartNew(() =>
        {
            Process process = new();
            process.StartInfo.FileName = _interpretatorPath;
            process.StartInfo.Arguments = scriptName + $" {args}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = createNoWindow;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            if (showOutput) Debug.Log($"Результат: {output}\n{error}");

            return (output, error);
        });

        return (null, null);
    }

    public string GetDetectorDataPath()
    {
        return @"E:\UnityProjects\siriusinternal\AI\datas_people\";
    }

    public string GetRoadDataPath()
    {
        return @"E:\UnityProjects\siriusinternal\AI\datas\";
    }

    private void OnApplicationQuit()
    {
        if (!init) return;

        foreach (string workPath in _workPaths)
        {
            Debug.Log($"{workPath}: Завершаем процесс...");
            File.Create(workPath + "stop");
        }
    }
}