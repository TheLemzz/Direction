using Newtonsoft.Json.Linq;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    private void Start()
    {
        ZeroMqManager.GetInstance().OnPythonEventReceived += HandlePythonEvent;
    }

    private void OnDestroy()
    {
        if (ZeroMqManager.Instance != null)
            ZeroMqManager.GetInstance().OnPythonEventReceived -= HandlePythonEvent;
    }

    private void HandlePythonEvent(string topic, string jsonPayload)
    {
        JObject data = JObject.Parse(jsonPayload);
        string eventName = data["event"]?.ToString();
        string sender = data["sender"]?.ToString();

        if (eventName == "PERSON_DETECTED")
        {
            int count = data["data"]["count"].Value<int>();
            Debug.Log($"<color=cyan>[C#] {sender} обнаружил {count} людей!</color>");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ZeroMqManager.GetInstance().SendCommand("ALL", "RESET_SIMULATION");
            Debug.Log("Sent RESET to ALL");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            var payload = new { value = Random.Range(0f, 1f) };
            ZeroMqManager.GetInstance().SendCommand("detector.py", "SET_THRESHOLD", payload);
            Debug.Log("Sent SET_THRESHOLD to detector.py");
        }
    }
}