using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public sealed class MLManager : MonoBehaviour
{
    [SerializeField] private Transform _camerasTransform;
    [SerializeField] private TrainingDataGenerator[] _trainingDataGenerators;
    [SerializeField] private TrainingData[] _trainingData;

    private float split = 15;
    private int criticalMemory;

    private List<string> outputFolders;

    private int _imageCap;
    private string currentOutputFolder = string.Empty;
    private string labelsPath;
    private string imagesPath;

    private int _totalSavedPhotos = 0;
    private Stopwatch _stopwatch;


    private void OnValidate()
    {
        if (_camerasTransform == null)
        {
            _trainingDataGenerators = new TrainingDataGenerator[0];
            return;
        }

        if (_trainingDataGenerators.Length == 0 && _camerasTransform != null)
        {
            _trainingDataGenerators = _camerasTransform.GetComponentsInChildren<TrainingDataGenerator>();
        }
    }

    private void Awake()
    {
        _stopwatch = Stopwatch.StartNew();

        LoadSettings();

        currentOutputFolder = GetAvailableSavePath();

        imagesPath = Path.Combine(currentOutputFolder, "Dataset", "images");
        labelsPath = Path.Combine(currentOutputFolder, "Dataset", "labels");

        InitDirectories();

        foreach (TrainingDataGenerator data in _trainingDataGenerators)
        {
            data.Init(_trainingData, this);
        }
    }

    private void FixedUpdate()
    {
        DriveInfo disk = new(currentOutputFolder[0].ToString());

        if (!disk.IsReady || disk.AvailableFreeSpace / (1024 * 1024) <= criticalMemory)
        {
            Debug.Log($"Out of memory for path {currentOutputFolder[0]}");
            string newPath = GetAvailableSavePath();

            if (newPath != null)
            {
                outputFolders.Remove(newPath);
                ChangePath(newPath);
            }
        }

        if (_imageCap > 0 && _totalSavedPhotos >= _imageCap)
        {
            Debug.Log($"<b>[MLManager]</b> рубеж фотографий преодолен. Выключение симуляции..");
            SceneManager.LoadScene(0);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _stopwatch.Stop();

        double totalSeconds = _stopwatch.Elapsed.TotalSeconds;

        double speed = totalSeconds > 0.001 ? _totalSavedPhotos / totalSeconds : 0;

        Debug.Log($"<b>[MLManager Statistics]</b> Симуляция завершена.\n" +
                  $"Сохранено фотографий: <b>{_totalSavedPhotos}</b>\n" +
                  $"Время: <b>{totalSeconds:F2} сек</b>\n" +
                  $"Средняя скорость: <color=green><b>{speed:F2} фото/с</b></color>");
    }


    private void LoadSettings()
    {
        BaseSaveData save = MLSaver.GetInstance().GetCurrentSaveData<BaseSaveData>();

        _imageCap = int.Parse(save.ImageCap);
        split = save.Split;
        criticalMemory = save.CriticalMemory;
        outputFolders = save.SavePaths.ToList();

        Debug.Log($"MLManager: Загрузка данных:\nBG: {save.BackgroundPercent}\nIQ:{save.ImageQuality}\nSPLT:{save.Split}\nCM:{save.CriticalMemory}\nOF(F/L):{save.SavePaths[0]}, {save.SavePaths.Length}");
    }

    private string GetAvailableSavePath()
    {
        string path = outputFolders.FirstOrDefault(x => new DriveInfo(x[0].ToString()).AvailableFreeSpace / (1024 * 1024) >= criticalMemory * 3);

        if (path == null || outputFolders.Count == 0)
        {
            Debug.LogError("MLManager: свободного места не осталось ни на одном носителе информации. Изображения более не будут сохраняться.");

            foreach (TrainingDataGenerator data in _trainingDataGenerators)
            {
                data.enabled = false;
            }
            enabled = false;

            throw new NullReferenceException("Все диски заполнены.");
        }

        return path;
    }

    private void ChangePath(string newPath)
    {
        currentOutputFolder = newPath;
        labelsPath = Path.Combine(newPath, "Dataset", "labels");
        imagesPath = Path.Combine(newPath, "Dataset", "images");

        InitDirectories(true);
    }

    private void InitDirectories(bool onlyDirectories = false)
    {
        if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);
        if (!Directory.Exists(labelsPath)) Directory.CreateDirectory(labelsPath);

        Directory.CreateDirectory($"{imagesPath}/train");
        Directory.CreateDirectory($"{imagesPath}/test");
        Directory.CreateDirectory($"{imagesPath}/val");

        Directory.CreateDirectory($"{labelsPath}/train");
        Directory.CreateDirectory($"{labelsPath}/test");
        Directory.CreateDirectory($"{labelsPath}/val");

        Directory.CreateDirectory($"{currentOutputFolder}/Used");

        if (onlyDirectories) return;

        if (!File.Exists(Path.Combine(currentOutputFolder, "obj.names"))) File.Create(Path.Combine(currentOutputFolder, "obj.names")).Close();
        if (!File.Exists(Path.Combine(currentOutputFolder, "data.yaml")))
        {
            File.Create(Path.Combine(currentOutputFolder, "data.yaml")).Close();
            File.WriteAllText(Path.Combine(currentOutputFolder, "data.yaml"),
                $"names:\n- {string.Join("\n- ", _trainingData.Select(x => x.Classification))}\nnc: {_trainingData.Length}\npath: Dataset\ntrain: images/train\nval: images/val\ntest: images/val");
        }

        Task.Run(() => GenerateObjNamesAsync());
    }

    private async Task GenerateObjNamesAsync()
    {
        Debug.Log("Generating obj.names...");

        string namesFilePath = Path.Combine(currentOutputFolder, "obj.names");

        await File.WriteAllLinesAsync(namesFilePath, _trainingData.Select(x => x.Classification));
    }

    public async Task SavePhotoAndAnnotationAsync(byte[] imageBytes, IEnumerable<string> annotation)
    {
        bool forValidation = new Random().Next(0, 100) < split;
        string name = $"image_{Guid.NewGuid().ToString()[..28]}";

        await File.WriteAllBytesAsync($"{imagesPath}{(forValidation ? "/val/" : "/train/")}/{name}.jpg", imageBytes);
        await File.WriteAllLinesAsync($"{labelsPath}{(forValidation ? "/val/" : "/train/")}/{name}.txt", annotation);

        Interlocked.Increment(ref _totalSavedPhotos);
    }
}