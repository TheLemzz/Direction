using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PeopleDetector : MonoBehaviour
{
    [SerializeField] private CarCameraRecorder _cameraRecorder;

    private CarIntelligentSystem _intelligentSystem;
    private Car _car;

    private const string WARNING = "Человек обнаружен! Внимание на дорогу.";
    private const string OUTPUT_FILE_PATH = @"E:\UnityProjects\siriusinternal\AI\person_output.txt";


    private void OnEnable()
    {
        EntryPoint.OnApplicationStarted += OnApplicationStarted;
    }

    private void OnDisable()
    {
        EntryPoint.OnApplicationStarted -= OnApplicationStarted;
    }

    private void OnApplicationStarted()
    {
        _intelligentSystem = GetComponent<CarIntelligentSystem>();
        _car = GetComponent<Car>();

        StartCoroutine(CheckDetector());
        StartCoroutine(Photo());
    }

    private IEnumerator Photo()
    {
        WaitForSeconds wait = new(0.4f);

        while (true)
        {
            yield return wait;
            _cameraRecorder.MakePhoto(false, PyModule.Instance.GetDetectorDataPath());
        }
    }

    private IEnumerator CheckDetector()
    {
        WaitForSeconds wait = new(1f);

        while (true)
        {
            yield return wait;
            try
            {
                if (File.Exists(OUTPUT_FILE_PATH))
                {
                    string text = File.ReadAllText(OUTPUT_FILE_PATH);
                    if (float.TryParse(text.Replace('.', ','), out float boolean) && boolean == 1)
                    {
                        _intelligentSystem.SendWarningAlert(WARNING);
                    }
                    else
                    {
                        _intelligentSystem.DeleteWarningWithDescription(WARNING);
                    }
                }
                else
                {
                    Debug.LogError($"File not found: {OUTPUT_FILE_PATH}");
                }
            }

            catch (System.Exception e)
            {
                Debug.LogError($"Error reading: {e.Message}");
            }
        }
    }
}
