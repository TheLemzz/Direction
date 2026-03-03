using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

[Icon("Assets/Design/App/Python.png")]
public sealed class PyModule : MonoBehaviorSingleton<PyModule>
{
    [SerializeField] private bool _scriptsWorking = true;

    private string _interpretatorPath;

    private bool init = false;

    public void Init()
    {
        if (init || _instance != null) return;

        BaseSaveData data = MLSaver.GetInstance().GetCurrentSaveData<BaseSaveData>();

        _interpretatorPath = data.PyIntepretatorPath;

        if (!_interpretatorPath.EndsWith(".exe"))
        {
            Debug.LogWarning("PyModule: некорректный путь до интерпретатора. Старт отменен.");
            return;
        }

        init = true;

        SetInstance(this);

        Debug.Log($"{gameObject.name}: PyModule успешно запущен. Запуск скриптов : {_scriptsWorking}");

        if (!_scriptsWorking)
        {
            Debug.Log("PythonModule: работы скриптов отключена. Отмена.");
            return;
        }

        foreach (string item in data.PyScripts)
        {
            StartScript(StringToPythonData(item));
        }
    }

    public PythonData StringToPythonData(string value)
    {
        string[] values = value.Split(';');

        if (values.Length != 5) throw new ArgumentException($"Incorrect argument given! String must contain 5 element, got {values.Length}");


        return new PythonData()
        {
            ScriptPath = values[0].Sanitize(),
            ShowOutput = values[1] != "1",
            HideConsole = values[2] == "1",
            Eternal = values[3] == "1",
            Args = values[4]
        };
    }

    public void StartScript(PythonData data)
    {
        StartScript(data.ScriptPath, data.Args, data.HideConsole);
    }

    public void StartScript(string scriptName, string args = "", bool createNoWindow = true, bool showOutput = true)
    {
        Debug.Log($"Запуск скрипта: {scriptName}");

        string scriptFileName = Path.GetFileName(scriptName);
        string finalArgs = $"\"{scriptName}\" --name \"{scriptFileName}\" {args}";

        Task.Run(() =>
        {
            try
            {
                Process process = new();
                process.StartInfo.FileName = _interpretatorPath;
                process.StartInfo.Arguments = $"\"{scriptName}\" {finalArgs}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = createNoWindow;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptName);

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data)) Debug.Log($"{scriptName} Python Output: {e.Data}");
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data)) Debug.LogError($"{scriptName} Python Error: {e.Data}");
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                Debug.Log($"Скрипт {scriptName} завершился с кодом: {process.ExitCode}");
            }

            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при запуске скрипта {scriptName}: {ex.Message}");
            }

        });
    }

    public string GetDetectorDataPath()
    {
        return @"E:\UnityProjects\siriusinternal\AI\datas_people\";
    }

    public string GetRoadDataPath()
    {
        return @"E:\UnityProjects\siriusinternal\AI\datas\";
    }

    public bool IsWorking()
    {
        return _scriptsWorking;
    }
}